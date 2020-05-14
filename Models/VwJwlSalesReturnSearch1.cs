using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwJwlSalesReturnSearch1
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_DebitNoteId")]
        public int NDebitNoteId { get; set; }
        [Column("X_DebitNoteNo")]
        [StringLength(50)]
        public string XDebitNoteNo { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("D_ReturnDate")]
        [StringLength(8000)]
        public string DReturnDate { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? ReturnedDate { get; set; }
        [Column("B_YearEndProcess")]
        public bool? BYearEndProcess { get; set; }
    }
}
