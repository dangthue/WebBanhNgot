using Microsoft.AspNetCore.Mvc;
using Web_Đồ_An.Models;

namespace Web_Đồ_An.Controllers
{
	public class ProductCategoryController : Controller
	{
		public IActionResult Index()
		{
			return View();
		}
	}
}
