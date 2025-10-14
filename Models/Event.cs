
//************************************************ start of file ***************************************************//


//------------------------------ start of imports ---------------------------------//
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

//---------------------------------- end of imports --------------------------------//

namespace Programming_7312_Part_1.Models
{
    public class Event
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // event id variable getter and setter

        [Required]
        [MaxLength(200)] // length restriction
        public string? Title { get; set; } // event title variable getter and setter

        [Required]
        [MaxLength(1000)] // length restriction to 10000 characters 
        public string? Description { get; set; } // description variable gettter and setter 
        
                                                 

        [Required]
        [MaxLength(100)] // total length restriction 100 characters
        public string? Category { get; set; } // category variable getter and setter 

        [Required]
        public DateTime EventDate { get; set; } // date getter and setter 

        [Required]
        [MaxLength(200)]
        public string? Location { get; set; } // location getter and setter 

        [MaxLength(500)]
        public string? ImagePath { get; set; } // image getter and setter 

        public DateTime CreatedDate { get; set; } = DateTime.Now; // setting the date to the current one 

        /*
         *
         * the below method is used to track the number of views an event has had
         *
         * this output feom the daata set to this is used to determin how popular a search is
         *
         * the event shaall be recommended based on whats popular 
         * 
         */
        public int ViewCount { get; set; } = 0;

        /*
         *
         * the below getter and setter for the upvotes system
         *
         * the upvote system is used to capture the amount of times an event has been invoked
         *
         * this is tested against the downvotes
         *
         * based on the outcome the popularity will be evaluated and the event will be recommended based on this
         *
         *
         * 
         */
        public int Upvotes { get; set; } = 0; // default set to zero so neutral 
        
        /*
         *
         * the inverse of the above method is true for this
         *
         * 
         * dislikes will result in the popularity being less popular
         *
         * 
         */
        public int Downvotes { get; set; } = 0;

        // For recommendation system
        public List<string> Tags { get; set; } = new List<string>();

        // For popular events based on searches
        public int SearchCount { get; set; } = 0;
    }
}

//********************************************************************** end of file *****************************************************//