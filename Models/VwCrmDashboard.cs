using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwCrmDashboard
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_CRMID")]
        public int NCrmid { get; set; }
        [Column("X_CRMCode")]
        [StringLength(50)]
        public string XCrmcode { get; set; }
        [Column("D_Date")]
        [StringLength(8000)]
        public string DDate { get; set; }
        [Column("X_ClientName")]
        [StringLength(100)]
        public string XClientName { get; set; }
        [Column("X_Salesman")]
        [StringLength(100)]
        public string XSalesman { get; set; }
        [Column("N_PartyID")]
        public int NPartyId { get; set; }
        [Column("X_Notes")]
        [StringLength(250)]
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
        [Column("N_BillAmount")]
        [StringLength(30)]
        public string NBillAmount { get; set; }
        [Column("N_Discount")]
        [StringLength(30)]
        public string NDiscount { get; set; }
        [Column("N_OtherCharge")]
        [StringLength(30)]
        public string NOtherCharge { get; set; }
        [StringLength(30)]
        public string NetAmount { get; set; }
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
        [Required]
        [Column("N_probability")]
        [StringLength(32)]
        public string NProbability { get; set; }
        [Column("d_followupdate")]
        [StringLength(8000)]
        public string DFollowupdate { get; set; }
        [Column("T_Time", TypeName = "time(5)")]
        public TimeSpan? TTime { get; set; }
        [Column("T_TimeTo", TypeName = "time(5)")]
        public TimeSpan? TTimeTo { get; set; }
        [Required]
        [Column("x_followupdesc")]
        [StringLength(250)]
        public string XFollowupdesc { get; set; }
        [Column("N_RefID")]
        public int NRefId { get; set; }
        [Required]
        [Column("N_RefTypeID")]
        [StringLength(10)]
        public string NRefTypeId { get; set; }
        [Column("x_status")]
        [StringLength(50)]
        public string XStatus { get; set; }
        [Column("B_IsComplete")]
        public bool? BIsComplete { get; set; }
        [Column("app_N_ID")]
        public int? AppNId { get; set; }
    }
}
