using Microsoft.AspNetCore.Mvc;
using Web_Đồ_An.Models.Entities;
using Web_Đồ_An.Models;
using X.PagedList;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Web_Đồ_An.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class ProductController : Controller
    {
        private readonly AppDbContext _db;

        public object DataLocal { get; private set; }

        public ProductController(AppDbContext db)
        {
            _db = db;
        }
        public IActionResult Index(string Searchtext, int? page)
        {
            var products = _db.Products.Include(x => x.ProductCategory).ToList();

            var pageSize = 10;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<Product> items = _db.Products.OrderByDescending(x => x.ProductId);
            if (!string.IsNullOrEmpty(Searchtext))
            {
                items = _db.Products.Where(x => x.ProductCategory.Title.Contains(Searchtext) || x.Title.Contains(Searchtext));
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;
            return View(items);
        }

        [HttpGet]
        public ActionResult Add()
        {
            ViewBag.ProductCategory = new SelectList(_db.ProductCategories.ToList(), "ProductCategoryId", "Title");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Product model)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count() > 0 && files[0].Length > 0) // kiểm tra xem tập có đc gửi từ file lên không 
                {
                    var file = files[0];
                    var FileName = file.FileName;
                    // upload ảnh vào thư mục wwwroot\\images\\category
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Content\\Images\\sanpham", FileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                        model.Image = "/Content/Images/sanpham/" + FileName; // gán tên ảnh cho thuộc tinh Image
                    }

                }
                model.CreatedDate = DateTime.Now;
                model.Alias = Web_Đồ_An.Models.Common.Filter.FilterChar(model.Title);

                _db.Add(model);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }


        public ActionResult View(int id)
        {
            var products = _db.Products.Include(x => x.ProductCategory).ToList();

            var item = _db.Products.Find(id);

         
            return View(item);
        }

        [HttpGet]
        public ActionResult Edit(int id)
        {
            ViewBag.ProductCategoryId = new SelectList(_db.ProductCategories.ToList(), "ProductCategoryId", "Title");
            var items = _db.Products.Find(id);
            return View(items);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product model)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count() > 0 && files[0].Length > 0) // kiểm tra xem tập có đc gửi từ file lên không 
                {
                    var file = files[0];
                    var FileName = file.FileName;
                    // upload ảnh vào thư mục wwwroot\\images\\category
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Content\\Images\\sanpham", FileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                        model.Image = "/Content/Images/sanpham/" + FileName; // gán tên ảnh cho thuộc tinh Image
                    }

                }
                _db.Products.Attach(model);
                model.Alias = Web_Đồ_An.Models.Common.Filter.FilterChar(model.Title);

                _db.Entry(model).Property(x => x.Title).IsModified = true;
                _db.Entry(model).Property(x => x.Description).IsModified = true;
                _db.Entry(model).Property(x => x.ModifiedDate).IsModified = true;
                /*                _db.Entry(TinTuc).Property(x => x.Image).IsModified = true;
                */
                _db.Entry(model).Property(x => x.IsActive).IsModified = true;
                _db.Entry(model).Property(x => x.IsHome).IsModified = true;
                _db.Entry(model).Property(x => x.IsSale).IsModified = true;
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
/*            ViewBag.DanhMuc = new SelectList(_db.ProductCategories.ToList(), "ProductCategoryId", "Title");
*/
            return View(model);
        }
        //XÓA 
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = _db.Products.Find(id);
            if (item != null)
            {
                var products = _db.OrderDetails.Where(x => x.ProductId == id).ToList();
                if (products.Count() > 0)
                {
                    return Json(new { success = false });
                }

                //xoá ảnh
                var proImg = _db.ProductImages.Where(x => x.ProductId == id).ToList();
                if (proImg.Any())
                {
                    _db.RemoveRange(proImg);
                }

                _db.Products.Remove(item);
                _db.SaveChanges();
                return Json(new { success = true });
            }
            return Json(new { success = false });

        }



        //HIỆN THỊ
        [HttpPost]
        public ActionResult IsActive(int id)
        {
            var item = _db.Products.Find(id);
            if (item != null)
            {
                item.IsActive = !item.IsActive;
                _db.SaveChanges();
                return Json(new { success = true, isAcive = item.IsActive });
            }

            return Json(new { success = false });
        }




        //HIỆN THỊ ở trang chủ
        [HttpPost]
        public ActionResult IsHome(int id)
        {
            var item = _db.Products.Find(id);
            if (item != null)
            {
                item.IsHome = !item.IsHome;
                _db.SaveChanges();
                return Json(new { success = true, ishome = item.IsHome });
            }

            return Json(new { success = false });
        }



        // khuyến mãi 
        [HttpPost]
        public ActionResult IsSale(int id)
        {
            var item = _db.Products.Find(id);
            if (item != null)
            {
                item.IsSale = !item.IsSale;

                _db.SaveChanges();
                return Json(new { success = true, isSale = item.IsSale });
            }

            return Json(new { success = false });
        }
    }
}
