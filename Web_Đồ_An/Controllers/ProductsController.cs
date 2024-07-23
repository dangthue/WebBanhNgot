using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Đồ_An.Models;
using Web_Đồ_An.Models.Entities;
using X.PagedList;

namespace Web_Đồ_An.Controllers
{
    public class ProductsController : Controller
    {
        private readonly AppDbContext _db;

        public ProductsController(AppDbContext db)
        {
            _db = db;
        }

      
        public ActionResult Index(string Searchtext, int? page)
        {
            var pageSize = 8;
            if (page == null)
            {
                page = 1;
            }
            IEnumerable<Product> items = _db.Products.ToList();
           /* if (!string.IsNullOrEmpty(Searchtext))
            {
                items = _db.News.Where(x => x.Alias.Contains(Searchtext) || x.Title.Contains(Searchtext));
            }*/
            var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
            items = items.ToPagedList(pageIndex, pageSize);
            ViewBag.PageSize = pageSize;
            ViewBag.Page = page;
            return View(items);
        }


        public ActionResult Detail(string alias, int id)
		{

            var products = _db.Products.Include(p => p.ProductCategory).ToList();
            var productImg = _db.ProductImages.Include(p => p.Product).Where(x => x.ProductId == id).ToList();
            ViewBag.productImg = productImg;

            var item = _db.Products.Find(id) ;
            if (item != null)
			{
				_db.Products.Attach(item);
				item.ViewCount = item.ViewCount + 1;
				_db.Entry(item).Property(x => x.ViewCount).IsModified = true;
				_db.SaveChanges();
			}

			return View(item);
		}




		/*  public ActionResult ProductCategory(string alias, int id, string name, int page = 1)
		  {
			  int limit = 8;
			  var items = _db.Products.ToPagedList(page, limit);
			  if (id > 0)
			  {
				  items = items.Where(x => x.ProductCategoryId == id).ToPagedList(page, limit);
			  }
			  var cate = _db.ProductCategories.Find(id);
			  if (cate != null)
			  {
				  ViewBag.CateName = cate.Title;
			  }

			  ViewBag.CateId = id;
			  return View(items);
		  }
  */
		public ActionResult ProductCategory(string alias, int id, int ? page )
		{
			var pageSize = 10;
			if (page == null)
			{
				page = 1;
			}
			IEnumerable<Product> items = _db.Products.ToList();
			if (id > 0)
			{
				items = items.Where(x => x.ProductCategoryId == id).ToList();
			}
			var cate = _db.ProductCategories.Find(id);
			if (cate != null)
			{
				ViewBag.CateName = cate.Title;
			}

			ViewBag.CateId = id;
			var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
			items = items.ToPagedList(pageIndex, pageSize);
			ViewBag.PageSize = pageSize;
			ViewBag.Page = page;
			return View(items);
			
			
		}
	}
}
