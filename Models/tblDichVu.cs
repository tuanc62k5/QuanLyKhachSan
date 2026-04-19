using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAn.Models
{
    [Table("tblDichVu")]
    public class tblDichVu
    {
        [Key]
        public int DV_ID { get; set; }

        public string DV_TenDichVu { get; set; } = "";
        public decimal DV_GiaTien { get; set; }
        public string DV_MoTa { get; set; } = "";
        public string DV_HinhAnh { get; set; } = "";
        public bool DV_TrangThai { get; set; }
    }
}