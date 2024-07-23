

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using Web_Đồ_An.Models.Entities;

namespace Web_Đồ_An.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class StatisticalController : Controller
    {

        private readonly AppDbContext _db;

        public object DataLocal { get; private set; }

        public StatisticalController(AppDbContext db)
        {
            _db = db;
        }
       
            public IActionResult Index()
            {
                var soLuongHD = _db.Orders.Count();
                ViewBag.soLuongHD = soLuongHD;
                var soLuongSP = _db.Products.Count();
                ViewBag.soLuongSP = soLuongSP;
                var soLuongDonCho = _db.Orders.Count(x => x.TypePayment == 1);

                ViewBag.soLuongDonCho = soLuongDonCho;
                var soLuongDonHT = _db.Orders.Count(x => x.TypePayment == 2);
                ViewBag.soLuongDonHT = soLuongDonHT;
                var soLuongDonHuy = _db.Orders.Count(x => x.TypePayment == 3);
                ViewBag.soLuongDonHuy = soLuongDonHuy;

                return View();
            }

        [HttpGet]
        public IActionResult GetThongKe(string fromDate, string toDate)
        {
            var query = from o in _db.Orders
                        join od in _db.OrderDetails
                        on o.OderId equals od.OrderId
                        join p in _db.Products
                        on od.ProductId equals p.ProductId

 
 where o.TypePayment == 2 & o.CreatedDate > DateTime.Now.AddDays(-7)
                        select new
                        {
                            CreatedDate = o.CreatedDate,
                            Quantity = od.Quantity,
                            Price = od.Price,
                            OriginalPrice = p.OriginalPrice
                        };

            if (!string.IsNullOrEmpty(fromDate))
            {
                DateTime startDate = DateTime.ParseExact(fromDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.CreatedDate >= startDate);
            }

            if (!string.IsNullOrEmpty(toDate))
            {
                DateTime endDate = DateTime.ParseExact(toDate, "dd/MM/yyyy", null);
                query = query.Where(x => x.CreatedDate < endDate);
            }

            var result = query.GroupBy(x => x.CreatedDate.Date).Select(x => new
            {
                Date = x.Key,
                TotalBuy = x.Sum(y => y.Quantity * y.OriginalPrice),
                TotalSell = x.Sum(y => y.Quantity * y.Price),
            }).Select(x => new
            {
                Date = x.Date,
                DoanhThu = x.TotalSell,
                LoiNhuan = x.TotalSell - x.TotalBuy
            }).ToList();

            return Json(new { Data = result });
        }

    }
}
