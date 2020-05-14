using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_ProductionCost")]
    public partial class InvProductionCost
    {
        [Key]
        [Column("N_TransID")]
        public int NTransId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_AssembleID")]
        public int NAssembleId { get; set; }
        [Column("N_CompID")]
        public int NCompId { get; set; }
        [Column("X_CompCode")]
        [StringLength(100)]
        public string XCompCode { get; set; }
        [Column("X_CompName")]
        [StringLength(100)]
        public string XCompName { get; set; }
        [Column("N_UsedHours")]
        public double? NUsedHours { get; set; }
        [Column("N_TotalHours")]
        public double? NTotalHours { get; set; }
        [Column("N_UnitCost", TypeName = "money")]
        public decimal? NUnitCost { get; set; }
        [Column("N_FormTypeID")]
        public int NFormTypeId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
