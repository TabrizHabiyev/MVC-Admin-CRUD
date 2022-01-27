using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FrontToBack.Models
{
    public class Comments
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        public string UserId { get; set; }
        public virtual AppUser User { get; set; }
        public DateTime Date { get; set; }
    }
}
