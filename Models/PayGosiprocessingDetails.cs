using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_GOSIProcessingDetails")]
    public partial class PayGosiprocessingDetails
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_TransDetailsID")]
        public int NTransDetailsId { get; set; }
        [Column("N_TransID")]
        public int NTransId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("N_PayID")]
        public int? NPayId { get; set; }
        [Column("N_Payrate", TypeName = "money")]
        public decimal? NPayrate { get; set; }
        [Column("B_Posted")]
        public bool? BPosted { get; set; }
        [Column("D_PostedDate", TypeName = "datetime")]
        public DateTime? DPostedDate { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("B_BeginingBalEntry")]
        public bool? BBeginingBalEntry { get; set; }
        [Column("X_Remarks")]
        public string XRemarks { get; set; }
        [Column("N_EmpAmount", TypeName = "money")]
        public decimal? NEmpAmount { get; set; }
        [Column("N_CompAmount", TypeName = "money")]
        public decimal? NCompAmount { get; set; }
        [Column("N_AgentID")]
        public int? NAgentId { get; set; }
    }
}
