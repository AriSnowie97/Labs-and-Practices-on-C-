using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace LibraryProject
{
    // --- МОДЕЛІ (Entities) ---
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        [cite_start]// Нове поле згідно з завданням про міграції 
        public string Country { get; set; } = "Ukraine";
        public List<Book> Books { get; set; } = new();
    }

    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int AuthorId { get; set; }
        public Author? Author { get; set; }
    }

    // --- КОНТЕКСТ (DbContext) ---
    public class AppDbContext : DbContext
    {
        public DbSet<Author> Authors { get; set; } = null!;
        public DbSet<Book> Books { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Рядок підключення до LocalDB
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=LibraryPractice10;Trusted_Connection=True;");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            [cite_start]// Демонстрація життєвого циклу: створення контексту для конкретної операції 
            using (var db = new AppDbContext())
            {
                Console.WriteLine("Оновлення бази даних...");
                db.Database.EnsureDeleted(); // Тільки для тестів!
                db.Database.EnsureCreated(); // Створює БД на основі поточної моделі [cite: 32]

                // --- CREATE ---
                var author = new Author { Name = "Іван Франко", Country = "Україна" };
                author.Books.Add(new Book { Title = "Захар Беркут", Price = 250 });
                db.Authors.Add(author);
                db.SaveChanges(); // Тут Added перетворюється на INSERT [cite: 23]
                Console.WriteLine("Дані додано.");
            }

            [cite_start]// --- UPDATE (Демонстрація Change Tracking)  ---
            using (var db = new AppDbContext())
            {
                var book = db.Books.First(b => b.Title == "Захар Беркут");

                Console.WriteLine($"\nСтан до зміни: {db.Entry(book).State}"); // Unchanged [cite: 26]

                book.Price = 300; // Змінюємо властивість об'єкта

                Console.WriteLine($"Стан після зміни: {db.Entry(book).State}"); // Modified [cite: 24]

                db.SaveChanges(); // Виконує UPDATE [cite: 24]
                Console.WriteLine("Ціну оновлено через Change Tracking.");
            }

            // --- DELETE ---
            using (var db = new AppDbContext())
            {
                var author = db.Authors.Include(a => a.Books).First();
                db.Authors.Remove(author); // Стан стає Deleted [cite: 25]
                db.SaveChanges(); // Виконує DELETE
                Console.WriteLine("\nАвтора та його книги видалено.");
            }

            Console.WriteLine("\nСценарій завершено успішно.");
        }
    }
}
