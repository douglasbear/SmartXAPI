using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPrjCommissionDisp
    {
        [Column("N_ItemID")]
        public int NItemId { get; set; }
        [StringLength(50)]
        public string Code { get; set; }
        [StringLength(600)]
        public string Name { get; set; }
        [Column("B_InActive")]
        public bool? BInActive { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
    }
}
