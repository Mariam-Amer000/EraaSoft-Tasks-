using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P02_SalesDatabase.Models;

[Table(name: "Customers")]
internal class Customer
{
    public int CustomerId { get; set; }
    [Unicode]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    [Unicode(false)]
    [MaxLength(80)]
    public string Email { get; set; }

    public string CreaditCardNumber { get; set; }
    [Precision(10, 2)]
    public ICollection<Sale> Sales { get; set; } = new List<Sale>();
}
