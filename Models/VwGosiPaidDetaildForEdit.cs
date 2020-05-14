using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwGosiPaidDetaildForEdit
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_TransID")]
        public int NTransId { get; set; }
        [Column("N_Payrate", TypeName = "money")]
        public decimal? NPayrate { get; set; }
        [Column("D_TransDate", TypeName = "datetime")]
        public DateTime? DTransDate { get; set; }
        [Column("X_PayrunText")]
        [StringLength(50)]
        public string XPayrunText { get; set; }
        [Column("B_PostedAccount")]
        public bool? BPostedAccount { get; set; }
        [Column("N_AcYearID")]
        public int? NAcYearId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal NAmount { get; set; }
        [Column("N_Discount", TypeName = "money")]
        public decimal? NDiscount { get; set; }
        [Column("N_ReceiptID")]
        public int? NReceiptId { get; set; }
        [Column("N_InvoiceDueAmt", TypeName = "money")]
        public decimal? NInvoiceDueAmt { get; set; }
        [Required]
        [Column("N_EntryFrom")]
        [StringLength(3)]
        public string NEntryFrom { get; set; }
    }
}
