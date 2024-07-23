using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Đồ_An.Models.Entities;

namespace Web_Đồ_An.ViewComponents
{
	public class QuantityViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

	public QuantityViewComponent(AppDbContext context)
	{
		_context = context;
	}

	public async Task<IViewComponentResult> InvokeAsync()
	{
		var product = await _context.ProductCategories.ToListAsync();

		var cartTotalQuantity = HttpContext.Session.GetInt32("CartTotalQuantity") ?? 0;
		ViewBag.CartTotalQuantity = cartTotalQuantity;

		var dataLogin = _context.Customers.ToListAsync();
		ViewBag.Customer = dataLogin;

		return View(product);
	}
}
}