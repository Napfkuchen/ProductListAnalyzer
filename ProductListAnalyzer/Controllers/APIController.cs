using Microsoft.AspNetCore.Mvc;
using ProductListAnalyzer.Models;
using ProductListAnalyzer.Services;

namespace ProductListAnalyzer.Controllers {
    [ApiController]
    [Route("api/[Controller]")]
    public class APIController : ControllerBase {
        private readonly ListAnalyzer _listanAnalyzer;
        private readonly ProductData _productData;

        public APIController(ListAnalyzer listanAnalyzer, ProductData productData) {
            _listanAnalyzer = listanAnalyzer;
            _productData = productData;
        }

        // Route for sorting List by price with "/api/APIController/sortByPrice"
        [HttpGet("sortByPrice")]
        public IActionResult SortByPrice() {
            var articles = _productData.Articles;
            var sortedArticles = _listanAnalyzer.SortByPrice(articles);
            return Ok(sortedArticles);
        }


        // Route for returning list of most expensive articles with "/api/APIController/getMostExpensive"
        [HttpGet("getMostExpensive")]
        public IActionResult GetMostExpensive() {
            var articles = _productData.Articles;
            var mostExpensiveArticle = _listanAnalyzer.GetMostExpensive(articles);
            return Ok(mostExpensiveArticle);
        }

        // Route for returning list of cheapest articles with "/api/APIController/getCheapest"
        [HttpGet("getMostExpensive")]
        public IActionResult GetCheapest() {
            var articles = _productData.Articles;
            var mostCheapestArticle = _listanAnalyzer.GetCheapest(articles);
            return Ok(mostCheapestArticle);
        }

        // Route for returning list of articles with most bottles with "/api/APIController/getMostBottles"
        [HttpGet("getMostBottles")]
        public IActionResult GetMostBottles() {
            var articles = _productData.Articles;
            var mostBottles = _listanAnalyzer.GetMostBottles(articles);
            return Ok(mostBottles);
        }

        // Route for returning list of articles including results of routs of all other routes with "/api/APIController/doAllRoutes"
        [HttpGet("doAllRoutes")]
        public IActionResult DoAllRoutes() {
            var articles = _productData.Articles;
            var allResults = _listanAnalyzer.DoAll(articles);
            return Ok(allResults);
        }
    }
}
