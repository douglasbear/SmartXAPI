using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvStockAdjstmentReasonDisp
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_ReasonID")]
        public int NReasonId { get; set; }
        [Column("X_ReasonCode")]
        [StringLength(50)]
        public string XReasonCode { get; set; }
        [Column("X_Description")]
        [StringLength(50)]
        public string XDescription { get; set; }
        [Column("X_Description_Ar")]
        [StringLength(50)]
        public string XDescriptionAr { get; set; }
        [Column("N_LedgerId")]
        public int? NLedgerId { get; set; }
        [Column("B_IsStockIn")]
        public bool? BIsStockIn { get; set; }
        [Column("X_LedgerCode")]
        [StringLength(50)]
        public string XLedgerCode { get; set; }
        [Column("X_LedgerName")]
        [StringLength(100)]
        public string XLedgerName { get; set; }
        [Column("N_GroupID")]
        public int? NGroupId { get; set; }
        [Column("X_GroupCode")]
        [StringLength(50)]
        public string XGroupCode { get; set; }
        [Column("X_GroupName")]
        [StringLength(100)]
        public string XGroupName { get; set; }
    }
}
