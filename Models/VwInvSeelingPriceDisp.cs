using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvSeelingPriceDisp
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_PkeyId")]
        public int NPkeyId { get; set; }
        [Column("X_PkeyCode")]
        [StringLength(5)]
        public string XPkeyCode { get; set; }
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [Column("N_ReferId")]
        public int? NReferId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_ItemID")]
        public int NItemId { get; set; }
        [Column("N_PriceVal", TypeName = "money")]
        public decimal? NPriceVal { get; set; }
        [StringLength(50)]
        public string PriceVal { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
    }
}
