using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("__Conv_Customer")]
    public partial class ConvCustomer
    {
        [Column("Cus_Code")]
        [StringLength(200)]
        public string CusCode { get; set; }
        [Column("Cus_Name")]
        [StringLength(300)]
        public string CusName { get; set; }
        [Column("Bal_Amount", TypeName = "money")]
        public decimal? BalAmount { get; set; }
    }
}
