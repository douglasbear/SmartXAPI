using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvSalesWiseReturn
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("X_DebitNoteNo")]
        [StringLength(50)]
        public string XDebitNoteNo { get; set; }
        [Column("N_SalesId")]
        public int? NSalesId { get; set; }
        [Column("D_ReturnDate", TypeName = "datetime")]
        public DateTime? DReturnDate { get; set; }
        [Column("N_TotalPaidAmount", TypeName = "money")]
        public decimal? NTotalPaidAmount { get; set; }
        [Column("N_RetAmount", TypeName = "money")]
        public decimal? NRetAmount { get; set; }
        [Column("N_RetQty")]
        public double? NRetQty { get; set; }
        [Column("N_Amount")]
        public double? NAmount { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
    }
}
