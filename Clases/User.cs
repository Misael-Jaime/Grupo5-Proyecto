using System;

namespace Grupo5_Proyecto.Clases
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = "";
        public string? Email { get; set; }
        public string Password { get; set; } = "";
        public string Name { get; set; } = "";
        public int RoleId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}