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

    // ===== 3. Віртуальний метод 2 (Новий) + Обробка виключень =====
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

    // Перевизначення нового віртуального методу
    public override void Refuel(double liters)
    {
        if (liters <= 0)
        {
            throw new ArgumentException($"Кількість палива має бути позитивною. Отримано: {liters}");
        }
        Console.WriteLine($"({Make} {Model}) Заправлено {liters} л. бензину (A-95).");
    }
}

// ===== 4. Клас-нащадок 2: Вантажівка (Розширено) =====
public class Truck : Vehicle
{
    public double CargoCapacity { get; set; } // Вантажопідйомність в тоннах

    public override void DisplayInfo()
    {
        Console.WriteLine("--- Вантажівка ---");
        Console.WriteLine($"ID: {Id}, Марка: {Make}, Модель: {Model}, Рік: {Year}");
        Console.WriteLine($"Вантажопідйомність: {CargoCapacity} тонн");
    }

    public override void StartEngine()
    {
        Console.WriteLine($"({Make} {Model}) Вантажівка гучно заводить дизельний двигун! Р-Р-Р!");
    }

    // Перевизначення нового віртуального методу
    public override void Refuel(double liters)
    {
        if (liters <= 0)
        {
            throw new ArgumentException($"Кількість палива має бути позитивною. Отримано: {liters}");
        }
        Console.WriteLine($"({Make} {Model}) Заправлено {liters} л. дизельного палива.");
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

    // Перевизначення нового віртуального методу
    public override void Refuel(double liters)
    {
        if (liters <= 0)
        {
            throw new ArgumentException($"Кількість палива має бути позитивною. Отримано: {liters}");
        }
        Console.WriteLine($"({Make} {Model}) Заправлено {liters} л. дизеля (або метану).");
    }
}

// ===== 5. Новий клас-нащадок 4: Мотоцикл =====
public class Motorcycle : Vehicle
{
    public string BikeType { get; set; } // "Sport", "Chopper", "Enduro"

    public override void DisplayInfo()
    {
        Console.WriteLine("--- Мотоцикл ---");
        Console.WriteLine($"ID: {Id}, Марка: {Make}, Модель: {Model}, Рік: {Year}");
        Console.WriteLine($"Тип: {BikeType}");
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

    // ===== Головна демонстрація виключення =====
    // Електромобіль НЕ МОЖНА заправити паливом.
    // Тому ми перевизначаємо метод, щоб він генерував помилку.
    public override void Refuel(double liters)
    {
        throw new InvalidOperationException($"Неможливо заправити {Make} {Model} паливом. Це електромобіль!");
    }

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
    // Ми використовуємо вбудований делегат EventHandler<T>

    // Подія спрацьовує при успішному додаванні ТЗ
    public event EventHandler<VehicleEventArgs> VehicleAdded;

    // Подія спрацьовує при успішному видаленні ТЗ
    public event EventHandler<VehicleEventArgs> VehicleRemoved;

    // Подія спрацьовує, якщо операція (напр. заправка) провалилася
    public event EventHandler<VehicleEventArgs> OperationFailed;

    // Приватна колекція, якою керує сервіс
    private List<Vehicle> _vehicles = new List<Vehicle>();
    public IReadOnlyList<Vehicle> Vehicles => _vehicles.AsReadOnly();

    // 2. Методи-тригери подій (найкраща практика - protected virtual)
    // Ці методи "викликають" подію і сповіщають всіх підписників.
    protected virtual void OnVehicleAdded(Vehicle vehicle)
    {
        // Перевірка, чи є хоча б один підписник (чи подія не null)
        VehicleAdded?.Invoke(this, new VehicleEventArgs(vehicle, "ТЗ успішно додано."));
    }

    protected virtual void OnVehicleRemoved(Vehicle vehicle)
    {
        VehicleRemoved?.Invoke(this, new VehicleEventArgs(vehicle, "ТЗ успішно видалено."));
    }

    protected virtual void OnOperationFailed(Vehicle vehicle, string errorMessage)
    {
        // Передаємо і ТЗ (якщо він є), і повідомлення про помилку
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
            // Якщо дані невалідні, генеруємо подію про помилку
            OnOperationFailed(null, "Критична помилка: Спроба додати 'null' в автопарк.");
            return;
        }
        _vehicles.Add(vehicle);
        // Якщо все добре, генеруємо подію про успіх
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
            // Ми викликаємо метод, який *може* згенерувати виключення
            // (наприклад, Refuel для Tesla або amount < 0)
            vehicle.Refuel(amount);
            Console.WriteLine($"[Операція успішна] {vehicle.Make} {vehicle.Model} заправлено.");
        }
        catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
        {
            // Ми перехоплюємо *очікувані* бізнес-помилки
            // і перетворюємо їх на подію 'OperationFailed'.
            // Програма при цьому не "падає".
            OnOperationFailed(vehicle, $"[ПОМИЛКА ЗАПРАВКИ] {ex.Message}");
        }
        // Інші виключення (напр. NullReferenceException) не будуть тут перехоплені
        // і зупинять програму, що є коректною поведінкою для непередбачуваних помилок.
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
            // Перевіряємо, чи це взагалі ElectricCar
            if (vehicle is ElectricCar ec)
            {
                ec.Recharge(amount);
                Console.WriteLine($"[Операція успішна] {ec.Make} {ec.Model} заряджено.");
            }
            else
            {
                // Генеруємо виключення, якщо це не електромобіль
                throw new InvalidOperationException($"Неможливо зарядити {vehicle.Make} {vehicle.Model}. Це не електромобіль!");
            }
        }
        catch (Exception ex) when (ex is ArgumentException || ex is InvalidOperationException)
        {
            // Перехоплюємо очікувані помилки
            OnOperationFailed(vehicle, $"[ПОМИЛКА ЗАРЯДКИ] {ex.Message}");
        }
    }
}

// ===== 8. Клас-Підписник (Реагує на події) =====
// Цей клас нічого не знає про логіку AutoParkService,
// він просто вміє реагувати на події, на які його підписали.
public class Logger
{
    // Метод-обробник події (має відповідати сигнатурі делегата)
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
        // Використовуємо делегати для зв'язування події та обробника
        autoPark.VehicleAdded += logger.OnVehicleAdded;
        autoPark.VehicleRemoved += logger.OnVehicleRemoved;
        autoPark.OperationFailed += logger.OnOperationFailed;

        // 4. Виконуємо дії, які викличуть події
        Console.WriteLine("--- Додавання ТЗ (має викликати подію 'VehicleAdded') ---");
        autoPark.AddVehicle(new Car { Id = 1, Make = "Toyota", Model = "Camry", Year = 2021 });
        autoPark.AddVehicle(new ElectricCar { Id = 5, Make = "Tesla", Model = "Model S", Year = 2023 });
        autoPark.AddVehicle(new Truck { Id = 2, Make = "Volvo", Model = "FH16", Year = 2019 });

        Console.WriteLine("\n--- Видалення ТЗ (має викликати 'VehicleRemoved' та 'OperationFailed') ---");
        autoPark.RemoveVehicle(2); // Успішне видалення
        autoPark.RemoveVehicle(99); // Помилка (неіснуючий ID)

        Console.WriteLine("\n--- Демонстрація заправки (має викликати 'OperationFailed' для Tesla) ---");

        // Успішна заправка
        autoPark.RefuelVehicle(1, 40.5);

        // Помилка логіки (InvalidOperationException), яка буде перехоплена
        // і перетворена на подію 'OperationFailed'. Програма не впаде.
        autoPark.RefuelVehicle(5, 30.0); // Спроба заправити Tesla

        Console.WriteLine("\n--- Демонстрація зарядки (має викликати 'OperationFailed' для Toyota) ---");

        // Успішна зарядка
        autoPark.RechargeVehicle(5, 75.0);

        // Помилка логіки (InvalidOperationException)
        autoPark.RechargeVehicle(1, 50.0); // Спроба зарядити Toyota
    }
}

