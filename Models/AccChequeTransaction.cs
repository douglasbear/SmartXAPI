using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_ChequeTransaction")]
    public partial class AccChequeTransaction
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_ChequeTranID")]
        public int NChequeTranId { get; set; }
        [Key]
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_BankID")]
        [StringLength(50)]
        public string NBankId { get; set; }
        [Column("X_ChequeNO")]
        [StringLength(50)]
        public string XChequeNo { get; set; }
        [Column("D_IssueDate", TypeName = "datetime")]
        public DateTime? DIssueDate { get; set; }
        [Column("D_ChequeDate", TypeName = "datetime")]
        public DateTime? DChequeDate { get; set; }
        [Column("N_BeneficiaryID")]
        public int? NBeneficiaryId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("X_Remark")]
        [StringLength(1000)]
        public string XRemark { get; set; }
        [Column("N_InventoryID")]
        public int? NInventoryId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_BeneficiaryName")]
        [StringLength(60)]
        public string XBeneficiaryName { get; set; }
        [Column("X_AmtInWords")]
        [StringLength(200)]
        public string XAmtInWords { get; set; }
        [Column("X_PlaceOfIssue")]
        [StringLength(500)]
        public string XPlaceOfIssue { get; set; }
    }
}
