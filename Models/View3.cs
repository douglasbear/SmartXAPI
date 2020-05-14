using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class View3
    {
        [Column("N_SalesId")]
        public int? NSalesId { get; set; }
        [Column("N_SalesDetailsId")]
        public int? NSalesDetailsId { get; set; }
        public int? Expr1 { get; set; }
    }
}
