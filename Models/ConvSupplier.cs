using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("__Conv_Supplier")]
    public partial class ConvSupplier
    {
        [Column("Sup_Code")]
        [StringLength(200)]
        public string SupCode { get; set; }
        [Column("Sup_Name")]
        [StringLength(300)]
        public string SupName { get; set; }
        [Column("Bal_Amount", TypeName = "money")]
        public decimal? BalAmount { get; set; }
    }
}
