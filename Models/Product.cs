using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FrontToBack.Models
{
    public class Product
    {
        public int Id { get; set; }
        [Required,MinLength(5)]
       
      
        public string Name { get; set; }
        [MinLength(25,ErrorMessage ="Only 25 charachter")]
        public string ImageUrl { get; set; }
        public double Price { get; set; }
         [Required(ErrorMessage ="Please enter price")]
        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }

        [NotMapped]
        [Required]
        public IFormFile Photos { get; set; }
    }
}
