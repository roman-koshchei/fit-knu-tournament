// Load env vars from .env
using Data;
using Data.Tables;
using Lib;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using Web.Config;
using Web.Services;

Env.LoadFile(Path.Combine(Directory.GetCurrentDirectory(), ".env"));

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "QuasarSoft auth solution",
        Description = "The best auth solution in the world.",
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Scheme = "bearer",
        Description = "Please insert JWT token into field"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// DB
builder.Services.AddDbContext<Db>(options => options.UseNpgsql(Secrets.DB_CONNECTION_STRING));

// JWT
JwtSecrets jwtSecrets = new(Secrets.JWT_ISSUER, Secrets.JWT_SECRET);
builder.Services.AddScoped<Jwt>(_ => new(jwtSecrets));

builder.Services
        .AddIdentity<User, IdentityRole>()
        .AddEntityFrameworkStores<Db>()
        .AddDefaultTokenProviders()
        .AddTokenProvider<DataProtectorTokenProvider<User>>("email");

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = Secrets.JWT_ISSUER,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Secrets.JWT_SECRET)),
        };
    })
    .AddCookie()
    .AddGoogle(options =>
    {
        options.SignInScheme = IdentityConstants.ExternalScheme;
        options.ClientId = Secrets.GOOGLE_CLIENT_ID;
        options.ClientSecret = Secrets.GOOGLE_CLIENT_SECRET;
    });

builder.Services.AddScoped<GoogleService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseCors(options =>
{
    options
        .SetIsOriginAllowed(_ => true)
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
});

app.UseRouting();

app.UseStatusCodePages(context =>
{
    var isApi = context.HttpContext.Request.Path.ToString().StartsWith("/api");
    if (!isApi && context.HttpContext.Response.StatusCode == 401)
    {
        context.HttpContext.Response.Cookies.Delete("token");
        context.HttpContext.Response.Redirect("/");
    }
    return Task.CompletedTask;
});

//app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseTokenTransferMiddleware();

app.UseAuthentication();
app.UseAuthorization();

app.UseCustomAuthMiddleware();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();