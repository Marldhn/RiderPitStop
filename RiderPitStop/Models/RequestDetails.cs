using RiderPitStop.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RiderPitStop.Models
{
    public class RequestDetails
    {
        [Key]
        [Required]
        public int DetailsId { get; set; }

        public DateTime DateTime { get; set; }

        public string Note { get; set; }

        public string PreparedBy { get; set; }

        public bool IsComplete { get; set; }


        public virtual ICollection<RequestOrder> RequestOrders { get; set; }
    }
}
