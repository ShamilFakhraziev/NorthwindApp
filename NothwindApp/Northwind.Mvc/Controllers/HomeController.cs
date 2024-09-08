using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Northwind.Mvc.Models;
using Northwind.Common;
using System.Diagnostics;
using Packt.Shared;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.IO.Pipelines;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using System;
using Newtonsoft.Json;
using System.Text;
using System.Xml.Linq;
using Grpc.Net.Client;

namespace Northwind.Mvc.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly NorthwindContext db;
        private readonly IHttpClientFactory? httpClientFactory;
        public HomeController(ILogger<HomeController> logger, NorthwindContext db, IHttpClientFactory httpClientFactory)
        {
            this.db = db;
            _logger = logger;
            this.httpClientFactory = httpClientFactory;
        }
        public HomeController(ILogger<HomeController> logger, NorthwindContext db)
        {
            this.db = db;
            _logger = logger;
        }

        public IActionResult Chat()
        {
            return View();
        }
        public async Task<IActionResult> Services()
        {
            try
            {
                using(GrpcChannel channel = GrpcChannel.ForAddress("https://localhost:5006"))
                {
                    Shipr.ShiprClient shipr = new(channel);
                    ShipperReply reply = await shipr.GetShipperAsync(new ShipperRequest { ShipperId = 3});
                    ViewData["shipr"] = new Shipper
                    {
                        ShipperId = reply.ShipperId,
                        CompanyName = reply.CompanyName,
                        Phone = reply.Phone
                    };

                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning($"Northwind.GraphQL service exception: {ex.Message}");
            }
            return View();
        }

        [HttpGet]
        public IActionResult CreateCustomer()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateCustomer(Customer? c)
        {
            if(c is null)
            {
                return BadRequest();
            }
            
            HttpClient client = httpClientFactory.CreateClient("Northwind.WebApi");
            
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post,"api/customers");
            string customerJson = JsonConvert.SerializeObject(c);
            requestMessage.Content = new StringContent(customerJson, Encoding.UTF8, "application/json");
            HttpResponseMessage httpResponse = await client.SendAsync(requestMessage);

            if (httpResponse.IsSuccessStatusCode)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
        [HttpGet]
        public IActionResult DeleteCustomerById()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> DeleteCustomerById(string? id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("id was null");
            }

            HttpClient client = httpClientFactory.CreateClient("Northwind.WebApi");
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Delete, $"api/customers/{id}");
            HttpResponseMessage httpResponse = await client.SendAsync(requestMessage);

            if (httpResponse.IsSuccessStatusCode)
            {
                return Ok("Customer was deleted!");
            }
            else
            {
                return BadRequest(httpResponse);
            }
        }

        public async Task<IActionResult> Customers(string country)
        {
            string uri;

            if (string.IsNullOrEmpty(country))
            {
                ViewData["Title"] = "All Customers Worldwide";
                uri = "api/customers/";
            }
            else
            {
                ViewData["Title"] = $"Customers in {country}";
                uri = $"api/customers/?country={country}";
            }
            HttpClient client = httpClientFactory.CreateClient("Northwind.WebApi");
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, uri);
            HttpResponseMessage httpResponse = await client.SendAsync(requestMessage);
            IEnumerable<Customer>? customers = await httpResponse.Content.ReadFromJsonAsync<IEnumerable<Customer>>();

            return View(customers);
        }

        public IActionResult CurrentCategory(int? id)
        {
            if (!id.HasValue)
            {
                return BadRequest("Category id was null");
            }

            Category? category = db.Categories.SingleOrDefault(c=>c.CategoryId==id);

            if(category is null)
            {
                return NotFound("No categories with this id");
            }
            return View(category);
        }
        public IActionResult ProductsThatCostMoreThan(decimal? price)
        {
            if (!price.HasValue)
            {
                return BadRequest("You must pass a product price in the query string, for example, / Home / ProductsThatCostMoreThan ? price = 50");
            }
            IEnumerable<Product> model = db.Products
            .Include(p => p.Category)
            .Include(p => p.Supplier)
            .Where(p => p.UnitPrice > price);
            if (!model.Any())
            {
                return NotFound($"No products cost more than {price:C}.");
            }
            ViewData["MaxPrice"] = price.Value.ToString("C");
            return View(model); // передача модели представлению
        }

        [ResponseCache(Duration =10, Location =ResponseCacheLocation.Any)]
        public async Task<IActionResult> Index()
        {
            try
            {
                HttpClient client = httpClientFactory.CreateClient("Minimal.WebApi");
                HttpRequestMessage req = new HttpRequestMessage(HttpMethod.Get,"api/weather");
                HttpResponseMessage resp = await client.SendAsync(req);
                ViewData["weather"] = await resp.Content.ReadFromJsonAsync<WeatherForecast[]>();
            }
            catch(Exception ex)
            {
                _logger.LogWarning($"The Minimal.WebApi service is not responding.Exception: { ex.Message}");
                ViewData["weather"] = Enumerable.Empty<WeatherForecast>().ToArray();
            }

            _logger.LogError("This is a serious error (not really!)");
            _logger.LogWarning("This is your first warning!");
            _logger.LogWarning("Second warning!");
            _logger.LogInformation("I am in the Index method of the HomeController.");

            HomeIndexViewModel model = new
            (
                VisitorCount: (new Random().Next(1,1001)),
                Categories: await db.Categories.ToListAsync(),
                Products: await db.Products.ToListAsync()
                );
            return View(model);
        }
        [Route("private")]
        [Authorize(Roles = "Administrators")]
        public IActionResult Privacy()
        {
            return View();
        }

        public async Task<IActionResult> ProductDetail(int? id)
        {
            if(!id.HasValue)
            {
                return BadRequest("You must pass a product ID in the route, for example,/Home/ProductDetail/21");
            }

            Product? model =  await db.Products.SingleOrDefaultAsync(p=>p.ProductId== id);
            if(model == null)
            {
                return NotFound($"ProductId {id} was not found");
            }

            return View(model);
        }
        public IActionResult ModelBinding()
        {
            return View(); // страница с формой
        }
        [HttpPost]
        public IActionResult ModelBinding(Thing thing)
        {
            HomeModelBindingViewModel model = new(
                thing,
                !ModelState.IsValid,
                ModelState.Values
                    .SelectMany(state => state.Errors)
                    .Select(error => error.ErrorMessage)
            );
            return View(model);
            //return View(thing); // привязанный к модели объект
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
