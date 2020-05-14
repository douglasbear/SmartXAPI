using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAssTransactionDisp
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_DeprID")]
        public int NDeprId { get; set; }
        [StringLength(30)]
        public string DepriciationNo { get; set; }
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("Start Date")]
        [StringLength(8000)]
        public string StartDate { get; set; }
        [Column("End Date")]
        [StringLength(8000)]
        public string EndDate { get; set; }
        [Column("D_RunDate", TypeName = "datetime")]
        public DateTime? DRunDate { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("X_DepriciationNo")]
        [StringLength(50)]
        public string XDepriciationNo { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
    }
}
