using System.Text;
using System.Text.Json.Serialization;
using E_Clubs;
using E_Clubs.Attendances.Repositories;
using E_Clubs.Attendances.Services;
using E_Clubs.Auth.Services;
using E_Clubs.Clubs.Repositories;
using E_Clubs.Clubs.Services;
using E_Clubs.Database;
using E_Clubs.Messages.Repositories;
using E_Clubs.Messages.Services;
using E_Clubs.Users.Repositories;
using E_Clubs.Users.Services;
using E_Clubs.WorkPlans.Repositories;
using E_Clubs.WorkPlans.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidAudience = builder.Configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"] ?? throw new InvalidOperationException()))
    };
});
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddScoped<JWTService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<ClubService>();
builder.Services.AddScoped<WorkPlansService>();
builder.Services.AddScoped<MessageService>();
builder.Services.AddScoped<AttendanceService>();

// Repositories
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<ClubRepository>();
builder.Services.AddScoped<ClubStudentRepository>();
builder.Services.AddScoped<WorkPlansRepository>();
builder.Services.AddScoped<MessageRepository>();
builder.Services.AddScoped<AttendanceRepository>();

// CORS
builder.Services.AddCors(options => options.AddPolicy("AllowAll", policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader()));

builder.Services.AddAutoMapper(_ => { }, typeof(MappingProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors("AllowAll");

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();