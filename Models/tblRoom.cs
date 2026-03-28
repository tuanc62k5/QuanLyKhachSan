using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAn.Models
{
    [Table("tblRoom")]
    public class tblRoom
    {
        [Key]
        public int RoomID { get; set; }
        public string? RoomName { get; set; }
        public double Price { get; set; }
        public string? Image { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
    }
}