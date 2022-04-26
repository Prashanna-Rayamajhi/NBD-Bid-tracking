using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NBD_BID_SYSTEM.Models
{
    public class Inventory
    {

        public int ID { get; set; }
        [Required(ErrorMessage = "Inventory Code is required")]
        [Display(Name ="Inventory Code")]
        [MaxLength(10, ErrorMessage ="Code cannot be longer than 10 characters")]
        public string Code { get; set; }

        [Required(ErrorMessage = "Name cannot be empty")]
        [Display(Name = "Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Description is required field")]
        [Display(Name = "Description")]
        [MaxLength(30, ErrorMessage = "Description cannot be longer than 30 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Size is required field")]
        [Display(Name = "Size")]
        [MaxLength(30, ErrorMessage = "Size cannot be longer than 10 characters")]
        public string Size { get; set; }

        [Required(ErrorMessage = "Price is required field")]
        [Display(Name = "Price")]
        public double Price { get; set; }

        [Display(Name ="Inventory Type")]
        public int InventoryTypeID { get; set; }

        public InventoryType InventoryType { get; set; }

        public ICollection<Material> Materials { get; set; }
    }
}
