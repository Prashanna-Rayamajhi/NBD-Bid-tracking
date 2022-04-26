using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace NBD_BID_SYSTEM.Models
{
    public class Client
    {
        //primary key
        public Client()
        {
            this.Projects = new HashSet<Project>();
        }
        public int ID { get; set; }

        [Display(Name = "Phone")]
        public string PhoneFormatted
        {
            get
            {
                return "(" + PhoneNumber.Substring(0, 3) + ") " + PhoneNumber.Substring(3, 3) + "-" + PhoneNumber[6..]; 
            }
        }
        
        [Display(Name = "Contact Person")]
        public string CpFullName
        {
            get
            {
                return CpFName + " " + CpLName;
            }
        }

        [Required]
        [MaxLength(50, ErrorMessage = "Institution Name cannto be longer than 50 characters")]
        [Display(Name = "Client Name")]
        public string  Name { get; set; }

        //Cp ==> Contact Person
        [Required]
        [MaxLength(25, ErrorMessage = "Contact Person First Name cannot be longer than 25 characters")]
        [Display(Name= "Contact Person First Name")]
        public string CpFName { get; set; }

        [Required]
        [MaxLength(25, ErrorMessage = "Contact Person First Name cannot be longer than 25 characters")]
        [Display(Name= "Contact Person Last Name")]
        public string CpLName { get; set; }

        [Required]
        [MaxLength(30, ErrorMessage = "Position of Contact person cannot be longer than 30 characters")]
        [Display(Name = "Contact Person Position")]
        public string CpPosition { get; set; }

        [Required]
        [MaxLength(30, ErrorMessage = "Address cannot be longer than 30 characters")]
        [Display(Name = "Address")]
        public string Address { get; set; }

        [Required]
        [MaxLength(30, ErrorMessage = "City cannot be longer than 30 characters")]
        [Display(Name = "City")]
        public string City { get; set; }
        
        //Foreign Key
        [Required]
        [Display(Name = "Province")]
        public int ProvinceID { get; set; }
        public Province Province { get; set; }

        [Required]
        [MaxLength(7, ErrorMessage = "Postal Code cannot be longer than 7 characters")]
        [RegularExpression("^[ABCEGHJKLMNPRSTVXY][0-9][ABCEGHJKLMNPRSTVWXYZ] ?[0-9][ABCEGHJKLMNPRSTVWXYZ][0-9]", ErrorMessage = "Provide a postal code in proper format")]
        [Display(Name = "Postal Code")]
        public string PostalCode { get; set; }

        [Required]
        [MaxLength(10, ErrorMessage = "Phone Number cannot be longer than 10 digits")]
        [Display(Name = "Contact Number")]
        public string PhoneNumber { get; set; }

        //child relations
        [Display(Name = "Projects")]
        public ICollection<Project> Projects { get; set; }
    }
}
