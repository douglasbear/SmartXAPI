using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Ass_AssetStatusUpdate")]
    public partial class AssAssetStatusUpdate
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_StatusUpdateID")]
        public int NStatusUpdateId { get; set; }
        [Column("X_DocNo")]
        [StringLength(50)]
        public string XDocNo { get; set; }
        [Column("N_AssetID")]
        public int? NAssetId { get; set; }
        [Column("N_StatusID")]
        public int? NStatusId { get; set; }
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
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("D_ExpReturnDate", TypeName = "datetime")]
        public DateTime? DExpReturnDate { get; set; }
        [Column("D_ActReturnDate", TypeName = "datetime")]
        public DateTime? DActReturnDate { get; set; }
        [Column("N_ReturnStatusID")]
        public int? NReturnStatusId { get; set; }
        [Column("N_Expense", TypeName = "money")]
        public decimal? NExpense { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
        [Column("N_RefID")]
        public int? NRefId { get; set; }
    }
}
