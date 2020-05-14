using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchBookDisp
    {
        [Column("X_ItemCode")]
        [StringLength(50)]
        public string XItemCode { get; set; }
        [Column("X_Category")]
        [StringLength(100)]
        public string XCategory { get; set; }
        [Column("N_ClassID")]
        public int? NClassId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ItemID")]
        public int NItemId { get; set; }
    }
}
