using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("___AlFarquan_ChartOfAccount")]
    public partial class AlFarquanChartOfAccount
    {
        [Column("b")]
        [StringLength(100)]
        public string B { get; set; }
        [Column("c")]
        [StringLength(100)]
        public string C { get; set; }
        [Column("d")]
        [StringLength(100)]
        public string D { get; set; }
        [Column("e")]
        [StringLength(100)]
        public string E { get; set; }
        [Column("f")]
        [StringLength(100)]
        public string F { get; set; }
        [Column("z")]
        [StringLength(100)]
        public string Z { get; set; }
    }
}
