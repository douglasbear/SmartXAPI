using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwItemSoPo
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_SalesOrderId")]
        public int NSalesOrderId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_POrderID")]
        public int NPorderId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
    }
}
