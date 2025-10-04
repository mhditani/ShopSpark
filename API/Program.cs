using Entity.Data;
using Entity.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Services.IRepositories;
using Services.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add DB
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Services.Map_Classes.MapperClass));

// Add Repositories
builder.Services.AddScoped<IProductRepo, ProductRepo>();
builder.Services.AddScoped<IOrderRepo, OrderRepo>();

// Add Authentication
// Fix Authentication - Use this exact setup
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.Authority = "https://localhost:5001";
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateAudience = false,
            ValidateIssuer = true,
            ValidIssuer = "https://localhost:5001",
            // Add these to temporarily bypass signature validation
            ValidateLifetime = true,
            ValidateIssuerSigningKey = false, // ? Temporarily disable
            RequireExpirationTime = true,
            RequireSignedTokens = true // ? Temporarily disable
        };

        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Authentication failed: {context.Exception.Message}");
                return Task.CompletedTask;
            }
        };
    });

// Add Authorization with simple policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ReadProducts", policy => policy.RequireClaim("scope", "products.read"));
    options.AddPolicy("ManageProducts", policy => policy.RequireClaim("scope", "products.write", "products.delete"));
    options.AddPolicy("ReadOrders", policy => policy.RequireClaim("scope", "orders.read"));
    options.AddPolicy("ManageOrders", policy => policy.RequireClaim("scope", "orders.write"));
    options.AddPolicy("ReadCustomers", policy => policy.RequireClaim("scope", "customers.read"));
    options.AddPolicy("ManageCustomers", policy => policy.RequireClaim("scope", "customers.manage"));
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/api/test", () => "API is working!");

app.Run();