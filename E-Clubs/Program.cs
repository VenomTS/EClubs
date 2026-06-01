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
using E_Clubs.Reports.Repositories;
using E_Clubs.Reports.Services;
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

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            context.Token = context.Request.Cookies["TOKEN"];
            return Task.CompletedTask;
        }
    };
});
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Services
builder.Services.AddScoped<JWTService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IClubService, ClubService>();
builder.Services.AddScoped<IWorkPlansService, WorkPlansService>();
builder.Services.AddScoped<IMessageService, MessageService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<IReportService, ReportService>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IClubRepository, ClubRepository>();
builder.Services.AddScoped<IClubStudentRepository, ClubStudentRepository>();
builder.Services.AddScoped<IWorkPlansRepository, WorkPlansRepository>();
builder.Services.AddScoped<IMessageRepository, MessageRepository>();
builder.Services.AddScoped<IAttendanceRepository, AttendanceRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();

// CORS
builder.Services.AddCors(options => options.AddPolicy("AllowAll", policy =>
{
    policy.SetIsOriginAllowed(_ => true)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
}));

builder.Services.AddAutoMapper(_ => { }, typeof(MappingProfile));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();