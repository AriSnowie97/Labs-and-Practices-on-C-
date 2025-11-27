using System;
using System.Collections.Generic;
using System.Linq;

// ===== 1. Абстрактний базовий клас (Оновлено) =====
// Додано реалізацію ICloneable та IComparable<Vehicle>
public abstract class Vehicle : ICloneable, IComparable<Vehicle>
{
    // Загальні властивості
    public int Id { get; set; }
    public string Make { get; set; } // Марка
    public string Model { get; set; } // Модель
    public int Year { get; set; }

    // ===== Реалізація IComparable (Сортування за замовчуванням) =====
    // Сортуємо за Роком (зростання). Якщо роки рівні — за Маркою.
    public int CompareTo(Vehicle other)
    {
        if (other == null) return 1;

        int yearComparison = this.Year.CompareTo(other.Year);
        if (yearComparison != 0) return yearComparison;

        return string.Compare(this.Make, other.Make, StringComparison.Ordinal);
    }

    // ===== Реалізація ICloneable (Клонування) =====
    // Поверхневе копіювання (достатньо для типів int, double, string)
    public virtual object Clone()
    {
        return this.MemberwiseClone();
    }

    // ===== Абстрактні та віртуальні методи (з Lab2) =====
    public abstract void DisplayInfo();

    public virtual void StartEngine()
    {
        Console.WriteLine($"({Make} {Model}) Базовий запуск двигуна.");
    }

    // Базова реалізація заправки з обробкою виключень
    public virtual void Refuel(double liters)
    {
        if (liters <= 0)
        {
            throw new ArgumentException($"Кількість палива має бути позитивною. Отримано: {liters}");
        }
        Console.WriteLine($"({Make} {Model}) Базова заправка на {liters} л.");
    }

    // Метод для вантажу
    public virtual void LoadCargo(double weight)
    {
        throw new InvalidOperationException($"({Make} {Model}) не є вантажним ТЗ.");
    }
}

// ===== Клас для альтернативного сортування (Comparer) =====
// Дозволяє сортувати за Маркою (алфавітний порядок)
public class VehicleMakeComparer : IComparer<Vehicle>
{
    public int Compare(Vehicle x, Vehicle y)
    {
        if (x == null || y == null) return 0;
        return string.Compare(x.Make, y.Make, StringComparison.Ordinal);
    }
}

// ===== 4. Класи-нащадки (з Lab2) =====

public class Car : Vehicle
{
    public int NumberOfDoors { get; set; }
    public string BodyType { get; set; }

    public override void DisplayInfo()
    {
        Console.WriteLine("--- Легковий автомобіль ---");
        Console.WriteLine($"ID: {Id}, Марка: {Make}, Модель: {Model}, Рік: {Year}");
        Console.WriteLine($"Тип кузова: {BodyType}, Кількість дверей: {NumberOfDoors}");
    }

    public override void StartEngine()
    {
        Console.WriteLine($"({Make} {Model}) Легковик тихо заводить двигун... вжжж.");
    }

    public override void Refuel(double liters)
    {
        if (liters <= 0)
        {
            throw new ArgumentException($"Кількість палива має бути позитивною. Отримано: {liters}");
        }
        Console.WriteLine($"({Make} {Model}) Заправлено {liters} л. бензину (A-95).");
    }

    public override void LoadCargo(double weight)
    {
        throw new InvalidOperationException($"({Make} {Model}) Легковий автомобіль не призначений для вантажу. Використовуйте багажник.");
    }
}

public class Truck : Vehicle
{
    public double CargoCapacity { get; set; }
    private double _currentCargoWeight = 0;

    public override void DisplayInfo()
    {
        Console.WriteLine("--- Вантажівка ---");
        Console.WriteLine($"ID: {Id}, Марка: {Make}, Модель: {Model}, Рік: {Year}");
        Console.WriteLine($"Вантажопідйомність: {CargoCapacity} тонн (Завантажено: {_currentCargoWeight} тонн)");
    }

    public override void StartEngine()
    {
        Console.WriteLine($"({Make} {Model}) Вантажівка гучно заводить дизельний двигун! Р-Р-Р!");
    }

    public override void Refuel(double liters)
    {
        if (liters <= 0)
        {
            throw new ArgumentException($"Кількість палива має бути позитивною. Отримано: {liters}");
        }
        Console.WriteLine($"({Make} {Model}) Заправлено {liters} л. дизельного палива.");
    }

    public override void LoadCargo(double weight)
    {
        if (weight <= 0)
            throw new ArgumentException($"Вага вантажу має бути позитивною. Отримано: {weight}");

        double weightInTonnes = weight / 1000.0;

        if ((_currentCargoWeight + weightInTonnes) > CargoCapacity)
        {
            throw new ArgumentException($"({Make} {Model}) Перевантаження! Макс: {CargoCapacity} т, вже є: {_currentCargoWeight} т.");
        }

        _currentCargoWeight += weightInTonnes;
        Console.WriteLine($"({Make} {Model}) Завантажено {weightInTonnes} т. Загальна вага: {_currentCargoWeight} т.");
    }
}

public class Bus : Vehicle
{
    public int PassengerCapacity { get; set; }

    public override void DisplayInfo()
    {
        Console.WriteLine("--- Автобус ---");
        Console.WriteLine($"ID: {Id}, Марка: {Make}, Модель: {Model}, Рік: {Year}");
        Console.WriteLine($"Пасажиромісткість: {PassengerCapacity} осіб");
    }

    public override void Refuel(double liters)
    {
        if (liters <= 0) throw new ArgumentException($"Кількість палива має бути позитивною.");
        Console.WriteLine($"({Make} {Model}) Заправлено {liters} л. дизеля (або метану).");
    }

    public override void LoadCargo(double weight)
    {
        throw new InvalidOperationException($"({Make} {Model}) Автобус призначений для пасажирів, а не для вантажу.");
    }
}

public class Motorcycle : Vehicle
{
    public string BikeType { get; set; }
    private double _currentCargoWeight = 0;
    private const double MaxCargoWeight = 10.0;

    public override void DisplayInfo()
    {
        Console.WriteLine("--- Мотоцикл ---");
        Console.WriteLine($"ID: {Id}, Марка: {Make}, Модель: {Model}, Рік: {Year}");
        Console.WriteLine($"Тип: {BikeType}, Вантаж: {_currentCargoWeight} кг.");
    }

    public override void StartEngine()
    {
        Console.WriteLine($"({Make} {Model}) Запуск мото-двигуна... Р-р-р-ом!");
    }

    public override void Refuel(double liters)
    {
        if (liters <= 0) throw new ArgumentException($"Кількість палива має бути позитивною.");
        if (liters > 20) throw new ArgumentException($"({Make} {Model}) Бак мотоцикла занадто малий!");
        Console.WriteLine($"({Make} {Model}) Заправлено {liters} л. бензину (A-98).");
    }

    public override void LoadCargo(double weight)
    {
        if (weight <= 0) throw new ArgumentException($"Вага вантажу має бути позитивною.");
        if ((_currentCargoWeight + weight) > MaxCargoWeight)
            throw new ArgumentException($"({Make} {Model}) Перевантаження! Макс. вага: {MaxCargoWeight} кг.");
        _currentCargoWeight += weight;
        Console.WriteLine($"({Make} {Model}) Завантажено {weight} кг.");
    }
}

public class ElectricCar : Car
{
    public int BatteryCapacityKWh { get; set; }

    public override void DisplayInfo()
    {
        base.DisplayInfo();
        Console.WriteLine($"Ємність батареї: {BatteryCapacityKWh} кВт·год");
    }

    public override void StartEngine()
    {
        Console.WriteLine($"({Make} {Model}) Безшумний запуск... Електродвигун активний.");
    }

    public override void Refuel(double liters)
    {
        throw new InvalidOperationException($"Неможливо заправити {Make} {Model} паливом. Це електромобіль!");
    }

    public void Recharge(double kwh)
    {
        if (kwh <= 0) throw new ArgumentException($"Кількість кВт·год має бути позитивною.");
        Console.WriteLine($"({Make} {Model}) Заряджається... Додано {kwh} кВт·год.");
    }
}

// ===== 6. Допоміжні класи (Події та Сервіс) =====

public class VehicleEventArgs : EventArgs
{
    public Vehicle Vehicle { get; }
    public string Message { get; }

    public VehicleEventArgs(Vehicle vehicle, string message = "")
    {
        Vehicle = vehicle;
        Message = message;
    }
}

public class AutoParkService
{
    public event EventHandler<VehicleEventArgs> VehicleAdded;
    public event EventHandler<VehicleEventArgs> VehicleRemoved;
    public event EventHandler<VehicleEventArgs> OperationFailed;

    private List<Vehicle> _vehicles = new List<Vehicle>();
    public List<Vehicle> Vehicles => _vehicles; // Змінив на List для можливості сортування в Main

    protected virtual void OnVehicleAdded(Vehicle vehicle)
    {
        VehicleAdded?.Invoke(this, new VehicleEventArgs(vehicle, "ТЗ успішно додано."));
    }

    protected virtual void OnVehicleRemoved(Vehicle vehicle)
    {
        VehicleRemoved?.Invoke(this, new VehicleEventArgs(vehicle, "ТЗ успішно видалено."));
    }

    protected virtual void OnOperationFailed(Vehicle vehicle, string errorMessage)
    {
        OperationFailed?.Invoke(this, new VehicleEventArgs(vehicle, errorMessage));
    }

    public void AddVehicle(Vehicle vehicle)
    {
        if (vehicle == null)
        {
            OnOperationFailed(null, "Критична помилка: Спроба додати 'null'.");
            return;
        }
        _vehicles.Add(vehicle);
        OnVehicleAdded(vehicle);
    }

    public void RemoveVehicle(int id)
    {
        Vehicle vehicle = _vehicles.Find(v => v.Id == id);
        if (vehicle != null)
        {
            _vehicles.Remove(vehicle);
            OnVehicleRemoved(vehicle);
        }
        else
        {
            OnOperationFailed(null, $"Помилка: Спроба видалити неіснуючий ТЗ (ID: {id}).");
        }
    }
}

public class Logger
{
    public void OnVehicleAdded(object sender, VehicleEventArgs e)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[LOG]: Додано {e.Vehicle.Make} {e.Vehicle.Model} (ID: {e.Vehicle.Id})");
        Console.ResetColor();
    }
}

// ===== 9. Демонстрація роботи =====
class Program
{
    static void Main()
    {
        Console.WriteLine("=== АВТОПАРК: Сортування та Клонування ===");

        AutoParkService autoPark = new AutoParkService();
        Logger logger = new Logger();
        autoPark.VehicleAdded += logger.OnVehicleAdded;

        // 1. Заповнення автопарку
        Console.WriteLine("\n--- 1. Додавання автомобілів ---");
        autoPark.AddVehicle(new Car { Id = 1, Make = "Toyota", Model = "Camry", Year = 2021, BodyType = "Sedan" });
        autoPark.AddVehicle(new Truck { Id = 2, Make = "Volvo", Model = "FH16", Year = 2018, CargoCapacity = 20 });
        autoPark.AddVehicle(new ElectricCar { Id = 3, Make = "Tesla", Model = "Model 3", Year = 2023, BatteryCapacityKWh = 75 });
        autoPark.AddVehicle(new Car { Id = 4, Make = "Ford", Model = "Fiesta", Year = 2010, BodyType = "Hatchback" });
        autoPark.AddVehicle(new Motorcycle { Id = 5, Make = "Yamaha", Model = "MT-07", Year = 2021, BikeType = "Naked" });

        // Отримуємо список для маніпуляцій
        List<Vehicle> myVehicles = autoPark.Vehicles;

        // 2. Демонстрація СОРТУВАННЯ
        Console.WriteLine("\n--- 2. Список ДО сортування ---");
        PrintList(myVehicles);

        // Сортування за замовчуванням (IComparable: Рік зростання)
        myVehicles.Sort();
        Console.WriteLine("\n--- Список ПІСЛЯ сортування (за Роком) ---");
        PrintList(myVehicles);

        // Сортування за допомогою окремого Comparer (Марка A-Z)
        myVehicles.Sort(new VehicleMakeComparer());
        Console.WriteLine("\n--- Список ПІСЛЯ сортування (за Маркою) ---");
        PrintList(myVehicles);

        // 3. Демонстрація КЛОНУВАННЯ
        Console.WriteLine("\n--- 3. Демонстрація Клонування (ICloneable) ---");

        // Беремо Tesla (індекс може змінитись після сортування, знайдемо по ID)
        ElectricCar originalTesla = (ElectricCar)myVehicles.Find(v => v.Make == "Tesla");

        Console.WriteLine("-> Створюємо клон Tesla...");
        ElectricCar clonedTesla = (ElectricCar)originalTesla.Clone();

        // Змінюємо клон
        clonedTesla.Id = 999;
        clonedTesla.Year = 2025;
        clonedTesla.Model = "CyberTruck (Clone)";

        Console.WriteLine("\nПеревірка результату:");
        Console.WriteLine($"[ОРИГІНАЛ] ID: {originalTesla.Id}, {originalTesla.Make} {originalTesla.Model}, Рік: {originalTesla.Year}");
        Console.WriteLine($"[КЛОН]     ID: {clonedTesla.Id}, {clonedTesla.Make} {clonedTesla.Model}, Рік: {clonedTesla.Year}");

        if (originalTesla.Year != clonedTesla.Year)
        {
            Console.WriteLine("=> Успіх: Зміна клона не вплинула на оригінал.");
        }
    }

    static void PrintList(List<Vehicle> vehicles)
    {
        foreach (var v in vehicles)
        {
            Console.WriteLine($"ID: {v.Id} | Рік: {v.Year} | {v.Make} {v.Model} ({v.GetType().Name})");
        }
    }
}
