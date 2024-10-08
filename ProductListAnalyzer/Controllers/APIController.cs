﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ProductListAnalyzer.Models;
using ProductListAnalyzer.Services;
using System.Net.Http;

namespace ProductListAnalyzer.Controllers
{
    [ApiController]
    [Route("API")]
    public class APIController : ControllerBase
    {
        private readonly ListAnalyzer _listAnalyzer;
        private readonly ILogger<APIController> _logger;

        public APIController(ListAnalyzer listanAnalyzer, ILogger<APIController> logger)
        {
            _listAnalyzer = listanAnalyzer;
            _logger = logger;
        }

        // Route for returning list of most expensive and cheapest articles with "/api/APIController/getMostExpensive"
        [HttpGet("mostExpensiveAndCheapest")]
        public IActionResult GetMostExpensiveAndCheapest(string url) {
            try {
                var products = FetchAndDeserializeJson(url);
                var articles = products.SelectMany(p => p.Articles).ToList();
                var mostExpensive = _listAnalyzer.GetMostExpensive(articles);
                var cheapest = _listAnalyzer.GetCheapest(articles);
                var result = new {
                    MostExpensive = products
                        .Where(p => p.Articles.Any(a => mostExpensive.Contains(a)))
                        .Select(p => new {
                            ProductName = p.Name,
                            BrandName = p.BrandName,
                            Articles = mostExpensive.Where(a => p.Articles.Contains(a))
                        }).ToList(),
                    Cheapest = products
                        .Where(p => p.Articles.Any(a => cheapest.Contains(a)))
                        .Select(p => new {
                            ProductName = p.Name,
                            BrandName = p.BrandName,
                            Articles = cheapest.Where(a => p.Articles.Contains(a))
                    }).ToList()
                };

                // Only serialise, if this route is not part of "doAllRoutes"-request
                if (!IsDoAllRoutesRequest()) {
                    return Ok(JsonConvert.SerializeObject(result, Formatting.Indented));
                }

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
                var products = FetchAndDeserializeJson(url);
                var articles = products.SelectMany(p => p.Articles)
                    .Where(a => a.Price == 17.99) // Filter by price exactly €17.99
                    .OrderBy(a => a.PricePerUnitText) // Sort by price per litre
                    .ToList();

                if (!articles.Any()) {
                    return NotFound("There are no articles found with price of 17.99€.");
                }

                var result = products // Create result with name and brand name
                    .Where(p => p.Articles.Any(a => a.Price == 17.99))
                    .Select(p => new {
                        ProductName = p.Name,
                        BrandName = p.BrandName,
                        Articles = p.Articles
                        .Where(a => a.Price == 17.99)
                        .OrderBy(a => a.PricePerUnitText)
                    }).ToList();

                // Only serialise, if this route is not part of "doAllRoutes"-request
                if (!IsDoAllRoutesRequest()) {
                    return Ok(JsonConvert.SerializeObject(result, Formatting.Indented));
                }

                return Ok(result);
            } catch (Exception ex) {
                _logger.LogError($"Error: {ex.Message}");
                return StatusCode(500, "Error fetching data");
            }
        }

        // Route list of articles with most bottles with "/api/getMostBottles"
        [HttpGet("mostBottles")]
        public IActionResult GetMostBottles(string url)
        {
            try
            {
                var products = FetchAndDeserializeJson(url);
                var articles = products.SelectMany(p => p.Articles).ToList();
                var mostBottles = _listAnalyzer.GetMostBottles(articles);

                var result = products
                    .Where(p => p.Articles.Any(a => mostBottles.Contains(a)))  // Filter articles with specific requirement without other articles within product
                    .Select(p => new {
                        ProductName = p.Name,
                        BrandName = p.BrandName,
                        Articles = mostBottles.Where(a => p.Articles.Contains(a)).ToList()
                }).ToList();

                // Only serialise, if this route is not part of "doAllRoutes"-request
                if (!IsDoAllRoutesRequest()) {
                    return Ok(JsonConvert.SerializeObject(result, Formatting.Indented));
                }

                return Ok(result);
            } catch (Exception ex) {
                _logger.LogError($"Error: {ex.Message}");
                return StatusCode(500, "Error fetching data");
            }
        }

        // Route for returning list of articles including results of routes of all other routes with "/api/APIController/doAllRoutes"
        [HttpGet("doAllRoutes")]
        public IActionResult DoAllRoutes(string url) {
            try {
                // All other routes are called within this route
                var mostExpensiveAndCheapestResult = GetMostExpensiveAndCheapest(url) as OkObjectResult;
                var priceExactly1799Result = GetPriceExactly1799(url) as OkObjectResult;
                var mostBottlesResult = GetMostBottles(url) as OkObjectResult;
                // Combined results not serialised, yet
                var result = new {
                    MostExpensiveAndCheapest = mostExpensiveAndCheapestResult?.Value,
                    PriceExactly1799 = priceExactly1799Result?.Value,
                    MostBottles = mostBottlesResult?.Value
                };
                // Serialisation of all results from all routes
                return Ok(JsonConvert.SerializeObject(result, Formatting.Indented));
            } catch (Exception ex) {
                _logger.LogError($"Error: {ex.Message}");
                return StatusCode(500, "Error fetching data");
            }
        }

        // Support-method for Extracting JSON File, deserialize Products from that list and extract the articles within the products
        private List<Article> GetArticlesFromUrl(string url)
        {
            try
            {
                var jsonContent = FetchJsonFromUrl(url);
                if (string.IsNullOrEmpty(jsonContent))
                { // Checks if JSON File input contains content in the first place
                    throw new Exception("The URL does not send any valid JSON-Content");
                }

                var products = JsonConvert.DeserializeObject<List<Product>>(jsonContent);  // Deserialisiere Products
                if (products == null)
                { // Checks, if desirialisation of products was successfull
                    throw new Exception("Error while desirialising Product data");
                }

                var articles = products.SelectMany(p => p.Articles).ToList();  // Extract all Articles
                if (articles == null || articles.Count == 0)
                { // Checks if there are any articles in the first place
                    throw new Exception("No articles where found in JSON data");
                }
                return articles;
            }
            catch (HttpRequestException httpEx)
            { // Catches http-related errors
                throw new Exception("Error while loading the URL: " + httpEx.Message);
            }
            catch (JsonSerializationException jsonEx)
            { // Catches JSSON-Deserialisation-related errors
                throw new Exception("Error while processing JSON-File-content: " + jsonEx.Message);
            }
            catch (Exception ex)
            { // Catches all other unrelated errors
                throw new Exception("An unexpected error occured: " + ex.Message);
            }
        }

        // Support-method to determine if serialization is needed
        private bool IsDoAllRoutesRequest() {
            var routeName = Request.Path.Value;
            return routeName.Contains("doAllRoutes");
        }

        private List<Product> FetchAndDeserializeJson(string url) { // Fetch and deserialize the JSON from the URL
            try {
                var jsonContent = FetchJsonFromUrl(url);

                if (string.IsNullOrEmpty(jsonContent))
                {
                    throw new Exception("No data fetched from URL.");
                }

                var products = JsonConvert.DeserializeObject<List<Product>>(jsonContent);

                if (products == null || !products.Any())
                {
                    throw new Exception("Failed to deserialize the product data.");
                }

                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in fetching or deserializing data: {ex.Message}");
                throw new Exception("Error in fetching or deserializing data.", ex);
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