using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);

// 1. НАСТРОЙКА КОНТЕКСТА (Практическая №10)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EnterpriseWebDb;Trusted_Connection=True;"));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IUserRepository, UserRepository>(); // Внедрение зависимостей (DI)

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Автоматическое создание БД для тестов
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// ==========================================
// 2. МОДЕЛИ И DTO (Лабораторная №11)
// ==========================================

// Доменная сущность
public class User
{
    public int Id { get; set; }
    [Required] public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

// DTO для передачи данных (безопасность и валидация)
public record UserDto(string FullName, string Email);

// ==========================================
// 3. РЕПОЗИТОРИЙ (Лабораторная №10)
// ==========================================
public interface IUserRepository
{
    Task<IEnumerable<User>> GetAllAsync();
    Task<User?> GetByIdAsync(int id);
    Task AddAsync(User user);
    Task UpdateAsync(User user);
    Task DeleteAsync(int id);
}

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _db;
    public UserRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<User>> GetAllAsync() => await _db.Users.ToListAsync();
    
    public async Task<User?> GetByIdAsync(int id) => await _db.Users.FindAsync(id);

    public async Task AddAsync(User user)
    {
        await _db.Users.AddAsync(user);
        await _db.SaveChangesAsync(); // Change Tracking в действии
    }

    public async Task UpdateAsync(User user)
    {
        _db.Users.Update(user);
        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var user = await _db.Users.FindAsync(id);
        if (user != null)
        {
            _db.Users.Remove(user);
            await _db.SaveChangesAsync();
        }
    }
}

// ==========================================
// 4. КОНТЕКСТ ДАННЫХ
// ==========================================
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<User> Users { get; set; } = null!;
}

// ==========================================
// 5. API ENDPOINTS (Контроллеры/Маршруты)
// ==========================================

app.MapGet("/api/users", async (IUserRepository repo) => 
    Results.Ok(await repo.GetAllAsync()));

app.MapGet("/api/users/{id}", async (int id, IUserRepository repo) =>
{
    var user = await repo.GetByIdAsync(id);
    return user is not null ? Results.Ok(user) : Results.NotFound();
});

app.MapPost("/api/users", async (UserDto dto, IUserRepository repo) =>
{
    var user = new User { FullName = dto.FullName, Email = dto.Email };
    await repo.AddAsync(user);
    return Results.Created($"/api/users/{user.Id}", user); // 201 Created
});

app.MapPut("/api/users/{id}", async (int id, UserDto dto, IUserRepository repo) =>
{
    var user = await repo.GetByIdAsync(id);
    if (user is null) return Results.NotFound();

    user.FullName = dto.FullName;
    user.Email = dto.Email;
    
    await repo.UpdateAsync(user);
    return Results.NoContent();
});

app.MapDelete("/api/users/{id}", async (int id, IUserRepository repo) =>
{
    await repo.DeleteAsync(id);
    return Results.Ok();
});

app.Run();
