using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseProject
{
    // ==========================================
    [cite_start]// 1. ДОМЕННА МОДЕЛЬ (3 сутності) [cite: 63]
    // ==========================================
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public List<Order> Orders { get; set; } = new(); // 1 -> * [cite: 75]
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; } // * -> 1 [cite: 82]
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }

    // ==========================================
    [cite_start]// 2. КОНТЕКСТ ДАНИХ [cite: 55, 64]
    // ==========================================
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            [cite_start]// Рядок підключення до SQL Server LocalDB [cite: 64]
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EnterpriseDb;Trusted_Connection=True;");
        }
    }

    // ==========================================
    [cite_start]// 3. РЕПОЗИТОРІЙ (Інкапсуляція логіки) [cite: 57, 92]
    // ==========================================
    public class OrderRepository
    {
        private readonly AppDbContext _db;
        public OrderRepository(AppDbContext db) => _db = db;

        public async Task AddOrderAsync(Order order)
        {
            await _db.Orders.AddAsync(order); // Added стан [cite: 23]
            await _db.SaveChangesAsync(); // SQL INSERT [cite: 94]
        }

        public async Task<List<Order>> GetOrdersWithDetailsAsync()
        {
            [cite_start]// Складний запит з Include (Eager Loading) [cite: 67, 96]
            return await _db.Orders
                .Include(o => o.User)
                .Include(o => o.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }
    }

    // ==========================================
    [cite_start]// 4. ДЕМОНСТРАЦІЙНИЙ СЦЕНАРІЙ [cite: 33, 66]
    // ==========================================
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            [cite_start]// Життєвий цикл: коротка операція [cite: 18, 33]
            using (var db = new AppDbContext())
            {
                Console.WriteLine("Синхронізація моделі з БД...");
                await db.Database.EnsureDeletedAsync();
                await db.Database.EnsureCreatedAsync(); // Замість міграцій для швидкості [cite: 11]

                // CREATE
                var user = new User { FullName = "Олексій Петренко" };
                var product = new Product { Name = "Ноутбук", Price = 35000 };
                var order = new Order { User = user, Product = product, OrderDate = DateTime.Now };

                var repo = new OrderRepository(db);
                await repo.AddOrderAsync(order);
                Console.WriteLine("Дані успішно збережено в SQL Server.");
            }

            [cite_start]// UPDATE з перевіркою Change Tracking [cite: 34, 41]
            using (var db = new AppDbContext())
            {
                var user = await db.Users.FirstAsync();
                Console.WriteLine($"\nПоточний стан: {db.Entry(user).State}"); // Unchanged [cite: 26]

                user.FullName = "Олексій В. Петренко"; // Зміна в коді
                Console.WriteLine($"Стан після зміни поля: {db.Entry(user).State}"); // Modified [cite: 24]

                await db.SaveChangesAsync();
                Console.WriteLine("FullName оновлено в БД через SaveChanges().");
            }

            [cite_start]// READ (Складний запит) [cite: 67]
            using (var db = new AppDbContext())
            {
                var repo = new OrderRepository(db);
                var details = await repo.GetOrdersWithDetailsAsync();

                Console.WriteLine("\n--- Звіт по замовленнях (Include) ---");
                foreach (var o in details)
                {
                    Console.WriteLine($"Дата: {o.OrderDate} | Клієнт: {o.User?.FullName} | Товар: {o.Product?.Name}");
                }
            }
        }
    }
}
