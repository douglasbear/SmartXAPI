using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_StockAdjustment")]
    public partial class InvStockAdjustment
    {
        public InvStockAdjustment()
        {
            InvStockAdjustmentDetails = new HashSet<InvStockAdjustmentDetails>();
        }

        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Key]
        [Column("N_AdjustmentID")]
        public int NAdjustmentId { get; set; }
        [Column("X_RefNo")]
        [StringLength(50)]
        public string XRefNo { get; set; }
        [Column("D_AdjustDate", TypeName = "datetime")]
        public DateTime? DAdjustDate { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
        [Column("N_LoactionID")]
        public int? NLoactionId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_Remarks")]
        [StringLength(1000)]
        public string XRemarks { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
        [Column("N_ApproveLevel")]
        public int? NApproveLevel { get; set; }
        [Column("N_ProcStatus")]
        public int? NProcStatus { get; set; }

        [InverseProperty("NAdjustment")]
        public virtual ICollection<InvStockAdjustmentDetails> InvStockAdjustmentDetails { get; set; }
    }
}
