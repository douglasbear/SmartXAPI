using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class PayMonthLeaveRpt
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [StringLength(60)]
        public string MonthNmbr { get; set; }
        [StringLength(36)]
        public string D2 { get; set; }
    }
}
