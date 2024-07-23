using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Web_Đồ_An.Models.Entities;
using Web_Đồ_An.Models;

namespace Web_Đồ_An.Controllers
{
    public class FavoriteController : Controller
    {
        private readonly AppDbContext _db;

        private List<Favorite> favorites = new List<Favorite>();

        private readonly ILogger<FavoriteController> _logger;

        public FavoriteController(AppDbContext db, ILogger<FavoriteController> logger)
        {
            _db = db;
            _logger = logger;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var favoriteInSession = HttpContext.Session.GetString("My-Favorite");
            if (favoriteInSession != null)
            {
                // nếu cartInSession không null thì gán dữ liệu cho biến carts
                // Chuyển san dữ liệu json
                favorites = JsonConvert.DeserializeObject<List<Favorite>>(favoriteInSession);
            }
            base.OnActionExecuting(context);
        }

        public IActionResult Add()
        {
            var customer = JsonConvert.DeserializeObject<Customer>(HttpContext.Session.GetString("Member"));
            if (customer == null)
            {
                // Khách hàng chưa đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login", "Customer");
            }
            var item = _db.Favorites.Where(x => x.CustomerID == customer.CustomerID).ToList();
            return View(item);
        }

        public IActionResult AddFavorite(int? productId)
        {
            if (HttpContext.Session.GetString("Member") != null)
            {
                // Session tồn tại, tiếp tục DeserializeObject
                var customer = JsonConvert.DeserializeObject<Customer>(HttpContext.Session.GetString("Member"));
                // Tiếp tục xử lý
            }
            else
            {
                // Khách hàng chưa đăng nhập, chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login", "Customer", new { url = Url.Action("Add", new { productId }) });
            }
            var customers = JsonConvert.DeserializeObject<Customer>(HttpContext.Session.GetString("Member"));
            bool alreadyExists = _db.Favorites.Any(f => f.ProductId == productId && f.CustomerID == customers.CustomerID);

            if (!alreadyExists)
            {
                var p = _db.Products.FirstOrDefault(x => x.ProductId == productId);

                var item = new Favorite()
                {
                    ProductId = p.ProductId,
                    CustomerID = customers.CustomerID,
                    Name = p.Title,
                    CreatedDate = DateTime.Now

                };
                _db.Favorites.Add(item);
                _db.SaveChanges();
                return RedirectToAction("Add");
            }
            return RedirectToAction("Index", "Products");
        }
    }
}
