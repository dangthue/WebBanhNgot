using Web_Đồ_An.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web_Đồ_An.ViewComponents
{
    public class MenuAriivalsViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public MenuAriivalsViewComponent(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
	 
                var product = await _context.ProductCategories.Include(x=>x.Products).ToListAsync();
            
            return View(product);
        }
    }
}
