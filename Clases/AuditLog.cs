using System;

namespace Grupo5_Proyecto.Clases
{
    public class AuditLog
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Action { get; set; } = "";
        public string Entity { get; set; } = "";
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}