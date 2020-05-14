using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAssItemHistory
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("D_StartDate", TypeName = "datetime")]
        public DateTime? DStartDate { get; set; }
        [Column("D_EndDate", TypeName = "datetime")]
        public DateTime? DEndDate { get; set; }
        [Required]
        [Column("X_Description")]
        [StringLength(13)]
        public string XDescription { get; set; }
        [Column("X_RefNo")]
        [StringLength(50)]
        public string XRefNo { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("N_BookValue", TypeName = "money")]
        public decimal? NBookValue { get; set; }
        [Column("N_ItemID")]
        public int? NItemId { get; set; }
        [Column("X_Branch")]
        [StringLength(50)]
        public string XBranch { get; set; }
        [Required]
        [Column("N_TypeOrder")]
        [StringLength(1)]
        public string NTypeOrder { get; set; }
        [Column("N_Status")]
        public int? NStatus { get; set; }
        [Column("X_CostcentreName")]
        [StringLength(100)]
        public string XCostcentreName { get; set; }
        [Column("X_ItemName")]
        [StringLength(100)]
        public string XItemName { get; set; }
        [Column("N_projectId")]
        public int? NProjectId { get; set; }
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        public int ProjectPeriod { get; set; }
        [Required]
        [StringLength(9)]
        public string Status { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("Dep_Amount", TypeName = "money")]
        public decimal? DepAmount { get; set; }
    }
}
