using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("_Arabic")]
    public partial class Arabic
    {
        [Column("a")]
        public int A { get; set; }
        [Column("b")]
        public int? B { get; set; }
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
        [Column("g")]
        [StringLength(100)]
        public string G { get; set; }
    }
}
