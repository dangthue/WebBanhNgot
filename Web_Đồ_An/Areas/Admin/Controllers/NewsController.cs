using Web_Đồ_An.Models.Entities;
using Web_Đồ_An.Models;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;
using System.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Web_Đồ_An.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class NewsController : Controller
    {
        private readonly AppDbContext _db;

        public object DataLocal { get; private set; }

        private readonly IWebHostEnvironment _env;

        public NewsController(AppDbContext db, IWebHostEnvironment env)
        {
            _db = db;
            _env = env;
        }
        public ActionResult Index(string Searchtext, int? page)
        {
            var pageSize = 5;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<News> items = _db.News.OrderByDescending(x => x.NewId);
            if (!string.IsNullOrEmpty(Searchtext))
            {
                items = _db.News.Where(x => x.Alias.Contains(Searchtext) || x.Title.Contains(Searchtext));
            }
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;
            return View(items);
        }
	



        public async Task<IActionResult> View(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _db.News
                
                .FirstOrDefaultAsync(m => m.NewId == id);
            if (product == null)
            {
                return NotFound();
            }

          
            return View(product);
        }
        //ckeditor
        public IActionResult UploadImage(List<IFormFile> files)
        {
            var filepath = "";
            foreach (IFormFile photo in Request.Form.Files)
            {
                string serverMapPath = Path.Combine(_env.WebRootPath, "Images", photo.FileName);
                using (var stream = new FileStream(serverMapPath, FileMode.Create))
                {
                    photo.CopyTo(stream);
                }
                filepath = "https://localhost:7286/" + "Images/" + photo.FileName;
            }
            return Json(new { url = filepath });
        }


        //THÊM 
        [HttpGet]
        public IActionResult ThemTinTuc()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ThemTinTuc(News TinTuc)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count() > 0 && files[0].Length > 0) // kiểm tra xem tập có đc gửi từ file lên không 
                {
                    var file = files[0];
                    var FileName = file.FileName;
                    // upload ảnh vào thư mục wwwroot\\images\\category
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Content\\Images\\tintuc", FileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                        TinTuc.Image = "/Content/Images/tintuc/" + FileName; // gán tên ảnh cho thuộc tinh Image
                    }

                }
                TinTuc.CreatedDate = DateTime.Now;
				TinTuc.Alias = Web_Đồ_An.Models.Common.Filter.FilterChar(TinTuc.Title);

				_db.Add(TinTuc);
                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(TinTuc);
        }


      
        //SỬA 
        [HttpGet]
        public IActionResult SuaTinTuc(int id)
        {

            var items = _db.News.Find(id);
            return View(items);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SuaTinTuc( News TinTuc)
        {
            if (ModelState.IsValid)
            {
                var files = HttpContext.Request.Form.Files;
                if (files.Count() > 0 && files[0].Length > 0) // kiểm tra xem tập có đc gửi từ file lên không 
                {
                    var file = files[0];
                    var FileName = file.FileName;
                    // upload ảnh vào thư mục wwwroot\\images\\category
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\Content\\Images\\tintuc", FileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        file.CopyTo(stream);
                        TinTuc.Image = "/Content/Images/tintuc/" + FileName; // gán tên ảnh cho thuộc tinh Image
                    }

                }
        
                _db.News.Attach(TinTuc);
                TinTuc.CreatedDate = DateTime.Now;
                TinTuc.ModifiedDate = DateTime.Now;
				TinTuc.Alias = Web_Đồ_An.Models.Common.Filter.FilterChar(TinTuc.Title);

				_db.Entry(TinTuc).Property(x => x.Title).IsModified = true;
                _db.Entry(TinTuc).Property(x => x.Description).IsModified = true;
                _db.Entry(TinTuc).Property(x => x.ModifiedDate).IsModified = true;
                _db.Entry(TinTuc).Property(x => x.IsActive).IsModified = true;
                _db.Entry(TinTuc).Property(x => x.Detail).IsModified = true;


                _db.SaveChanges();
                return RedirectToAction(nameof(Index));
            }

            return View(TinTuc);
        }
        //XÓA 

        [HttpPost]
        public ActionResult Delete(int id)
        {
            var item = _db.News.Find(id);
            if (item != null)
            {
                _db.News.Remove(item);
                _db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false });
        }
        //HIỆN THỊ
        [HttpPost]
        public ActionResult IsActive(int id)
        {
            var item = _db.News.Find(id);
            if (item != null)
            {
                item.IsActive = !item.IsActive;
                _db.SaveChanges();
                return Json(new { success = true, isAcive = item.IsActive });
            }

            return Json(new { success = false });
        }
       

    }
}