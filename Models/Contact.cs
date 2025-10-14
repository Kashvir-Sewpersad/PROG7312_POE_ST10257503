
//************************************************************** start of file ***************************************//


//------------------------------ start of imports ---------------------------------//
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//---------------------------------- end of imports --------------------------------//
namespace Programming_7312_Part_1.Models
{
    public class Contact
    {
        /*
         *
         * all required fields must be filled out or the submission process will not be sucessful
         *
         * this is to ensure that the input is integral and viable
         *
         * 
         * for fields which are nnot required the variables are either set to current such as date or not needed to run the code 
         *
         * there has been lenght limits added to prevent users crom crashing the file
         *
         * 
         * 
         */
        
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // id variable getter and setter 

        [Required]
        [MaxLength(100)] // length restriction 
        public string? Name { get; set; } // name variable getter and setter 

        [Required]
        [EmailAddress]
        [MaxLength(200)] // length restriction 
        public string? Email { get; set; } // email variable getter and setter 

        [Required]
        [MaxLength(500)] // max length 500 characters 
        public string? Subject { get; set; }

        [Required]
        [MaxLength(2000)] 
        public string? Message { get; set; } // message length 

        [MaxLength(20)]
        public string? Phone { get; set; } // phone number variable 

        [MaxLength(100)]
        public string? Category { get; set; } // catergory getter and setter 

        public DateTime CreatedDate { get; set; } = DateTime.Now; // daate set to the current one

        public bool IsRead { get; set; } = false;

        public bool IsResponded { get; set; } = false;
    }
}

//**************************************************************** end of file ***************************************************//