namespace ProductListAnalyzer.Models
{
    public class Product {
        public int Id { get; set; }

        // mandatory field
        public string BrandName { get; set; } = string.Empty;

        // mandatory field
        public string PricePerUnitText { get; set; } = string.Empty;

        // nullable, as not mandatory
        public string? DescriptionText { get; set; }
    }
}