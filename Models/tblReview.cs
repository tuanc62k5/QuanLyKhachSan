using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAn.Models
{
    [Table("tblReview")]
    public class tblReview
    {
        [Key]
        public int ReviewID { get; set; }
        public int RoomID { get; set; }
        public string? UserName { get; set; }
        public string? Comment { get; set; }
        public int Rating { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}