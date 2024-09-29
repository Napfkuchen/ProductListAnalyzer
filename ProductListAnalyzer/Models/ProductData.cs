using Newtonsoft.Json;

namespace ProductListAnalyzer.Models
{
    public class ProductData {
        public List<Article> Articles { get; set; } = new List<Article>();

        // load Data from JSON-File by given path
        public void LoadFromJson(string pathToJsonFile) {
            string jsonContent = System.IO.File.ReadAllText(pathToJsonFile);
            Articles = JsonConvert.DeserializeObject<List<Article>>(jsonContent);
        }
    }
}