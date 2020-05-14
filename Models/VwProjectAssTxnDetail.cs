using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwProjectAssTxnDetail
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ProjectID")]
        public int NProjectId { get; set; }
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Column("X_ProjectDescription")]
        [StringLength(250)]
        public string XProjectDescription { get; set; }
        [Column("N_CustomerID")]
        public int? NCustomerId { get; set; }
        [Column("X_ContactPerson")]
        [StringLength(50)]
        public string XContactPerson { get; set; }
        [Column("X_ProjectCode")]
        [StringLength(100)]
        public string XProjectCode { get; set; }
        [Column("D_StartDate", TypeName = "datetime")]
        public DateTime? DStartDate { get; set; }
        [Column("D_EndDate", TypeName = "datetime")]
        public DateTime? DEndDate { get; set; }
        [Column("N_EstimateCost", TypeName = "money")]
        public decimal? NEstimateCost { get; set; }
        [Column("N_ContractAmt", TypeName = "money")]
        public decimal? NContractAmt { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("N_Price", TypeName = "money")]
        public decimal? NPrice { get; set; }
        [Column("N_LifePeriod")]
        public double? NLifePeriod { get; set; }
        [Column("N_BookValue", TypeName = "money")]
        public decimal? NBookValue { get; set; }
        [Column("N_AssetInventoryDetailsID")]
        public int? NAssetInventoryDetailsId { get; set; }
        [Column("N_ItemCodeId")]
        public int? NItemCodeId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime? DEntrydate { get; set; }
        [Column("X_CategoryPrefix")]
        [StringLength(20)]
        public string XCategoryPrefix { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_CostCentreID")]
        public int? NCostCentreId { get; set; }
        [Column("N_CategoryID")]
        public int? NCategoryId { get; set; }
        [Column("N_Status")]
        public int? NStatus { get; set; }
        [Column("X_Location")]
        [StringLength(50)]
        public string XLocation { get; set; }
    }
}
