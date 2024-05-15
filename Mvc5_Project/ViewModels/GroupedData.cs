using Mvc5_Project.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Mvc5_Project.ViewModels
{
    public class GroupedData
    {
        public string Key { get; set; } 
        public IEnumerable<Sale> Data { get; set; } = new List<Sale>();
    }
}