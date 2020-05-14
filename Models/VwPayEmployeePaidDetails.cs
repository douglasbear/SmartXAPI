﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayEmployeePaidDetails
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_AcYearID")]
        public int NAcYearId { get; set; }
        [Column("N_EmpId")]
        public int? NEmpId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal NAmount { get; set; }
        [Column("N_Discount", TypeName = "money")]
        public decimal? NDiscount { get; set; }
        [Column("N_SalesID")]
        public int? NSalesId { get; set; }
        [Column("N_ReceiptID")]
        public int NReceiptId { get; set; }
        [Column("X_Remarks")]
        [StringLength(150)]
        public string XRemarks { get; set; }
        [Column("N_PaymentID")]
        public int NPaymentId { get; set; }
        [Column("N_PayTypeID")]
        public int NPayTypeId { get; set; }
        [Column("N_Entryfrom")]
        public int? NEntryfrom { get; set; }
    }
}
