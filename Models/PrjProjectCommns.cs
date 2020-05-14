using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Prj_ProjectCommns")]
    public partial class PrjProjectCommns
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ProjectCommnID")]
        public int? NProjectCommnId { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("N_CommnID")]
        public int? NCommnId { get; set; }
        [Column("N_CommnAmt", TypeName = "money")]
        public decimal? NCommnAmt { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }

        [ForeignKey(nameof(NCommnId))]
        public virtual PrjCommission NCommn { get; set; }
        [ForeignKey(nameof(NProjectId))]
        public virtual PrjProjectMaster NProject { get; set; }
    }
}
