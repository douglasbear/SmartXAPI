using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("__Conv_Location")]
    public partial class ConvLocation
    {
        [Column("PKey_Code")]
        public int? PkeyCode { get; set; }
        [Column("Location_Code")]
        [StringLength(200)]
        public string LocationCode { get; set; }
        [Column("Location_Name")]
        [StringLength(300)]
        public string LocationName { get; set; }
        [Column("Branch_Name")]
        [StringLength(300)]
        public string BranchName { get; set; }
    }
}
