using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P02_SalesDatabase.Models;

[Table(name:"Stores")]
internal class Store
{
    public int StoreId { get; set; }
    [Unicode]
    [MaxLength(80)]
    public string Name { get; set; } = string.Empty;

    [Precision(10, 2)]
    public ICollection<Sale> Sales { get; set; }  = new List<Sale>();
}
