using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAccDayClosing
    {
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("D_ClosedDate")]
        [StringLength(8000)]
        public string DClosedDate { get; set; }
        [Column("N_CloseID")]
        public int NCloseId { get; set; }
        [Column("N_CashBalance", TypeName = "money")]
        public decimal? NCashBalance { get; set; }
        [Column("N_userID")]
        public int? NUserId { get; set; }
        [Column("B_Closed")]
        public bool? BClosed { get; set; }
        [Column("X_UserID")]
        [StringLength(50)]
        public string XUserId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("X_BranchName")]
        [StringLength(50)]
        public string XBranchName { get; set; }
    }
}
