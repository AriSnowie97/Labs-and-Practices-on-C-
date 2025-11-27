using System;
using System.Threading.Tasks;

// ===== 1. Інтерфейс Стану =====
// Визначає контракт: як машина реагує на дії в різних ситуаціях.
public interface IVehicleState
{
    // Кожен метод приймає контекст (саму машину), щоб мати змогу змінити її стан
    Task DriveAsync(Vehicle context);
    Task RefuelAsync(Vehicle context, double liters);
    Task RepairAsync(Vehicle context);
    string Name { get; } // Назва поточного стану для виводу
}

// ===== 2. Контекст (Vehicle) =====
// Це наш клас машини, який тепер делегує поведінку поточному об'єкту-стану.
public class Vehicle
{
    private IVehicleState _state;

    public string Make { get; set; }
    public string Model { get; set; }

    // Властивість для доступу до поточного стану (тільки для читання ззовні)
    public string CurrentStateName => _state.Name;

    public Vehicle(string make, string model)
    {
        Make = make;
        Model = model;
        // Початковий стан - Простій
        _state = new IdleState();
    }

    // Метод для зміни стану (викликається зсередини самих станів)
    public void SetState(IVehicleState newState)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[State Machine] Зміна стану: {_state.Name} -> {newState.Name}");
        Console.ResetColor();
        _state = newState;
    }

    // === Публічні дії користувача ===
    // Замість логіки if/else ми просто передаємо запит поточному стану.

    public async Task Drive()
    {
        await _state.DriveAsync(this);
    }

    public async Task Refuel(double liters)
    {
        await _state.RefuelAsync(this, liters);
    }

    public async Task Repair()
    {
        await _state.RepairAsync(this);
    }
}

// ===== 3. Реалізація Конкретних Станів =====

// --- СТАН 1: ПРОСТІЙ (IDLE) ---
// Машина стоїть на парковці. Доступні всі дії.
public class IdleState : IVehicleState
{
    public string Name => "На парковці (Idle)";

    public async Task DriveAsync(Vehicle context)
    {
        Console.WriteLine("Запуск двигуна... Виїжджаємо на маршрут.");
        context.SetState(new ActiveState()); // Перехід у стан "В дорозі"
        await Task.CompletedTask;
    }

    public async Task RefuelAsync(Vehicle context, double liters)
    {
        Console.WriteLine("Вирішили заправитись перед поїздкою.");
        context.SetState(new RefuelingState()); // Перехід у стан "Заправка"
        // Викликаємо логіку заправки вже в новому стані
        await context.Refuel(liters);
    }

    public async Task RepairAsync(Vehicle context)
    {
        Console.WriteLine("Відправляємо машину на плановий техогляд.");
        context.SetState(new MaintenanceState()); // Перехід у стан "Ремонт"
        await context.Repair();
    }
}

// --- СТАН 2: В ДОРОЗІ (ACTIVE) ---
// Машина їде. Обмежені можливості.
public class ActiveState : IVehicleState
{
    public string Name => "В дорозі (Active)";

    public async Task DriveAsync(Vehicle context)
    {
        Console.WriteLine("Машина вже їде! Продовжуємо рух...");
        // Імітація поїздки
        await Task.Delay(1000);
        Console.WriteLine("Поїздку завершено. Повертаємось на базу.");
        context.SetState(new IdleState());
    }

    public async Task RefuelAsync(Vehicle context, double liters)
    {
        // Не можна заправлятись на ходу
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("ПОМИЛКА: Неможливо заправитись під час руху! Спочатку зупиніться.");
        Console.ResetColor();
        await Task.CompletedTask;
    }

    public async Task RepairAsync(Vehicle context)
    {
        // Симуляція поломки в дорозі
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("КРИТИЧНО: Поломка в дорозі! Евакуація на СТО...");
        Console.ResetColor();
        await Task.Delay(500);
        context.SetState(new MaintenanceState());
    }
}

// --- СТАН 3: ЗАПРАВКА (REFUELING) ---
// Тимчасовий процес.
public class RefuelingState : IVehicleState
{
    public string Name => "Заправка (Refueling)";

    public async Task DriveAsync(Vehicle context)
    {
        Console.WriteLine("Небезпечно! Не можна їхати з пістолетом у баку.");
        await Task.CompletedTask;
    }

    public async Task RefuelAsync(Vehicle context, double liters)
    {
        Console.WriteLine($"Процес заправки {liters} л...");
        // Асинхронне очікування (імітація часу заправки)
        await Task.Delay(2000);
        Console.WriteLine("Заправку завершено.");

        // Автоматичне повернення в стан спокою
        context.SetState(new IdleState());
    }

    public async Task RepairAsync(Vehicle context)
    {
        Console.WriteLine("Зачекайте завершення заправки.");
        await Task.CompletedTask;
    }
}

// --- СТАН 4: РЕМОНТ (MAINTENANCE) ---
// Машина недієздатна, поки не полагодять.
public class MaintenanceState : IVehicleState
{
    public string Name => "На ремонті (Maintenance)";

    public async Task DriveAsync(Vehicle context)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("ПОМИЛКА: Машина розібрана. Їхати неможливо.");
        Console.ResetColor();
        await Task.CompletedTask;
    }

    public async Task RefuelAsync(Vehicle context, double liters)
    {
        Console.WriteLine("Спочатку треба полагодити машину.");
        await Task.CompletedTask;
    }

    public async Task RepairAsync(Vehicle context)
    {
        Console.WriteLine("Механіки працюють над авто...");
        // Довга операція ремонту
        await Task.Delay(3000);
        Console.WriteLine("Ремонт успішно завершено! Машина як нова.");

        context.SetState(new IdleState());
    }
}

// ===== 4. Програма демонcтрації =====
class Program
{
    static async Task Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;
        Console.WriteLine("=== Паттерн State Machine (Машина Станів) ===\n");

        // 1. Створюємо машину (початковий стан - Idle)
        Vehicle myCar = new Vehicle("Toyota", "Camry");
        Console.WriteLine($"Створено авто: {myCar.Make} {myCar.Model}. Стан: {myCar.CurrentStateName}\n");

        bool running = true;

        // 2. Інтерактивне меню для керування станами
        while (running)
        {
            Console.WriteLine($"\n[ПОТОЧНИЙ СТАН]: {myCar.CurrentStateName}");
            Console.WriteLine("Оберіть дію:");
            Console.WriteLine("1. Їхати (Drive)");
            Console.WriteLine("2. Заправити (Refuel)");
            Console.WriteLine("3. Ремонт (Repair)");
            Console.WriteLine("0. Вихід");
            Console.Write("> ");

            string choice = Console.ReadLine();
            Console.WriteLine();

            switch (choice)
            {
                case "1":
                    // Спроба поїхати. Поведінка залежить від поточного стану.
                    await myCar.Drive();
                    break;
                case "2":
                    // Спроба заправитись.
                    await myCar.Refuel(20.0);
                    break;
                case "3":
                    // Спроба ремонту.
                    await myCar.Repair();
                    break;
                case "0":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Невідома команда.");
                    break;
            }
        }

        Console.WriteLine("Програму завершено.");
    }
}
