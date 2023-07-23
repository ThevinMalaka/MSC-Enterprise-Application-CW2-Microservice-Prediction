using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using predictionService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen();


// Get Another Service data
builder.Services.AddHttpClient();
builder.Services.AddHttpContextAccessor(); // TESTING

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
       options.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")));

// Getting var from appsetting.json file
string UserServiceEndpoint = builder.Configuration["UserServiceEndpoint"];
string WorkoutServiceEndpoint = builder.Configuration["WorkoutServiceEndpoint"];

// Add JWT authentication services
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = "thevinmalaka.com",
            ValidAudience = "thevinmalaka.com",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("ThisIsMySuperSecretKeyForFitnessAppInMyMSCourseWork"))
        };
    });


// Add Services
builder.Services.AddScoped<PredictionService>();

// Add Swagger services
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Prediction MicroService", Version = "v1" });

    var securityScheme = new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "JWT Authorization header using the Bearer scheme.",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer", // must be lower case
        Reference = new OpenApiReference
        {
            Type = ReferenceType.SecurityScheme,
            Id = "Bearer"
        }
    };
    c.AddSecurityDefinition("Bearer", securityScheme);

    var securityRequirement = new OpenApiSecurityRequirement
    {
        {securityScheme, new[] {"Bearer"}}
    };
    c.AddSecurityRequirement(securityRequirement);
});

var app = builder.Build();

app.UseSwagger();

app.UseSwaggerUI();

app.UseHttpsRedirection();

// Use authentication middleware
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

