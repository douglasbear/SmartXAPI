using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Prj_CustomerMaterialDetails")]
    public partial class PrjCustomerMaterialDetails
    {
        [Key]
        [Column("N_MaterialDetailsID")]
        public int NMaterialDetailsId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_MaterialID")]
        public int? NMaterialId { get; set; }
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
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
    }
}
