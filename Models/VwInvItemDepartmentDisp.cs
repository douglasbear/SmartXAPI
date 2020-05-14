﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvItemDepartmentDisp
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_DepartmentID")]
        public int NDepartmentId { get; set; }
        [StringLength(50)]
        public string Code { get; set; }
        [StringLength(100)]
        public string Description { get; set; }
    }
}
