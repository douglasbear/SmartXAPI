using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwVsaFileDisp
    {
        [Column("N_FileID")]
        public int NFileId { get; set; }
        [Column("X_FileNo")]
        [StringLength(50)]
        public string XFileNo { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Required]
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [Column("N_MainStatus")]
        public int NMainStatus { get; set; }
    }
}
