using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAssetStatusUpdate
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_StatusUpdateID")]
        public int NStatusUpdateId { get; set; }
        [Column("X_DocNo")]
        [StringLength(50)]
        public string XDocNo { get; set; }
        [Column("N_AssetID")]
        public int? NAssetId { get; set; }
        [Column("X_Reason")]
        [StringLength(400)]
        public string XReason { get; set; }
        [Column("D_StartDate", TypeName = "datetime")]
        public DateTime? DStartDate { get; set; }
        [Column("X_NoOfDays")]
        [StringLength(50)]
        public string XNoOfDays { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("X_TypeName")]
        [StringLength(50)]
        public string XTypeName { get; set; }
        [Column("N_DefaultId")]
        public int? NDefaultId { get; set; }
        [Column("N_StatusID")]
        public int? NStatusId { get; set; }
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(100)]
        public string XItemName { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("D_ExpReturnDate", TypeName = "datetime")]
        public DateTime? DExpReturnDate { get; set; }
        [Column("D_ActReturnDate", TypeName = "datetime")]
        public DateTime? DActReturnDate { get; set; }
        [Column("N_Expense", TypeName = "money")]
        public decimal? NExpense { get; set; }
        [Column("N_ReturnStatusID")]
        public int? NReturnStatusId { get; set; }
        [Column("X_ReturnStatus")]
        [StringLength(50)]
        public string XReturnStatus { get; set; }
    }
}
