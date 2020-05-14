using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_InvoiceCounterDeleted")]
    public partial class InvInvoiceCounterDeleted
    {
        [Key]
        [Column("N_EntryID")]
        public int NEntryId { get; set; }
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearID")]
        public int? NFnYearId { get; set; }
        [Column("N_FormID")]
        public int? NFormId { get; set; }
        [Column("N_DeletedNo")]
        public int? NDeletedNo { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
