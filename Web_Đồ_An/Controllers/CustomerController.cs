
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Protocol;
using Web_Đồ_An.Models;
using Web_Đồ_An.Models.EF;
using Web_Đồ_An.Models.Entities;

namespace Web_Đồ_An.Controllers
{
    public class CustomerController : Controller
    {
        private readonly AppDbContext _context;
        public CustomerController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Login(string url)
        {
            if (HttpContext.Session.GetString("Member") != null)
            {
                var dataLogin = JsonConvert.DeserializeObject<Customer>(HttpContext.Session.GetString("Member"));
                ViewBag.Customer = dataLogin;
            }
            ViewBag.UrlAction = url;
            return View();
        }

        [HttpPost]
        public IActionResult Login(Customer model, string urlAction)
        {
            var pass = Utilitties.Utils.GetSHA26Hash(model.Password);
            var data = _context.Customers.Where(x => x.IsActive == true).Where(x => x.Email.Equals(model.Email))
                .FirstOrDefault(x => x.Password.Equals(pass));
            var dataLogin = data.ToJson();
            if (data != null)
            {
                //lưu session khi đăng nhập thành công
                HttpContext.Session.SetString("Member", dataLogin);
                if (!string.IsNullOrEmpty(urlAction))
                {
                    return RedirectToAction(urlAction);
                }
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Email hoặc mật khẩu khôg đúng! Vui lòng nhập lại");
                return View(model);
            }
            //ViewData["errorLogin"] = "Lỗi đăng nhập";
            return RedirectToAction("Login");
        }
        public IActionResult Registy()
        {
            Customer model = new Customer();
            return View(model);
        }
        [HttpPost]
        public IActionResult Registy(Customer model)
        {
            try
            {
                var taiKhoan = _context.Customers.Where(x => x.Email == model.Email).FirstOrDefault();
                if (taiKhoan == null)
                {
                    var pass = Utilitties.Utils.GetSHA26Hash(model.Password);
                    model.Password = pass;
                    model.CreateDate = DateTime.Now;
                    model.IsActive = true;
                    _context.Add(model);
                    _context.SaveChanges();
                    return RedirectToAction("Login", "Customer");
                }
                else
                {
                    //TempData["errorRegisty"] = "Email đã được sử dụng! Vui lòng sử dụng email khác";
                    //return View();
                    ModelState.AddModelError("Email", "Email đã được sử dụng! Vui lòng sử dụng email khác");
                    return View(model);
                }

            }
            catch (Exception ex)
            {
                return RedirectToAction("Registy");
            }
        }

        // edit lưu thông tin khách hàng rồi đẩy lên detail

        [HttpPost]
        public IActionResult Edit(Customer model)
        {
            if (ModelState.IsValid)
            {
                // Lấy đối tượng Customer hiện tại từ cơ sở dữ liệu
                var customer = _context.Customers.Find(model.CustomerID);
                if (customer == null)
                {
                    return NotFound();
                }
                // Cập nhật thông tin từ form vào đối tượng Customer
                customer.Fullname = model.Fullname;
                customer.Email = model.Email;
                customer.Address = model.Address;
                customer.Phone = model.Phone;
                customer.Brithday = model.Brithday;
                customer.Avatar = model.Avatar;

                _context.SaveChanges();

                //Lưu lại thông tin vào session
                var updatedCustomer = JsonConvert.SerializeObject(customer);
                HttpContext.Session.SetString("Member", updatedCustomer);

                return RedirectToAction("Detail");
            }
            return View("Detail", model);
        }
        //xem thông tin đơn tài khoản lấy bảng order View

        /*  public IActionResult Detail()
          {
              var customer = JsonConvert.DeserializeObject<Customer>(HttpContext.Session.GetString("Member"));

              if (customer != null)
              {
                  var order = _context.Orders
                                           .Where(x => x.CustomerID == customer.CustomerID)
  ?                                         .ToList();

                  var orderViews = new List<OrderView>();
                  foreach (var item in order)
                  {
                      var orderView = new OrderView()
                      {
                          OrderId = item.OderId,
                          CodeOrder = item.Code,
                          OrderDate = item.CreatedDate,
                          TotalMoney = item.TotalAmount, //tổng tiền 
                          ReceiveName = item.NameReciver,
                          ReceiveAddress = item.Address,
                          ReceivePhone = item.Phone,
                          Notes = item.Notes,
                          CustomerName = customer.Fullname,
                          PaymentName =item.TypePayment, //trạng đơn hàng 
                      };
                      orderViews.Add(orderView);
                  }

                  var orderDetails = _context.OrderDetails
                                             .Where(od => order.Select(ob => ob.OderId).Contains(od.OrderId))
                                             .Include(od => od.Product)
                                             .ToList();

                  var orderDetailsViews = orderDetails.Select(od => new ProductDetailViewModel
                  {
                      OrderDetailId = od.OderDetailId,
                      ProductName = od.Product.Title,
                      Quantity = od.Quantity,
                      Price = od.Price,
                      TotalMoney = od.TotalMoney, //giá bán cuối 
                      OrderId = od.OrderId,
                      Description = od.Product.Description,
                      Images = od.Product.Image,
                  }).ToList();

                  ViewBag.orderViews = orderViews;
                  ViewBag.orderDetailsViews = orderDetailsViews;
                  return View(customer);
              }

              return View("Home");
          }


        */
        //xem thông tin đơn tài khoản
        public IActionResult Detail()
        {
            var customer = JsonConvert.DeserializeObject<Customer>(HttpContext.Session.GetString("Member"));

            if (customer != null)
            {
                var order=  _context.Orders
                                         .Where(x => x.CustomerID == customer.CustomerID)
                                         
                                         .ToList();
             
                ViewBag.order = order;
                return View(customer);
            }
            return View("Home");
        }
        //thông tin chi tiết đơn hàng
        public IActionResult orderDetail(int id)
        {
            var customer = JsonConvert.DeserializeObject<Customer>(HttpContext.Session.GetString("Member"));

            if (customer != null)
            {
                var orderDetails = _context.OrderDetails
                                         .Where(x => x.OrderId == id)
                                         .Include(p => p.Product)
                                         .Include(p => p.Order)
                                         .ToList();
                var order = _context.Orders.FirstOrDefault(x => x.OderId == id);
                ViewBag.totalMoney = order.TotalAmount;
                var orders = _context.Orders.Where(x => x.CustomerID == customer.CustomerID).ToList();
                ViewBag.orders = orders;
                return View(orderDetails);
            }
            return View("Home");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Member");
            return RedirectToAction("Index","Home");
        }


    }
}
