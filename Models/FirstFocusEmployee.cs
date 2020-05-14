using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("___FirstFocus_employee")]
    public partial class FirstFocusEmployee
    {
        [Column("a")]
        public int? A { get; set; }
        [Column("b")]
        [StringLength(500)]
        public string B { get; set; }
        [Column("c")]
        [StringLength(500)]
        public string C { get; set; }
        [Column("d", TypeName = "date")]
        public DateTime? D { get; set; }
        [Column("e")]
        [StringLength(500)]
        public string E { get; set; }
        [Column("f")]
        [StringLength(500)]
        public string F { get; set; }
        [Column("g", TypeName = "date")]
        public DateTime? G { get; set; }
        [Column("h")]
        [StringLength(100)]
        public string H { get; set; }
        [Column("i")]
        [StringLength(100)]
        public string I { get; set; }
        [Column("j")]
        [StringLength(100)]
        public string J { get; set; }
        [Column("k")]
        [StringLength(100)]
        public string K { get; set; }
        [Column("l", TypeName = "date")]
        public DateTime? L { get; set; }
        [Column("m")]
        [StringLength(100)]
        public string M { get; set; }
        [Column("n")]
        [StringLength(100)]
        public string N { get; set; }
        [Column("o")]
        [StringLength(100)]
        public string O { get; set; }
        [Column("p")]
        [StringLength(100)]
        public string P { get; set; }
        [Column("q")]
        [StringLength(100)]
        public string Q { get; set; }
        [Column("r", TypeName = "date")]
        public DateTime? R { get; set; }
        [Column("s", TypeName = "date")]
        public DateTime? S { get; set; }
        [Column("t")]
        [StringLength(100)]
        public string T { get; set; }
        [Column("u")]
        [StringLength(100)]
        public string U { get; set; }
        [Column("v")]
        [StringLength(100)]
        public string V { get; set; }
        [Column("w")]
        [StringLength(100)]
        public string W { get; set; }
        [Column("x")]
        [StringLength(100)]
        public string X { get; set; }
        [Column("y")]
        [StringLength(100)]
        public string Y { get; set; }
        [Column("z")]
        [StringLength(100)]
        public string Z { get; set; }
    }
}
