//CREATING BUILDER
using APILibrary.Data;
using APILibrary.Services.AI.Repository;
using APILibrary.Services.AI.Services;
using APILibrary.Services.Interface;
using APILibrary.Services.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using SmartLedgerAPI.AutoMapper;
using SmartLedgerAPI.Middlewares;
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
builder.Services.AddDbContext<SmartLedgerDbContext>
    (options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default"),b => b.MigrationsAssembly("SmartLedgerAPI")
    ));

//**********Injecting {Authentication} Jwt Token
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))  //! tells it wont be null
    });

// INJECTING CONSTRUCTOR to use external api
        //builder.Services.AddHttpClient();
//Injecting AI Repository
builder.Services.AddHttpClient<IExpenseCategorizerByAi, ExpenseCategorizerByAi>();

//**********Injecting repository--whenever IRepository is requested call Repository class runs

builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IjwtTokenRepository, JwtTokenRepository>();
builder.Services.AddScoped<IExpenseRepository , ExpenseRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IBudgetRepository, BudgetRepository>();


//**********Injecting Mapping -- IMapper
builder.Services.AddAutoMapper(typeof(MappingProfile));




var app = builder.Build();

// Configure the HTTP request pipeline.
         //USING THIS WE ADD MIDDLE WARE  


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Injecting Global Exception Handler
app.UseMiddleware<ExceptionHandlerMiddleware>();

//injecting cors to allow frontend to use the api
app.UseCors("AllowFrontend");


//*********BEFORE Authorization
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
