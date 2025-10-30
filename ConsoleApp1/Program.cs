using System;
using System.Collections.Generic;
using System.Linq;

namespace AutoServiceManagement
{
    // Перечисление для статусов заказа
    public enum OrderStatus
    {
        Pending,      // Ожидает решения
        InProgress,   // В работе
        Completed,    // Завершен успешно
        Refused,      // Отказано
        Failed        // Неудачно завершен
    }

    // Класс детали
    public class Part
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }

        public Part() { }

        public Part(string name, decimal price, int quantity, string description = "")
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Название детали не может быть пустым");
            if (price < 0)
                throw new ArgumentException("Цена не может быть отрицательной");
            if (quantity < 0)
                throw new ArgumentException("Количество не может быть отрицательным");

            Name = name;
            Price = price;
            Quantity = quantity;
            Description = description;
        }

        public override string ToString()
        {
            return $"{Name} - {Price} руб. (осталось: {Quantity})";
        }
    }

    // Класс автомобиля
    public class Car
    {
        public int Id { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }
        public int Year { get; set; }
        public string LicensePlate { get; set; }

        public Car() { }

        public Car(string brand, string model, int year, string licensePlate)
        {
            if (string.IsNullOrWhiteSpace(brand))
                throw new ArgumentException("Марка не может быть пустой");
            if (string.IsNullOrWhiteSpace(model))
                throw new ArgumentException("Модель не может быть пустой");
            if (year < 1900 || year > DateTime.Now.Year + 1)
                throw new ArgumentException("Некорректный год");
            if (string.IsNullOrWhiteSpace(licensePlate))
                throw new ArgumentException("Номерной знак не может быть пустым");

            Brand = brand;
            Model = model;
            Year = year;
            LicensePlate = licensePlate;
        }

        public override string ToString()
        {
            return $"{Brand} {Model} ({Year}) - {LicensePlate}";
        }
    }

    // Класс клиента
    public class Client
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public Car Car { get; set; }

        public Client() { }

        public Client(string name, string phone, Car car)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Имя клиента не может быть пустым");
            if (car == null)
                throw new ArgumentException("Автомобиль не может быть null");

            Name = name;
            Phone = phone;
            Car = car;
        }

        public override string ToString()
        {
            return $"{Name} ({Phone}) - {Car}";
        }
    }

    // Класс заказа на ремонт
    public class RepairOrder
    {
        public int Id { get; set; }
        public Client Client { get; set; }
        public Part BrokenPart { get; set; }
        public Part UsedPart { get; set; }
        public decimal RepairCost { get; set; }
        public decimal LaborCost { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? CompletedDate { get; set; }

        public RepairOrder() { }

        public RepairOrder(Client client, Part brokenPart, decimal laborCost)
        {
            if (client == null)
                throw new ArgumentException("Клиент не может быть null");
            if (brokenPart == null)
                throw new ArgumentException("Сломанная деталь не может быть null");
            if (laborCost < 0)
                throw new ArgumentException("Стоимость работы не может быть отрицательной");

            Client = client;
            BrokenPart = brokenPart;
            LaborCost = laborCost;
            RepairCost = CalculateTotalCost();
            Status = OrderStatus.Pending;
            CreatedDate = DateTime.Now;
        }

        public decimal CalculateTotalCost()
        {
            return BrokenPart.Price + LaborCost;
        }

        public void CompleteOrder(Part usedPart)
        {
            if (usedPart == null)
                throw new ArgumentException("Использованная деталь не может быть null");

            UsedPart = usedPart;
            Status = OrderStatus.Completed;
            CompletedDate = DateTime.Now;
        }

        public void RefuseOrder()
        {
            Status = OrderStatus.Refused;
            CompletedDate = DateTime.Now;
        }

        public void FailOrder()
        {
            Status = OrderStatus.Failed;
            CompletedDate = DateTime.Now;
        }

        public override string ToString()
        {
            return $"Заказ #{Id}: {Client.Name} - {BrokenPart.Name} - {Status} - {RepairCost} руб.";
        }
    }

    // Класс склада
    public class Warehouse
    {
        public List<Part> Parts { get; set; }

        public Warehouse()
        {
            Parts = new List<Part>();
        }

        public void AddPart(Part part)
        {
            if (part == null)
                throw new ArgumentException("Деталь не может быть null");

            var existingPart = Parts.FirstOrDefault(p => p.Name.ToLower() == part.Name.ToLower());
            if (existingPart != null)
            {
                existingPart.Quantity += part.Quantity;
            }
            else
            {
                Parts.Add(part);
            }
        }

        public void RemovePart(Part part, int quantity)
        {
            if (part == null)
                throw new ArgumentException("Деталь не может быть null");
            if (quantity <= 0)
                throw new ArgumentException("Количество должно быть положительным");

            var existingPart = Parts.FirstOrDefault(p => p.Name.ToLower() == part.Name.ToLower());
            if (existingPart != null && existingPart.Quantity >= quantity)
            {
                existingPart.Quantity -= quantity;
            }
            else
            {
                throw new InvalidOperationException($"Недостаточно деталей {part.Name} на складе");
            }
        }

        public Part GetPartByName(string partName)
        {
            if (string.IsNullOrWhiteSpace(partName))
                return null;

            return Parts.FirstOrDefault(p => p.Name.ToLower() == partName.ToLower());
        }

        public bool HasPart(string partName, int quantity = 1)
        {
            var part = GetPartByName(partName);
            return part != null && part.Quantity >= quantity;
        }

        public void DisplayInventory()
        {
            Console.WriteLine("\n=== СКЛАД ===");
            if (!Parts.Any(p => p.Quantity > 0))
            {
                Console.WriteLine("Склад пуст!");
                return;
            }

            foreach (var part in Parts.Where(p => p.Quantity > 0).OrderBy(p => p.Name))
            {
                Console.WriteLine($"- {part.Name}: {part.Quantity} шт. - {part.Price} руб./шт.");
            }
        }

        public List<Part> GetAvailableParts()
        {
            return Parts.Where(p => p.Quantity > 0).ToList();
        }
    }

    // Класс автосервиса
    public class AutoService
    {
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public Warehouse Warehouse { get; set; }
        public List<RepairOrder> Orders { get; set; }
        public List<RepairOrder> OrderHistory { get; set; }
        public decimal LaborCostMultiplier { get; set; }
        public decimal RefusalPenalty { get; set; }
        public decimal FailurePenaltyMultiplier { get; set; }
        private int nextOrderId = 1;

        public AutoService(string name, decimal initialBalance)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Название сервиса не может быть пустым");
            if (initialBalance < 0)
                throw new ArgumentException("Начальный баланс не может быть отрицательным");

            Name = name;
            Balance = initialBalance;
            Warehouse = new Warehouse();
            Orders = new List<RepairOrder>();
            OrderHistory = new List<RepairOrder>();
            LaborCostMultiplier = 1.5m;
            RefusalPenalty = 100;
            FailurePenaltyMultiplier = 2.0m;
        }

        public RepairOrder CreateOrder(Client client, Part brokenPart)
        {
            var laborCost = brokenPart.Price * (LaborCostMultiplier - 1);
            var order = new RepairOrder(client, brokenPart, laborCost)
            {
                Id = nextOrderId++
            };
            return order;
        }

        public void AcceptOrder(RepairOrder order)
        {
            if (order == null)
                throw new ArgumentException("Заказ не может быть null");

            Orders.Add(order);
            Console.WriteLine($"\n Принят заказ: {order}");
        }

        public bool ProcessOrder(RepairOrder order)
        {
            if (order == null)
                return false;

            try
            {
                if (Warehouse.HasPart(order.BrokenPart.Name))
                {
                    var usedPart = Warehouse.GetPartByName(order.BrokenPart.Name);
                    Warehouse.RemovePart(usedPart, 1);
                    order.CompleteOrder(usedPart);
                    Balance += order.RepairCost;

                    Orders.Remove(order);
                    OrderHistory.Add(order);

                    Console.WriteLine($"\n Заказ #{order.Id} завершен успешно!");
                    Console.WriteLine($" Получено: {order.RepairCost} руб.");
                    return true;
                }
                else
                {
                    // Попытка использовать другую деталь (неудачный ремонт)
                    var availablePart = Warehouse.GetAvailableParts().FirstOrDefault();
                    if (availablePart != null)
                    {
                        Warehouse.RemovePart(availablePart, 1);
                        order.FailOrder();
                        var penalty = order.RepairCost * FailurePenaltyMultiplier;
                        Balance -= penalty;

                        Orders.Remove(order);
                        OrderHistory.Add(order);

                        Console.WriteLine($"\n❌ Заказ #{order.Id} завершен НЕУДАЧНО!");
                        Console.WriteLine($" Штраф: {penalty} руб.");
                        Console.WriteLine($"  Использована неправильная деталь: {availablePart.Name}");
                    }
                    else
                    {
                        order.RefuseOrder();
                        Balance -= RefusalPenalty;

                        Orders.Remove(order);
                        OrderHistory.Add(order);

                        Console.WriteLine($"\n Отказ от заказа #{order.Id}");
                        Console.WriteLine($" Штраф за отказ: {RefusalPenalty} руб.");
                    }
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n Ошибка при обработке заказа: {ex.Message}");
                return false;
            }
        }

        public void BuyParts(Part partToBuy, int quantity)
        {
            if (partToBuy == null)
                throw new ArgumentException("Деталь не может быть null");
            if (quantity <= 0)
                throw new ArgumentException("Количество должно быть положительным");

            var totalCost = partToBuy.Price * quantity;
            if (Balance < totalCost)
            {
                throw new InvalidOperationException($"Недостаточно средств. Нужно: {totalCost} руб., доступно: {Balance} руб.");
            }

            var part = new Part(partToBuy.Name, partToBuy.Price, quantity, partToBuy.Description);
            Warehouse.AddPart(part);
            Balance -= totalCost;

            Console.WriteLine($"\n Куплено {quantity} шт. {part.Name} за {totalCost} руб.");
        }

        public void UpdateBalance(decimal amount)
        {
            Balance += amount;
            if (Balance < 0) Balance = 0;
        }

        public void DisplayStatus()
        {
            Console.WriteLine($"\n=== {Name.ToUpper()} ===");
            Console.WriteLine($" Баланс: {Balance} руб.");
            Console.WriteLine($" Деталей на складе: {Warehouse.Parts.Sum(p => p.Quantity)} шт.");
            Console.WriteLine($" Активных заказов: {Orders.Count}");
            Console.WriteLine($" Завершенных заказов: {OrderHistory.Count}");
        }

        public void DisplayStatistics()
        {
            var completed = OrderHistory.Count(o => o.Status == OrderStatus.Completed);
            var refused = OrderHistory.Count(o => o.Status == OrderStatus.Refused);
            var failed = OrderHistory.Count(o => o.Status == OrderStatus.Failed);
            var totalIncome = OrderHistory.Where(o => o.Status == OrderStatus.Completed).Sum(o => o.RepairCost);
            var totalExpenses = OrderHistory.Where(o => o.Status != OrderStatus.Completed).Sum(o =>
                o.Status == OrderStatus.Refused ? RefusalPenalty : o.RepairCost * FailurePenaltyMultiplier);

            Console.WriteLine("\n=== СТАТИСТИКА ===");
            Console.WriteLine($" Успешных ремонтов: {completed}");
            Console.WriteLine($" Отказов: {refused}");
            Console.WriteLine($" Неудач: {failed}");
            Console.WriteLine($" Общий доход: {totalIncome} руб.");
            Console.WriteLine($" Общие расходы: {totalExpenses} руб.");
            Console.WriteLine($" Чистая прибыль: {totalIncome - totalExpenses} руб.");
        }
    }

    // Главный класс программы
    public class Program
    {
        private static AutoService autoService;
        private static List<Part> availableParts;
        private static List<Car> availableCars;
        private static List<string> clientNames;
        private static Random random = new Random();
        private static int clientCounter = 0;

        public static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;
            InitializeGame();
            RunGame();
        }

        private static void InitializeGame()
        {
            InitializeAvailableParts();
            InitializeAvailableCars();
            InitializeClientNames();

            autoService = new AutoService("Мой Автосервис", 10000);
            InitializeWarehouse();

            Console.WriteLine(" === АВТОСЕРВИС ===");
            Console.WriteLine("Добро пожаловать! Управляйте своим автосервисом и зарабатывайте деньги!");
            Console.WriteLine("Старайтесь не разориться и заработать как можно больше!");
        }

        private static void InitializeAvailableParts()
        {
            availableParts = new List<Part>
            {
                new Part("Тормозные колодки", 2500, 0, "Передние тормозные колодки"),
                new Part("Масляный фильтр", 800, 0, "Фильтр моторного масла"),
                new Part("Воздушный фильтр", 600, 0, "Фильтр воздуха"),
                new Part("Свечи зажигания", 1200, 0, "Иридиевые свечи зажигания"),
                new Part("Аккумулятор", 7500, 0, "Аккумулятор 60Ah"),
                new Part("Шины", 4500, 0, "Летние шины R16"),
                new Part("Тормозные диски", 3800, 0, "Передние тормозные диски"),
                new Part("Амортизаторы", 5200, 0, "Передние амортизаторы"),
                new Part("Ремень ГРМ", 3200, 0, "Ремень газораспределительного механизма"),
                new Part("Топливный фильтр", 900, 0, "Фильтр топлива")
            };
        }

        private static void InitializeAvailableCars()
        {
            availableCars = new List<Car>
            {
                new Car("Toyota", "Camry", 2020, "А123ВС77"),
                new Car("Honda", "Civic", 2019, "В456ОР77"),
                new Car("BMW", "X5", 2021, "С789ТТ77"),
                new Car("Lada", "Vesta", 2022, "Е321КХ77"),
                new Car("Kia", "Rio", 2020, "М654НН77"),
                new Car("Hyundai", "Solaris", 2021, "Н987УУ77"),
                new Car("Skoda", "Octavia", 2019, "Р555АА77"),
                new Car("Volkswagen", "Polo", 2020, "У888ММ77"),
                new Car("Nissan", "Qashqai", 2021, "Х333НН77"),
                new Car("Ford", "Focus", 2018, "Т222ВВ77")
            };
        }

        private static void InitializeClientNames()
        {
            clientNames = new List<string>
            {
                "Иванов Иван",
                "Петров Петр",
                "Сидорова Анна",
                "Кузнецов Дмитрий",
                "Смирнова Ольга",
                "Васильев Алексей",
                "Николаева Мария",
                "Орлов Сергей",
                "Павлова Екатерина",
                "Федоров Андрей"
            };
        }

        private static void InitializeWarehouse()
        {
            // Начальные детали на складе
            autoService.Warehouse.AddPart(new Part("Тормозные колодки", 2500, 3));
            autoService.Warehouse.AddPart(new Part("Масляный фильтр", 800, 5));
            autoService.Warehouse.AddPart(new Part("Воздушный фильтр", 600, 4));
            autoService.Warehouse.AddPart(new Part("Свечи зажигания", 1200, 6));
        }

        private static void RunGame()
        {
            while (true)
            {
                try
                {
                    Console.WriteLine("\n" + new string('=', 50));
                    autoService.DisplayStatus();
                    ShowMainMenu();

                    var choice = GetUserChoice(1, 5);
                    ProcessMainMenuChoice(choice);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"\n Ошибка: {ex.Message}");
                }
            }
        }

        private static void ShowMainMenu()
        {
            Console.WriteLine("\n=== ГЛАВНОЕ МЕНЮ ===");
            Console.WriteLine("1.  Обработать следующего клиента");
            Console.WriteLine("2.  Просмотреть склад");
            Console.WriteLine("3.  Купить детали");
            Console.WriteLine("4.  Статистика");
            Console.WriteLine("5.  Выход");
            Console.Write("Выберите действие: ");
        }

        private static void ProcessMainMenuChoice(int choice)
        {
            switch (choice)
            {
                case 1:
                    ProcessClient();
                    break;
                case 2:
                    autoService.Warehouse.DisplayInventory();
                    break;
                case 3:
                    ShowBuyMenu();
                    break;
                case 4:
                    autoService.DisplayStatistics();
                    break;
                case 5:
                    Console.WriteLine("\nСпасибо за игру! До свидания!");
                    Environment.Exit(0);
                    break;
            }
        }

        private static void ProcessClient()
        {
            clientCounter++;
            Console.WriteLine($"\n === КЛИЕНТ #{clientCounter} ===");

            var client = GenerateRandomClient();
            var brokenPart = GetRandomBrokenPart();

            Console.WriteLine($" Клиент: {client}");
            Console.WriteLine($" Сломалась: {brokenPart.Name}");
            Console.WriteLine($" Стоимость ремонта: {brokenPart.Price + (brokenPart.Price * 0.5m):F2} руб.");

            var order = autoService.CreateOrder(client, brokenPart);

            Console.WriteLine("\nВаши действия:");
            Console.WriteLine("1.  Принять заказ");
            Console.WriteLine("2.  Отказаться");
            Console.Write("Выберите действие: ");

            var choice = GetUserChoice(1, 2);

            if (choice == 1)
            {
                autoService.AcceptOrder(order);
                var success = autoService.ProcessOrder(order);

                if (!success && autoService.Balance <= 0)
                {
                    Console.WriteLine("\n БАНКРОТСТВО! Игра окончена.");
                    autoService.DisplayStatistics();
                    Environment.Exit(0);
                }
            }
            else
            {
                order.RefuseOrder();
                autoService.OrderHistory.Add(order);
                autoService.UpdateBalance(-autoService.RefusalPenalty);
                Console.WriteLine($"\n Вы отказались от заказа. Штраф: {autoService.RefusalPenalty} руб.");
            }
        }

        private static void ShowBuyMenu()
        {
            Console.WriteLine("\n === МАГАЗИН ДЕТАЛЕЙ ===");
            Console.WriteLine($" Ваш баланс: {autoService.Balance} руб.");

            for (int i = 0; i < availableParts.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {availableParts[i].Name} - {availableParts[i].Price} руб./шт.");
            }

            Console.WriteLine($"0. ↩️  Назад");
            Console.Write("Выберите деталь для покупки: ");

            var partChoice = GetUserChoice(0, availableParts.Count);
            if (partChoice == 0) return;

            var selectedPart = availableParts[partChoice - 1];

            Console.Write($"Сколько шт. {selectedPart.Name} купить? (цена: {selectedPart.Price} руб./шт.): ");
            var quantity = GetUserChoice(1, 100);

            try
            {
                autoService.BuyParts(selectedPart, quantity);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Не удалось купить детали: {ex.Message}");
            }
        }

        private static Client GenerateRandomClient()
        {
            var name = clientNames[random.Next(clientNames.Count)];
            var car = availableCars[random.Next(availableCars.Count)];
            var phone = $"+7-9{random.Next(10, 100)}-{random.Next(100, 1000)}-{random.Next(10, 100)}-{random.Next(10, 100)}";

            return new Client(name, phone, car);
        }

        private static Part GetRandomBrokenPart()
        {
            return availableParts[random.Next(availableParts.Count)];
        }

        private static int GetUserChoice(int min, int max)
        {
            while (true)
            {
                if (int.TryParse(Console.ReadLine(), out int choice))
                {
                    if (choice >= min && choice <= max)
                    {
                        return choice;
                    }
                }
                Console.Write($"Пожалуйста, введите число от {min} до {max}: ");
            }
        }
    }
}