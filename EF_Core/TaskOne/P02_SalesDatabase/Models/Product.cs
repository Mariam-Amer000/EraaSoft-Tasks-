using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P02_SalesDatabase.Models;

[Table(name:"Products")]
internal class Product
{
    public int ProductId { get; set; }
    [Unicode]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
    public int Quantity { get; set; }
    [Precision(10, 2)]
    public decimal Price { get; set; }
    [Precision(10, 2)]
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();

    [MaxLength(250)]
    public string Description { get; set; } = "No description";
}
