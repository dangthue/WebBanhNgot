using Microsoft.AspNetCore.Mvc;
using Web_Đồ_An.Models;
using Web_Đồ_An.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace Web_Đồ_An.ViewComponents
{
    public class ProductSaleViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public ProductSaleViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int catid)
        {
			var items = await _context.Products.Include(c => c.ProductCategory).Where(x => x.IsSale && x.IsActive).Take(12).ToListAsync();
            //var categories = await _context.Categories.Include(c => c.ProductCategories).ToListAsync();
            return View(items);
        }
    }
}
