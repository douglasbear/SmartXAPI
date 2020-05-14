using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvJwlType
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("TypeID")]
        public int TypeId { get; set; }
        [StringLength(100)]
        public string JewelType { get; set; }
    }
}
