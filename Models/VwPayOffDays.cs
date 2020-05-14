using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayOffDays
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_DayID")]
        public int NDayId { get; set; }
        [Column("D_Date", TypeName = "date")]
        public DateTime? DDate { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("X_Remarks")]
        [StringLength(500)]
        public string XRemarks { get; set; }
    }
}
