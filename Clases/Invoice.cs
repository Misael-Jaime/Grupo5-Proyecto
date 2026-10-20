using System;

namespace Grupo5_Proyecto.Clases
{
    public class Invoice
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public float Total { get; set; }
        public float Tax { get; set; } = 0;
        public string? PaymentMethod { get; set; }
        public string Status { get; set; } = "PAID";
        public string? Notes { get; set; }
        public int? OwnerId { get; set; }
    }
}