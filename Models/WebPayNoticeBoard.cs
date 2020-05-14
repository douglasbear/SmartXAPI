using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Web_Pay_NoticeBoard")]
    public partial class WebPayNoticeBoard
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_NoticeBoardID")]
        public int NNoticeBoardId { get; set; }
        [Column("X_NBTitle")]
        [StringLength(100)]
        public string XNbtitle { get; set; }
        [Column("X_NBDesc")]
        [StringLength(500)]
        public string XNbdesc { get; set; }
        [Column("D_NBPostDate", TypeName = "datetime")]
        public DateTime? DNbpostDate { get; set; }
        [Column("B_Active")]
        public bool? BActive { get; set; }
        [Column("X_NoticeBoardCode")]
        [StringLength(50)]
        public string XNoticeBoardCode { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
