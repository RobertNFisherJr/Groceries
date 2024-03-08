using GroceryAssistant.Models;
using GroceryAssistant.Views.Home;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Mail;
using System.Net;
using System.Net.Mime;

namespace GroceryAssistant.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        static readonly HttpClient _httpClient = new HttpClient();

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            using HttpResponseMessage ReceipeResponse = await _httpClient.GetAsync("http://localhost:5000/Groceries/Receipes");
            ViewBag.Recipes = JsonConvert.DeserializeObject<List<string>>(await ReceipeResponse.Content.ReadAsStringAsync());
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CreateItem([FromForm]CreateItemModel model)
        {
            try
            {
                JsonContent content = JsonContent.Create(model);

                using HttpResponseMessage response = await _httpClient.PostAsync("http://localhost:5000/Groceries", content);
                if (response.IsSuccessStatusCode)
                {
                    ViewBag.Success = true;
                    return View("index", model);
                }
            }
            catch (Exception ex) 
            {
                ViewBag.Success = false;

                return View("ErrorView",ex); 
            }
            ViewBag.Success = false;

            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetGroceryList([FromQuery] DateOnly date)
        {
            try
            {
                JsonContent content = JsonContent.Create(date);

                using HttpResponseMessage response = await _httpClient.GetAsync("http://localhost:5000/Groceries?Date=" + date.ToString());
                if (response.IsSuccessStatusCode)
                {
                    var resContent = await response.Content.ReadAsStringAsync();
                    List<GroceryItem> groceryList = JsonConvert.DeserializeObject<List<GroceryItem>>(resContent);
                    var list = new GetGroceryList();
                    if (groceryList != null)
                    {
                        list = new GetGroceryList()
                        {
                            GroceryList = groceryList,
                            Date = date,
                        };
                    }
                
                    return View(list);
                }
                else
                {
                    return View("index",new CreateItemModel());
                }
            }catch(Exception ex)
            {
                return View("ErrorView",ex.Message);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public async Task<ActionResult> AddItemsFromReceipes(string selectedValue)
        {
            using HttpResponseMessage response = await _httpClient.GetAsync("http://localhost:5000/Groceries/Receipes/List?receipe=" + selectedValue);
            var resContent = await response.Content.ReadAsStringAsync();
            List<CreateItemModel> groceryList = JsonConvert.DeserializeObject<List<CreateItemModel>>(resContent);
           
            foreach(var item in groceryList)
            {
                // Test data should refer to Meijers Camby
                item.Store = "Meijers Camby";
                JsonContent content = JsonContent.Create(item);

                using HttpResponseMessage itemResponse = await _httpClient.PostAsync("http://localhost:5000/Groceries", content);
            }

            return View("Index"); // Return a JSON response if needed
        }

        public async Task<ActionResult> EmailGroceryList([FromQuery] DateOnly date)
        {
            try
            {
                JsonContent content = JsonContent.Create(date);

                using HttpResponseMessage response = await _httpClient.GetAsync("http://localhost:5000/Groceries?Date=" + date.ToString());
                if (response.IsSuccessStatusCode)
                {
                    var resContent = await response.Content.ReadAsStringAsync();
                    List<GroceryItem> groceryList = JsonConvert.DeserializeObject<List<GroceryItem>>(resContent);
                    GetGroceryList list = new GetGroceryList()
                    {
                        GroceryList = groceryList
                    };

                    var smtpClient = new SmtpClient("smtp.mail.yahoo.com")
                    {
                        Port = 587,
                        Credentials = new NetworkCredential("rnfchanged@yahoo.com", "kfmyydqgosaplgha"),
                        EnableSsl = true,
                    };

                    var body = "<table>" +
                                "<thead>" +
                                    "<tr>" +
                                        "<th>Product</th>" +
                                        "<th>Quantity</th>" +
                                        "<th>Scalar</th>" +
                                        "<th>Location</th></tr>" +
                                "</thead><tbody>";

                    foreach (var item in list.GroceryList)
                    {
                        body += $"<tr>" +
                                    $"<td>{item.product}</td><td>{item.qty}</td><td>{item.scalar}</td><td>{@item.loc}</td><td> <input class=\"form-check-input\" type=\"checkbox\" value=\"\" id=\"flexCheckDefault\"></td></tr>";
                    }

                    body += "</tbody><table></div>";
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress("rnfchanged@yahoo.com"),
                        Subject = "TestGroceryList",
                        Body = body,
                        IsBodyHtml = true,
                    };
                    mailMessage.To.Add("rnfchanged@yahoo.com");
                    mailMessage.To.Add("FalynTilley22@gmail.com");

                    smtpClient.Send(mailMessage);

                    return View("Index", new CreateItemModel());
                }
                else
                {
                    return View("index", new CreateItemModel());
                }
            }
            catch (Exception ex)
            {
                return View("ErrorView", ex.Message);
            }

        }

        public async Task<ActionResult> purchaseItem(int id, string date)
        {
            using HttpResponseMessage response = await _httpClient.PostAsync($"http://localhost:5000/Grocery/{id}", null);
            return RedirectToAction("GetGroceryList", new {Date = date});
        }
    }
}