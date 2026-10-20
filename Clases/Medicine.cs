namespace Grupo5_Proyecto.Clases
{
    public class Medicine
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public string? Description { get; set; }
        public string? Category { get; set; }
        public float Price { get; set; }
        public int Stock { get; set; } = 0;
        public int MinStock { get; set; } = 10;
        public int? SupplierId { get; set; }
    }
}