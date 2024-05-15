using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Mvc5_Project.Models
{
        public class Seller
        {
            public int SellerId { get; set; }
            [Required, StringLength(50), Display(Name = "Saller Name")]
            public string SallerName { get; set; }
            [Required, StringLength(50), Display(Name = "SellerAddress")]
            public string SellerAddress { get; set; }
            public virtual ICollection<Product> Products { get; set; } = new List<Product>();
            
        }
        public class Product
        {
            public int ProductId { get; set; }
            [Required, StringLength(50), Display(Name = "Product Name")]
            public string ProductName { get; set; }
            [Required, Column(TypeName = "money")]
            public decimal? Price { get; set; }
            [Required, Column(TypeName = "date"), Display(Name = "Ex. Date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            public DateTime ExpireDate { get; set; }
            [Required, StringLength(40)]
            public string Picture { get; set; }
            public bool InStock { get; set; }
            [Required, ForeignKey("Seller")]
            public int SellerId { get; set; }
            public virtual Seller Seller { get; set; }
            public virtual ICollection<Sale> Sales { get; set; } = new List<Sale>();

        }

        public class Sale
        {
            public int SaleId { get; set; }
            [Required, Column(TypeName = "date"), Display(Name = "Sale Date"), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
            public DateTime SaleDate { get; set; }
            [Required]
            public int Quantity { get; set; }
            [Required, ForeignKey("Product")]
            public int ProductId { get; set; }
            public virtual Product Product { get; set; }
            

        }

        public class ProductDbContext : DbContext
        {
            public ProductDbContext()
            {
                Configuration.LazyLoadingEnabled = false;
            }
            public DbSet<Seller> Sellers { get; set; }
            public DbSet<Product> Products { get; set; }
            public DbSet<Sale> Sales { get; set; }
        }
    
}