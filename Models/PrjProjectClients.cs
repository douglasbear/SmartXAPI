using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Prj_ProjectClients")]
    public partial class PrjProjectClients
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ProjectClientID")]
        public int? NProjectClientId { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("N_ClientID")]
        public int? NClientId { get; set; }
        [Column("N_ClientAmt", TypeName = "money")]
        public decimal? NClientAmt { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }

        [ForeignKey(nameof(NClientId))]
        public virtual PrjClient NClient { get; set; }
        [ForeignKey(nameof(NProjectId))]
        public virtual PrjProjectMaster NProject { get; set; }
    }
}
