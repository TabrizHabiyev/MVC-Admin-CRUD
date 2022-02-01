using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontToBack.Models
{
    public class Sales
    {
        public int Id { get; set; }

        public double Total { get; set; }

        public string AppUserId { get; set; }

        public string CustomerName { get; set; }

        public string CustomerSurname { get; set; }

        public string CustomerEmail { get; set; }

        public string CustomerAddress { get; set; }

        public string CustomerAddress2 { get; set; }

        public string CustomerZipCode { get; set; }

        public DateTime SaleDate { get; set; }
        public AppUser AppUser { get; set; }
        public List<SalesProduct> SalesProducts { get; set; }

    }
}
