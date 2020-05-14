using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_PaymentDetails")]
    public partial class InvPaymentDetails
    {
        [Key]
        [Column("N_PaymentRefID")]
        public int NPaymentRefId { get; set; }
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_PaymentID")]
        public int? NPaymentId { get; set; }
        [Column("N_BeneficiaryID")]
        public int? NBeneficiaryId { get; set; }
        [Column("X_PrfNo")]
        [StringLength(50)]
        public string XPrfNo { get; set; }
        [Column("X_Requestor")]
        [StringLength(50)]
        public string XRequestor { get; set; }
        [Column("N_PayableAmount", TypeName = "money")]
        public decimal? NPayableAmount { get; set; }
        [Column("N_TotalAmount", TypeName = "money")]
        public decimal? NTotalAmount { get; set; }
        [Column("N_BalanceAmount", TypeName = "money")]
        public decimal? NBalanceAmount { get; set; }
        [Column("X_Documents")]
        [StringLength(500)]
        public string XDocuments { get; set; }
        [Column("X_Notes")]
        [StringLength(1000)]
        public string XNotes { get; set; }
        [Column("X_OracleNo")]
        [StringLength(50)]
        public string XOracleNo { get; set; }
        [Column("N_StatusID")]
        public int? NStatusId { get; set; }
        [Column("X_AmountInWords")]
        [StringLength(200)]
        public string XAmountInWords { get; set; }
    }
}
