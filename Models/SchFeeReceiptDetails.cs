﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Sch_FeeReceiptDetails")]
    public partial class SchFeeReceiptDetails
    {
        [Key]
        [Column("N_ReceiptID")]
        public int NReceiptId { get; set; }
        [Key]
        [Column("N_ReceiptDetailsID")]
        public int NReceiptDetailsId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_AcYearID")]
        public int NAcYearId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal NAmount { get; set; }
        [Column("N_Discount", TypeName = "money")]
        public decimal? NDiscount { get; set; }
        [Column("N_SalesID")]
        public int? NSalesId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
