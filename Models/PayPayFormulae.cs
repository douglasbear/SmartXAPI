using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_PayFormulae")]
    public partial class PayPayFormulae
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_FormulaeID")]
        public int NFormulaeId { get; set; }
        [Column("N_PayID")]
        public int? NPayId { get; set; }
        [Column("N_PayItemID")]
        public int? NPayItemId { get; set; }
        [Column("N_Percentage")]
        public double? NPercentage { get; set; }
        [Column("X_Operator")]
        [StringLength(1)]
        public string XOperator { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
