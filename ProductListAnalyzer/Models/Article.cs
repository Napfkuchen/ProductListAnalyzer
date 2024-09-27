namespace ProductListAnalyzer.Models
{
    public class Article
    {
        public int Id { get; set; }

        // nullable, as not mandatory
        public string? ShortDescription { get; set; }

        public double Price { get; set; } 

        // mandatory field
        public string Unit { get; set; } = string.Empty;

        // mandatory field
        public string PricePerUnitText { get; set; } = string.Empty;

        // nullable, as not mandatory
        public string? Image { get; set; }
    }
}
