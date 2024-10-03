using System;
public abstract class Product
{
   public string Name { get; private set; }
   public decimal Price { get; private set; }
   public int Stock { get; private set; }
   public event Action<string> OutOfStockEvent;
   public Product(string name, decimal price, int stock)
   {
       Name = name;
       Price = price;
       Stock = stock;
   }
   public bool ValidateStock(int quantity)
   {
       return Stock >= quantity;
   }
   public void DeductStock(int quantity)
   {
       Stock -= quantity;
       if (Stock == 0)
       {
           OutOfStockEvent?.Invoke($"{Name} is now out of stock!");
       }
   }
   public abstract void DisplayDetails();
}
public class PhysicalProduct : Product
{
   public PhysicalProduct(string name, decimal price, int stock) : base(name, price, stock) { }
   public override void DisplayDetails()
   {
       Console.WriteLine($"{Name} - ${Price} - Stock: {Stock}");
   }
}
public class DigitalProduct : Product
{
   public DigitalProduct(string name, decimal price) : base(name, price, int.MaxValue) { }
   public override void DisplayDetails()
   {
       Console.WriteLine($"{Name} (Digital) - ${Price} - Unlimited Stock");
   }
}
public class Customer
{
   public string FirstName { get; private set; }
   public string LastName { get; private set; }
   public Customer(string firstName, string lastName)
   {
       FirstName = firstName;
       LastName = lastName;
   }
   public void DisplayCustomerInfo()
   {
       Console.WriteLine($"{FirstName} {LastName}");
   }
}
public interface IDiscount
{
   decimal ApplyDiscount(decimal price);
}
public class PercentageDiscount : IDiscount
{
   private readonly int percentage;
   public PercentageDiscount(int percentage)
   {
       this.percentage = percentage;
   }
   public decimal ApplyDiscount(decimal price)
   {
       return price - (price * percentage / 100);
   }
}
public class FixedDiscount : IDiscount
{
   private readonly decimal amount;
   public FixedDiscount(decimal amount)
   {
       this.amount = amount;
   }
   public decimal ApplyDiscount(decimal price)
   {
       return price - amount;
   }
}
public class Order
{
   private Product product;
   private Customer customer;
   private int quantity;
   private IDiscount discount;
   public bool CreateOrder(Customer customer, Product product, int quantity)
   {
       if (product.ValidateStock(quantity))
       {
           this.customer = customer;
           this.product = product;
           this.quantity = quantity;
           return true;
       }
       else
       {
           Console.WriteLine($"Insufficient stock for {product.Name}. Available: {product.Stock}");
           return false;
       }
   }
   public void ApplyDiscount(IDiscount discount)
   {
       this.discount = discount;
   }
   public void CompleteOrder()
   {
       if (discount != null)
       {
           decimal finalPrice = discount.ApplyDiscount(product.Price * quantity);
           Console.WriteLine($"Order created for {quantity} unit(s) of {product.Name}.");
           Console.WriteLine($"Discount applied. Final price: ${finalPrice}");
       }
       else
       {
           Console.WriteLine($"Order created for {quantity} unit(s) of {product.Name}.");
           Console.WriteLine($"Final price: ${product.Price * quantity}");
       }
       product.DeductStock(quantity);
       Console.WriteLine($"Order completed for {customer.FirstName} {customer.LastName}. Product shipped.");
   }
}
public class Program
{
   public static void Main(string[] args)
   {
       Customer customer = new Customer("John", "Doe");
       PhysicalProduct laptop = new PhysicalProduct("Laptop", 1000.00m, 5);
       PhysicalProduct smartphone = new PhysicalProduct("Smartphone", 700.00m, 2);
       laptop.OutOfStockEvent += message => Console.WriteLine($"[EVENT] {message}");
       smartphone.OutOfStockEvent += message => Console.WriteLine($"[EVENT] {message}");
       Order laptopOrder = new Order();
       Order smartphoneOrder = new Order();
       if (laptopOrder.CreateOrder(customer, laptop, 3))
       {
           laptopOrder.ApplyDiscount(new PercentageDiscount(10));
           laptopOrder.CompleteOrder();
       }
       Console.WriteLine();
       if (smartphoneOrder.CreateOrder(customer, smartphone, 2))
       {
           smartphoneOrder.ApplyDiscount(new FixedDiscount(50));
           smartphoneOrder.CompleteOrder();
       }
       Console.WriteLine();
       if (!laptopOrder.CreateOrder(customer, laptop, 3))
       {
           Console.WriteLine("Failed to create second laptop order due to insufficient stock.");
       }
   }
}