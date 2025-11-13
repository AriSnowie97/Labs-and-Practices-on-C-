using System;
using System.Collections.Generic;
using System.Linq;

// ===== 1. Абстрактний базовий клас =====
public abstract class Vehicle
{
    // Загальні властивості
    public int Id { get; set; }
    public string Make { get; set; } // Марка
    public string Model { get; set; } // Модель
    public int Year { get; set; }

    // ===== 2. Абстрактний метод =====
    public abstract void DisplayInfo();

    // ===== 3. Віртуальний метод 1 (Існуючий) =====
    public virtual void StartEngine()
    {
        Console.WriteLine($"({Make} {Model}) Базовий запуск двигуна.");
    }

    // ===== 3. Віртуальний метод 2 (Існуючий) + Обробка виключень =====
    // Базова реалізація заправки.
    public virtual void Refuel(double liters)
    {
        // Базова перевірка на коректність даних
        if (liters <= 0)
        {
            // Генеруємо виключення, якщо дані некоректні
            throw new ArgumentException($"Кількість палива має бути позитивною. Отримано: {liters}");
        }
        Console.WriteLine($"({Make} {Model}) Базова заправка на {liters} л.");
    }

    // ===== 3. Віртуальний метод 3 (НОВИЙ) + Обробка виключень =====
    public virtual void LoadCargo(double weight)
    {
        // Базова реалізація - більшість ТЗ не призначені для вантажу
        throw new InvalidOperationException($"({Make} {Model}) не є вантажним ТЗ.");
    }
}

// ===== 4. Клас-нащадок 1: Легковий автомобіль (Розширено) =====
public class Car : Vehicle
{
    public int NumberOfDoors { get; set; }
    public string BodyType { get; set; } // "Седан", "Хетчбек" і т.д.

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

    // Перевизначення методу заправки
    public override void Refuel(double liters)
    {
        if (liters <= 0)
        {
            throw new ArgumentException($"Кількість палива має бути позитивною. Отримано: {liters}");
        }
        Console.WriteLine($"({Make} {Model}) Заправлено {liters} л. бензину (A-95).");
    }

    // НОВЕ: Перевизначення методу завантаження
    public override void LoadCargo(double weight)
    {
        throw new InvalidOperationException($"({Make} {Model}) Легковий автомобіль не призначений для вантажу. Використовуйте багажник.");
    }
}

// ===== 4. Клас-нащадок 2: Вантажівка (Розширено) =====
public class Truck : Vehicle
{
    public double CargoCapacity { get; set; } // Вантажопідйомність в тоннах
    private double _currentCargoWeight = 0; // Поточна вага вантажу

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

    // Перевизначення методу заправки
    public override void Refuel(double liters)
    {
        if (liters <= 0)
        {
            throw new ArgumentException($"Кількість палива має бути позитивною. Отримано: {liters}");
        }
        Console.WriteLine($"({Make} {Model}) Заправлено {liters} л. дизельного палива.");
    }

    // НОВЕ: Перевизначення методу завантаження з логікою та виключеннями
    public override void LoadCargo(double weight)
    {
        if (weight <= 0)
        {
             throw new ArgumentException($"Вага вантажу має бути позитивною. Отримано: {weight}");
        }

        double weightInTonnes = weight / 1000.0; // Переводимо кг в тонни

        if ((_currentCargoWeight + weightInTonnes) > CargoCapacity)
        {
            throw new ArgumentException($"({Make} {Model}) Перевантаження! Макс: {CargoCapacity} т, вже є: {_currentCargoWeight} т, спроба додати: {weightInTonnes} т.");
        }
        
        _currentCargoWeight += weightInTonnes;
        Console.WriteLine($"({Make} {Model}) Завантажено {weightInTonnes} т. Загальна вага: {_currentCargoWeight} т.");
    }
}

// ===== 4. Клас-нащадок 3: Автобус (Розширено) =====
public class Bus : Vehicle
{
    public int PassengerCapacity { get; set; } // Кількість пасажирських місць

    public override void DisplayInfo()
    {
        Console.WriteLine("--- Автобус ---");
        Console.WriteLine($"ID: {Id}, Марка: {Make}, Модель: {Model}, Рік: {Year}");
        Console.WriteLine($"Пасажиромісткість: {PassengerCapacity} осіб");
    }

    // Використовує базовий StartEngine()

    // Перевизначення методу заправки
    public override void Refuel(double liters)
    {
         if (liters <= 0)
        {
            throw new ArgumentException($"Кількість палива має бути позитивною. Отримано: {liters}");
        }
        Console.WriteLine($"({Make} {Model}) Заправлено {liters} л. дизеля (або метану).");
    }

    // НОВЕ: Перевизначення методу завантаження
    public override void LoadCargo(double weight)
    {
        throw new InvalidOperationException($"({Make} {Model}) Автобус призначений для пасажирів, а не для вантажу.");
    }
}

// ===== 5. Новий клас-нащадок 4: Мотоцикл =====
public class Motorcycle : Vehicle
{
    public string BikeType { get; set; } // "Sport", "Chopper", "Enduro"
    private double _currentCargoWeight = 0;
    private const double MaxCargoWeight = 10.0; // Макс. вага вантажу в кг

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
        if (liters <= 0)
        {
            throw new ArgumentException($"Кількість палива має бути позитивною. Отримано: {liters}");
        }
        // Специфічна перевірка для мотоцикла
        if (liters > 20)
        {
            throw new ArgumentException($"({Make} {Model}) Бак мотоцикла занадто малий для {liters} л.!");
        }
        Console.WriteLine($"({Make} {Model}) Заправлено {liters} л. бензину (A-98).");
    }

    // НОВЕ: Перевизначення методу завантаження з логікою та виключеннями
    public override void LoadCargo(double weight)
    {
        if (weight <= 0)
        {
             throw new ArgumentException($"Вага вантажу має бути позитивною. Отримано: {weight}");
        }
        if ((_currentCargoWeight + weight) > MaxCargoWeight)
        {
            throw new ArgumentException($"({Make} {Model}) Перевантаження! Макс. вага: {MaxCargoWeight} кг, спроба додати: {weight} кг.");
        }
        _currentCargoWeight += weight;
        Console.WriteLine($"({Make} {Model}) Завантажено {weight} кг. Загальна вага: {_currentCargoWeight} кг.");
    }
}

// ===== 5. Новий клас-нащадок 5: Електромобіль (спадкує від Car) =====
public class ElectricCar : Car
{
    public int BatteryCapacityKWh { get; set; }

    // Перевизначаємо DisplayInfo, щоб додати інформацію про батарею
    public override void DisplayInfo()
    {
        // Викликаємо реалізацію базового класу (Car)
        base.DisplayInfo(); 
        Console.WriteLine($"Ємність батареї: {BatteryCapacityKWh} кВт·год");
    }

    public override void StartEngine()
    {
        Console.WriteLine($"({Make} {Model}) Безшумний запуск... Електродвигун активний.");
    }

    // Головна демонстрація виключення (Заправка)
    public override void Refuel(double liters)
    {
        throw new InvalidOperationException($"Неможливо заправити {Make} {Model} паливом. Це електромобіль!");
    }
    
    // НОВЕ: Перевизначення методу завантаження (успадковано від Car)
    // Ми не перевизначаємо LoadCargo, тому він буде використовувати 
    // реалізацію від 'Car', яка генерує помилку.

    // Новий метод, специфічний для ElectricCar
    public void Recharge(double kwh)
    {
        if (kwh <= 0)
        {
            throw new ArgumentException($"Кількість кВт·год має бути позитивною. Отримано: {kwh}");
        }
        Console.WriteLine($"({Make} {Model}) Заряджається... Додано {kwh} кВт·год.");
    }
}

// ===== 6. Клас для даних події =====
// Цей клас передає інформацію про подію підписникам.
public class VehicleEventArgs : EventArgs
{
    // Який транспортний засіб бере участь у події
    public Vehicle Vehicle { get; } 
    // Додаткове повідомлення (наприклад, текст помилки)
    public string Message { get; }

    public VehicleEventArgs(Vehicle vehicle, string message = "")
    {
        Vehicle = vehicle;
        Message = message;
    }
}

// ===== 7. Сервіс-клас (Генератор подій) =====
// Цей клас керує колекцією і генерує події.
public class AutoParkService
{
    // 1. Визначення делегата та подій
    public event EventHandler<VehicleEventArgs> VehicleAdded;
    public event EventHandler<VehicleEventArgs> VehicleRemoved;
    public event EventHandler<VehicleEventArgs> OperationFailed;

    // Приватна колекція, якою керує сервіс
    private List<Vehicle> _vehicles = new List<Vehicle>();
    public IReadOnlyList<Vehicle> Vehicles => _vehicles.AsReadOnly();

    // 2. Методи-тригери подій
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

    // 3. Публічні методи для управління парком
    public Vehicle FindVehicleById(int id)
    {
        return _vehicles.Find(v => v.Id == id);
    }
    
    public void AddVehicle(Vehicle vehicle)
    {
        if (vehicle == null)
        {
            OnOperationFailed(null, "Критична помилка: Спроба додати 'null' в автопарк.");
            return;
        }
        _vehicles.Add(vehicle);
        OnVehicleAdded(vehicle);
    }

    public void RemoveVehicle(int id)
    {
        Vehicle vehicle = FindVehicleById(id);
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

    // 4. Інтеграція виключень та подій
    
    // Спроба заправити ТЗ за ID
    public void RefuelVehicle(int id, double amount)
    {
        Vehicle vehicle = FindVehicleById(id);
        if (vehicle == null)
        {
            OnOperationFailed(null, $"Помилка: Спроба заправити неіснуючий ТЗ (ID: {id}).");
            return;
        }

        try
        {
            vehicle.Refuel(amount);
            Console.WriteLine($"[Операція успішна] {vehicle.Make} {vehicle.Model} заправлено.");
        }
        catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
        {
            OnOperationFailed(vehicle, $"[ПОМИЛКА ЗАПРАВКИ] {ex.Message}");
        }
    }

    // Спроба зарядити ТЗ за ID
    public void RechargeVehicle(int id, double amount)
    {
        Vehicle vehicle = FindVehicleById(id);
         if (vehicle == null)
        {
            OnOperationFailed(null, $"Помилка: Спроба зарядити неіснуючий ТЗ (ID: {id}).");
            return;
        }

        try
        {
            if (vehicle is ElectricCar ec)
            {
                ec.Recharge(amount);
                Console.WriteLine($"[Операція успішна] {ec.Make} {ec.Model} заряджено.");
            }
            else
            {
                throw new InvalidOperationException($"Неможливо зарядити {vehicle.Make} {vehicle.Model}. Це не електромобіль!");
            }
        }
        catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
        {
            OnOperationFailed(vehicle, $"[ПОМИЛКА ЗАРЯДКИ] {ex.Message}");
        }
    }

    // НОВИЙ метод для завантаження вантажу з обробкою виключень
    public void LoadCargoOnVehicle(int id, double weight)
    {
        Vehicle vehicle = FindVehicleById(id);
         if (vehicle == null)
        {
            OnOperationFailed(null, $"Помилка: Спроба завантажити неіснуючий ТЗ (ID: {id}).");
            return;
        }

        try
        {
            // Викликаємо метод, що може згенерувати виключення
            vehicle.LoadCargo(weight);
            Console.WriteLine($"[Операція успішна] {vehicle.Make} {vehicle.Model} завантажено.");
        }
        catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
        {
            // Перехоплюємо очікувані помилки
            OnOperationFailed(vehicle, $"[ПОМИЛКА ЗАВАНТАЖЕННЯ] {ex.Message}");
        }
    }

    // ===== 5. Демонстрація делегатів =====
    public void ProcessVehicles(Action<Vehicle> action)
    {
        Console.WriteLine($"\nОбробка {_vehicles.Count} ТЗ за допомогою делегата Action...");
        foreach (Vehicle v in _vehicles)
        {
            action(v); // Виклик делегата
        }
    }

    public List<Vehicle> FindVehicles(Func<Vehicle, bool> condition)
    {
        Console.WriteLine($"\nПошук ТЗ за допомогою делегата Func<Vehicle, bool>...");
        return _vehicles.Where(condition).ToList();
    }

    public double CalculateTotal(Func<Vehicle, double> valueSelector)
    {
        Console.WriteLine($"\nПідрахунок загального значення за допомогою делегата Func<Vehicle, double>...");
        return _vehicles.Sum(valueSelector);
    }
}

// ===== 8. Клас-Підписник (Реагує на події) =====
public class Logger
{
    // ... (Методи OnVehicleAdded, OnVehicleRemoved, OnOperationFailed без змін) ...
    public void OnVehicleAdded(object sender, VehicleEventArgs e)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[ЛОГГЕР]: {DateTime.Now:T}: {e.Message} -> {e.Vehicle.Make} {e.Vehicle.Model} (ID: {e.Vehicle.Id})");
        Console.ResetColor();
    }

    public void OnVehicleRemoved(object sender, VehicleEventArgs e)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[ЛОГГЕР]: {DateTime.Now:T}: {e.Message} -> {e.Vehicle.Make} {e.Vehicle.Model} (ID: {e.Vehicle.Id})");
        Console.ResetColor();
    }

    public void OnOperationFailed(object sender, VehicleEventArgs e)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        string vehicleInfo = e.Vehicle != null ? $"{e.Vehicle.Make} {e.Vehicle.Model}" : "N/A";
        Console.WriteLine($"[ПОМИЛКА СИСТЕМИ]: {DateTime.Now:T}: {e.Message} (ТЗ: {vehicleInfo})");
        Console.ResetColor();
    }
}


// ===== 9. Демонстрація роботи (Оновлено) =====
class Program
{
    static void Main()
    {
        Console.WriteLine("Ласкаво просимо до АВТОПАРКУ (з подіями)!");
        Console.WriteLine("======================================\n");

        // 1. Створюємо генератора подій (Сервіс)
        AutoParkService autoPark = new AutoParkService();

        // 2. Створюємо підписника (Логгер)
        Logger logger = new Logger();

        // 3. Підписуємо методи логгера на події сервісу
        autoPark.VehicleAdded += logger.OnVehicleAdded;
        autoPark.VehicleRemoved += logger.OnVehicleRemoved;
        autoPark.OperationFailed += logger.OnOperationFailed;

        // 4. Виконуємо дії, які викличуть події
        Console.WriteLine("--- Додавання ТЗ (має викликати подію 'VehicleAdded') ---");
        autoPark.AddVehicle(new Car { Id = 1, Make = "Toyota", Model = "Camry", Year = 2021 });
        autoPark.AddVehicle(new ElectricCar { Id = 5, Make = "Tesla", Model = "Model S", Year = 2023 });
        // Додаємо вантажівку з реальною вантажопідйомністю (20 тонн)
        autoPark.AddVehicle(new Truck { Id = 2, Make = "Volvo", Model = "FH16", Year = 2019, CargoCapacity = 20 });
        autoPark.AddVehicle(new Motorcycle { Id = 7, Make = "Honda", Model = "CBR500R", Year = 2022, BikeType = "Sport"});
        
        Console.WriteLine("\n--- Видалення ТЗ (має викликати 'VehicleRemoved' та 'OperationFailed') ---");
        autoPark.RemoveVehicle(2); // Тимчасово видалимо вантажівку, щоб потім додати знову
        autoPark.RemoveVehicle(99); // Помилка (неіснуючий ID)

        Console.WriteLine("\n--- Демонстрація заправки (має викликати 'OperationFailed' для Tesla) ---");
        autoPark.RefuelVehicle(1, 40.5); // Успіх
        autoPark.RefuelVehicle(5, 30.0); // Помилка (Tesla)

        Console.WriteLine("\n--- Демонстрація зарядки (має викликати 'OperationFailed' для Toyota) ---");
        autoPark.RechargeVehicle(5, 75.0); // Успіх
        autoPark.RechargeVehicle(1, 50.0); // Помилка (Toyota)

        // ===== 5. Демонстрація роботи з делегатами =====
        Console.WriteLine("\n==============================================");
        Console.WriteLine("--- Демонстрація роботи з делегатами ---");

        // --- Делегат 1: Action<Vehicle> (Виведення інформації) ---
        Action<Vehicle> displayAction = DisplayShortInfo;
        autoPark.ProcessVehicles(displayAction);

        // --- Делегат 2: Func<Vehicle, bool> (Пошук) ---
        Func<Vehicle, bool> isOldVehicle = v => v.Year < 2022;
        List<Vehicle> oldVehicles = autoPark.FindVehicles(isOldVehicle);
        Console.WriteLine($"Знайдено {oldVehicles.Count} старих ТЗ (до 2022 року):");
        foreach (var v in oldVehicles)
        {
            Console.WriteLine($"- {v.Make} {v.Model} ({v.Year})");
        }

        // --- Делегат 3: Func<Vehicle, double> (Підрахунок) ---
        autoPark.AddVehicle(new Bus { Id = 6, Make = "Ikarus", Model = "280", Year = 1990, PassengerCapacity = 150 });
        Func<Vehicle, double> passengerCounter = v => (v is Bus b) ? b.PassengerCapacity : 0;
        double totalCapacity = autoPark.CalculateTotal(passengerCounter);
        Console.WriteLine($"Загальна пасажиромісткість автобусів: {totalCapacity} осіб.");

        // --- Делегат 4: (Анонімний метод) ---
        Console.WriteLine("\n--- Демонстрація Action з анонімним методом (Запуск двигунів) ---");
        autoPark.ProcessVehicles(delegate(Vehicle v) 
        {
            v.StartEngine();
        });

        // ===== 6. НОВА Демонстрація Завантаження Вантажу (з виключеннями) =====
        Console.WriteLine("\n==============================================");
        Console.WriteLine("--- Демонстрація Завантаження Вантажу ---");
        
        // Повертаємо вантажівку
        autoPark.AddVehicle(new Truck { Id = 2, Make = "Volvo", Model = "FH16", Year = 2019, CargoCapacity = 20 });

        // 1. Успішне завантаження вантажівки (15 тонн = 15000 кг)
        autoPark.LoadCargoOnVehicle(2, 15000);
        
        // 2. Помилка: Перевантаження вантажівки (спроба додати ще 6 тонн)
        autoPark.LoadCargoOnVehicle(2, 6000);

        // 3. Успішне завантаження мотоцикла (5 кг)
        autoPark.LoadCargoOnVehicle(7, 5);

        // 4. Помилка: Перевантаження мотоцикла (спроба додати ще 10 кг)
        autoPark.LoadCargoOnVehicle(7, 10);

        // 5. Помилка: Спроба завантажити Автобус
        autoPark.LoadCargoOnVehicle(6, 100);

        // 6. Помилка: Спроба завантажити Електромобіль
        autoPark.LoadCargoOnVehicle(5, 50);

        // 7. Помилка: Негативна вага
        autoPark.LoadCargoOnVehicle(2, -100);
    }

    // --- Методи-обробники для делегатів ---
    public static void DisplayShortInfo(Vehicle v)
    {
        Console.WriteLine($"[INFO] ТЗ ID: {v.Id}, Марка: {v.Make}, Модель: {v.Model}");
    }
}
