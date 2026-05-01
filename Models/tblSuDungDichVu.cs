using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAn.Models
{
    [Table("tblSuDungDichVu")]
    public class tblSuDungDichVu
    {
        [Key]
        public int SDDV_ID { get; set; }

        public int DP_ID { get; set; }

        public int DV_ID { get; set; }

        public int SDDV_SoLuong { get; set; } = 1;

        [Column(TypeName = "decimal(10,0)")]
        public decimal SDDV_ThanhTien { get; set; }

        public DateTime SDDV_NgaySuDung { get; set; } = DateTime.Now;

        public string? SDDV_TrangThai { get; set; }

        [ForeignKey("DP_ID")]
        public virtual tblDatPhong? DatPhong { get; set; }

        [ForeignKey("DV_ID")]
        public virtual tblDichVu? DichVu { get; set; }
    }
}