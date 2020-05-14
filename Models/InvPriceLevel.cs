using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_PriceLevel")]
    public partial class InvPriceLevel
    {
        [Key]
        [Column("N_PriceLevelID")]
        public int NPriceLevelId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_Price", TypeName = "money")]
        public decimal? NPrice { get; set; }
        [Column("X_Method")]
        [StringLength(100)]
        public string XMethod { get; set; }
        [Column("X_Operand1")]
        [StringLength(20)]
        public string XOperand1 { get; set; }
        [Column("X_Operator")]
        [StringLength(5)]
        public string XOperator { get; set; }
        [Column("N_Operand2")]
        public double? NOperand2 { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
