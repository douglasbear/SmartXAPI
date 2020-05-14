using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSalesFunnel
    {
        [Column("value")]
        public int? Value { get; set; }
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
    }
}
