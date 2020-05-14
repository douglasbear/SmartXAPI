﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwProjectAndCompany
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
        [Column("N_CustomerID")]
        public int? NCustomerId { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
    }
}
