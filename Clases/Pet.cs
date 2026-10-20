using System;

namespace Grupo5_Proyecto.Clases
{
    public class Pet
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string Species { get; set; } = "";
        public string? Breed { get; set; }
        public int? Age { get; set; }
        public string? Gender { get; set; }
        public float? Weight { get; set; }
        public int OwnerId { get; set; }
        public DateTime? NextVaccineDate { get; set; }
    }
}