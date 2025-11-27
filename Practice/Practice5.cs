using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;      // Для Thread, Monitor, Semaphore
using System.Threading.Tasks; // Для Task

// ===== 1. Абстрактний базовий клас =====
public abstract class Vehicle : ICloneable, IComparable<Vehicle>
{
    public int Id { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }

    public int CompareTo(Vehicle other)
    {
        if (other == null) return 1;
        int yearComparison = this.Year.CompareTo(other.Year);
        if (yearComparison != 0) return yearComparison;
        return string.Compare(this.Make, other.Make, StringComparison.Ordinal);
    }

    public virtual object Clone() => this.MemberwiseClone();

    public abstract void DisplayInfo();

    public virtual void StartEngine()
    {
        Console.WriteLine($"[Thread {Thread.CurrentThread.ManagedThreadId}] ({Make} {Model}) Двигун запущено.");
    }
}

// ===== 2. Класи-нащадки (Скорочено для економії місця, логіка та ж) =====
public class Car : Vehicle
{
    public override void DisplayInfo() => Console.WriteLine($"[Car] {Id} | {Make} {Model}");
}

public class Truck : Vehicle
{
    public double CargoCapacity { get; set; }
    public override void DisplayInfo() => Console.WriteLine($"[Truck] {Id} | {Make} {Model}");
}

public class Bus : Vehicle
{
    public int PassengerCapacity { get; set; }
    public override void DisplayInfo() => Console.WriteLine($"[Bus] {Id} | {Make} {Model}");
}

// ===== 3. Клас із СЕМАФОРОМ: Заправна станція =====
// Демонструє обмеження доступу до ресурсу (обмежена кількість колонок)
public class FuelStation
{
    // SemaphoreSlim - легковага версія семафора. 
    // initialCount: скільки потоків можуть зайти одразу (кількість колонок).
    private SemaphoreSlim _pumps;
    private int _pumpCount;

    public FuelStation(int pumps)
    {
        _pumpCount = pumps;
        _pumps = new SemaphoreSlim(pumps, pumps);
    }

    // Метод, який виконується в окремому потоці
    public void Refuel(Vehicle vehicle)
    {
        Console.WriteLine($"[Заправка] {vehicle.Make} {vehicle.Model} під'їхав до черги.");

        // Спроба зайняти слот (колонку). Якщо всі зайняті - потік чекає тут.
        _pumps.Wait();

        try
        {
            Console.WriteLine($"--> [Заправка] {vehicle.Make} {vehicle.Model} ЗАПРАВЛЯЄТЬСЯ (Потік {Thread.CurrentThread.ManagedThreadId})...");

            // Імітація тривалої роботи (заправка триває 1-3 секунди)
            Thread.Sleep(new Random().Next(1000, 3000));

            Console.WriteLine($"<-- [Заправка] {vehicle.Make} {vehicle.Model} заправився і поїхав.");
        }
        finally
        {
            // Обов'язково звільняємо слот для інших машин
            _pumps.Release();
        }
    }
}

// ===== 4. Потокобезпечний Сервіс (MONITOR / LOCK) =====
public class AutoParkService
{
    private List<Vehicle> _vehicles = new List<Vehicle>();

    // Об'єкт для синхронізації (замок)
    private readonly object _syncRoot = new object();

    // Додавання авто (Пишемо дані)
    public void AddVehicle(Vehicle vehicle)
    {
        // lock гарантує, що тільки один потік може виконувати цей блок коду одночасно.
        // Це запобігає пошкодженню списку _vehicles при одночасному записі.
        lock (_syncRoot)
        {
            _vehicles.Add(vehicle);
            Console.WriteLine($"[Service] + Додано {vehicle.Make} (Всього: {_vehicles.Count})");
        }
    }

    // Отримання копії списку (Читаємо дані)
    // Важливо блокувати і читання, якщо в цей момент хтось може писати, 
    // інакше отримаємо помилку "Collection was modified".
    public List<Vehicle> GetAllVehiclesSnapshot()
    {
        lock (_syncRoot)
        {
            // Повертаємо нову копію списку, щоб робота з нею зовні не блокувала сервіс
            return new List<Vehicle>(_vehicles);
        }
    }

    // Метод для безпечного видалення
    public void RemoveVehicle(int id)
    {
        lock (_syncRoot)
        {
            var v = _vehicles.FirstOrDefault(x => x.Id == id);
            if (v != null)
            {
                _vehicles.Remove(v);
                Console.WriteLine($"[Service] - Видалено {v.Make}");
            }
        }
    }
}

// ===== 5. Головна програма (Multithreading Demo) =====
class Program
{
    static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=== Лабораторна 5: Багатопоточність (Task, Lock, Semaphore) ===\n");

        // 1. Створюємо сервіс та станцію
        AutoParkService parkService = new AutoParkService();
        // Заправка має лише 2 колонки!
        FuelStation gasStation = new FuelStation(2);

        // 2. Створюємо список задач (Tasks)
        List<Task> tasks = new List<Task>();

        // --- ЗАДАЧА 1: Потік-Генератор (Додає машини) ---
        // Використовуємо Task.Run для запуску в фоновому потоці
        Task producerTask = Task.Run(() =>
        {
            Console.WriteLine(">>> Запущено потік додавання авто...");
            for (int i = 1; i <= 5; i++)
            {
                parkService.AddVehicle(new Car { Id = i, Make = $"Auto-{i}", Model = "Model X", Year = 2020 + i });
                Thread.Sleep(500); // Імітація затримки надходження
            }
            Console.WriteLine(">>> Потік додавання завершено.");
        });
        tasks.Add(producerTask);

        // --- ЗАДАЧА 2: Потік-Аналітик (Читає дані паралельно) ---
        Task consumerTask = Task.Run(() =>
        {
            Console.WriteLine(">>> Запущено потік статистики...");
            for (int i = 0; i < 5; i++)
            {
                // Тут спрацює lock у методі GetAllVehiclesSnapshot, 
                // щоб ми не читали список, поки туди пише producerTask
                var currentList = parkService.GetAllVehiclesSnapshot();
                Console.WriteLine($"    [Статистика] Зараз у парку: {currentList.Count} машин.");
                Thread.Sleep(700);
            }
        });
        tasks.Add(consumerTask);

        // Чекаємо завершення роботи з наповнення парку
        await Task.WhenAll(producerTask, consumerTask);

        Console.WriteLine("\n------------------------------------------------");
        Console.WriteLine("Етап 2: Масовий виїзд на заправку (Семафор)");
        Console.WriteLine("------------------------------------------------\n");

        // Отримуємо всі машини
        var fleet = parkService.GetAllVehiclesSnapshot();
        List<Task> refuelingTasks = new List<Task>();

        // --- ЗАДАЧА 3: Симуляція черги на заправку ---
        // Запускаємо для кожної машини окрему задачу "Поїхати заправитись"
        foreach (var vehicle in fleet)
        {
            // Створюємо Task, але не чекаємо його завершення відразу
            Task t = Task.Run(() =>
            {
                // Кожен потік намагатиметься зайти в семафор всередині Refuel
                gasStation.Refuel(vehicle);
            });
            refuelingTasks.Add(t);
        }

        // Чекаємо, поки всі машини заправляться
        await Task.WhenAll(refuelingTasks);

        Console.WriteLine("\n=== Роботу програми завершено ===");
    }
}
