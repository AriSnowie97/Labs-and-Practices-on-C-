using System;
using System.Collections.Generic;
using System.Linq; // Додаємо для пошуку (Find)

// ===== 1. Абстрактний базовий клас (Розширено) =====
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


// ===== 6. Демонстрація роботи (Розширена) =====
class Program
{
    static void Main()
    {
        List<Vehicle> autoPark = new List<Vehicle>();

        // Додаємо старі та нові транспортні засоби
        autoPark.Add(new Car { Id = 1, Make = "Toyota", Model = "Camry", Year = 2021, NumberOfDoors = 4, BodyType = "Седан" });
        autoPark.Add(new Truck { Id = 2, Make = "Volvo", Model = "FH16", Year = 2019, CargoCapacity = 25.5 });
        autoPark.Add(new Bus { Id = 3, Make = "Mercedes-Benz", Model = "Sprinter", Year = 2020, PassengerCapacity = 18 });
        autoPark.Add(new Motorcycle { Id = 4, Make = "Honda", Model = "CBR500R", Year = 2022, BikeType = "Sport" });
        autoPark.Add(new ElectricCar { Id = 5, Make = "Tesla", Model = "Model S", Year = 2023, NumberOfDoors = 4, BodyType = "Ліфтбек", BatteryCapacityKWh = 100 });

        Console.WriteLine("Ласкаво просимо до нашого АВТОПАРКУ!");
        Console.WriteLine("======================================\n");
        Console.WriteLine("--- Загальна інформація та запуск двигунів ---");

        foreach (Vehicle v in autoPark)
        {
            v.DisplayInfo();
            v.StartEngine();
            Console.WriteLine();
        }

        // ===== Демонстрація заправки та обробки виключень =====
        Console.WriteLine("\n======================================");
        Console.WriteLine("--- Демонстрація заправки та виключень ---");

        foreach (Vehicle v in autoPark)
        {
            Console.WriteLine($"\nОбслуговуємо: {v.Make} {v.Model} (ID: {v.Id})");
            try
            {
                // Використовуємо 'is' (pattern matching) для перевірки типу
                if (v is ElectricCar ec)
                {
                    // Якщо це електромобіль, викликаємо його специфічний метод
                    ec.Recharge(75);
                }
                else
                {
                    // Якщо це інший ТЗ, пробуємо заправити
                    v.Refuel(50);
                }
            }
            // Обробляємо виключення, якщо щось пішло не так
            catch (ArgumentException ex) // Помилка в даних (наприклад, > 20л для мотоцикла)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[ПОМИЛКА ДАНИХ] {ex.Message}");
                Console.ResetColor();
            }
        }

        // --- Окрема демонстрація InvalidOperationException ---
        Console.WriteLine("\n--- Демонстрація помилки (InvalidOperationException) ---");

        // Знаходимо Tesla в нашому автопарку
        Vehicle tesla = autoPark.Find(v => v.Model == "Model S");

        if (tesla != null)
        {
            try
            {
                Console.WriteLine($"Спроба помилково заправити {tesla.Make} {tesla.Model} бензином...");
                // Цей виклик 100% згенерує виключення
                tesla.Refuel(30);
            }
            catch (InvalidOperationException ex) // Помилка логіки (не та операція)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"[ПОМИЛКА ОПЕРАЦІЇ] {ex.Message}");
                Console.ResetColor();
            }
        }
    }
}

