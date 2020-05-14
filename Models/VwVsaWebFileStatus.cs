using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwVsaWebFileStatus
    {
        [Column("X_RegCode")]
        [StringLength(50)]
        public string XRegCode { get; set; }
        [Required]
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [Column("X_mobileNo")]
        [StringLength(20)]
        public string XMobileNo { get; set; }
        [Column("N_FileID")]
        public int? NFileId { get; set; }
        [Column("N_MaxID")]
        public int? NMaxId { get; set; }
        [Column("X_StageName")]
        [StringLength(100)]
        public string XStageName { get; set; }
        [Column("X_StatusName")]
        [StringLength(500)]
        public string XStatusName { get; set; }
        [Column("X_Email")]
        [StringLength(30)]
        public string XEmail { get; set; }
        [Column("X_FileNo")]
        [StringLength(50)]
        public string XFileNo { get; set; }
    }
}
