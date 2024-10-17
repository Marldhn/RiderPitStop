using RiderPitStop.Models;
using System.ComponentModel.DataAnnotations;

namespace RiderPitStop.Models
{
    public class RequestOrder
    {

        [Required]
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
        //ReCreate

        [Required]
        public int DetailsId { get; set; }
        public virtual RequestDetails requestDetails { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]

        public int Quantity { get; set; }

    }
}
