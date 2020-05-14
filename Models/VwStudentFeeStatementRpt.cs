using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwStudentFeeStatementRpt
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_SalesId")]
        public int NSalesId { get; set; }
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        [Column("N_AdmissionID")]
        public int? NAdmissionId { get; set; }
        [Column("N_ClassTypeID")]
        public int? NClassTypeId { get; set; }
        [Column("N_ClassID")]
        public int? NClassId { get; set; }
        [Column("N_Feecode")]
        public int NFeecode { get; set; }
        [Column("N_FeeTypeID")]
        public int NFeeTypeId { get; set; }
        [Required]
        [Column("X_FeeType")]
        [StringLength(50)]
        public string XFeeType { get; set; }
        [Column("N_FrequencyID")]
        public int? NFrequencyId { get; set; }
        [Column("N_FeeCategoryID")]
        public int NFeeCategoryId { get; set; }
        [Column("X_FeeCategory")]
        [StringLength(50)]
        public string XFeeCategory { get; set; }
        [Column("transaction_date", TypeName = "datetime")]
        public DateTime? TransactionDate { get; set; }
        [Column("N_DiscountAmt", TypeName = "money")]
        public decimal? NDiscountAmt { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_Type")]
        public int? NType { get; set; }
        [Column(TypeName = "money")]
        public decimal? Debit { get; set; }
        [Column(TypeName = "money")]
        public decimal? Credit { get; set; }
        [Column("N_DueAmount", TypeName = "money")]
        public decimal? NDueAmount { get; set; }
        [Column("B_IsRemoved")]
        public int? BIsRemoved { get; set; }
        [Required]
        [Column("X_Type")]
        [StringLength(50)]
        public string XType { get; set; }
        [Column("X_Remarks")]
        [StringLength(150)]
        public string XRemarks { get; set; }
        [Column("X_ReceiptNo")]
        [StringLength(50)]
        public string XReceiptNo { get; set; }
        [Required]
        [Column("X_Status")]
        [StringLength(3)]
        public string XStatus { get; set; }
        [Column("B_IsTransfer")]
        public bool? BIsTransfer { get; set; }
    }
}
