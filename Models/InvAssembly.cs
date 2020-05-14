using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_Assembly")]
    public partial class InvAssembly
    {
        public InvAssembly()
        {
            InvAssemblyDetails = new HashSet<InvAssemblyDetails>();
            InvAssemblyStockWise = new HashSet<InvAssemblyStockWise>();
        }

        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Key]
        [Column("N_AssemblyID")]
        public int NAssemblyId { get; set; }
        [Column("X_Action")]
        [StringLength(20)]
        public string XAction { get; set; }
        [Column("X_ReferenceNo")]
        [StringLength(20)]
        public string XReferenceNo { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("N_Cost", TypeName = "money")]
        public decimal? NCost { get; set; }
        [Column("N_Rate", TypeName = "money")]
        public decimal? NRate { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_OtherCharges", TypeName = "money")]
        public decimal? NOtherCharges { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_LaborCost", TypeName = "money")]
        public decimal? NLaborCost { get; set; }
        [Column("N_Machinery_Cost", TypeName = "money")]
        public decimal? NMachineryCost { get; set; }
        [Column("B_IsProcess")]
        public bool? BIsProcess { get; set; }
        [Column("N_ReqID")]
        public int? NReqId { get; set; }
        [Column("X_Description")]
        [StringLength(500)]
        public string XDescription { get; set; }
        [Column("D_OrderDate", TypeName = "datetime")]
        public DateTime? DOrderDate { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Column("D_ReleaseDate", TypeName = "datetime")]
        public DateTime? DReleaseDate { get; set; }

        [InverseProperty("NAssembly")]
        public virtual ICollection<InvAssemblyDetails> InvAssemblyDetails { get; set; }
        [InverseProperty("NAssembly")]
        public virtual ICollection<InvAssemblyStockWise> InvAssemblyStockWise { get; set; }
    }
}
