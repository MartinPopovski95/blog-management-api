using BlogManagementApi.Domain.Models;
using BlogManagementApi.ExceptionHandlers;
using BlogManagementApi.Helpers.DependencyInjectionHelper;
using BlogManagementApi.Helpers.ServiceCollectionHelper;
using BlogManagementApi.Mappers;
using Microsoft.AspNetCore.Identity;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddOpenApi();
builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();
builder.Services.AddAutoMapper(cfg => { }, typeof(AutoMapperProfile).Assembly)
                .AddBlogDbContext(builder.Configuration)
                .AddRateLimiter()
                .AddIdentity()
                .AddAuthentication()
                .AddJwt(builder.Configuration)
                .AddCors(builder.Configuration)
                .AddSwagger();

builder.Services.RegisterRepositories();
builder.Services.RegisterServices();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
    var adminEmail = app.Configuration["AdminSeed:Email"]!;
    var adminPassword = app.Configuration["AdminSeed:Password"]!;

    if (!string.IsNullOrWhiteSpace(adminEmail) && !string.IsNullOrWhiteSpace(adminPassword)
        && await userManager.FindByEmailAsync(adminEmail) is null)
    {
        var admin = new User
        {
            FirstName = "Admin",
            LastName = "User",
            Email = adminEmail,
            UserName = adminEmail
        };
        await userManager.CreateAsync(admin, adminPassword);
        await userManager.AddToRoleAsync(admin, "Admin");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseExceptionHandler();
app.UseCors("CORSPolicy");
app.UseRateLimiter();
app.UseStaticFiles();
app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
