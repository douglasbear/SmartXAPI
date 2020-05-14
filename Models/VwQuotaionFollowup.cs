using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwQuotaionFollowup
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_crmID")]
        public int? NCrmId { get; set; }
        [Column("D_followupdate")]
        [StringLength(8000)]
        public string DFollowupdate { get; set; }
        [Column("Day_Name")]
        [StringLength(30)]
        public string DayName { get; set; }
        [Column("N_RefID")]
        public int NRefId { get; set; }
        [Column("N_RefTypeID")]
        [StringLength(10)]
        public string NRefTypeId { get; set; }
        [Column("X_Description")]
        [StringLength(250)]
        public string XDescription { get; set; }
        [Column("X_SalesmanName")]
        [StringLength(100)]
        public string XSalesmanName { get; set; }
        [Column("X_ContactName")]
        [StringLength(100)]
        public string XContactName { get; set; }
        [Column("T_Time", TypeName = "time(5)")]
        public TimeSpan? TTime { get; set; }
        [Column("T_TimeTo", TypeName = "time(5)")]
        public TimeSpan? TTimeTo { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("X_crmCode")]
        [StringLength(50)]
        public string XCrmCode { get; set; }
        [Column("B_IsComplete")]
        public bool? BIsComplete { get; set; }
    }
}
