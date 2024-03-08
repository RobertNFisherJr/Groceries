using GroceryAssistant.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GroceryAssistant.Views.Home
{
    public class GetGroceryList
    {
        public GetGroceryList() 
        {
            GroceryList = new List<GroceryItem>();
        }
        public List<GroceryItem> GroceryList { get; set; }
        public DateOnly Date { get; set; }
        public void OnGet()
        {
        }
    }
}
