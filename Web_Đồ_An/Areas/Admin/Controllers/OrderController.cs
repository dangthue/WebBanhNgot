using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Web_Đồ_An.Models;
using Web_Đồ_An.Models.Entities;
using X.PagedList;

namespace Web_Đồ_An.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly AppDbContext _db;

        public object DataLocal { get; private set; }

        public OrderController(AppDbContext db)
        {
            _db = db;
        }
      
        public IActionResult Index(string name, int? TypePayment, int page = 1)
        {
            int limit = 10;
            IQueryable<Order> orders = _db.Orders.Include(p => p.Customer).OrderByDescending(p => p.OderId);

            if (!string.IsNullOrEmpty(name))
            {
                orders = orders.Where(x => x.Code.Contains(name));
            }

            // Lọc theo trạng thái
            if (TypePayment.HasValue && TypePayment > 0)
            {
                orders = orders.Where(x => x.TypePayment == TypePayment);
            }

            // Phân trang
            IPagedList<Order> pagedOrders = orders.ToPagedList(page, limit);

            // Chuẩn bị SelectList cho trạng thái
            var TypePayments = new List<SelectListItem>
    {
        new SelectListItem { Value = "1", Text = "Chờ thanh toán" },
        new SelectListItem { Value = "2", Text = "Đã thanh toán" },
        new SelectListItem { Value = "3", Text = "Hủy đơn hàng" }
        // Thêm các trạng thái khác nếu cần
    };
            ViewBag.TypePayment = new SelectList(TypePayments, "Value", "Text", TypePayment ?? 0);

            // Chuẩn bị SelectList cho trang
            ViewBag.PageSize = limit;
            ViewBag.Page = page;

            ViewBag.Keyword = name;

            return View(pagedOrders);
        }

        public ActionResult View(int id)
        {
			var item = _db.Orders.Find(id);

			var productDetail = _db.OrderDetails.Where(x => x.OrderId == id).Include(p => p.Product).ToList();
			ViewBag.productDetail = productDetail;

			return View(item);
		}

        [HttpPost]
        public ActionResult UpdateTT(int id, int trangthai)
        {
            var item = _db.Orders.Find(id);
            if (item != null)
            {
				if (trangthai == 3) // nếu là trạng thái 
				{
					var orderDetail = _db.OrderDetails.Where(x => x.OrderId == id).ToList();
					foreach (var detail in orderDetail)
					{
						var product = _db.Products.Find(detail.ProductId);
						if (product != null)
						{
							product.Quantity += detail.Quantity;
						}
					}
				}
				_db.Orders.Attach(item);
                item.TypePayment = trangthai;
                _db.Entry(item).Property(x => x.TypePayment).IsModified = true;
                _db.SaveChanges();
                return Json(new { message = "Success", su = true });
            }
            return Json(new { message = "Unsuccess", su = false });
        }
        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, int TypePayment)
        {
            var order = await _db.Orders.FindAsync(orderId);
            if (order != null)
            {
                if (TypePayment == 3) // nếu là trạng thái 
                {
                    var orderDetail = _db.OrderDetails.Where(x => x.OrderId == orderId).ToList();
                    foreach (var detail in orderDetail)
                    {
                        var product = _db.Products.Find(detail.ProductId);
                        if (product != null)
                        {
                            product.Quantity += detail.Quantity;
                        }
                    }
                }
                order.TypePayment = TypePayment;
                _db.Update(order);
                await _db.SaveChangesAsync();
                return Json(new { success = true });
            }
            return Json(new { success = false });
           
        }


    }

}
