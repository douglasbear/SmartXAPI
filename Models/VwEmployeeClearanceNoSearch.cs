using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwEmployeeClearanceNoSearch
    {
        [Column("X_ClearanceCode")]
        [StringLength(20)]
        public string XClearanceCode { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ClearanceID")]
        public int NClearanceId { get; set; }
        [Column("D_RequestDate")]
        [StringLength(30)]
        public string DRequestDate { get; set; }
        [Column("X_EmpName")]
        [StringLength(100)]
        public string XEmpName { get; set; }
        [Column("D_HireDate")]
        [StringLength(30)]
        public string DHireDate { get; set; }
        [Required]
        [Column("X_Purpose")]
        [StringLength(50)]
        public string XPurpose { get; set; }
        [Column("X_Department")]
        [StringLength(100)]
        public string XDepartment { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("N_PurposeID")]
        public int? NPurposeId { get; set; }
        [Column("D_ReqDate", TypeName = "datetime")]
        public DateTime? DReqDate { get; set; }
        [Column("D_StartDate", TypeName = "datetime")]
        public DateTime? DStartDate { get; set; }
        [Column("D_EndDate", TypeName = "datetime")]
        public DateTime? DEndDate { get; set; }
    }
}
