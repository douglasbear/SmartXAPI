using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvJobNoSearch
    {
        [Column("Customer Code")]
        [StringLength(50)]
        public string CustomerCode { get; set; }
        [Column("Customer Name")]
        [StringLength(100)]
        public string CustomerName { get; set; }
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
        [Column("N_BranchID")]
        public int? NBranchId { get; set; }
        [Column("N_ServiceID")]
        public int NServiceId { get; set; }
        [Column(TypeName = "datetime")]
        public DateTime? Expr1 { get; set; }
        [Column("X_ProductType")]
        [StringLength(100)]
        public string XProductType { get; set; }
        [Required]
        [Column("X_ServiceTag")]
        [StringLength(50)]
        public string XServiceTag { get; set; }
        [Required]
        [Column("X_ModelNo")]
        [StringLength(100)]
        public string XModelNo { get; set; }
        [Required]
        [Column("X_SerialNo")]
        [StringLength(50)]
        public string XSerialNo { get; set; }
        [Required]
        [Column("X_Make")]
        [StringLength(100)]
        public string XMake { get; set; }
        public int Expr2 { get; set; }
    }
}
