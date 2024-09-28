using ProductListAnalyzer.Models;
using System.Text.RegularExpressions;

namespace ProductListAnalyzer.Services
{
    public class ListAnalyzer
    {
        // Returns Items with a price of €17.99 sorted by ascending unit price 
        public List<Article> GetByPriceAndSortByUnitPrice(List<Article> articles, double targetPrice) {
            return sortByUnitPrice(articles)
                .Where(a => a.Price == targetPrice)
                .ToList();
        }

        // Returns most expensive item(s) 
        public List<Article> GetMostExpensive(List<Article> articles) {
            return null;
        }

        // Returns cheapest item(s) 
        public List<Article> GetCheapest(List<Article> articles)
        {
            return null;
        }

        // Return item(s) with most bottles
        public List<Article> GetMostBottles(List<Article> articles)
        {
            return null;
        }

        // Returns list with all three main Listing/sorting requirements
        public List<Article> doAll(List<Article> articles) {
            return null;
        }

        // Returns Items with a price of €17.99 sorted by ascending unit price 
        public List<Article> sortByUnitPrice(List<Article> articles)
        {
            return articles
                .OrderBy(a => a.PricePerUnitText)
                .ToList();
        }
    }
}