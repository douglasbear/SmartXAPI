using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAccDetails4ChequePrint
    {
        [Column("N_BeneficiaryID")]
        public int? NBeneficiaryId { get; set; }
        [Column("X_BeneficiaryCode")]
        [StringLength(50)]
        public string XBeneficiaryCode { get; set; }
        [Column("X_BeneficiaryName")]
        [StringLength(60)]
        public string XBeneficiaryName { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_ChequeTranID")]
        public int NChequeTranId { get; set; }
        [Column("X_ChequeNO")]
        [StringLength(50)]
        public string XChequeNo { get; set; }
        [Column("D_IssueDate", TypeName = "datetime")]
        public DateTime? DIssueDate { get; set; }
        public int? Expr1 { get; set; }
        [Column("D_ChequeDate", TypeName = "datetime")]
        public DateTime? DChequeDate { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("X_AmtInWords")]
        [StringLength(200)]
        public string XAmtInWords { get; set; }
        [Column("X_Remark")]
        [StringLength(1000)]
        public string XRemark { get; set; }
        [Column("X_PlaceOfIssue")]
        [StringLength(500)]
        public string XPlaceOfIssue { get; set; }
    }
}
