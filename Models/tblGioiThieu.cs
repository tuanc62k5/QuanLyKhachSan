using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAn.Models
{
    [Table("tblGioiThieu")]
    public class tblGioiThieu
    {
        [Key]
        public int GT_ID { get; set; }
        public int P_ID { get; set; }
        public string GT_TieuDe { get; set; } = "";
        public string GT_NoiDung { get; set; } = "";
        public DateTime GT_NgayTao { get; set; }
        public bool GT_TrangThai { get; set; }
    }
}