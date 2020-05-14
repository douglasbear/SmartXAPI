using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Rst_TenentInvoiceDetail")]
    public partial class RstTenentInvoiceDetail
    {
        [Key]
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_BatchID")]
        public int NBatchId { get; set; }
        [Key]
        [Column("N_InvoiceID")]
        public int NInvoiceId { get; set; }
        [Column("N_TenentID")]
        public int NTenentId { get; set; }
    }
}
