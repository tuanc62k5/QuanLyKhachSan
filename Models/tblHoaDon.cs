using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAn.Models
{
    [Table("tblHoaDon")]
    public class tblHoaDon
    {
        [Key]
        public int HD_ID { get; set; }

        [Required]
        public int DP_ID { get; set; }

        [Column(TypeName = "decimal(10,0)")]
        public decimal HD_TienPhong { get; set; }

        [Column(TypeName = "decimal(10,0)")]
        public decimal HD_TienDichVu { get; set; }

        [Column(TypeName = "decimal(10,0)")]
        public decimal HD_TongTien { get; set; }

        public DateTime HD_NgayLap { get; set; } = DateTime.Now;

        public string? HD_TrangThai { get; set; }

        public string? HD_PhuongThuc { get; set; }

        [ForeignKey("DP_ID")]
        public virtual tblDatPhong? DatPhong { get; set; }
    }
}