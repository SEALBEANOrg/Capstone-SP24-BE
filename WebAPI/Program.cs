using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using WebAPI;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using WebAPI.Middlewares;
using Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddInfrastructure();
builder.Services.AddServices();

#region JWT(use)

IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", true, true)
                .Build();
builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(config["JwtAuth:Key"]))
    };
});

#endregion

#region OAuth(not use)

//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
//})
//.AddCookie()
//.AddGoogle(GoogleDefaults.AuthenticationScheme, options =>
//{
//    options.ClientId = builder.Configuration["GoogleKeys:ClientId"];
//    options.ClientSecret = builder.Configuration["GoogleKeys:ClientSecret"];
//});

#endregion


#region Swagger_Jwt

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TrialAPI", Version = "v1" });

    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer"
        //Type = SecuritySchemeType.OAuth2,
        //Flows = new OpenApiOAuthFlows
        //{
        //    ClientCredentials = new OpenApiOAuthFlow
        //    {
        //        AuthorizationUrl = new Uri("https://accounts.google.com/o/oauth2/auth"),
        //        TokenUrl = new Uri("https://oauth2.googleapis.com/token"),
        //        Scopes = new Dictionary<string, string>
        //        {
        //            { "api1", "Exagen API" }
        //        }
        //    }
        //}
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                //Id = "oauth2"
                                Id = JwtBearerDefaults.AuthenticationScheme
                            }
                        },
                        new string[] { }
                    }
                });
});

#endregion

#region Cors

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "AllowCors",
        policy =>
        {
            policy.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
        });
});

#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowCors");


app.UseMiddleware<GlobalExceptionMiddleware>();

//app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
