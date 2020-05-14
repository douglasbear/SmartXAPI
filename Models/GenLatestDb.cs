using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Gen_LatestDB")]
    public partial class GenLatestDb
    {
        [Column("X_Backup")]
        [StringLength(1000)]
        public string XBackup { get; set; }
        [Column("D_BackupTime", TypeName = "smalldatetime")]
        public DateTime? DBackupTime { get; set; }
    }
}
