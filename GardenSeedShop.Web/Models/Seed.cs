namespace BestShop.Models
{
    public class Seed
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string Type { get; set; }
        
        public string Subtype { get; set; }
        
        public int Height { get; set; }
        
        public int GerminationDays { get; set; }
        
        public string SeedDepth { get; set; }
        
        public int PlantSpacing { get; set; }

        public string SunRequirement { get; set; }
        
        public string Season { get; set; }

        public string ImageFilename { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public decimal Price { get; set; }
    }
}
