using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwAccCostCentreMasterList
    {
        [Column("N_CostCentreID")]
        public int NCostCentreId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
    }
}
