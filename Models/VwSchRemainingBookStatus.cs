using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwSchRemainingBookStatus
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_AcYearID")]
        public int NAcYearId { get; set; }
        [Required]
        [Column("Admission_No")]
        [StringLength(25)]
        public string AdmissionNo { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Column("X_Class")]
        [StringLength(50)]
        public string XClass { get; set; }
        [Column("Item_Code")]
        [StringLength(50)]
        public string ItemCode { get; set; }
        [Column("Book_Name")]
        [StringLength(600)]
        public string BookName { get; set; }
        [Column("Item_Qty")]
        public double? ItemQty { get; set; }
        [Column("Distr_Qty")]
        public double? DistrQty { get; set; }
        [Column("Rem_Qty")]
        public double? RemQty { get; set; }
        public int BookId { get; set; }
        [Column("X_ClassDivision")]
        [StringLength(50)]
        public string XClassDivision { get; set; }
        [Column("X_ClassType")]
        [StringLength(50)]
        public string XClassType { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
    }
}
