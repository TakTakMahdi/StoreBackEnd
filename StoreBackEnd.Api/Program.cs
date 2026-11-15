
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StoreBackEnd.Api.Application.Dto;
using StoreBackEnd.Api.Application.Services;
using StoreBackEnd.Api.Core.Interfaces;
using StoreBackEnd.Api.Infrastructure.Data;
using StoreBackEnd.Api.Infrastructure.Repositories;
using StoreBackEnd.Api.Infrastructure.Security;
using StoreBackEnd.Api.Infrastructure.Seed;
using StoreBackEnd.Api.Infrastructure.Services;
using StoreBackEnd.Api.Middleware;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ---------- DbContext ----------
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHttpContextAccessor();

// ---------- Repositories / Unit of Work ----------
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUtilisateurRepository, UtilisateurRepository>();
builder.Services.AddScoped<IProduitRepository, ProduitRepository>();
builder.Services.AddScoped<ICommandeRepository, CommandeRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

// ---------- Application services ----------
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IProduitService, ProduitService>();
builder.Services.AddScoped<ICategorieService, CategorieService>();
builder.Services.AddScoped<ICommandeService, CommandeService>();
builder.Services.AddScoped<IAdminService, AdminService>();

// ---------- Infrastructure services ----------
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuditService, AuditService>();

// ---------- AutoMapper ----------
builder.Services.AddAutoMapper(typeof(MappingProfile));

// ---------- JWT Authentication ----------
var jwtCle = builder.Configuration["Jwt:Cle"]
    ?? throw new InvalidOperationException("JWT secret key is missing.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Emetteur"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtCle)),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// ---------- Controllers ----------
builder.Services.AddControllers();

// ---------- Swagger JWT Configuration ----------
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "StoreBackEnd API",
        Version = "v1",
        Description = "StoreBackEnd REST API with JWT authentication"
    });

    var xmlFile = $"{typeof(Program).Assembly.GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFile));

    // Define JWT Bearer authentication
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Description = "Enter your JWT token without the 'Bearer ' prefix.",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header
    });

    // Apply JWT authentication to Swagger operations
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

// ---------- Data Protection ----------
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/app/keys"))
    .SetApplicationName("StoreBackEnd");

var app = builder.Build();

// ---------- Database migrations + seed ----------
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationContext>();
    await context.Database.MigrateAsync();
    await DataSeeder.SeedAsync(context);
}

// ---------- Middleware pipeline ----------
app.UseMiddleware<ErrorHandlingMiddleware>();

// ---------- Swagger ----------
if (app.Environment.IsDevelopment() ||
    app.Environment.IsEnvironment("Docker"))
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "StoreBackEnd API v1");
        options.DocumentTitle = "StoreBackEnd API";
    });
}

// ---------- HTTPS ----------
if (!app.Environment.IsEnvironment("Docker"))
{
    app.UseHttpsRedirection();
}

// ---------- Authentication / Authorization ----------
app.UseAuthentication();
app.UseAuthorization();

// ---------- Controllers ----------
app.MapControllers();

app.Run();
