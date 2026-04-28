using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAn.Models
{
    [Table("tblKhachHang")]
    public class tblKhachHang
    {
        [Key]
        public int KH_ID { get; set; }
        public string? KH_HinhAnh { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên khách hàng!")]
        public string KH_TenKhach { get; set; } = "";
        public string KH_Email { get; set; } = "";
        public string KH_MatKhau { get; set; } = "";
        public string? KH_DienThoai { get; set; }
        public string? KH_VaiTro { get; set; }
        public bool KH_TrangThai { get; set; }
    }
}