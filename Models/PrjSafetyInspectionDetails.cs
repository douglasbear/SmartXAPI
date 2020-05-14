using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Prj_SafetyInspectionDetails")]
    public partial class PrjSafetyInspectionDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_InspectionID")]
        public int NInspectionId { get; set; }
        [Key]
        [Column("N_InspectionDetailsID")]
        public int NInspectionDetailsId { get; set; }
        [Column("I_ImageBefore", TypeName = "image")]
        public byte[] IImageBefore { get; set; }
        [Column("I_ImageAfter", TypeName = "image")]
        public byte[] IImageAfter { get; set; }
        [Column("D_ModifiedDate", TypeName = "datetime")]
        public DateTime DModifiedDate { get; set; }
        [Column("D_SupervisedDate", TypeName = "datetime")]
        public DateTime DSupervisedDate { get; set; }
        [Column("X_ObservedNote")]
        [StringLength(800)]
        public string XObservedNote { get; set; }
    }
}
