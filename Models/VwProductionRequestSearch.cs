using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwProductionRequestSearch
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_PRSID")]
        public int NPrsid { get; set; }
        [Column("N_SalesOrderId")]
        public int? NSalesOrderId { get; set; }
        [Column("X_PRSNo")]
        [StringLength(50)]
        public string XPrsno { get; set; }
        [Column("D_EntryDate")]
        [StringLength(8000)]
        public string DEntryDate { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_TransTypeID")]
        public int? NTransTypeId { get; set; }
        [Column("D_ExpDeliveryDate", TypeName = "smalldatetime")]
        public DateTime? DExpDeliveryDate { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_PRSDetailsID")]
        public int? NPrsdetailsId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("N_Processed")]
        public int? NProcessed { get; set; }
        [Column("X_Location")]
        [StringLength(50)]
        public string XLocation { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("N_DepQty")]
        public double? NDepQty { get; set; }
    }
}
