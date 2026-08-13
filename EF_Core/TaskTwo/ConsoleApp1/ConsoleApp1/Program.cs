using ConsoleApp1.Data;
using ConsoleApp1.Models;

namespace ConsoleApp1;

internal class Program
{
    public static int TakeId()
    {
        Console.Write("Enter the specific id: ");
        int id = Convert.ToInt32(Console.ReadLine());
        return id;
    }

    static void Main(string[] args)
    {
        BikeStores532Context db = new BikeStores532Context();

        IQueryable<Category> categories = db.Categories;

        foreach (Category category in categories)
        {
            Console.WriteLine(
                $"Category id: {category.CategoryId}, Category name: {category.CategoryName}");
        }

        Console.WriteLine("---------------------------------------------------------------------");


        var product = db.Products
            .OrderBy(p => p.ProductId)
            .FirstOrDefault();

        if (product != null)
        {
            Console.WriteLine($"Product name: {product.ProductName}");
        }

        Console.WriteLine("---------------------------------------------------------------------");


        int id = TakeId();

        var product2 = db.Products
            .Where(p => p.ProductId == id)
            .SingleOrDefault();

        if (product2 != null)
        {
            Console.WriteLine($"Product name: {product2.ProductName}");
        }
        else
        {
            Console.WriteLine("Product not found.");
        }

        Console.WriteLine("---------------------------------------------------------------------");


        Console.Write("Enter model year: ");
        int modelYear = Convert.ToInt32(Console.ReadLine());

        var productsByModelYear = db.Products
            .Where(p => p.ModelYear == modelYear);

        foreach (Product product1 in productsByModelYear)
        {
            Console.WriteLine(
                $"Name: {product1.ProductName}, Model year: {product1.ModelYear}");
        }

        Console.WriteLine("---------------------------------------------------------------------");

        id = TakeId();

        var customer = db.Customers
            .Where(c => c.CustomerId == id)
            .SingleOrDefault();

        if (customer != null)
        {
            Console.WriteLine(
                $"Customer name: {customer.FirstName} {customer.LastName}");
        }
        else
        {
            Console.WriteLine("Customer not found.");
        }

        Console.WriteLine("---------------------------------------------------------------------");


        var ProductNameWithBrandName = db.Products
            .Select(e => new
            {
                BrandName = e.Brand.BrandName,
                e.ProductName
            });

        foreach (var item in ProductNameWithBrandName)
        {
            Console.WriteLine(
                $"Product: {item.ProductName}, Brand: {item.BrandName}");
        }

        Console.WriteLine("---------------------------------------------------------------------");


        id = TakeId();

        var productCount = db.Products
            .Count(p => p.CategoryId == id);

        Console.WriteLine($"Number of products: {productCount}");

        Console.WriteLine("---------------------------------------------------------------------");

        id = TakeId();

        var totalListPrice = db.Products
            .Where(p => p.CategoryId == id)
            .Sum(p => p.ListPrice);

        Console.WriteLine($"Total list price: {totalListPrice}");

        Console.WriteLine("---------------------------------------------------------------------");


        var averageListPrice = db.Products
            .Average(p => p.ListPrice);

        Console.WriteLine($"Average list price: {averageListPrice}");

        Console.WriteLine("---------------------------------------------------------------------");


        var completedOrders = db.Orders
            .Where(o => o.OrderStatus == 4);

        foreach (var order in completedOrders)
        {
            Console.WriteLine(
                $"Order ID: {order.OrderId}, Customer ID: {order.CustomerId}, Status: {order.OrderStatus}");
        }
    }
}