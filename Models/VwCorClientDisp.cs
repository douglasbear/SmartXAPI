using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwCorClientDisp
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("X_ClientCode")]
        [StringLength(25)]
        public string XClientCode { get; set; }
        [Column("X_Description")]
        [StringLength(50)]
        public string XDescription { get; set; }
        [Column("N_ClientId")]
        public int NClientId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
    }
}
