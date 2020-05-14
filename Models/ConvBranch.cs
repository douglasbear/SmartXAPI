using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("__Conv_Branch")]
    public partial class ConvBranch
    {
        [Column("PKey_Code")]
        public int? PkeyCode { get; set; }
        [Column("Branch_Code")]
        [StringLength(200)]
        public string BranchCode { get; set; }
        [Column("Branch_Name")]
        [StringLength(300)]
        public string BranchName { get; set; }
    }
}
