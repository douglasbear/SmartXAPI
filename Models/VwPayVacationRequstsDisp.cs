using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayVacationRequstsDisp
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_VacRequestID")]
        public int NVacRequestId { get; set; }
        [Column("N_EmpID")]
        public int? NEmpId { get; set; }
        [Column("Vacation Type")]
        [StringLength(50)]
        public string VacationType { get; set; }
        [Column("Request Date", TypeName = "datetime")]
        public DateTime? RequestDate { get; set; }
        [Column("Date From", TypeName = "datetime")]
        public DateTime? DateFrom { get; set; }
        [Column("Date To", TypeName = "datetime")]
        public DateTime? DateTo { get; set; }
        public int? Days { get; set; }
        [StringLength(250)]
        public string Remarks { get; set; }
        [StringLength(50)]
        public string Status { get; set; }
        [Column("Approved Date", TypeName = "datetime")]
        public DateTime? ApprovedDate { get; set; }
        [StringLength(250)]
        public string Comments { get; set; }
        [Column("HR Comments")]
        [StringLength(250)]
        public string HrComments { get; set; }
    }
}
