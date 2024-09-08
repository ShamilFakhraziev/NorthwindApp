using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Packt.Shared;
namespace Northwind.Web.Pages
{
    public class ClientsModel : PageModel
    {
        private NorthwindContext db;
        public ClientsModel(NorthwindContext db) {
        this.db = db;
        }

        public ILookup<string?,Customer>? Customers { get; set; }
        public void OnGet()
        {
            Customers = db.Customers.ToLookup(c=>c.Country);
        }
    }
}
