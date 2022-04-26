using System.ComponentModel.DataAnnotations;
using System;

namespace NBD_BID_SYSTEM.Models
{
    public class Staff
    {
        public Staff()
        {
            Active = true;
        }
        public int ID { get; set; }

        [Display(Name = "Staff")]
        public string FullName
        {
            get
            {
                return FirstName + " " + LastName;
            }
        }
        [Display(Name = "Phone")]
        public string PhoneNumber
        {
            get
            {
                if (String.IsNullOrEmpty(Phone))
                {
                    return "";
                }
                else
                {
                    return "(" + Phone.Substring(0, 3) + ") " + Phone.Substring(3, 3) + "-" + Phone.Substring(6, 4);
                }
            }
        }

        [Display(Name = "Staff First Name")]
        [Required(ErrorMessage = "You cannot leave the first name blank.")]
        [StringLength(50, ErrorMessage = "First name cannot be more than 50 characters long.")]
        public string FirstName { get; set; }

        [Display(Name = "Staff Last Name")]
        [Required(ErrorMessage = "You cannot leave the last name blank.")]
        [StringLength(100, ErrorMessage = "Last name cannot be more than 50 characters long.")]
        public string LastName { get; set; }

        [Display(Name = "PhoneNum")]
        [Required(ErrorMessage = "10 digit code for staffPhoneNumber is required")]
        [RegularExpression("^\\d{10}$", ErrorMessage = "The staffPhoneNumber must be exactly 10 numeric digits.")]
        [StringLength(10)]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Email Address is required.")]
        [StringLength(255)]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public bool Active { get; set; }


        [Display(Name = "Postion")]
        [Required(ErrorMessage = "Postion is required field")]
        public int PositionID { get; set; }
        public Position Position { get; set; }

    }
}
