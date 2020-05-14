using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Prj_ProjectMaster")]
    public partial class PrjProjectMaster
    {
        public PrjProjectMaster()
        {
            PrjPaymentReceipt = new HashSet<PrjPaymentReceipt>();
        }

        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_ProjectID")]
        public int NProjectId { get; set; }
        [Column("X_ProjectCode")]
        [StringLength(50)]
        public string XProjectCode { get; set; }
        [Column("X_ProjectDescr")]
        [StringLength(100)]
        public string XProjectDescr { get; set; }
        [Column("B_Inactive")]
        public bool? BInactive { get; set; }
        [Column("D_StartDate", TypeName = "datetime")]
        public DateTime? DStartDate { get; set; }
        [Column("D_StopDate", TypeName = "datetime")]
        public DateTime? DStopDate { get; set; }
        [Column("D_EstimatedStopDate", TypeName = "datetime")]
        public DateTime? DEstimatedStopDate { get; set; }
        [Column("X_Remarks")]
        [StringLength(100)]
        public string XRemarks { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }

        [InverseProperty("NProject")]
        public virtual ICollection<PrjPaymentReceipt> PrjPaymentReceipt { get; set; }
    }
}
