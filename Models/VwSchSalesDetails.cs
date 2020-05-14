using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchSalesDetails
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_CustomerID")]
        public int? NCustomerId { get; set; }
        [Column("N_ItemID")]
        public int NItemId { get; set; }
        [Column("N_YearTotal", TypeName = "money")]
        public decimal? NYearTotal { get; set; }
        [Column("B_Issued")]
        public int? BIssued { get; set; }
        [Column("B_Paid")]
        public int? BPaid { get; set; }
        [Required]
        [Column("X_FeeType")]
        [StringLength(50)]
        public string XFeeType { get; set; }
        [Column("N_FeeCategoryID")]
        public int? NFeeCategoryId { get; set; }
        [Required]
        [Column("X_FrequencyName")]
        [StringLength(15)]
        public string XFrequencyName { get; set; }
        [Column("N_Frequency")]
        public int NFrequency { get; set; }
        [Column("N_FrequencyID")]
        public int? NFrequencyId { get; set; }
        [Column("N_Amount", TypeName = "money")]
        public decimal? NAmount { get; set; }
        [Column("X_ClassType")]
        [StringLength(50)]
        public string XClassType { get; set; }
        [Column("N_ClassTypeID")]
        public int? NClassTypeId { get; set; }
        [Column("X_Class")]
        [StringLength(50)]
        public string XClass { get; set; }
        [Column("N_ClassID")]
        public int? NClassId { get; set; }
        [Column("B_IsRemoved")]
        public int? BIsRemoved { get; set; }
        [Column("N_Frequency2")]
        public int? NFrequency2 { get; set; }
    }
}
