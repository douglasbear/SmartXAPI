using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPaymentStatusDetail
    {
        [Column("N_PayReceiptId")]
        public int NPayReceiptId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime DDate { get; set; }
        [Column("X_ChequeNO")]
        [StringLength(50)]
        public string XChequeNo { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_ChequeAnount", TypeName = "money")]
        public decimal? NChequeAnount { get; set; }
        [Column("X_Remark")]
        [StringLength(1000)]
        public string XRemark { get; set; }
        [Column("X_BeneficiaryName")]
        [StringLength(100)]
        public string XBeneficiaryName { get; set; }
        [Column("N_BeneficiaryID")]
        public int NBeneficiaryId { get; set; }
        [Column("N_StatusID")]
        public int? NStatusId { get; set; }
        [Column("X_StatusName")]
        [StringLength(500)]
        public string XStatusName { get; set; }
        [Column("X_Notes")]
        [StringLength(1000)]
        public string XNotes { get; set; }
        [Column("X_ProjectName")]
        [StringLength(100)]
        public string XProjectName { get; set; }
    }
}
