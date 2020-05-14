using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwVehCustomerRemider
    {
        [Column("N_CustomerID")]
        public int NCustomerId { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("X_Address")]
        [StringLength(250)]
        public string XAddress { get; set; }
        [Column("X_PhoneNo1")]
        [StringLength(20)]
        public string XPhoneNo1 { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        public int? CalcDay { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Lastdate { get; set; }
        [Column("N_Balance", TypeName = "money")]
        public decimal? NBalance { get; set; }
    }
}
