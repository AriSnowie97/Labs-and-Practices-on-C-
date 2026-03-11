using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using Microsoft.EntityFrameworkCore.Sqlite;

namespace LibraryProject
{
    // ==========================================
    // 1. МОДЕЛІ (Entities)
    // ==========================================
    public class Author
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

        // Зв'язок "один до багатьох"
        public List<Book> Books { get; set; } = new();
    }

    public class Book
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public decimal Price { get; set; }

        // Зовнішній ключ та навігаційна властивість
        public int AuthorId { get; set; }
        public Author? Author { get; set; }
    }

    // ==========================================
    // 2. КОНТЕКСТ БАЗИ ДАНИХ (DbContext)
    // ==========================================
    public class LibraryContext : DbContext
    {
        public DbSet<Author> Authors { get; set; } = null!;
        public DbSet<Book> Books { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=LibraryDemo.db");
        }
    }

    // ==========================================
    // 3. ОСНОВНА ПРОГРАМА (Сценарій CRUD)              
    // ==========================================
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            using var db = new LibraryContext();

            // Підготовка бази (видаляємо стару і створюємо нову для демонстрації)
            Console.WriteLine("Підготовка бази даних...");
            db.Database.EnsureDeleted();
            db.Database.EnsureCreated();

            // --- CREATE (Створення) ---
            Console.WriteLine("\n[CREATE] Додаємо автора та книги...");
            var author = new Author { Name = "Іван Франко" };
            author.Books.Add(new Book { Title = "Захар Беркут", Price = 200 });
            author.Books.Add(new Book { Title = "Лис Микита", Price = 150 });

            db.Authors.Add(author);
            db.SaveChanges();
            Console.WriteLine("Дані успішно додано.");

            // --- READ (Читання) ---
            PrintAllBooks(db, "READ (Початкові дані)");

            // --- UPDATE (Оновлення) ---
            Console.WriteLine("\n[UPDATE] Оновлюємо ціну для 'Захар Беркут'...");
            var bookToUpdate = db.Books.FirstOrDefault(b => b.Title == "Захар Беркут");
            if (bookToUpdate != null)
            {
                bookToUpdate.Price = 275.50m;
                db.SaveChanges();
                Console.WriteLine("Ціну змінено.");
            }

            // --- DELETE (Видалення) ---
            Console.WriteLine("\n[DELETE] Видаляємо книгу 'Лис Микита'...");
            var bookToDelete = db.Books.FirstOrDefault(b => b.Title == "Лис Микита");
            if (bookToDelete != null)
            {
                db.Books.Remove(bookToDelete);
                db.SaveChanges();
                Console.WriteLine("Книгу видалено.");
            }

            // ПЕРЕВІРКА РЕЗУЛЬТАТУ
            PrintAllBooks(db, "FINAL STATE (Після оновлення та видалення)");

            Console.WriteLine("\nРоботу завершено. Натисніть будь-яку клавішу...");
            Console.ReadKey();
        }

        static void PrintAllBooks(LibraryContext db, string message)
        {
            Console.WriteLine($"\n--- {message} ---");
            // Використовуємо Include для завантаження пов'язаних даних автора (Eager Loading)
            var books = db.Books.Include(b => b.Author).ToList();

            if (!books.Any())
                Console.WriteLine("Список порожній.");

            foreach (var b in books)
            {
                Console.WriteLine($"- ID: {b.Id} | Книга: {b.Title} | Автор: {b.Author?.Name} | Ціна: {b.Price} грн");
            }
        }
    }
}
