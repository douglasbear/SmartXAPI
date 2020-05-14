using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Web_Pay_VacationRequest")]
    public partial class WebPayVacationRequest
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Key]
        [Column("N_VacRequestID")]
        public int NVacRequestId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("D_VacReqestDate", TypeName = "datetime")]
        public DateTime? DVacReqestDate { get; set; }
        [Column("D_VacDateFrom", TypeName = "datetime")]
        public DateTime? DVacDateFrom { get; set; }
        [Column("D_VacDateTo", TypeName = "datetime")]
        public DateTime? DVacDateTo { get; set; }
        [Column("N_VacTypeID")]
        public int? NVacTypeId { get; set; }
        [Column("N_VacDays")]
        public int? NVacDays { get; set; }
        [Column("X_VacRemarks")]
        [StringLength(250)]
        public string XVacRemarks { get; set; }
        [Column("N_VacStatus")]
        public int? NVacStatus { get; set; }
        [Column("X_VacApprovedDate", TypeName = "datetime")]
        public DateTime? XVacApprovedDate { get; set; }
        [Column("X_VacApprovedComments")]
        [StringLength(250)]
        public string XVacApprovedComments { get; set; }
        [Column("N_HRComments")]
        [StringLength(250)]
        public string NHrcomments { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_VacContact")]
        [StringLength(20)]
        public string NVacContact { get; set; }
    }
}
