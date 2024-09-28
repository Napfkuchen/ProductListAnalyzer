using ProductListAnalyzer.Models;
using System.Text.RegularExpressions;

namespace ProductListAnalyzer.Services
{
    public class ListAnalyzer
    {
        // Returns Items with a price of €17.99 sorted by ascending unit price 
        public List<Article> GetArticlesByPriceAndSortByUnitPrice(List<Article> articles, double targetPrice) { 
            return articles
                .Where(a => a.Price == targetPrice)
                .OrderBy(a => a.PricePerUnitText)
                .ToList();
        }
    }
}
