using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("vsa_FileStages")]
    public partial class VsaFileStages
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_StageID")]
        public int NStageId { get; set; }
        [Column("X_StageName")]
        [StringLength(100)]
        public string XStageName { get; set; }
        [Column("N_Order")]
        public int? NOrder { get; set; }
        [Column("X_Description")]
        [StringLength(50)]
        public string XDescription { get; set; }
    }
}
