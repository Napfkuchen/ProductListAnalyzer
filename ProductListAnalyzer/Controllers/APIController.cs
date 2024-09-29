using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProductListAnalyzer.Models;
using ProductListAnalyzer.Services;
using System.Net.Http;

namespace ProductListAnalyzer.Controllers {
    [ApiController]
    [Route("API")]
    public class APIController : ControllerBase {
        private readonly ListAnalyzer _listAnalyzer;
        private readonly ILogger<APIController> _logger;

        public APIController(ListAnalyzer listanAnalyzer, ILogger<APIController> logger) {
            _listAnalyzer = listanAnalyzer;
            _logger = logger;
        }

        // Route for returning list of most expensive and cheapest articles with "/api/APIController/getMostExpensive"
        [HttpGet("mostExpensiveAndCheapest")]
        public IActionResult GetMostExpensiveAndCheapest(string url) {
            var jsonContent = FetchJsonFromUrl(url);
            var articles = JsonConvert.DeserializeObject<List<Article>>(jsonContent);
            var mostExpensive = _listAnalyzer.GetMostExpensive(articles);
            var cheapest = _listAnalyzer.GetCheapest(articles);
            var result = new {
                MostExpensive = mostExpensive,
                Cheapest = cheapest
            };

            return Ok(result);
        }

        // Route for Beer cost exactly €17.99, order by price per litre
        [HttpGet("priceExactly1799")]
        public IActionResult GetPriceExactly1799(string url) {
            var jsonContent = FetchJsonFromUrl(url);
            var articles = JsonConvert.DeserializeObject<List<Article>>(jsonContent);
            var foundArticles = _listAnalyzer.GetByPriceAndSortByUnitPrice(articles, 17.99);
            
            return Ok(foundArticles);
        }

        // Route list of articles with most bottles with "/api/APIController/getMostBottles"
        [HttpGet("mostBottles")]
        public IActionResult GetMostBottles(string url) {
            try {
                var jsonContent = FetchJsonFromUrl(url);
                var articles = JsonConvert.DeserializeObject<List<Article>>(jsonContent);
                var mostBottles = _listAnalyzer.GetMostBottles(articles);
                _logger.LogInformation($"Most bottles result: {JsonConvert.SerializeObject(mostBottles)}");

                return Ok(mostBottles);
            }
            catch (Exception ex) {
                _logger.LogError($"Error: {ex.Message}");
                return StatusCode(500, "Error fetching data");
            }
        }

        // Route for returning list of articles including results of routs of all other routes with "/api/APIController/doAllRoutes"
        [HttpGet("doAllRoutes")]
        public IActionResult DoAllRoutes(string url) {
            var jsonContent = FetchJsonFromUrl(url);
            var articles = JsonConvert.DeserializeObject<List<Article>>(jsonContent);
            var mostExpensive = _listAnalyzer.GetMostExpensive(articles);
            var cheapest = _listAnalyzer.GetMostExpensive(articles);
            var foundArticles = _listAnalyzer.GetByPriceAndSortByUnitPrice(articles, 17.99);
            var mostBottles = _listAnalyzer.GetMostBottles(articles);
            var result = new {
                MostExpensive = mostExpensive,
                Cheapest = cheapest,
                GetPriceExactly1799 = foundArticles,
                MostBottles = mostBottles
            };
            
            return Ok(result);
        }

        // Support-method to load content from JSON File
        private string FetchJsonFromUrl(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                // Führe eine HTTP-GET-Anfrage aus, um die JSON-Daten abzurufen
                var response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    // Lese die JSON-Daten als String
                    return response.Content.ReadAsStringAsync().Result;
                }
                throw new Exception("Failed to fetch data from URL");
            }
        }
    }
}