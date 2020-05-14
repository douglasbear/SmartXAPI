using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Jwl_SalesReturnDetails")]
    public partial class JwlSalesReturnDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_DebitNoteDetailsID")]
        public int NDebitNoteDetailsId { get; set; }
        [Column("N_DebitNoteId")]
        public int? NDebitNoteId { get; set; }
        [Column("N_SalesDetailsId")]
        public int? NSalesDetailsId { get; set; }
        [Column("N_RetQty")]
        public int? NRetQty { get; set; }
        [Column("N_RetAmount", TypeName = "money")]
        public decimal? NRetAmount { get; set; }
        [Column("N_StockID")]
        public int? NStockId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_IMEI")]
        [StringLength(50)]
        public string NImei { get; set; }
        [Column("X_ReturnType")]
        [StringLength(25)]
        public string XReturnType { get; set; }
        [Column("X_ReturnRemarks")]
        [StringLength(150)]
        public string XReturnRemarks { get; set; }
    }
}
