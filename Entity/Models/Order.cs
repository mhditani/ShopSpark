using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string CustomerId { get; set; } = string.Empty; // Link to Identity user
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        [Precision(18, 2)]
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Processing, Shipped, Delivered, Cancelled

        // Navigation properties
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ApplicationUser? Customer { get; set; }
    }
}
