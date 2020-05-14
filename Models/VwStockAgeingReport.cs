using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwStockAgeingReport
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(800)]
        public string XItemName { get; set; }
        [Column("N_CurrentStock")]
        public double? NCurrentStock { get; set; }
        public double? Stock30b { get; set; }
        [Column("stock30a")]
        public double? Stock30a { get; set; }
        [Column("stock90b")]
        public double? Stock90b { get; set; }
        [Column("stock90a")]
        public double? Stock90a { get; set; }
        [Column("stock180a")]
        public double? Stock180a { get; set; }
        [Column("X_Category")]
        [StringLength(100)]
        public string XCategory { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
    }
}
