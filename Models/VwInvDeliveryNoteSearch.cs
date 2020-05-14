using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvDeliveryNoteSearch
    {
        [Column("N_DeliveryNoteId")]
        public int NDeliveryNoteId { get; set; }
        [Column("Invoice No")]
        [StringLength(50)]
        public string InvoiceNo { get; set; }
        [Column("Invoice Date")]
        [StringLength(8000)]
        public string InvoiceDate { get; set; }
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_CustomerId")]
        public int? NCustomerId { get; set; }
        [Column("Customer Code")]
        [StringLength(50)]
        public string CustomerCode { get; set; }
        [StringLength(100)]
        public string Customer { get; set; }
        [Column("B_BeginingBalEntry")]
        public bool? BBeginingBalEntry { get; set; }
        [Column("N_DeliveryType")]
        public int? NDeliveryType { get; set; }
        [Column("X_TransType")]
        [StringLength(50)]
        public string XTransType { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("D_DeliveryDate", TypeName = "smalldatetime")]
        public DateTime? DDeliveryDate { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("X_LocationName")]
        public string XLocationName { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_ApproveLevel")]
        public int? NApproveLevel { get; set; }
        [Column("N_ProcStatus")]
        public int? NProcStatus { get; set; }
        [Column("B_YearEndProcess")]
        public bool? BYearEndProcess { get; set; }
        [Column("x_Notes")]
        [StringLength(250)]
        public string XNotes { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Required]
        [Column("type")]
        [StringLength(5)]
        public string Type { get; set; }
    }
}
