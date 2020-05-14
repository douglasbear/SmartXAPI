using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchFeesDetailsAnalysisRpt
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
        [Column("D_SalesDate", TypeName = "smalldatetime")]
        public DateTime? DSalesDate { get; set; }
        [Column("N_DiscountAmt", TypeName = "money")]
        public decimal NDiscountAmt { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_Type")]
        public int? NType { get; set; }
        [Column("N_SalesAmt", TypeName = "money")]
        public decimal? NSalesAmt { get; set; }
        [Column("N_ReceivedAmount", TypeName = "money")]
        public decimal? NReceivedAmount { get; set; }
        [Column("N_DueAmount", TypeName = "money")]
        public decimal? NDueAmount { get; set; }
        [Column("B_IsRemoved")]
        public int? BIsRemoved { get; set; }
        [Column("N_DivisionID")]
        public int? NDivisionId { get; set; }
        [Column("X_Name")]
        [StringLength(200)]
        public string XName { get; set; }
        [Column("X_Gender")]
        [StringLength(10)]
        public string XGender { get; set; }
        [Column("X_StudentMobile")]
        [StringLength(50)]
        public string XStudentMobile { get; set; }
        [Column("N_ReceiptID")]
        public int? NReceiptId { get; set; }
        [Column("B_BeginningBalance")]
        public bool? BBeginningBalance { get; set; }
        [Column("D_ReceiptDate", TypeName = "datetime")]
        public DateTime? DReceiptDate { get; set; }
        [Column("student_count")]
        public int? StudentCount { get; set; }
    }
}
