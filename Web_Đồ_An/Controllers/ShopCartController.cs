
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Newtonsoft.Json;
using Web_Đồ_An.Models;
using Web_Đồ_An.Models.Entities;
namespace Web_Đồ_An.Controllers
{
	public class ShopCartController : Controller
	{
		private readonly AppDbContext _db;

		private List<ShopCart> carts = new List<ShopCart>();
		public ShopCartController(AppDbContext db)
		{
			_db = db;
		}
		public override void OnActionExecuting(ActionExecutingContext context)
		{
			var cartInSession = HttpContext.Session.GetString("My-Cart");
			if (cartInSession != null)
			{
				carts = JsonConvert.DeserializeObject<List<ShopCart>>(cartInSession);
			}
			base.OnActionExecuting(context);
		}
		public IActionResult Index()
		{
			decimal total = 0;
			var count = 0;
			foreach (var item in carts)
			{
				total += item.Quantity * item.PriceSale;
				count++;
                // Lấy số lượng sản phẩm trong Products
                var product = _db.Products.FirstOrDefault(p => p.ProductId == item.ProductId);
                if (product != null)
                {
                    item.MaxQuantity = product.Quantity;
                }
            }
			ViewBag.Total = total;
			ViewBag.Count = count;
			return View(carts);
		}


		
           
            public IActionResult Add(int id, int soluong = 1)
		{
			if (carts.Any(c => c.ProductId == id))
			{
				carts.Where(c => c.ProductId == id).First().Quantity += soluong;
			}
			 else
			{
				var p = _db.Products.FirstOrDefault(x => x.ProductId == id);

				var item = new ShopCart()
				{
					ProductId = p.ProductId,
					Title = p.Title,
					Price = (decimal)p.Price,
					PriceSale = (decimal)p.PriceSale,
					Quantity =soluong,
					Image = p.Image,
					TotalPrice = (decimal)p.PriceSale * 1
				};
				carts.Add(item);
				//đếm số sản phẩm vào giỏ hàng
				var totalQuantity = carts.Sum(c => c.Quantity);
				HttpContext.Session.SetInt32("CartTotalQuantity", totalQuantity);
				ViewBag.CartTotalQuantity = totalQuantity;

			}
			HttpContext.Session.SetString("My-Cart", JsonConvert.SerializeObject(carts));
			return Redirect(Request.Headers["Referer"].ToString());

		}

		public IActionResult Remove(int id)
		{
			if (carts.Any(c => c.ProductId == id))
			{
				var item = carts.Where(c => c.ProductId == id).First();
				carts.Remove(item);
				HttpContext.Session.SetString("My-Cart", JsonConvert.SerializeObject(carts));
			}
			return RedirectToAction("Index");
		}
		public IActionResult Update(int id, int quantity)
		{
			if (carts.Any(c => c.ProductId == id))
			{
				carts.Where(c => c.ProductId == id).First().Quantity = quantity;
				HttpContext.Session.SetString("My-Cart", JsonConvert.SerializeObject(carts));
			}
			return RedirectToAction("Index");
		}
		public IActionResult Clear()
		{
			HttpContext.Session.Remove("My-Cart");
			return RedirectToAction("Index");
		}
        /*  public IActionResult Checkout(int id)
          {

          }
  */
        public IActionResult Order()
        {
            //nếu chưa đăng nhập
            if (HttpContext.Session.GetString("Member") == null)
            {
                return Redirect("/Customer/Login/?url=/Shopcart/orders");
            }
            else
            {
                var dataMember = JsonConvert.DeserializeObject<Customer>(HttpContext.Session.GetString("Member"));
                ViewBag.Customer = dataMember;

                decimal total = 0;
                foreach (var item in carts)
                {
                    if (item.PriceSale > 0 && item.PriceSale < item.Price)
                    {
                        total += item.Quantity * item.PriceSale;
                    }
                    else
                    {
                        total += item.Quantity * item.Price;
                    }
                    ViewBag.Total = total;
                  }
            }
            return View(carts);
        }

        public async Task<IActionResult> OrderPay(IFormCollection form)
        {
            try
            {
                //thêm bảng orders hiện theo form 
                var order = new Order();
                order.NameReciver = form["NameReciver"];
                order.Address = form["Address"];
                order.Phone = form["Phone"];
                order.Notes = form["Notes"];
                order.TypePayment = int.Parse(form["TypePayment"]);
                order.CreatedDate = DateTime.Now;

                var dataMember = JsonConvert.DeserializeObject<Customer>(HttpContext.Session.GetString("Member"));
                order.CustomerID = dataMember.CustomerID;
                decimal total = 0;
                foreach (var item in carts)
                {
                    if (item.PriceSale > 0 && item.PriceSale < item.Price)
                    {
                        total += item.Quantity * item.PriceSale;
                    }
                    else
                    {
                        total += item.Quantity * item.Price;
                    }

                }
                //tổng tiền 
                   order.TotalAmount = total;

                    //tạo OrderId mã code 
                    var strOrderId = "DH";

                    string times = DateTime.Now.ToString("yyyy-MM-dd.HH-mm-ss.fff");
                    strOrderId += "." + times;
                    order.Code = strOrderId;

                    _db.Add(order);
                    await _db.SaveChangesAsync();

                    //Lấy id bảng order, orderdetail so sánh vs  shopcar
                    var dataOrder = _db.Orders.OrderByDescending(x => x.OderId).FirstOrDefault();
                    foreach (var items in carts)
                    {
                        OrderDetail orderDetails = new OrderDetail();
                     orderDetails.Code = dataOrder.Code;
                       orderDetails.OrderId = dataOrder.OderId;
                        orderDetails.ProductId = items.ProductId;
                        orderDetails.Quantity = items.Quantity;
                        orderDetails.Price = items.Price;
                        orderDetails.TotalMoney = items.TotalPrice;

                     //trừ số lượng đã thêm 
                    var quatityPro = _db.Products.Where(x => x.ProductId == items.ProductId).FirstOrDefault().Quantity;
                    _db.Products.Where(x => x.ProductId == items.ProductId).FirstOrDefault().Quantity = quatityPro - items.Quantity;
                    _db.Add(orderDetails);
                    await _db.SaveChangesAsync();
                    }
                    HttpContext.Session.Remove("My-Cart");
                }
            
            catch (Exception ex)
            {
                throw;
            }
            TempData["success"] = "Thanh toán thành công";
            return RedirectToAction("Index", "ShopCart");
        }
    }

}
