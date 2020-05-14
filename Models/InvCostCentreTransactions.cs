using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_CostCentreTransactions")]
    public partial class InvCostCentreTransactions
    {
        [Key]
        [Column("N_CostCenterTransID")]
        public int NCostCenterTransId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_InventoryType")]
        public int NInventoryType { get; set; }
        [Column("N_InventoryID")]
        public int NInventoryId { get; set; }
        [Column("N_InventoryDetailsID")]
        public int NInventoryDetailsId { get; set; }
        [Column("N_CostCentreID")]
        public int? NCostCentreId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_LedgerID")]
        public int NLedgerId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_Narration")]
        [StringLength(250)]
        public string XNarration { get; set; }
        [Column("X_Naration")]
        [StringLength(250)]
        public string XNaration { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_GridLineNo")]
        public int? NGridLineNo { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }

        [ForeignKey("NCompanyId,NLedgerId,NFnYearId")]
        [InverseProperty(nameof(AccMastLedger.InvCostCentreTransactions))]
        public virtual AccMastLedger N { get; set; }
        [ForeignKey(nameof(NCostCentreId))]
        [InverseProperty(nameof(AccCostCentreMaster.InvCostCentreTransactions))]
        public virtual AccCostCentreMaster NCostCentre { get; set; }
    }
}
