using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwDispatchSummRpt
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ProjectID")]
        public int NProjectId { get; set; }
        [Column("X_ProjectCode")]
        [StringLength(100)]
        public string XProjectCode { get; set; }
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Column("X_ProjectDescription")]
        [StringLength(250)]
        public string XProjectDescription { get; set; }
        [Column("N_DesignCost", TypeName = "money")]
        public decimal? NDesignCost { get; set; }
        [Column("X_ContactPerson")]
        [StringLength(50)]
        public string XContactPerson { get; set; }
        [Column("N_Progress")]
        public int? NProgress { get; set; }
        [Column("X_ItemCode")]
        [StringLength(100)]
        public string XItemCode { get; set; }
        [Column("X_ItemName")]
        [StringLength(800)]
        public string XItemName { get; set; }
        public double? Dqty { get; set; }
        [Column("DCost", TypeName = "money")]
        public decimal? Dcost { get; set; }
        [Column("DRCost", TypeName = "money")]
        public decimal? Drcost { get; set; }
        [Column("DRQty")]
        public double? Drqty { get; set; }
        [Column("X_DispatchReturnNo")]
        [StringLength(50)]
        public string XDispatchReturnNo { get; set; }
        [Column("X_DispatchNo")]
        [StringLength(50)]
        public string XDispatchNo { get; set; }
        [Column("X_Responsible")]
        [StringLength(200)]
        public string XResponsible { get; set; }
    }
}
