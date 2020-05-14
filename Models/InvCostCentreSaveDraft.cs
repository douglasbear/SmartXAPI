using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_CostCentre_SaveDraft")]
    public partial class InvCostCentreSaveDraft
    {
        [Key]
        [Column("N_CostCenterTransID")]
        public int NCostCenterTransId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_InventoryID")]
        public int NInventoryId { get; set; }
        [Column("N_CostCentreID")]
        public int? NCostCentreId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
    }
}
