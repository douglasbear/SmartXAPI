using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwVsaFileHistoryDisp
    {
        [Column("N_StatusID")]
        public int NStatusId { get; set; }
        [Column("X_StatusName")]
        [StringLength(500)]
        public string XStatusName { get; set; }
        [Column("X_StageName")]
        [StringLength(100)]
        public string XStageName { get; set; }
        [Column("D_Entrydate")]
        [StringLength(8000)]
        public string DEntrydate { get; set; }
        [Column("X_StatusDescription")]
        [StringLength(500)]
        public string XStatusDescription { get; set; }
        [Column("N_FileID")]
        public int NFileId { get; set; }
        [Column("N_FileStatusID")]
        public int NFileStatusId { get; set; }
    }
}
