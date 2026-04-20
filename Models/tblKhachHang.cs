using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAn.Models
{
    [Table("tblKhachHang")]
    public class tblKhachHang
    {
        [Key]
        public int KH_ID { get; set; }
        public string KH_TenKhach { get; set; } = "";
        public string KH_Email { get; set; } = "";
        public string KH_MatKhau { get; set; } = "";
        public string? KH_DienThoai { get; set; }
    }
}