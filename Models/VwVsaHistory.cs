using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwVsaHistory
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_RegId")]
        public int? NRegId { get; set; }
        [Column("X_RegCode")]
        [StringLength(50)]
        public string XRegCode { get; set; }
        [Column("N_FileID")]
        public int? NFileId { get; set; }
        [Column("X_FileNo")]
        [StringLength(50)]
        public string XFileNo { get; set; }
        [Column("D_Entrydate")]
        [StringLength(4000)]
        public string DEntrydate { get; set; }
        [Column("X_StatusName")]
        [StringLength(500)]
        public string XStatusName { get; set; }
        [Column("X_StatusDescription")]
        [StringLength(500)]
        public string XStatusDescription { get; set; }
        [Column("N_UserId")]
        public int? NUserId { get; set; }
        [Column("X_UserID")]
        [StringLength(50)]
        public string XUserId { get; set; }
        [Column("D_Entrydate1", TypeName = "datetime")]
        public DateTime? DEntrydate1 { get; set; }
    }
}
