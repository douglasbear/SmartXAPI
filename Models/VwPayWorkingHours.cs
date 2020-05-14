using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayWorkingHours
    {
        [Column("N_ID")]
        public int NId { get; set; }
        [Column("N_WHID")]
        public int NWhid { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("X_Day")]
        [StringLength(50)]
        public string XDay { get; set; }
        [Column("N_Workhours")]
        public double? NWorkhours { get; set; }
        [Column("N_MinWorkhours")]
        public double? NMinWorkhours { get; set; }
        [Column("N_CatagoryID")]
        public int NCatagoryId { get; set; }
        [Column("D_In1")]
        public TimeSpan? DIn1 { get; set; }
        [Column("D_Out1")]
        public TimeSpan? DOut1 { get; set; }
        [Column("D_In2")]
        public TimeSpan? DIn2 { get; set; }
        [Column("D_Out2")]
        public TimeSpan? DOut2 { get; set; }
    }
}
