using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_VacationReturn")]
    public partial class PayVacationReturn
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("D_ReturnDate", TypeName = "datetime")]
        public DateTime? DReturnDate { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Key]
        [Column("N_VacationReturnID")]
        public int NVacationReturnId { get; set; }
        [Column("X_VacationReturnCode")]
        [StringLength(50)]
        public string XVacationReturnCode { get; set; }
        [Column("N_VacationID")]
        public int? NVacationId { get; set; }
        [Column("X_Remarks")]
        [StringLength(250)]
        public string XRemarks { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_VacationGroupID")]
        public int? NVacationGroupId { get; set; }
    }
}
