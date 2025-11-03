using System;
using System.Collections.Generic;

// ===== 1. Абстрактний базовий клас =====
// Описує загальні властивості та поведінку для будь-якого транспортного засобу.
// Ми не можемо створити об'єкт типу Vehicle (new Vehicle()), 
// але можемо використовувати його як "контракт" для нащадків.
public abstract class Vehicle
{
    // Загальні властивості
    public int Id { get; set; }
    public string Make { get; set; } // Марка
    public string Model { get; set; } // Модель
    public int Year { get; set; }

    // ===== 2. Абстрактний метод =====
    // Не має реалізації. Кожен клас-нащадок ЗОБОВ'ЯЗАНИЙ 
    // надати власну реалізацію (через 'override').
    public abstract void DisplayInfo();

    // ===== 3. Віртуальний метод =====
    // Має базову (default) реалізацію.
    // Клас-нащадок МОЖЕ (але не зобов'язаний) 
    // надати власну реалізацію (через 'override').
    public virtual void StartEngine()
    {
        Console.WriteLine($"({Make} {Model}) Базовий запуск двигуна.");
    }
}

// ===== 4. Клас-нащадок 1: Легковий автомобіль =====
public class Car : Vehicle
{
    public int NumberOfDoors { get; set; }
    public string BodyType { get; set; } // "Седан", "Хетчбек" і т.д.

    // Перевизначення (override) абстрактного методу (ОБОВ'ЯЗКОВО)
    public override void DisplayInfo()
    {
        Console.WriteLine("--- Легковий автомобіль ---");
        Console.WriteLine($"ID: {Id}, Марка: {Make}, Модель: {Model}, Рік: {Year}");
        Console.WriteLine($"Тип кузова: {BodyType}, Кількість дверей: {NumberOfDoors}");
    }

    // Перевизначення (override) віртуального методу (ОПЦІОНАЛЬНО)
    public override void StartEngine()
    {
        Console.WriteLine($"({Make} {Model}) Легковик тихо заводить двигун... вжжж.");
    }
}

// ===== 4. Клас-нащадок 2: Вантажівка =====
public class Truck : Vehicle
{
    public double CargoCapacity { get; set; } // Вантажопідйомність в тоннах

    // Перевизначення (override) абстрактного методу (ОБОВ'ЯЗКОВО)
    public override void DisplayInfo()
    {
        Console.WriteLine("--- Вантажівка ---");
        Console.WriteLine($"ID: {Id}, Марка: {Make}, Модель: {Model}, Рік: {Year}");
        Console.WriteLine($"Вантажопідйомність: {CargoCapacity} тонн");
    }

    // Перевизначення (override) віртуального методу (ОПЦІОНАЛЬНО)
    public override void StartEngine()
    {
        Console.WriteLine($"({Make} {Model}) Вантажівка гучно заводить дизельний двигун! Р-Р-Р!");
    }
}

// ===== 4. Клас-нащадок 3: Автобус =====
public class Bus : Vehicle
{
    public int PassengerCapacity { get; set; } // Кількість пасажирських місць

    // Перевизначення (override) абстрактного методу (ОБОВ'ЯЗКОВО)
    public override void DisplayInfo()
    {
        Console.WriteLine("--- Автобус ---");
        Console.WriteLine($"ID: {Id}, Марка: {Make}, Модель: {Model}, Рік: {Year}");
        Console.WriteLine($"Пасажиромісткість: {PassengerCapacity} осіб");
    }

    // Цей клас НЕ перевизначає StartEngine(), 
    // тому він буде використовувати базову реалізацію з класу Vehicle.
}


// ===== 5. Демонстрація роботи =====
class Program
{
    static void Main()
    {
        // Створюємо колекцію, яка може зберігати БУДЬ-ЯКИЙ
        // об'єкт, що успадковує Vehicle.
        List<Vehicle> autoPark = new List<Vehicle>();

        // Створюємо та додаємо об'єкти різних типів
        autoPark.Add(new Car
        {
            Id = 1,
            Make = "Toyota",
            Model = "Camry",
            Year = 2021,
            NumberOfDoors = 4,
            BodyType = "Седан"
        });

        autoPark.Add(new Truck
        {
            Id = 2,
            Make = "Volvo",
            Model = "FH16",
            Year = 2019,
            CargoCapacity = 25.5
        });

        autoPark.Add(new Bus
        {
            Id = 3,
            Make = "Mercedes-Benz",
            Model = "Sprinter",
            Year = 2020,
            PassengerCapacity = 18
        });

        Console.WriteLine("Ласкаво просимо до нашого автопарку!");
        Console.WriteLine("======================================\n");

        // Демонстрація поліморфізму:
        // Ми працюємо з кожним об'єктом як з 'Vehicle',
        // але C# автоматично викликає правильну (перевизначену)
        // версію методу для кожного конкретного типу.
        foreach (Vehicle v in autoPark)
        {
            // Викличе DisplayInfo() для Car, Truck або Bus
            v.DisplayInfo();

            // Викличе StartEngine() для Car, Truck
            // або базовий StartEngine() для Bus
            v.StartEngine();

            Console.WriteLine(); // Додаємо відступ
        }
    }
}
