namespace ProductListAnalyzer.Models
{
    public class Product {
        public int Id { get; set; }

        // mandatory field
        public string BrandName { get; set; } = string.Empty;

        // mandatory fieldW
        public string Name { get; set; } = string.Empty;

        // nullable, as not mandatory
        public string? DescriptionText { get; set; }

        // List of Articles
        public List<Article> Articles { get; set; } = new List<Article>();
    }
}