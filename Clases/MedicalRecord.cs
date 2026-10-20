using System;

namespace Grupo5_Proyecto.Clases
{
    public class MedicalRecord
    {
        public int Id { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string Diagnosis { get; set; } = "";
        public string Treatment { get; set; } = "";
        public string? Medicines { get; set; }
        public string? Notes { get; set; }
        public int PetId { get; set; }
        public int UserId { get; set; }
    }
}