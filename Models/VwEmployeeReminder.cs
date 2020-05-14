using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwEmployeeReminder
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("S/N")]
        [StringLength(30)]
        public string SN { get; set; }
        [Column("Employee No")]
        [StringLength(50)]
        public string EmployeeNo { get; set; }
        [StringLength(100)]
        public string Name { get; set; }
        [StringLength(100)]
        public string Position { get; set; }
        [StringLength(100)]
        public string Department { get; set; }
        [StringLength(100)]
        public string Subject { get; set; }
        [Column("Expire Date")]
        [StringLength(8000)]
        public string ExpireDate { get; set; }
        public bool? Active { get; set; }
    }
}
