using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAn.Models
{
    [Table("tblthongbao")]
    public class ThongBao
    {
        [Key]
        public int TB_ID { get; set; }

        public string? TB_NoiDung { get; set; }

        public DateTime? TB_ThoiGian { get; set; }

        public bool TB_TrangThai { get; set; } // 0 = chưa đọc
    }
}