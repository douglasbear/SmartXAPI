using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwCheckCustUsed
    {
        [Column("N_Order")]
        public int NOrder { get; set; }
        [Column("N_TransID")]
        public int NTransId { get; set; }
        [Column("N_CustomerID")]
        public int? NCustomerId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Required]
        [Column("X_Msg")]
        [StringLength(20)]
        public string XMsg { get; set; }
    }
}
