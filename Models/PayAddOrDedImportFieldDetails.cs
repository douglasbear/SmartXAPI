using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Pay_AddOrDedImportFieldDetails")]
    public partial class PayAddOrDedImportFieldDetails
    {
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
        [Key]
        [Column("N_DetailsID")]
        public int NDetailsId { get; set; }
        [Column("N_TransID")]
        public int? NTransId { get; set; }
        [Column("X_FieldName")]
        [StringLength(100)]
        public string XFieldName { get; set; }
        [Column("X_FieldType")]
        [StringLength(50)]
        public string XFieldType { get; set; }
        [Column("X_PayCode")]
        [StringLength(50)]
        public string XPayCode { get; set; }
        [Column("N_PayID")]
        public int? NPayId { get; set; }
        [Column("D_EntryDate", TypeName = "datetime")]
        public DateTime? DEntryDate { get; set; }
    }
}
