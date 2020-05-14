using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("__Conv_Sql")]
    public partial class ConvSql
    {
        [Column("N_Order")]
        public double? NOrder { get; set; }
        [Column("X_SourceTable")]
        [StringLength(200)]
        public string XSourceTable { get; set; }
        [Column("X_TargetTable")]
        [StringLength(300)]
        public string XTargetTable { get; set; }
        [Column("X_Query")]
        public string XQuery { get; set; }
        [Column("N_Status")]
        public int? NStatus { get; set; }
    }
}
