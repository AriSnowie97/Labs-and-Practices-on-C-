using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;      // Для CancellationToken
using System.Threading.Tasks; // Для Task, async, await

// ===== 1. Абстрактний базовий клас (без змін логіки) =====
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

    // АСИНХРОННИЙ метод запуску двигуна (симуляція прогріву)
    public virtual async Task StartEngineAsync()
    {
        Console.WriteLine($"[{Make} {Model}] Початок прогріву двигуна...");
        await Task.Delay(500); // Імітація роботи (500 мс)
        Console.WriteLine($"[{Make} {Model}] Двигун готовий до роботи!");
    }
}

// ===== 2. Класи-нащадки =====
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

// ===== 3. Станція Заправки (Async + Cancellation) =====
public class FuelStation
{
    private SemaphoreSlim _pumps;

    public FuelStation(int pumps)
    {
        _pumps = new SemaphoreSlim(pumps, pumps);
    }

    // Метод приймає CancellationToken для можливості переривання
    public async Task RefuelAsync(Vehicle vehicle, CancellationToken token, IProgress<string> progress = null)
    {
        // Повідомляємо про прибуття
        progress?.Report($"[Заправка] {vehicle.Make} прибув у чергу.");

        // Чекаємо на вільну колонку (з підтримкою скасування)
        await _pumps.WaitAsync(token);

        try
        {
            // Якщо токен скасовано ДО початку роботи, кидаємо виключення
            token.ThrowIfCancellationRequested();

            progress?.Report($"--> [Заправка] {vehicle.Make} ПОЧАВ заправку...");

            // Імітація тривалої заправки (2 секунди)
            // Task.Delay також приймає токен, щоб перервати "сон" миттєво
            await Task.Delay(2000, token);

            progress?.Report($"<-- [Заправка] {vehicle.Make} ЗАВЕРШИВ заправку.");
        }
        finally
        {
            _pumps.Release();
        }
    }
}

// ===== 4. Сервіс-центр (Симуляція техогляду) =====
public class ServiceCenter
{
    // Демонстрація повернення значення з асинхронного методу (Task<bool>)
    public async Task<bool> InspectVehicleAsync(Vehicle v)
    {
        Console.WriteLine($"[СТО] Початок огляду {v.Make}...");
        await Task.Delay(1000); // Огляд триває 1 сек

        // Випадковий результат: пройдено або ні
        bool passed = new Random().Next(0, 10) > 2;

        Console.WriteLine($"[СТО] Результат для {v.Make}: {(passed ? "УСПІХ" : "ПОЛОМКА")}");
        return passed;
    }
}

// ===== 5. Потокобезпечний Сервіс Автопарку =====
public class AutoParkService
{
    private List<Vehicle> _vehicles = new List<Vehicle>();
    private readonly object _syncRoot = new object();

    public void AddVehicle(Vehicle vehicle)
    {
        lock (_syncRoot)
        {
            _vehicles.Add(vehicle);
        }
    }

    public List<Vehicle> GetAllVehiclesSnapshot()
    {
        lock (_syncRoot)
        {
            return new List<Vehicle>(_vehicles);
        }
    }
}

// ===== 6. Головна програма (Async/Await Demo) =====
class Program
{
    static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=== Лабораторна 6: Async/Await + Cancellation ===\n");

        // 1. Підготовка
        AutoParkService park = new AutoParkService();
        FuelStation station = new FuelStation(2); // 2 колонки
        ServiceCenter serviceCenter = new ServiceCenter();

        // Заповнюємо парк
        park.AddVehicle(new Car { Id = 1, Make = "Toyota", Model = "Camry", Year = 2020 });
        park.AddVehicle(new Truck { Id = 2, Make = "Volvo", Model = "FH", Year = 2018 });
        park.AddVehicle(new Bus { Id = 3, Make = "Mercedes", Model = "Sprinter", Year = 2021 });
        park.AddVehicle(new Car { Id = 4, Make = "Ford", Model = "Focus", Year = 2015 });
        park.AddVehicle(new Car { Id = 5, Make = "Tesla", Model = "Model 3", Year = 2023 });

        var fleet = park.GetAllVehiclesSnapshot();

        // -------------------------------------------------------------
        // Сценарій 1: Масовий асинхронний запуск двигунів
        // -------------------------------------------------------------
        Console.WriteLine("--- Етап 1: Прогрів двигунів (Task.WhenAll) ---");

        // Створюємо список задач (але не чекаємо їх тут)
        var startTasks = fleet.Select(v => v.StartEngineAsync());

        // Чекаємо завершення ВСІХ задач прогріву
        await Task.WhenAll(startTasks);
        Console.WriteLine(">>> Всі двигуни прогріто!\n");

        // -------------------------------------------------------------
        // Сценарій 2: Асинхронний техогляд (Task.WhenAny)
        // -------------------------------------------------------------
        Console.WriteLine("--- Етап 2: Техогляд (Хто перший?) ---");

        List<Task<bool>> inspectionTasks = new List<Task<bool>>();
        foreach (var v in fleet.Take(3)) // Беремо перші 3 машини
        {
            inspectionTasks.Add(serviceCenter.InspectVehicleAsync(v));
        }

        // Чекаємо, поки завершиться ХОЧА Б ОДНА задача
        Task<bool> firstFinished = await Task.WhenAny(inspectionTasks);
        bool result = await firstFinished;
        Console.WriteLine($">>> Перша машина завершила огляд. Результат: {result}\n");

        // (Для коректності чекаємо решту, щоб не "смітити" в консоль пізніше)
        await Task.WhenAll(inspectionTasks);


        // -------------------------------------------------------------
        // Сценарій 3: Заправка зі скасуванням (CancellationToken)
        // -------------------------------------------------------------
        Console.WriteLine("\n--- Етап 3: Заправка зі скасуванням (Натисніть 'C' для STOP) ---");

        // Створюємо джерело токена скасування
        CancellationTokenSource cts = new CancellationTokenSource();

        // Створюємо обробник прогресу (виводитиме повідомлення в консоль)
        IProgress<string> progressReporter = new Progress<string>(msg =>
        {
            Console.WriteLine($"[INFO] {msg}");
        });

        // Запускаємо задачу "Слухач клавіатури" для скасування
        Task inputTask = Task.Run(() =>
        {
            Console.WriteLine(">>> Натисніть 'C' протягом 3 секунд, щоб скасувати заправку...");
            // Чекаємо 1.5 секунди перед тим як користувач може натиснути (для демо)
            // або просто чекаємо натискання
            while (!cts.Token.IsCancellationRequested)
            {
                if (Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.C)
                {
                    Console.WriteLine("\n!!! ОТРИМАНО КОМАНДУ СКАСУВАННЯ !!!");
                    cts.Cancel(); // Відправляємо сигнал скасування
                    break;
                }
            }
        });

        List<Task> refuelingTasks = new List<Task>();
        foreach (var v in fleet)
        {
            // Передаємо токен і репортер у метод
            refuelingTasks.Add(station.RefuelAsync(v, cts.Token, progressReporter));
        }

        try
        {
            // Чекаємо завершення або заправки, або натискання кнопки
            await Task.WhenAll(refuelingTasks);
            Console.WriteLine("\n>>> Усі машини успішно заправлено.");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("\n>>> ОПЕРАЦІЮ ПЕРЕРВАНО: Парк евакуйовано!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n>>> Помилка: {ex.Message}");
        }
        finally
        {
            cts.Dispose();
        }

        Console.WriteLine("\n=== Програма завершила роботу ===");
    }
}
