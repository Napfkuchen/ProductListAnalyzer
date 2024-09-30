using ProductListAnalyzer.Models;
using System.Text.RegularExpressions;

namespace ProductListAnalyzer.Services {
    public class ListAnalyzer {
        // Returns Items with a price of €17.99 sorted by ascending unit price 
        public List<Article> GetByPriceAndSortByUnitPrice(List<Article> articles, double targetPrice) {
            return SortByUnitPrice(articles)
                .Where(a => a.Price == targetPrice)
                .ToList();
        }

        // Returns most expensive item(s) 
        public List<Article> GetMostExpensive(List<Article> articles) {
            if (articles == null || articles.Count == 0)
                return new List<Article>();

            var maximalPrice = articles.Max(a => a.Price);
            return articles.Where(a => a.Price == maximalPrice).ToList();
        }

        // Returns cheapest item(s) 
        public List<Article> GetCheapest(List<Article> articles) {
            if (articles == null || articles.Count == 0)
                return new List<Article>();

            var mininumPrice = articles.Min(a => a.Price);
            return articles.Where(a => a.Price == mininumPrice).ToList();
        }

        // Return item(s) with most bottles
        public List<Article> GetMostBottles(List<Article> articles) {
            // -ASSUMPTION- The amount of bottles is stated in field "shortDescription" in for example "20 x 0.5L", the twenty will be taken, bit fragile solunce since depending in specific string-format without variation but works at least for now (30.09.2024)
            var regex = new Regex(@"(\d+)\s*x"); // Searching for number stated before letter "x"
            var maxBottles = 0;
            var articlesWithMostBottles = new List<Article>();

            // Each time an article with an amount of bottles greater as the current saved number is found, the prior list will be cleared, the article is saved into the output list and the number will be arranged to the new max. If article with same number is found, the article will be add to the list without clearing it.
            foreach (var article in articles) {
                var hit = regex.Match(article.ShortDescription);
                if (hit.Success && int.TryParse(hit.Groups[1].Value, out int bottleAmount)) { 
                    if (bottleAmount > maxBottles) {
                        maxBottles = bottleAmount;
                        articlesWithMostBottles.Clear();
                        articlesWithMostBottles.Add(article);
                    } else if(bottleAmount == maxBottles) {
                        articlesWithMostBottles.Add(article);
                    }
                }
            }

            return articlesWithMostBottles;
        }

        // Returns list with all three main Listing/sorting requirements
        public List<Article> DoAll(List<Article> articles) {
            return null;
        }

        // Returns Items with a price of €17.99 sorted by ascending unit price 
        public List<Article> SortByPrice(List<Article> articles) {
            return articles
                .OrderBy(a => a.PricePerUnitText)
                .ToList();
        }

        // Returns Items with a price of €17.99 sorted by ascending unit price 
        public List<Article> SortByUnitPrice(List<Article> articles) {
            return articles
                .OrderBy(a => a.PricePerUnitText)
                .ToList();
        }
    }
}