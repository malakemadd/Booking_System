using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MVCBookingFinal_YARAB_.Controllers
{
    public class InvoiceController(IInvoiceService InvoiceService) : Controller
    {
        // GET: InvoiceController
        [Authorize(Roles="ADMIN")]
        public ActionResult Index()
        {
          return View(InvoiceService.GetAll());
        }

		// GET: InvoiceController/Details/5
		[ServiceFilter(typeof(AuthorAndAdminFilter))]
		public ActionResult Details(int id)
        {
            return View(InvoiceService.GetById(id));
        }
        [Authorize]
		public ActionResult myInvoices()
		{
			return View("Index",InvoiceService.GetByUserId(User.FindFirstValue(ClaimTypes.NameIdentifier)));
		}

		
       
    }
}
