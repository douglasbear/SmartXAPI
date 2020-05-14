using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwServiceDashboard
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Column("X_JobNo")]
        [StringLength(20)]
        public string XJobNo { get; set; }
        [Column("D_Date")]
        [StringLength(8000)]
        public string DDate { get; set; }
        [Column("X_ProductType")]
        [StringLength(100)]
        public string XProductType { get; set; }
        [Column("D_DeliveryDate")]
        [StringLength(8000)]
        public string DDeliveryDate { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("X_Priority")]
        [StringLength(50)]
        public string XPriority { get; set; }
        [Column("X_AssignTo")]
        [StringLength(50)]
        public string XAssignTo { get; set; }
        [Column("X_Status")]
        [StringLength(50)]
        public string XStatus { get; set; }
    }
}
