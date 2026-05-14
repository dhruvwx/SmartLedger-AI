//CREATING BUILDER
using APILibrary.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Inject ConnectionString
builder.Services.AddDbContext<SmartLedgerDbContext>
    (options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default"),b => b.MigrationsAssembly("SmartLedgerAPI")
    ));

//Injecting {Authentication} Jwt Token
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

var app = builder.Build();

// Configure the HTTP request pipeline.
         //USING THIS WE ADD MIDDLE WARE  
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();


//BEFORE Authorization
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
