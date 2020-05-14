using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwStockOut
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_StockId")]
        public int? NStockId { get; set; }
        [Column("N_SalesId")]
        public int? NSalesId { get; set; }
        [Column("N_SalesDetailsId")]
        public int? NSalesDetailsId { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_Cost", TypeName = "money")]
        public decimal? NCost { get; set; }
        [Column("X_Type")]
        [StringLength(20)]
        public string XType { get; set; }
    }
}
