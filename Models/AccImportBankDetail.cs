using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Acc_ImportBankDetail")]
    public partial class AccImportBankDetail
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Column("N_ID")]
        public int NId { get; set; }
        [Key]
        [Column("N_DetailID")]
        public int NDetailId { get; set; }
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
        [Column("N_Amount")]
        [StringLength(500)]
        public string NAmount { get; set; }
        [Column("X_Remarks")]
        [StringLength(500)]
        public string XRemarks { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
