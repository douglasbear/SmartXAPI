using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchFeeReturnDetails1
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_ReceiptID")]
        public int NReceiptId { get; set; }
        [Column("N_ReceiptDetailsID")]
        public int NReceiptDetailsId { get; set; }
        [Column("N_AdmissionID")]
        public int NAdmissionId { get; set; }
        [Column("D_ReceiptDate", TypeName = "datetime")]
        public DateTime? DReceiptDate { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal NAmount { get; set; }
        [Column("N_ReturnAmt", TypeName = "money")]
        public decimal? NReturnAmt { get; set; }
        [Column("N_SalesID")]
        public int? NSalesId { get; set; }
        [Column("N_ReturnID")]
        public int NReturnId { get; set; }
    }
}
