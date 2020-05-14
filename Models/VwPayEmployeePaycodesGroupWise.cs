using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwPayEmployeePaycodesGroupWise
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_GroupId")]
        public int NGroupId { get; set; }
        [Column("N_EmpID")]
        public int NEmpId { get; set; }
        [Column("X_PayCode")]
        [StringLength(50)]
        public string XPayCode { get; set; }
        [Column("X_Description")]
        [StringLength(100)]
        public string XDescription { get; set; }
        [Column("N_PayTypeID")]
        public int? NPayTypeId { get; set; }
        [Column("N_PayID")]
        public int? NPayId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
    }
}
