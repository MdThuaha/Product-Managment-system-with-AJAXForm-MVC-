using Mvc5_Project.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Mvc5_Project.ViewModels
{
    public class ProductInputModel
    {
        public int ProductId { get; set; }
        [Required, StringLength(50), Display(Name = "Product Name")]
        public string ProductName { get; set; }
        [Required, Column(TypeName = "money")]
        public decimal? Price { get; set; }
        [Required, Column(TypeName = "date"), Display(Name = "Ex. Date"), DisplayFormat(DataFormatString = "{0:yyyy-mm-dd}", ApplyFormatInEditMode = true)]
        public DateTime ExpireDate { get; set; }
        [Required]
        public HttpPostedFileBase Picture { get; set; }
        public bool InStock { get; set; }
        [Required]
        public int SellerId { get; set; }
        public List<Sale> Sales { get; set; } = new List<Sale>();
        public List <Seller> Sellers { get; set;} = new List<Seller>();
    }
}