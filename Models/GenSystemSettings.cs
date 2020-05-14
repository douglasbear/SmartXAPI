using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Gen_SystemSettings")]
    public partial class GenSystemSettings
    {
        [Column("X_AppVersion")]
        [StringLength(100)]
        public string XAppVersion { get; set; }
        [Column("B_AutoAppClose")]
        public bool? BAutoAppClose { get; set; }
        [Column("D_AppCloseTime", TypeName = "smalldatetime")]
        public DateTime? DAppCloseTime { get; set; }
        [Column("N_FileMajorPart")]
        public int? NFileMajorPart { get; set; }
        [Column("N_FileMinorPart")]
        public int? NFileMinorPart { get; set; }
        [Column("N_FileBuildPart")]
        public int? NFileBuildPart { get; set; }
        [Column("N_FilePrivatePart")]
        public int? NFilePrivatePart { get; set; }
        [Column("X_Executable")]
        [StringLength(30)]
        public string XExecutable { get; set; }
    }
}
