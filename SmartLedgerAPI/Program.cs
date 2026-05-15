//CREATING BUILDER
using APILibrary.Data;
using APILibrary.Services.Interface;
using APILibrary.Services.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using SmartLedgerAPI.AutoMapper;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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


//**********Injecting repository--whenever IRepository is requested call Repository class runs
builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IjwtTokenRepository, JwtTokenRepository>();


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


//*********BEFORE Authorization
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
