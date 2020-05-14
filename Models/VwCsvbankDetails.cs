using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwCsvbankDetails
    {
        [Column("N_BankID")]
        public int NBankId { get; set; }
        [Column("X_BankName")]
        [StringLength(100)]
        public string XBankName { get; set; }
        [Column("N_CsvTemplateID")]
        public int? NCsvTemplateId { get; set; }
        [Column("X_Name")]
        [StringLength(50)]
        public string XName { get; set; }
        [Column("N_ReferId")]
        public int? NReferId { get; set; }
        [Column("N_CompanyID")]
        public int NCompanyId { get; set; }
    }
}
