
using Web_Đồ_An.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Web_Đồ_An.Models.Entities
{
    public class AppDbContext : IdentityDbContext<AppUserModel>
    {
		public AppDbContext()
		{
		}

		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<News> News { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<ProductCategory> ProductCategories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
    }
}
