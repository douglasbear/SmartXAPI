using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_StockAdjstmentReason")]
    public partial class InvStockAdjstmentReason
    {
        public InvStockAdjstmentReason()
        {
            InvStockAdjustmentDetails = new HashSet<InvStockAdjustmentDetails>();
        }

        [Key]
        [Column("N_ReasonID")]
        public int NReasonId { get; set; }
        [Column("X_Description")]
        [StringLength(50)]
        public string XDescription { get; set; }
        [Column("X_Description_Ar")]
        [StringLength(50)]
        public string XDescriptionAr { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_LedgerId")]
        public int? NLedgerId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("X_ReasonCode")]
        [StringLength(50)]
        public string XReasonCode { get; set; }
        [Column("B_IsStockIn")]
        public bool? BIsStockIn { get; set; }

        [InverseProperty("NReason")]
        public virtual ICollection<InvStockAdjustmentDetails> InvStockAdjustmentDetails { get; set; }
    }
}
