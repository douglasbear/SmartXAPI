using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_PayCalulationMethod")]
    public partial class PayPayCalulationMethod
    {
        [Key]
        [Column("N_MethodID")]
        public int NMethodId { get; set; }
        [Column("N_IndexID")]
        public int NIndexId { get; set; }
        [Column("X_Method")]
        [StringLength(100)]
        public string XMethod { get; set; }
        [Column("N_SortOrder")]
        public int? NSortOrder { get; set; }
        [Column("B_Active")]
        public bool? BActive { get; set; }
    }
}
