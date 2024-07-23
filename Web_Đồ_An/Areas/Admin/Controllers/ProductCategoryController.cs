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
    public class ProductCategoryController : Controller
    {
        private readonly AppDbContext _db;

        public object DataLocal { get; private set; }

        public ProductCategoryController(AppDbContext db)
        {
            _db = db;
        }
        public IActionResult Index(int? page)
        {
            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;

            var items = _db.ProductCategories.OrderByDescending(x => x.ProductCategoryId).ToPagedList(pageIndex, pageSize);

            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;
            return View(items);
        }

        // THÊM 
        [HttpGet]
        public ActionResult Add()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(ProductCategory model)
        {
            if (ModelState.IsValid)
            {
                model.CreatedDate = DateTime.Now;
                model.ModifiedDate = DateTime.Now;
				model.Alias = Web_Đồ_An.Models.Common.Filter.FilterChar(model.Title);

				_db.ProductCategories.Add(model);
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }


        //SỬA 
        [HttpGet]

        public ActionResult Edit(int id)
        {
            var item = _db.ProductCategories.Find(id);
            return View(item);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductCategory model)
        {
            if (ModelState.IsValid)
            {
                _db.ProductCategories.Attach(model);
                model.ModifiedDate = DateTime.Now;
				model.Alias = Web_Đồ_An.Models.Common.Filter.FilterChar(model.Title);

				_db.Entry(model).Property(x => x.Title).IsModified = true;
                _db.Entry(model).Property(x => x.Description).IsModified = true;
                _db.Entry(model).Property(x => x.Alias).IsModified = true;
                _db.Entry(model).Property(x => x.SeoDescription).IsModified = true;
                _db.Entry(model).Property(x => x.SeoKeywords).IsModified = true;
                _db.Entry(model).Property(x => x.SeoTitle).IsModified = true;
                _db.Entry(model).Property(x => x.ModifiedDate).IsModified = true;
                _db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(model);
        }

        //XÓA 

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = _db.ProductCategories.Find(id);
            if (item != null)
            {
                var category = _db.ProductCategories.Include(pc => pc.Products).FirstOrDefault(pc => pc.ProductCategoryId == id);

                if (category != null && category.Products.Any())
                {
                    return Json(new { success = false });
                }
                _db.ProductCategories.Remove(item);
                _db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });

        }


    }
}
