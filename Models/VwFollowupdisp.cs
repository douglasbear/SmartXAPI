using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwFollowupdisp
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_RefID")]
        public int NRefId { get; set; }
        [Column("N_UserID")]
        public int NUserId { get; set; }
        [Column("N_LocationID")]
        public int NLocationId { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("CRM_Date")]
        [StringLength(8000)]
        public string CrmDate { get; set; }
        [Column("T_Time", TypeName = "time(5)")]
        public TimeSpan? TTime { get; set; }
        [Column("T_TimeTo", TypeName = "time(5)")]
        public TimeSpan? TTimeTo { get; set; }
        [Column("X_Description")]
        [StringLength(250)]
        public string XDescription { get; set; }
        [Column("N_RefTypeID")]
        [StringLength(10)]
        public string NRefTypeId { get; set; }
        [Column("N_EntryUserID")]
        public int? NEntryUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_CRMID")]
        public int NCrmid { get; set; }
        [Column("X_CRMCode")]
        [StringLength(50)]
        public string XCrmcode { get; set; }
        [Column("X_ClientName")]
        [StringLength(100)]
        public string XClientName { get; set; }
        [Column("X_Contact")]
        [StringLength(25)]
        public string XContact { get; set; }
        [Column("X_SalesmanName")]
        [StringLength(100)]
        public string XSalesmanName { get; set; }
        [Column("salescontactname")]
        [StringLength(100)]
        public string Salescontactname { get; set; }
        [Column("X_PhoneNo1")]
        [StringLength(20)]
        public string XPhoneNo1 { get; set; }
        [Column("B_IsComplete")]
        public bool? BIsComplete { get; set; }
    }
}
