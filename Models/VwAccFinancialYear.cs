using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAccFinancialYear
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("X_FnYearDescr")]
        [StringLength(20)]
        public string XFnYearDescr { get; set; }
        [Column("D_Start", TypeName = "smalldatetime")]
        public DateTime? DStart { get; set; }
        [Column("D_End", TypeName = "smalldatetime")]
        public DateTime? DEnd { get; set; }
        [Column("B_YearEndProcess")]
        public bool? BYearEndProcess { get; set; }
        [Column("B_TransferProcess")]
        public bool? BTransferProcess { get; set; }
        [Column("FnYearID")]
        [StringLength(10)]
        public string FnYearId { get; set; }
    }
}
