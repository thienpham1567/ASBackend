using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Backend.Data;
using Backend.Services;
using Backend.Models;
using System.Text;
using System;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCors(options =>
{
    options.AddPolicy("MyCors", policy =>
    {
        policy.WithOrigins("https://appointmentschedulerfrontend.azurewebsites.net").AllowAnyMethod().AllowAnyHeader().AllowAnyOrigin();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'AppointmentScheduleContext' not found.")));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/
// Add AppointmentRepository DI
builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();
builder.Services.AddScoped<IAccountRepository, AccountRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IEmailRepository, EmailRepository>();
builder.Services.Configure<Jwt>(builder.Configuration.GetSection("Jwt"));
var secretKey = builder.Configuration["Jwt:Key"]; // get secret key from AppSettings
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Tự cấp token
        ValidateIssuer = false,
        ValidateAudience = false,
        // Ký vào token
        ValidateIssuerSigningKey = true,
        // Sài thuật toán đối xứng tự động mã hóa
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),

        ClockSkew = TimeSpan.Zero
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

}

app.UseHttpsRedirection();
app.UseCors("MyCors");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
