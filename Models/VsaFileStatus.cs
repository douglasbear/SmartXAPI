using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("vsa_FileStatus")]
    public partial class VsaFileStatus
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_StatusID")]
        public int NStatusId { get; set; }
        [Column("N_StageID")]
        public int NStageId { get; set; }
        [Column("X_StatusName")]
        [StringLength(500)]
        public string XStatusName { get; set; }
        [Column("N_Order")]
        public int? NOrder { get; set; }
        [Column("X_Description")]
        [StringLength(50)]
        public string XDescription { get; set; }
        [Column("X_StatusCode")]
        [StringLength(50)]
        public string XStatusCode { get; set; }
    }
}
