using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_CRMMaster")]
    public partial class InvCrmmaster
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_CRMID")]
        public int NCrmid { get; set; }
        [Column("X_CRMCode")]
        [StringLength(50)]
        public string XCrmcode { get; set; }
        [Column("D_Date", TypeName = "smalldatetime")]
        public DateTime? DDate { get; set; }
        [Column("X_ClientName")]
        [StringLength(100)]
        public string XClientName { get; set; }
        [Column("X_Salesman")]
        [StringLength(100)]
        public string XSalesman { get; set; }
        [Column("N_SalesmanID")]
        public int? NSalesmanId { get; set; }
        [Column("X_Notes")]
        [StringLength(1000)]
        public string XNotes { get; set; }
        [Column("X_Contact")]
        [StringLength(25)]
        public string XContact { get; set; }
        [Column("X_ContactNo")]
        [StringLength(50)]
        public string XContactNo { get; set; }
        [Column("N_StatusID")]
        [StringLength(20)]
        public string NStatusId { get; set; }
        [Column("D_EntryDate", TypeName = "smalldatetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("X_Source")]
        [StringLength(50)]
        public string XSource { get; set; }
        [Column("X_Leadby")]
        [StringLength(50)]
        public string XLeadby { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_probability", TypeName = "numeric(5, 2)")]
        public decimal? NProbability { get; set; }
        [Column("N_BillAmount", TypeName = "money")]
        public decimal? NBillAmount { get; set; }
        [Column("N_Discount", TypeName = "money")]
        public decimal? NDiscount { get; set; }
        [Column("N_OtherCharge", TypeName = "money")]
        public decimal? NOtherCharge { get; set; }
        [Column("B_Processed")]
        public bool? BProcessed { get; set; }
        [Column("N_CustomerId")]
        public int? NCustomerId { get; set; }
        [Column("X_TenderName")]
        [StringLength(100)]
        public string XTenderName { get; set; }
        [Column("X_TenderReferance")]
        [StringLength(100)]
        public string XTenderReferance { get; set; }
        [Column("X_TenderDescription")]
        [StringLength(100)]
        public string XTenderDescription { get; set; }
        [Column("N_PurchaseAmount", TypeName = "money")]
        public decimal? NPurchaseAmount { get; set; }
        [Column("X_Payee")]
        [StringLength(100)]
        public string XPayee { get; set; }
        [Column("X_ScopeofSummary")]
        [StringLength(100)]
        public string XScopeofSummary { get; set; }
        [Column("D_LastdateofEnquiry", TypeName = "datetime")]
        public DateTime? DLastdateofEnquiry { get; set; }
        [Column("D_TenderSubmissionDate", TypeName = "datetime")]
        public DateTime? DTenderSubmissionDate { get; set; }
        [Column("D_TenderOpeningDate", TypeName = "datetime")]
        public DateTime? DTenderOpeningDate { get; set; }
        [Column("X_Waytype")]
        [StringLength(100)]
        public string XWaytype { get; set; }
        [Column("X_PlaceofTender")]
        [StringLength(100)]
        public string XPlaceofTender { get; set; }
        [Column("X_ProjectImplementation")]
        [StringLength(100)]
        public string XProjectImplementation { get; set; }
        [Column("X_TypeofClarification")]
        [StringLength(100)]
        public string XTypeofClarification { get; set; }
        [Column("N_TypeID")]
        public int? NTypeId { get; set; }
        [Column("N_EnquiryTypeID")]
        public int? NEnquiryTypeId { get; set; }
        [Column("N_ProjectTypeID")]
        public int? NProjectTypeId { get; set; }
        [Column("X_ProjectRegion")]
        [StringLength(200)]
        public string XProjectRegion { get; set; }
        [Column("Inv_OpportunitiesID")]
        public int? InvOpportunitiesId { get; set; }
    }
}
