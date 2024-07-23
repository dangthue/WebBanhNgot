using Web_Đồ_An.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Web_Đồ_An.ViewComponents
{
    public class MenuProductViewComponent : ViewComponent
    {
        private readonly AppDbContext _context;

        public MenuProductViewComponent(AppDbContext context)
        {
            _context = context;
        }


        public async Task<IViewComponentResult> InvokeAsync(string name)
        {
            var product = await _context.ProductCategories.ToListAsync();
            return View(product);
        }
    }
}