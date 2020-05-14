using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwServiceDashboard1
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("X_BookingNo")]
        [StringLength(20)]
        public string XBookingNo { get; set; }
        [Column("D_Date")]
        [StringLength(8000)]
        public string DDate { get; set; }
        [Required]
        [Column("N_IMEI")]
        [StringLength(50)]
        public string NImei { get; set; }
        [Column("X_ItemName")]
        [StringLength(600)]
        public string XItemName { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Required]
        [Column("X_Priority")]
        [StringLength(1)]
        public string XPriority { get; set; }
        [Required]
        [Column("D_Schedule_Date")]
        [StringLength(8000)]
        public string DScheduleDate { get; set; }
        [Required]
        [Column("X_prime_responsible")]
        [StringLength(50)]
        public string XPrimeResponsible { get; set; }
        [Column("N_SalesId")]
        public int NSalesId { get; set; }
        [Column("N_CustomerID")]
        public int NCustomerId { get; set; }
        [Column("N_ServiceID")]
        public int NServiceId { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
    }
}
