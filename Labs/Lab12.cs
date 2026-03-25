using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);

// --- ІНФРАСТРУКТУРА (Infrastructure) ---
builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=LayeredDemoDb;Trusted_Connection=True;"));

// Реєстрація залежностей (Dependency Injection)
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();
builder.Services.AddScoped<ICustomerService, CustomerService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Авто-створення БД
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

// ==========================================
// LAYER 1: DOMAIN (Сутності)
// ==========================================
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}

// ==========================================
// LAYER 2: INFRASTRUCTURE (Доступ до даних)
// ==========================================
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<Customer> Customers { get; set; } = null!;
}

public interface ICustomerRepository
{
    Task<Customer?> GetByIdAsync(int id);
    Task<List<Customer>> GetAllAsync();
    Task AddAsync(Customer customer);
    Task SaveChangesAsync();
}

public class CustomerRepository : ICustomerRepository
{
    private readonly AppDbContext _context;
    public CustomerRepository(AppDbContext context) => _context = context;

    public async Task<Customer?> GetByIdAsync(int id) => await _context.Customers.FindAsync(id);
    public async Task<List<Customer>> GetAllAsync() => await _context.Customers.ToListAsync();
    public async Task AddAsync(Customer customer) => await _context.Customers.AddAsync(customer);
    public async Task SaveChangesAsync() => await _context.SaveChangesAsync();
}

// ==========================================
// LAYER 3: APPLICATION (Сервіси та DTO)
// ==========================================
public record CustomerDto(int Id, string Name, string Email);
public record CreateCustomerDto(string Name, string Email);

public interface ICustomerService
{
    Task<List<CustomerDto>> GetCustomersAsync();
    Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto dto);
}

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _repository;
    public CustomerService(ICustomerRepository repository) => _repository = repository;

    public async Task<List<CustomerDto>> GetCustomersAsync()
    {
        var customers = await _repository.GetAllAsync();
        // Мапінг сутностей у DTO (замість AutoMapper для простоти коду)
        return customers.Select(c => new CustomerDto(c.Id, c.Name, c.Email)).ToList();
    }

    public async Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto dto)
    {
        var customer = new Customer { Name = dto.Name, Email = dto.Email };
        await _repository.AddAsync(customer);
        await _repository.SaveChangesAsync();
        return new CustomerDto(customer.Id, customer.Name, customer.Email);
    }
}

// ==========================================
// LAYER 4: PRESENTATION (Контролери / Endpoints)
// ==========================================
app.MapGet("/api/customers", async (ICustomerService service) => 
    Results.Ok(await service.GetCustomersAsync()));

app.MapPost("/api/customers", async (CreateCustomerDto dto, ICustomerService service) =>
{
    var result = await service.CreateCustomerAsync(dto);
    return Results.Created($"/api/customers/{result.Id}", result);
});

app.Run();
