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
            try {
                var articles = GetArticlesFromUrl(url);
                var mostExpensive = _listAnalyzer.GetMostExpensive(articles);
                _logger.LogInformation($"Most expensive result: {JsonConvert.SerializeObject(mostExpensive)}");
                var cheapest = _listAnalyzer.GetCheapest(articles);
                _logger.LogInformation($"Most expensive result: {JsonConvert.SerializeObject(cheapest)}");
                var result = new {
                    MostExpensive = mostExpensive,
                    Cheapest = cheapest
                };
                return Ok(result);
            } catch (Exception ex) {
                _logger.LogError($"Error: {ex.Message}");
                return StatusCode(500, "Error fetching data");
            }
        }

        // Route for Beer cost exactly €17.99, order by price per litre
        [HttpGet("priceExactly1799")]
        public IActionResult GetPriceExactly1799(string url) {
            try {
                var articles = GetArticlesFromUrl(url);
                var foundArticles = _listAnalyzer.GetByPriceAndSortByUnitPrice(articles, 17.99);
                _logger.LogInformation($"Most expensive result: {JsonConvert.SerializeObject(foundArticles)}");

                return Ok(foundArticles);
            } catch (Exception ex) {
                _logger.LogError($"Error: {ex.Message}");
                return StatusCode(500, "Error fetching data");
            }
        }

        // Route list of articles with most bottles with "/api/APIController/getMostBottles"
        [HttpGet("mostBottles")]
        public IActionResult GetMostBottles(string url) {
            try {
                var articles = GetArticlesFromUrl(url);
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
            try {
                var articles = GetArticlesFromUrl(url);
                var mostExpensive = _listAnalyzer.GetMostExpensive(articles);
                var cheapest = _listAnalyzer.GetMostExpensive(articles);
                var foundArticles = _listAnalyzer.GetByPriceAndSortByUnitPrice(articles, 17.99);
                var mostBottles = _listAnalyzer.GetMostBottles(articles);
                var result = new
                {
                    MostExpensive = mostExpensive,
                    Cheapest = cheapest,
                    GetPriceExactly1799 = foundArticles,
                    MostBottles = mostBottles
                };

                return Ok(result);
            } catch (Exception ex) {
                _logger.LogError($"Error: {ex.Message}");
                return StatusCode(500, "Error fetching data");
            }
        }

        // Support-method for Extracting JSON File, deserialize Products from that list and extract the articles within the products
        private List<Article> GetArticlesFromUrl(string url)
        {
            try {
                var jsonContent = FetchJsonFromUrl(url);
                if(string.IsNullOrEmpty(jsonContent)) { // Checks if JSON File input contains content in the first place
                    throw new Exception("The URL does not send any valid JSON-Content");
                }
                
                var products = JsonConvert.DeserializeObject<List<Product>>(jsonContent);  // Deserialisiere Products
                if(products == null) { // Checks, if desirialisation of products was successfull
                    throw new Exception("Error while desirialising Product data");
                }
                
                var articles = products.SelectMany(p => p.Articles).ToList();  // Extract all Articles
                if(articles == null || articles.Count == 0) { // Checks if there are any articles in the first place
                    throw new Exception("No articles where found in JSON data");
                }
                return articles;
            } catch (HttpRequestException httpEx) { // Catches http-related errors
                throw new Exception("Error while loading the URL: " + httpEx.Message);
            } catch (JsonSerializationException jsonEx) { // Catches JSSON-Deserialisation-related errors
                throw new Exception("Error while processing JSON-File-content: " + jsonEx.Message);
            } catch (Exception ex) { // Catches all other unrelated errors
                throw new Exception("An unexpected error occured: " + ex.Message);
            }
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