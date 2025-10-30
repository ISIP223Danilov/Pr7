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
            Name = name;
            Price = price;
            Quantity = quantity;
            Description = description;
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
            return $"Заказ #{Id}: {Client.Name} - {BrokenPart.Name} - {Status}";
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
            var existingPart = Parts.FirstOrDefault(p => p.Name == part.Name);
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
            var existingPart = Parts.FirstOrDefault(p => p.Name == part.Name);
            if (existingPart != null && existingPart.Quantity >= quantity)
            {
                existingPart.Quantity -= quantity;
            }
        }

        public Part GetPartByName(string partName)
        {
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
            foreach (var part in Parts.Where(p => p.Quantity > 0))
            {
                Console.WriteLine($"{part.Name}: {part.Quantity} шт. - {part.Price} руб.");
            }
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

        public AutoService(string name, decimal initialBalance)
        {
            Name = name;
            Balance = initialBalance;
            Warehouse = new Warehouse();
            Orders = new List<RepairOrder>();
            OrderHistory = new List<RepairOrder>();
            LaborCostMultiplier = 1.5m;
            RefusalPenalty = 100;
            FailurePenaltyMultiplier = 2.0m;
        }

        public void AcceptOrder(RepairOrder order)
        {
            Orders.Add(order);
            Console.WriteLine($"Принят заказ: {order}");
        }

        public void ProcessOrder(RepairOrder order)
        {
            // Будет реализовано в следующем коммите
        }

        public void BuyParts(Part part, int quantity)
        {
            // Будет реализовано в следующем коммите
        }

        public void UpdateBalance(decimal amount)
        {
            Balance += amount;
            Console.WriteLine($"Баланс обновлен: {Balance} руб.");
        }

        public void DisplayStatus()
        {
            Console.WriteLine($"\n=== {Name} ===");
            Console.WriteLine($"Баланс: {Balance} руб.");
            Console.WriteLine($"Активных заказов: {Orders.Count}");
            Console.WriteLine($"Завершенных заказов: {OrderHistory.Count}");
        }

        public void DisplayStatistics()
        {
            var completed = OrderHistory.Count(o => o.Status == OrderStatus.Completed);
            var refused = OrderHistory.Count(o => o.Status == OrderStatus.Refused);
            var failed = OrderHistory.Count(o => o.Status == OrderStatus.Failed);

            Console.WriteLine("\n=== СТАТИСТИКА ===");
            Console.WriteLine($"Успешных ремонтов: {completed}");
            Console.WriteLine($"Отказов: {refused}");
            Console.WriteLine($"Неудач: {failed}");
        }
    }
}