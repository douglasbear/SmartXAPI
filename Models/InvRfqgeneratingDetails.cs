using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    [Table("Inv_RFQGeneratingDetails")]
    public partial class InvRfqgeneratingDetails
    {
        [Column("N_CompanyID")]
        public int? NCompanyId { get; set; }
        [Column("N_ReqstID")]
        public int? NReqstId { get; set; }
        [Key]
        [Column("N_ReqstDetailsID")]
        public int NReqstDetailsId { get; set; }
        [Column("N_VendorID")]
        public int? NVendorId { get; set; }
        [Column("D_Entrydate", TypeName = "datetime")]
        public DateTime DEntrydate { get; set; }
    }
}
