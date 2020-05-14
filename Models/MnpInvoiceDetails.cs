using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Mnp_InvoiceDetails")]
    public partial class MnpInvoiceDetails
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_InvoiceID")]
        public int NInvoiceId { get; set; }
        [Key]
        [Column("N_InvoiceDetailsID")]
        public int NInvoiceDetailsId { get; set; }
        [Column("N_MobilizationID")]
        public int NMobilizationId { get; set; }
        [Column("N_MaintenanceID")]
        public int NMaintenanceId { get; set; }
        [Column("N_MobilizationDetailsID")]
        public int NMobilizationDetailsId { get; set; }
        [Column("N_Days")]
        public int NDays { get; set; }
        [Column("N_PayRate", TypeName = "money")]
        public decimal NPayRate { get; set; }
        [Column("N_Discount", TypeName = "money")]
        public decimal NDiscount { get; set; }
        [Column("N_UserID")]
        public int NUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime DEntryDate { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }

        [ForeignKey("NCompanyId,NInvoiceId")]
        [InverseProperty(nameof(MnpInvoice.MnpInvoiceDetails))]
        public virtual MnpInvoice N { get; set; }
    }
}
