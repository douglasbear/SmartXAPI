using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwCustomerMaterialDisp
    {
        [Column("X_ItemUnit")]
        [StringLength(500)]
        public string XItemUnit { get; set; }
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [StringLength(50)]
        public string TransferProject { get; set; }
        [StringLength(50)]
        public string Project { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_MaterialID")]
        public int NMaterialId { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("N_CustomerID")]
        public int? NCustomerId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
        [Column("X_RefNo")]
        [StringLength(100)]
        public string XRefNo { get; set; }
        [Column("X_Description")]
        [StringLength(1000)]
        public string XDescription { get; set; }
        [Column("N_MaterialDetailsID")]
        public int NMaterialDetailsId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(500)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(500)]
        public string XItemName { get; set; }
        [Column("X_ItemDescription")]
        [StringLength(1000)]
        public string XItemDescription { get; set; }
        [Column("N_StatusID")]
        public int? NStatusId { get; set; }
        [Column("N_UnitID")]
        public int? NUnitId { get; set; }
        [Column("N_TransferProjectID")]
        public int? NTransferProjectId { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_ReceiveQty")]
        public double? NReceiveQty { get; set; }
        [Column("N_IssueQty")]
        public double? NIssueQty { get; set; }
        [Column("N_ReturnQty")]
        public double? NReturnQty { get; set; }
        [Column("N_TransferQty")]
        public double? NTransferQty { get; set; }
        [Column("ProjectID")]
        public int? ProjectId { get; set; }
    }
}
