using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Gen_OnlineUplodeInterface")]
    public partial class GenOnlineUplodeInterface
    {
        [Key]
        [Column("N_ID")]
        public int NId { get; set; }
        [Column("X_TableName")]
        [StringLength(100)]
        public string XTableName { get; set; }
        [Column("N_PKey")]
        public int? NPkey { get; set; }
        [Column("B_UploadedFlag")]
        public bool? BUploadedFlag { get; set; }
        [Column("X_ContactPerson")]
        [StringLength(50)]
        public string XContactPerson { get; set; }
        [Column("X_RefId")]
        [StringLength(50)]
        public string XRefId { get; set; }
    }
}
