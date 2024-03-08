using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GroceryAssistant.Views.Home
{
    public class CreateItemModel 
    {
        public CreateItemModel() { }
        public void OnGet()
        {
        }
        public string Product { get; set; }
        public decimal Qty { get; set; }
        public string Scalar { get; set; }
        public string Category { get; set; }
        public string Receipe { get; set; }
        public string Store { get; set; }
    }
}
