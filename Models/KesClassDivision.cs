using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("__KES_ClassDivision")]
    public partial class KesClassDivision
    {
        [Column("PKey_Code")]
        public int? PkeyCode { get; set; }
        [Column("Class_Name")]
        [StringLength(200)]
        public string ClassName { get; set; }
        [StringLength(300)]
        public string Division { get; set; }
        [StringLength(100)]
        public string Section { get; set; }
    }
}
