using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Mnp_Invoice")]
    public partial class MnpInvoice
    {
        public MnpInvoice()
        {
            MnpInvoiceDetails = new HashSet<MnpInvoiceDetails>();
        }

        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Key]
        [Column("N_InvoiceID")]
        public int NInvoiceId { get; set; }
        [Column("N_CustomerID")]
        public int NCustomerId { get; set; }
        [Required]
        [Column("X_InvoiceNo")]
        [StringLength(50)]
        public string XInvoiceNo { get; set; }
        [Column("D_InvoiceDate", TypeName = "datetime")]
        public DateTime DInvoiceDate { get; set; }
        [Column("X_Month")]
        [StringLength(50)]
        public string XMonth { get; set; }
        [Column("X_PayrunText")]
        [StringLength(50)]
        public string XPayrunText { get; set; }
        [Column("X_Year")]
        [StringLength(50)]
        public string XYear { get; set; }
        [Column("N_PayRun")]
        public int? NPayRun { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("D_InvoiceFromDate", TypeName = "datetime")]
        public DateTime DInvoiceFromDate { get; set; }
        [Column("D_InvoiceToDate", TypeName = "datetime")]
        public DateTime DInvoiceToDate { get; set; }
        [Column("N_TotalAmount", TypeName = "money")]
        public decimal NTotalAmount { get; set; }
        [Column("N_TaxAmount", TypeName = "money")]
        public decimal NTaxAmount { get; set; }
        [Column("N_NetAmount", TypeName = "money")]
        public decimal NNetAmount { get; set; }
        [Column("N_UserID")]
        public int NUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime DEntryDate { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("N_GroupID")]
        public int? NGroupId { get; set; }
        [Column("N_Days")]
        public double? NDays { get; set; }
        [Column("X_Description")]
        [StringLength(500)]
        public string XDescription { get; set; }

        [InverseProperty("N")]
        public virtual ICollection<MnpInvoiceDetails> MnpInvoiceDetails { get; set; }
    }
}
