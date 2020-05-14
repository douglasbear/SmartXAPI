using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwReturnPendingVacation
    {
        [Column("X_Type")]
        [StringLength(5)]
        public string XType { get; set; }
        [Column("N_VacationID")]
        public int NVacationId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("D_VacDateFrom", TypeName = "datetime")]
        public DateTime? DVacDateFrom { get; set; }
        [Column("D_VacDateTo", TypeName = "datetime")]
        public DateTime? DVacDateTo { get; set; }
        [Column("D_ReturnDate", TypeName = "datetime")]
        public DateTime? DReturnDate { get; set; }
    }
}
