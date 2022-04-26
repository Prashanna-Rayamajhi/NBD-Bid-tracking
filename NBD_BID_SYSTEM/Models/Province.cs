using System.ComponentModel.DataAnnotations;
using System;

namespace NBD_BID_SYSTEM.Models
{
    public class Province
    {
        public int ID { get; set; }
        
        [Display(Name = "Province Name")]
        [MaxLength(30, ErrorMessage = "Name of Province cannot be longer than 30 characters")]
        public string Name { get; set; }
        
        [Display(Name = "Abbrevation")]
        [MaxLength(2, ErrorMessage = "Abbrevation for province cannto be longer thant 2 characters")]
        public string Abbrevation { get; set; }

        //Summary Property
        public string ProvinceFormatted
        {
            get
            {
                return $"{this.Name}, {this.Abbrevation}";
            }
        }

    }
}