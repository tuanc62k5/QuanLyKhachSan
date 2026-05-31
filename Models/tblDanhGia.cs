using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAn.Models
{
    [Table("tblDanhGia")]
    public class tblDanhGia
    {
        [Key]
        public int DG_ID { get; set; }

        public int P_ID { get; set; }
        public int KH_ID { get; set; }

        public int DG_Sao { get; set; }
        public string? DG_NoiDung { get; set; }

        public DateTime DG_NgayTao { get; set; }

        [ForeignKey("P_ID")]
        public tblPhong? Phong { get; set; }

        [ForeignKey("KH_ID")]
        public tblKhachHang? KhachHang { get; set; }
    }
}