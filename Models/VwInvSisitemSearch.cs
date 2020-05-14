using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvSisitemSearch
    {
        public double? TotalDeptQty { get; set; }
        [Column("N_DepartmentID")]
        public int? NDepartmentId { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("Item Code")]
        [StringLength(50)]
        public string ItemCode { get; set; }
        [StringLength(600)]
        public string Description { get; set; }
        [Column("X_DepartmentCode")]
        [StringLength(50)]
        public string XDepartmentCode { get; set; }
        [Column("X_Department")]
        public string XDepartment { get; set; }
        [Column("X_CostCentreCode")]
        [StringLength(50)]
        public string XCostCentreCode { get; set; }
        [StringLength(100)]
        public string Category { get; set; }
        [Column("Item Class")]
        [StringLength(25)]
        public string ItemClass { get; set; }
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("N_Qty")]
        public double? NQty { get; set; }
        [Column("B_InActive")]
        public bool? BInActive { get; set; }
        [Column("N_ItemUnitID")]
        public int? NItemUnitId { get; set; }
        [StringLength(500)]
        public string BaseUnit { get; set; }
        public double? BaseQty { get; set; }
        [Column("BaseUnitID")]
        public int? BaseUnitId { get; set; }
    }
}
