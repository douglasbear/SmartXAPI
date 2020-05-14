using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_ImportBank")]
    public partial class AccImportBank
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_ID")]
        public int NId { get; set; }
        [Column("X_Code")]
        [StringLength(50)]
        public string XCode { get; set; }
        [Column("D_ImportDate", TypeName = "datetime")]
        public DateTime? DImportDate { get; set; }
        [Column("N_userID")]
        public int? NUserId { get; set; }
        [Column("X_Remarks")]
        [StringLength(500)]
        public string XRemarks { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
        [Column("D_Date")]
        [StringLength(500)]
        public string DDate { get; set; }
        [Column("D_ValueDate")]
        [StringLength(500)]
        public string DValueDate { get; set; }
        [Column("N_TypeID")]
        [StringLength(500)]
        public string NTypeId { get; set; }
        [Column("X_ChequeNo")]
        [StringLength(500)]
        public string XChequeNo { get; set; }
        [StringLength(500)]
        public string DrAmount { get; set; }
        [StringLength(500)]
        public string CrAmount { get; set; }
        [Column("Date_Format")]
        [StringLength(500)]
        public string DateFormat { get; set; }

        [ForeignKey(nameof(NCompanyId))]
        [InverseProperty(nameof(AccCompany.AccImportBank))]
        public virtual AccCompany NCompany { get; set; }
    }
}
