using System;

namespace Grupo5_Proyecto.Clases
{
    public class Appointment
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string? Reason { get; set; }
        public string Status { get; set; } = "SCHEDULED";
        public int PetId { get; set; }
        public int UserId { get; set; }
    }
}