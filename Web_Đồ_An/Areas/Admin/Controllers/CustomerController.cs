using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Đồ_An.Models;
using Web_Đồ_An.Models.Entities;
using X.PagedList;

namespace Web_Đồ_An.Areas.Admin.Controllers
{
  
    [Area("Admin")]
    [Authorize]    
    public class CustomerController : Controller
        {
            private readonly AppDbContext _db;

            public CustomerController(AppDbContext db)
            {
                _db = db;
            }
            public IActionResult Index(string name, int? page)
            {


            var pageSize = 10;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<Customer> items = _db.Customers.OrderByDescending(x => x.CustomerID);
            if (!string.IsNullOrEmpty(name))
            {
                items = _db.Customers.Where(x => x.Fullname.Contains(name));
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;
                 ViewBag.keyword = name;
            return View(items);





            }
            public ActionResult View(int id)
            {
                var item = _db.Customers.Find(id);

                return View(item);
            }



    }

}
