using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_ReconciliationDetails")]
    public partial class AccReconciliationDetails
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_StatementID")]
        public int NStatementId { get; set; }
        [Key]
        [Column("N_StatementDetailID")]
        public int NStatementDetailId { get; set; }
        [Column("D_DetailDate", TypeName = "datetime")]
        public DateTime? DDetailDate { get; set; }
        [Column("X_Description")]
        [StringLength(200)]
        public string XDescription { get; set; }
        [Required]
        [Column("X_VoucherTypesID")]
        [StringLength(50)]
        public string XVoucherTypesId { get; set; }
        [Column("N_PaymentMethodID")]
        public int? NPaymentMethodId { get; set; }
        [Column("X_ChequeNo")]
        [StringLength(100)]
        public string XChequeNo { get; set; }
        [Column("D_ChequeDate", TypeName = "datetime")]
        public DateTime? DChequeDate { get; set; }
        [Column("D_BankDate", TypeName = "datetime")]
        public DateTime? DBankDate { get; set; }
        [Column("B_IsClr")]
        public bool? BIsClr { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_AccTransID")]
        public int? NAccTransId { get; set; }
        [Column("N_LineNo")]
        public int? NLineNo { get; set; }
        [Column("N_VoucherID")]
        public int? NVoucherId { get; set; }
        [Column("N_bal", TypeName = "money")]
        public decimal? NBal { get; set; }
        [Column("N_Bankbal", TypeName = "money")]
        public decimal? NBankbal { get; set; }
        [Column("X_Notes")]
        [StringLength(1000)]
        public string XNotes { get; set; }
    }
}
