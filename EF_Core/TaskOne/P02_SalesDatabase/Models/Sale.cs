using System.ComponentModel.DataAnnotations.Schema;

namespace P02_SalesDatabase.Models;

[Table(name:"Sales")]
internal class Sale
{
    public int SaleId { get; set; }
    public DateTime Date { get; set; }
    public Product Product { get; set; }
    public int ProductId { get; set; }
    public Customer Customer { get; set; }
    public int CustomerId { get; set; }
    public Store Store { get; set; }
    public int StoreId { get; set; }
    //public DateTime Date{ get; set; }=
}
