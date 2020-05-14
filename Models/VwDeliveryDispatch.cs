using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwDeliveryDispatch
    {
        [Required]
        [Column("Invoice No")]
        [StringLength(50)]
        public string InvoiceNo { get; set; }
        [Required]
        [Column("Invoice Date")]
        [StringLength(8000)]
        public string InvoiceDate { get; set; }
        [Column("N_DispatchID")]
        public int NDispatchId { get; set; }
        [Required]
        [Column("X_DispatchNo")]
        [StringLength(50)]
        public string XDispatchNo { get; set; }
        [Required]
        [Column("Delivery Date")]
        [StringLength(8000)]
        public string DeliveryDate { get; set; }
        [Required]
        [Column("Delivery Time")]
        [StringLength(8000)]
        public string DeliveryTime { get; set; }
        [Required]
        [Column("X_Location")]
        [StringLength(200)]
        public string XLocation { get; set; }
        [Column("N_TruckID")]
        public int NTruckId { get; set; }
        [Required]
        [Column("X_Driver")]
        [StringLength(100)]
        public string XDriver { get; set; }
        [Required]
        [Column("X_AssignedPerson")]
        [StringLength(100)]
        public string XAssignedPerson { get; set; }
        [Column("N_Status")]
        public int NStatus { get; set; }
        [Column("N_UserID")]
        public int NUserId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime DEntryDate { get; set; }
        [Column("N_BranchID")]
        public int NBranchId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Required]
        [Column("D_Date")]
        [StringLength(8000)]
        public string DDate { get; set; }
        [Required]
        [Column("Customer Name")]
        [StringLength(100)]
        public string CustomerName { get; set; }
        [Required]
        [Column("Customer Address")]
        [StringLength(250)]
        public string CustomerAddress { get; set; }
        [Required]
        [Column("Phone No")]
        [StringLength(20)]
        public string PhoneNo { get; set; }
        [Column("N_SalesId")]
        public int NSalesId { get; set; }
        [Required]
        [Column("D_CheckDate")]
        [StringLength(8000)]
        public string DCheckDate { get; set; }
        [Required]
        [StringLength(10)]
        public string DispatchStatus { get; set; }
    }
}
