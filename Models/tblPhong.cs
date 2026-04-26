using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAn.Models
{
    [Table("tblPhong")]
    public class tblPhong
    {
        [Key]
        public int P_ID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên phòng!")]
        public string P_TenPhong { get; set; } = "";
        public string P_LoaiPhong { get; set; } = "";

        [Required(ErrorMessage = "Vui lòng nhập giá phòng!")]
        public decimal P_GiaPhong { get; set; }
        public string? P_HinhAnh { get; set; }
        public string? P_MoTa { get; set; }
        public bool P_TrangThai { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập sức chứa!")]
        public int P_SucChua { get; set; }
    }
}