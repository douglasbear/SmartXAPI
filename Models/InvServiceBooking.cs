using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_ServiceBooking")]
    public partial class InvServiceBooking
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int NFnYearId { get; set; }
        [Key]
        [Column("N_ServiceID")]
        public int NServiceId { get; set; }
        [Column("X_JobNo")]
        [StringLength(20)]
        public string XJobNo { get; set; }
        [Column("D_Date", TypeName = "datetime")]
        public DateTime? DDate { get; set; }
        [Column("N_CheckedInID")]
        public int? NCheckedInId { get; set; }
        [Column("N_CustomerID")]
        public int NCustomerId { get; set; }
        [Column("X_JobTitle")]
        [StringLength(300)]
        public string XJobTitle { get; set; }
        [Column("N_ServiceTypeID")]
        public int? NServiceTypeId { get; set; }
        [Column("X_ProductType")]
        [StringLength(100)]
        public string XProductType { get; set; }
        [Column("X_Color")]
        [StringLength(20)]
        public string XColor { get; set; }
        [Column("N_EstimatedCost", TypeName = "money")]
        public decimal? NEstimatedCost { get; set; }
        [Column("D_DeliveryDate", TypeName = "datetime")]
        public DateTime? DDeliveryDate { get; set; }
        [Column("N_PriorityID")]
        public int? NPriorityId { get; set; }
        [Required]
        [Column("X_ServiceTag")]
        [StringLength(50)]
        public string XServiceTag { get; set; }
        [Required]
        [Column("X_SerialNo")]
        [StringLength(50)]
        public string XSerialNo { get; set; }
        [Required]
        [Column("X_ModelNo")]
        [StringLength(100)]
        public string XModelNo { get; set; }
        [Required]
        [Column("X_Make")]
        [StringLength(100)]
        public string XMake { get; set; }
        [Required]
        [Column("X_AdditionalNotes")]
        [StringLength(500)]
        public string XAdditionalNotes { get; set; }
        [Column("N_AssignedID")]
        public int? NAssignedId { get; set; }
        [Column("N_StatusID")]
        public int? NStatusId { get; set; }
        [Column("N_Processed")]
        public int? NProcessed { get; set; }
        [Column("N_UserID")]
        public int? NUserId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("X_Notes")]
        [StringLength(100)]
        public string XNotes { get; set; }
        [Column("B_Warranty")]
        public bool? BWarranty { get; set; }
        [Required]
        [Column("X_BillNo")]
        [StringLength(50)]
        public string XBillNo { get; set; }

        [ForeignKey(nameof(NAssignedId))]
        [InverseProperty(nameof(GenLookupTable.InvServiceBooking))]
        public virtual GenLookupTable NAssigned { get; set; }
    }
}
