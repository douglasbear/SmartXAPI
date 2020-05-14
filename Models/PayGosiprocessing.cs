using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_GOSIProcessing")]
    public partial class PayGosiprocessing
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_TransID")]
        public int NTransId { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
        [Column("X_PayrunText")]
        [StringLength(50)]
        public string XPayrunText { get; set; }
        [Column("B_PostedAccount")]
        public bool? BPostedAccount { get; set; }
        [Column("D_CreatedDate", TypeName = "datetime")]
        public DateTime? DCreatedDate { get; set; }
        [Column("D_ModifiedDate", TypeName = "datetime")]
        public DateTime? DModifiedDate { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_ProcessID")]
        public int? NProcessId { get; set; }
        [Column("N_AgentID")]
        public int? NAgentId { get; set; }
    }
}
