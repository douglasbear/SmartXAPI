using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("vsa_FileStatusHistory")]
    public partial class VsaFileStatusHistory
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime? DEntrydate { get; set; }
        [Key]
        [Column("N_FileStatusID")]
        public int NFileStatusId { get; set; }
        [Column("N_FileID")]
        public int NFileId { get; set; }
        [Column("N_StatusID")]
        public int NStatusId { get; set; }
        [Column("X_StatusDescription")]
        [StringLength(500)]
        public string XStatusDescription { get; set; }
        [Column("N_UserId")]
        public int? NUserId { get; set; }
    }
}
