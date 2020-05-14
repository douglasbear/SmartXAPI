using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvSaleDebAdv
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_SalesId")]
        public int NSalesId { get; set; }
        [Column("X_TransType")]
        [StringLength(50)]
        public string XTransType { get; set; }
        [Column("X_ReceiptNo")]
        [StringLength(50)]
        public string XReceiptNo { get; set; }
        [Column("D_SalesDate", TypeName = "datetime")]
        public DateTime? DSalesDate { get; set; }
    }
}
