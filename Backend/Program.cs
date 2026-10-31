
using Backend;
using Backend.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
// Configure SMTP settings
var smtpSection = builder.Configuration.GetSection("Smtp");
builder.Services.AddSingleton(new EmailService(
    smtpSection["Server"] ?? "smtp.example.com",
    int.TryParse(smtpSection["Port"], out var port) ? port : 587,
    smtpSection["User"] ?? "user@example.com",
    smtpSection["Pass"] ?? "password",
    smtpSection["From"] ?? "noreply@example.com"
));

// Add services to the container.
var jwtKey = builder.Configuration["Jwt:Key"] ?? "ThisIsASecretKeyForJwtTokenThatIsLongEnough";
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? "FlightApp";
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
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngular", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo { Title = "Flight API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'"
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<Backend.Repositories.IFlightRepository, Backend.Repositories.FlightRepository>();
builder.Services.AddScoped<Backend.Repositories.IUserRepository, Backend.Repositories.UserRepository>();
builder.Services.AddScoped<Backend.Repositories.IBookingRepository, Backend.Repositories.BookingRepository>();
builder.Services.AddScoped<Backend.Repositories.IPassengerRepository, Backend.Repositories.PassengerRepository>();
builder.Services.AddScoped<PaymentService>();

var app = builder.Build();

// Seed the database (e.g., admin user)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await Backend.Services.DataSeeder.SeedAsync(db);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAngular");

app.UseAuthentication();
app.UseAuthorization();


// Map controllers
app.MapControllers();

await app.RunAsync();
