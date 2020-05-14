using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("vsa_Customer_SpouseWorkExp")]
    public partial class VsaCustomerSpouseWorkExp
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_ExpId")]
        public int NExpId { get; set; }
        [Column("N_RegId")]
        public int? NRegId { get; set; }
        [Column("X_Position")]
        [StringLength(100)]
        public string XPosition { get; set; }
        [Column("X_Period")]
        [StringLength(50)]
        public string XPeriod { get; set; }
    }
}
