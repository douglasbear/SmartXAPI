using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwMonthlyProcess
    {
        [Column("N_ProcessID")]
        public int NProcessId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Required]
        [Column("X_ProcessCode")]
        [StringLength(20)]
        public string XProcessCode { get; set; }
        [Column("X_TransType")]
        [StringLength(20)]
        public string XTransType { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("D_ProcessDate")]
        [StringLength(100)]
        public string DProcessDate { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
    }
}
