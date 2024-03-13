using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RapidPay.Services;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(option =>
    {
        option.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = builder.Configuration["JwtOptions:Issuer"],
            ValidAudience = builder.Configuration["JwtOptions:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtOptions:SigningKey"]))
        };
    });

Log.Logger = new LoggerConfiguration()
            .WriteTo.File("../RapidPay/Logfile/logs.log")
            .CreateLogger();
Log.Information("Starting RapidPay API");

builder.Services.AddControllers();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<ApplicationDbContext>(options => {
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
});

builder.Services.AddSingleton<UniversalFeesExchange>();
builder.Services.AddHostedService<FeeUpdateService>();
builder.Services.AddSwaggerGen();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddLogging(loggingBuilder =>
{
    loggingBuilder.ClearProviders(); // Clear default providers
    loggingBuilder.AddSerilog(dispose: true); // Add Serilog
});

Log.Information("Services Registered");

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
   
}
app.UseAuthentication();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.UseSwagger();
    app.UseSwaggerUI(options => {
        options.SwaggerEndpoint("swagger/v1/swagger.json" , "v1");
        options.RoutePrefix = string.Empty;
        options.DocumentTitle = "Rapid Pay API";
    });
app.MapControllers();
Log.CloseAndFlush();
app.Run();
