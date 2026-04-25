using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAn.Models
{
    [Table("tblDatPhong")]
    public class tblDatPhong
    {
        [Key]
        public int DP_ID { get; set; }

        public int? KH_ID { get; set; }

        [ForeignKey("KH_ID")]
        public tblKhachHang? KhachHang { get; set; }
        public string KH_TenKhach { get; set; } = "";
        public string KH_Email { get; set; } = "";
        public string KH_DienThoai { get; set; } = "";

        public int P_ID { get; set; }

        public DateTime DP_NgayNhan { get; set; }
        public DateTime DP_NgayTra { get; set; }

        public int DP_SoNguoi { get; set; }

        public decimal DP_TongTien { get; set; }

        public DateTime DP_NgayTao { get; set; }
    }
}