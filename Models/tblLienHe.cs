using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DoAn.Models
{
    [Table("tblLienHe")]
    public class LienHe
    {
        [Key]
        public int LH_ID { get; set; }

        public string? LH_TenKhach { get; set; }
        public string? LH_Email { get; set; }
        public string? LH_NoiDung { get; set; }
        public DateTime? LH_ThoiGian { get; set; }
        public bool LH_TrangThai { get; set; }
    }
}