using Microsoft.AspNetCore.Mvc;
using Web_Đồ_An.Models;
using Web_Đồ_An.Models.Entities;
using X.PagedList;

namespace Web_Đồ_An.Controllers
{
	public class NewsController : Controller
	{
		private readonly AppDbContext _db;

		public NewsController(AppDbContext db)
		{
			_db = db;
		}

	    public ActionResult Index(int? page)
		{
				var pageSize = 2;
				if (page == null)
				{
					page = 1;
				}
				IEnumerable<News> items = _db.News.OrderByDescending(x => x.CreatedDate);
				var pageIndex = page.HasValue ? Convert.ToInt32(page) : 1;
				items = items.ToPagedList(pageIndex, pageSize);
				ViewBag.PageSize = pageSize;
				ViewBag.Page = page;
				return View(items);
			}
		public ActionResult Detail(int id)
		{
			var item = _db.News.Find(id);
			return View(item);
		}
	}
}
