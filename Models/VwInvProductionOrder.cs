using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvProductionOrder
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("Invoice_No")]
        [StringLength(20)]
        public string InvoiceNo { get; set; }
        [Column("N_AssemblyID")]
        public int NAssemblyId { get; set; }
        [Column("Item_code1")]
        [StringLength(100)]
        public string ItemCode1 { get; set; }
        [Column("Item_code2")]
        [StringLength(100)]
        public string ItemCode2 { get; set; }
        [Column("Item_Name1")]
        [StringLength(600)]
        public string ItemName1 { get; set; }
        [Column("Item_Name2")]
        [StringLength(600)]
        public string ItemName2 { get; set; }
        [Column("Production_Type")]
        [StringLength(20)]
        public string ProductionType { get; set; }
        public double? Qty1 { get; set; }
        public double? Qty2 { get; set; }
        [Column("UnitID1")]
        [StringLength(500)]
        public string UnitId1 { get; set; }
        [Column("UnitID2")]
        [StringLength(500)]
        public string UnitId2 { get; set; }
        [Column("Unit_Cost")]
        public double? UnitCost { get; set; }
        [Column("Total_Cost", TypeName = "money")]
        public decimal? TotalCost { get; set; }
        [Column(TypeName = "money")]
        public decimal? OtherCharges { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column(TypeName = "money")]
        public decimal? LaborCost { get; set; }
        [Column("Machinery_Cost", TypeName = "money")]
        public decimal? MachineryCost { get; set; }
        [Column("N_Type")]
        public int? NType { get; set; }
        [Column("B_IsProcess")]
        public bool? BIsProcess { get; set; }
        [Column("X_Description")]
        [StringLength(500)]
        public string XDescription { get; set; }
        [Column("N_ReqID")]
        public int? NReqId { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("X_Action")]
        [StringLength(20)]
        public string XAction { get; set; }
    }
}
