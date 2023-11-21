using Data;
using Data.Tables;
using Lib;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

//
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//JwtSecrets jwtSecrets = new(Secrets.JWT_ISSUER, Secrets.JWT_AUDIENCE, Secrets.JWT_SECRET);
//builder.Services.AddScoped<Jwt>(_ => new(jwtSecrets));

builder.Services
        .AddIdentity<User, IdentityRole>()
        .AddEntityFrameworkStores<Db>();
//.AddDefaultTokenProviders()
//.AddTokenProvider<DataProtectorTokenProvider<User>>("email");

//builder.Services
//    .AddAuthentication(options =>
//    {
//        options.DefaultAuthenticateScheme = RefreshOnly.Scheme;
//        options.DefaultForbidScheme = RefreshOnly.Scheme;
//        options.DefaultChallengeScheme = RefreshOnly.Scheme;
//        options.DefaultScheme = RefreshOnly.Scheme;
//    })
//    .AddScheme<AuthenticationSchemeOptions, RefreshOnlyHandler>(RefreshOnly.Scheme, options => { });

var app = builder.Build();

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
app.UseSwagger();
app.UseSwaggerUI();
//}

app.UseCors(options =>
{
    options
        .SetIsOriginAllowed(_ => true) // UseDomainCors handle it.
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
});

app.UseHttpsRedirection();

app.UseAuthorization();
app.UseAuthorization();

app.MapControllers();

app.Run();