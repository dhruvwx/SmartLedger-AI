//CREATING BUILDER
using APILibrary.Data;
using APILibrary.Services.AI.Repository;
using APILibrary.Services.AI.Services;
using APILibrary.Services.Interface;
using APILibrary.Services.Repository;
using APILibrary.Services.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SmartLedgerAPI.AutoMapper;
using SmartLedgerAPI.Middlewares;
using StackExchange.Redis;
using System.Text;


//Injecting Serilog
var logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("Logs/SmartLedgerLogs_log.txt", rollingInterval: RollingInterval.Hour)
    .CreateLogger();
Log.Logger = logger;


var builder = WebApplication.CreateBuilder(args);

//CONNECTING .NET TO SERILOG
builder.Logging.ClearProviders();
builder.Host.UseSerilog(logger);




// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle


//ADDING IMEMORYCACHE FOR GETTING CATEGORIES FROM DATABASE AS IT DONT CHANGE OFTEN SO WE STORE THEM IN LOCAL CACHE  -- using in category service (it rarely changes)
builder.Services.AddMemoryCache();

//ADDING REDIS
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
         {
             var redisConnectionString = builder.Configuration.GetConnectionString("Redis") ?? "redis:6379";

             var options = ConfigurationOptions.Parse(redisConnectionString);

             options.AbortOnConnectFail = false;

             return ConnectionMultiplexer.Connect(options);
         });


builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend",
                              policy => policy.WithOrigins("http://127.0.0.1:5500")
                              .AllowAnyHeader().AllowAnyMethod()
                              );
        });

builder.Services.AddEndpointsApiExplorer();

// adding authorization to swagger
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(
        JwtBearerDefaults.AuthenticationScheme,new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Type =Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = JwtBearerDefaults.AuthenticationScheme
        });

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference = new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        }
                },
                 new List<string>()
            }
        });
});

//*******Inject ConnectionString
//var connectionString = builder.Configuration["ConnectionStrings:Default"];
var connectionString = builder.Configuration.GetConnectionString("Default") ?? throw new InvalidOperationException("Missing Connection String");

builder.Services.AddDbContext<SmartLedgerDbContext>
    (options => options.UseSqlServer(connectionString, b => b.MigrationsAssembly("SmartLedgerAPI")
    ));


//Required configs for authentication
//var JwtIssuer = builder.Configuration["Jwt:Issuer"];  --good for loacal
var jwtIssuer = builder.Configuration ["Jwt:Issuer"] ?? throw new InvalidOperationException("Missing Jwt Issuer");
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("Missing Jwt Audience");
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("Missing Jwt Key");


//**********Injecting {Authentication} Jwt Token
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true, 
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)) 
    });

// INJECTING CONSTRUCTOR to use external api
        //builder.Services.AddHttpClient();
//Injecting AI Repository
builder.Services.AddHttpClient<IExpenseCategorizerByAi, ExpenseCategorizerByAi>();



//====INJECTING SERVICES====
builder.Services.AddScoped<IAuthService , AuthService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IBudgetService , BudgetService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IExpenseService, ExpenseService>();



//**********Injecting repository--whenever IRepository is requested call Repository class runs

//builder.Services.AddScoped<IjwtTokenRepository, JwtTokenRepository>();
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IExpenseRepository , ExpenseRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IBudgetRepository, BudgetRepository>();



//**********Injecting Mapping -- IMapper
//builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddAutoMapper(config => config.AddProfile<MappingProfile>());



var app = builder.Build();



//add after var app = builder.Build(); -TO ADD AUTOMATIC DATABASE MIGRATIONS FOR DOCKER
if(!app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope(); //create scope
    var db = scope.ServiceProvider.GetRequiredService<SmartLedgerDbContext>();
    db.Database.Migrate();
}


// Configure the HTTP request pipeline.
//USING THIS WE ADD MIDDLE WARE  

//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
    //out of is to run docker in browser -------- 
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

//Injecting Global Exception Handler
app.UseMiddleware<ExceptionHandlerMiddleware>();

//injecting cors to allow frontend to use the api
app.UseCors("AllowFrontend");


//*********BEFORE Authorization
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();



//HEALTH CHECK FOR DOCKER 
        //LIVENESS PROBE (/health)
app.MapGet("/health", () => Results.Ok("Healthy"));
        //READINESS PROBE(/ready)
app.MapGet("/ready", async (SmartLedgerDbContext db) =>
                          {
                              try
                              {
                                  await db.Database.CanConnectAsync();
                                  return Results.Ok("Ready");
                              }
                              catch
                              {
                                  return Results.StatusCode(503);
                              }
                          });

app.Run();
