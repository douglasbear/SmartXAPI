using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwCheckPrjUsed
    {
        [Column("N_Order")]
        public int NOrder { get; set; }
        [Column("N_TransID")]
        public int? NTransId { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Required]
        [Column("X_Msg")]
        [StringLength(23)]
        public string XMsg { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
    }
}
