using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_Reconciliation")]
    public partial class AccReconciliation
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_StatementID")]
        public int NStatementId { get; set; }
        [Column("X_StatementCOde")]
        [StringLength(50)]
        public string XStatementCode { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("N_Opening", TypeName = "money")]
        public decimal? NOpening { get; set; }
        [Column("N_Ending", TypeName = "money")]
        public decimal? NEnding { get; set; }
        [Column("N_Unreconcil", TypeName = "money")]
        public decimal? NUnreconcil { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_userID")]
        public int? NUserId { get; set; }
        [Column("X_Remarks")]
        [StringLength(500)]
        public string XRemarks { get; set; }
        [Column("X_ReferenceNo")]
        [StringLength(50)]
        public string XReferenceNo { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_BankID")]
        public int? NBankId { get; set; }
        [Column("N_Bankbal", TypeName = "money")]
        public decimal? NBankbal { get; set; }

        [ForeignKey(nameof(NCompanyId))]
        [InverseProperty(nameof(AccCompany.AccReconciliation))]
        public virtual AccCompany NCompany { get; set; }
    }
}
