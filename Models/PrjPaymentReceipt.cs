using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Prj_PaymentReceipt")]
    public partial class PrjPaymentReceipt
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_TransID")]
        public int NTransId { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("N_ProjectClientID")]
        public int? NProjectClientId { get; set; }
        [Column("N_ProjectVendorID")]
        public int? NProjectVendorId { get; set; }
        [Column("N_ProjectCommnID")]
        public int? NProjectCommnId { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("X_Remarks")]
        [StringLength(150)]
        public string XRemarks { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }

        [ForeignKey(nameof(NProjectId))]
        [InverseProperty(nameof(PrjProjectMaster.PrjPaymentReceipt))]
        public virtual PrjProjectMaster NProject { get; set; }
    }
}
