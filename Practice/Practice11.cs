using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EnterpriseProject
{
    // ==========================================
    // 1. ДОМЕННЫЕ СУЩНОСТИ (Домен: Магазин)
    // ==========================================
    public class User
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty; // Новое поле для задания по миграциям
        public List<Order> Orders { get; set; } = new(); // Связь 1 -> * [cite: 214]
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
        public User? User { get; set; } // Связь * -> 1 [cite: 221]
        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }

    // ==========================================
    // 2. КОНТЕКСТ ДАННЫХ (DbContext)
    // ==========================================
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<Product> Products { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            [cite_start]// Строка подключения к LocalDB [cite: 203]
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=EnterpriseDb;Trusted_Connection=True;");
        }
    }

    // ==========================================
    [cite_start]// 3. РЕПОЗИТОРИЙ (Слой доступа к данным) [cite: 195, 198]
    // ==========================================
    public class OrderRepository
    {
        private readonly AppDbContext _db;
        public OrderRepository(AppDbContext db) => _db = db;

        public async Task AddOrderAsync(Order order)
        {
            await _db.Orders.AddAsync(order);
            await _db.SaveChangesAsync(); // SQL INSERT [cite: 162]
        }

        public async Task<List<Order>> GetAllWithDetailsAsync()
        {
            [cite_start]// Сложный запрос с Include (Eager Loading) 
            return await _db.Orders
                .Include(o => o.User)
                .Include(o => o.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }
    }

    // ==========================================
    // 4. ДЕМОНСТРАЦИОННЫЙ СЦЕНАРИЙ
    // ==========================================
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            [cite_start]// Сценарий 1: Создание базы и данных (Жизненный цикл "одна операция") [cite: 157, 172]
            using (var db = new AppDbContext())
            {
                Console.WriteLine("--- ШАГ 1: Инициализация БД ---");
                await db.Database.EnsureDeletedAsync();
                await db.Database.EnsureCreatedAsync();

                var user = new User { FullName = "Иван Иванов", Email = "ivan@test.com" };
                var product = new Product { Name = "Смартфон", Price = 15000 };
                var order = new Order { User = user, Product = product, OrderDate = DateTime.Now };

                var repo = new OrderRepository(db);
                await repo.AddOrderAsync(order);
                Console.WriteLine("Данные успешно сохранены в SQL Server.");
            }

            [cite_start]// Сценарий 2: Демонстрация Change Tracking (Обновление) 
            using (var db = new AppDbContext())
            {
                Console.WriteLine("\n--- ШАГ 2: Демонстрация Change Tracking ---");
                var user = await db.Users.FirstAsync();
                
                Console.WriteLine($"Состояние до изменения: {db.Entry(user).State}"); // Unchanged [cite: 165]
                
                user.FullName = "Иван Сергеевич Иванов"; // Изменяем свойство
                
                Console.WriteLine($"Состояние после изменения: {db.Entry(user).State}"); // Modified [cite: 163]
                
                await db.SaveChangesAsync(); // SQL UPDATE
                Console.WriteLine("Изменения зафиксированы в БД.");
            }

            [cite_start]// Сценарий 3: Чтение со связями (Include) [cite: 206]
            using (var db = new AppDbContext())
            {
                Console.WriteLine("\n--- ШАГ 3: Чтение данных (Include) ---");
                var repo = new OrderRepository(db);
                var orders = await repo.GetAllWithDetailsAsync();

                foreach (var o in orders)
                {
                    Console.WriteLine($"Заказ #{o.Id} | Клиент: {o.User?.FullName} | Товар: {o.Product?.Name} | Цена: {o.Product?.Price}");
                }
            }
        }
    }
}
