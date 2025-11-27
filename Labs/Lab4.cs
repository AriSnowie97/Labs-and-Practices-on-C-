using System;
using System.Collections.Generic;
using System.Linq; // Необхідно для LINQ

// ===== 1. Абстрактний базовий клас (ICloneable, IComparable) =====
public abstract class Vehicle : ICloneable, IComparable<Vehicle>
{
    public int Id { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public int Year { get; set; }

    // Реалізація сортування за замовчуванням (Рік -> Марка)
    public int CompareTo(Vehicle other)
    {
        if (other == null) return 1;
        int yearComparison = this.Year.CompareTo(other.Year);
        if (yearComparison != 0) return yearComparison;
        return string.Compare(this.Make, other.Make, StringComparison.Ordinal);
    }

    // Реалізація клонування
    public virtual object Clone()
    {
        return this.MemberwiseClone();
    }

    public abstract void DisplayInfo();

    public virtual void StartEngine()
    {
        Console.WriteLine($"({Make} {Model}) Базовий запуск двигуна.");
    }

    public virtual void Refuel(double liters)
    {
        if (liters <= 0) throw new ArgumentException("Паливо має бути > 0");
        Console.WriteLine($"({Make} {Model}) Заправлено {liters} л.");
    }
}

// ===== 2. Класи-нащадки =====

public class Car : Vehicle
{
    public int NumberOfDoors { get; set; }
    public string BodyType { get; set; }

    public override void DisplayInfo()
    {
        Console.WriteLine($"[Car] {Id} | {Make} {Model} ({Year}) | {BodyType}");
    }
}

public class Truck : Vehicle
{
    public double CargoCapacity { get; set; }
    public double CurrentCargo { get; private set; }

    public override void DisplayInfo()
    {
        Console.WriteLine($"[Truck] {Id} | {Make} {Model} ({Year}) | Вантаж: {CurrentCargo}/{CargoCapacity}т");
    }

    public void LoadCargo(double tons)
    {
        if (CurrentCargo + tons > CargoCapacity) throw new Exception("Перевантаження!");
        CurrentCargo += tons;
    }
}

public class Bus : Vehicle
{
    public int PassengerCapacity { get; set; }

    public override void DisplayInfo()
    {
        Console.WriteLine($"[Bus] {Id} | {Make} {Model} ({Year}) | Місць: {PassengerCapacity}");
    }
}

public class Motorcycle : Vehicle
{
    public string BikeType { get; set; }

    public override void DisplayInfo()
    {
        Console.WriteLine($"[Moto] {Id} | {Make} {Model} ({Year}) | Тип: {BikeType}");
    }
}

// ===== 3. Допоміжні класи =====

public class VehicleEventArgs : EventArgs
{
    public Vehicle Vehicle { get; }
    public string Message { get; }
    public VehicleEventArgs(Vehicle v, string m) { Vehicle = v; Message = m; }
}

// ===== 4. Сервіс-клас (ОНОВЛЕНО: Iterator + LINQ Support) =====
public class AutoParkService
{
    private List<Vehicle> _vehicles = new List<Vehicle>();

    // Відкриваємо колекцію для LINQ (IEnumerable)
    public IEnumerable<Vehicle> Vehicles => _vehicles;

    public event EventHandler<VehicleEventArgs> VehicleAdded;

    public void AddVehicle(Vehicle vehicle)
    {
        _vehicles.Add(vehicle);
        VehicleAdded?.Invoke(this, new VehicleEventArgs(vehicle, "Додано"));
    }

    // === НОВЕ: Власний ітератор з використанням yield ===
    // Дозволяє перебирати елементи, що відповідають умові, не створюючи проміжний список.
    public IEnumerable<Vehicle> GetVehiclesByCondition(Func<Vehicle, bool> predicate)
    {
        foreach (var v in _vehicles)
        {
            // Перевіряємо умову
            if (predicate(v))
            {
                // Повертаємо елемент і призупиняємо виконання до наступного кроку
                yield return v;
            }
        }
    }
}

// ===== 5. Головна програма (DEMO) =====
class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=== Практична 4: Ітератори (yield) та LINQ ===\n");

        AutoParkService autoPark = new AutoParkService();

        // Заповнюємо даними
        autoPark.AddVehicle(new Car { Id = 1, Make = "Toyota", Model = "Camry", Year = 2021, BodyType = "Sedan" });
        autoPark.AddVehicle(new Car { Id = 2, Make = "Ford", Model = "Focus", Year = 2015, BodyType = "Hatchback" });
        autoPark.AddVehicle(new Truck { Id = 3, Make = "Volvo", Model = "FH16", Year = 2018, CargoCapacity = 20 });
        autoPark.AddVehicle(new Truck { Id = 4, Make = "MAN", Model = "TGX", Year = 2022, CargoCapacity = 25 });
        autoPark.AddVehicle(new Bus { Id = 5, Make = "Mercedes", Model = "Sprinter", Year = 2019, PassengerCapacity = 18 });
        autoPark.AddVehicle(new Bus { Id = 6, Make = "Ikarus", Model = "280", Year = 1995, PassengerCapacity = 100 });
        autoPark.AddVehicle(new Motorcycle { Id = 7, Make = "Honda", Model = "CBR", Year = 2021, BikeType = "Sport" });

        // ---------------------------------------------------------
        // 1. Демонстрація власного ітератора (YIELD)
        // ---------------------------------------------------------
        Console.WriteLine("\n--- 1. Власний ітератор (yield return) ---");
        Console.WriteLine("Транспортні засоби новіше 2020 року:");

        // Використовуємо метод з yield. Він не виконається одразу, а буде повертати по одному.
        foreach (var v in autoPark.GetVehiclesByCondition(v => v.Year > 2020))
        {
            Console.WriteLine($" -> {v.Make} {v.Model} ({v.Year})");
        }

        // ---------------------------------------------------------
        // 2. Демонстрація LINQ-запитів
        // ---------------------------------------------------------
        Console.WriteLine("\n--- 2. LINQ: Фільтрація та Вибірка (Where + Select) ---");
        // Завдання: Знайти всі вантажівки та отримати лише їх назву (Марка + Модель)
        var truckNames = autoPark.Vehicles
            .Where(v => v is Truck)                 // Фільтрація
            .Select(v => $"{v.Make} {v.Model}");    // Проекція

        foreach (var name in truckNames)
        {
            Console.WriteLine($" Вантажівка: {name}");
        }

        Console.WriteLine("\n--- 3. LINQ: Сортування (OrderByDescending) ---");
        // Завдання: Відсортувати всі ТЗ за роком (від нових до старих)
        var sortedVehicles = autoPark.Vehicles
            .OrderByDescending(v => v.Year)
            .ThenBy(v => v.Make);

        foreach (var v in sortedVehicles)
        {
            Console.WriteLine($" {v.Year} - {v.Make} {v.Model}");
        }

        Console.WriteLine("\n--- 4. LINQ: Агрегування (Count, Sum, Average, Max) ---");

        int totalVehicles = autoPark.Vehicles.Count();
        double totalCapacity = autoPark.Vehicles.OfType<Truck>().Sum(t => t.CargoCapacity);
        double averageYear = autoPark.Vehicles.Average(v => v.Year);
        int maxPassengerCapacity = autoPark.Vehicles.OfType<Bus>().Max(b => b.PassengerCapacity);

        Console.WriteLine($" Загальна кількість ТЗ: {totalVehicles}");
        Console.WriteLine($" Загальна вантажопідйомність усіх фур: {totalCapacity} т");
        Console.WriteLine($" Середній рік випуску автопарку: {averageYear:F1}");
        Console.WriteLine($" Максимальна місткість серед автобусів: {maxPassengerCapacity} місць");

        Console.WriteLine("\n--- 5. LINQ: Групування (GroupBy) ---");
        // Завдання: Згрупувати техніку за типом (назвою класу)
        var groupedVehicles = autoPark.Vehicles
            .GroupBy(v => v.GetType().Name);

        foreach (var group in groupedVehicles)
        {
            Console.WriteLine($" Група: {group.Key} (Кількість: {group.Count()})");
            foreach (var v in group)
            {
                Console.WriteLine($"   - {v.Make} {v.Model}");
            }
        }

        // ---------------------------------------------------------
        // 3. Перевірка розширених методів LINQ (All/Any)
        // ---------------------------------------------------------
        Console.WriteLine("\n--- 6. LINQ: Перевірка умов (All / Any) ---");
        bool hasOldVehicles = autoPark.Vehicles.Any(v => v.Year < 2000);
        bool allAreNew = autoPark.Vehicles.All(v => v.Year > 2010);

        Console.WriteLine($" Чи є дуже старі авто (<2000)? -> {hasOldVehicles}");
        Console.WriteLine($" Чи всі авто новіші 2010?      -> {allAreNew}");
    }
}
