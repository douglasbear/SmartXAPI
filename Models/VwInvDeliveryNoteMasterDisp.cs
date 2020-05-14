﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartxAPI.Models
{
    public partial class VwInvDeliveryNoteMasterDisp
    {
        [Column("N_CompanyId")]
        public int? NCompanyId { get; set; }
        [Column("N_FnYearId")]
        public int? NFnYearId { get; set; }
        [Column("N_DeliveryNoteId")]
        public int NDeliveryNoteId { get; set; }
        [Column("X_DeliveryNoteNo")]
        [StringLength(50)]
        public string XDeliveryNoteNo { get; set; }
        [Column("D_DeliveryDate", TypeName = "smalldatetime")]
        public DateTime? DDeliveryDate { get; set; }
        [Column("N_CustomerId")]
        public int? NCustomerId { get; set; }
        [Column("x_Notes")]
        [StringLength(1000)]
        public string XNotes { get; set; }
        [Column("N_QuotationID")]
        public int? NQuotationId { get; set; }
        [Column("N_SalesOrderID")]
        public int? NSalesOrderId { get; set; }
        [Column("X_POrderNo")]
        [StringLength(50)]
        public string XPorderNo { get; set; }
        [Column("N_ProjectID")]
        public int? NProjectId { get; set; }
        [Column("B_BeginingBalEntry")]
        public bool? BBeginingBalEntry { get; set; }
        [Column("N_DeliveryType")]
        public int? NDeliveryType { get; set; }
        [Column("N_DeliveryRefID")]
        public int? NDeliveryRefId { get; set; }
        [Column("N_SalesmanID")]
        public int? NSalesmanId { get; set; }
        [Column("X_SalesmanName")]
        [StringLength(100)]
        public string XSalesmanName { get; set; }
        [Column("N_LocationID")]
        public int? NLocationId { get; set; }
        [Column("N_BranchId")]
        public int? NBranchId { get; set; }
        [Column("X_TransType")]
        [StringLength(50)]
        public string XTransType { get; set; }
        [Column("B_IsSaveDraft")]
        public bool? BIsSaveDraft { get; set; }
        [Column("X_QuotationNo")]
        [StringLength(50)]
        public string XQuotationNo { get; set; }
        [Column("X_ProjectName")]
        [StringLength(50)]
        public string XProjectName { get; set; }
        [Column("X_CustomerCode")]
        [StringLength(50)]
        public string XCustomerCode { get; set; }
        [Column("X_CustomerName")]
        [StringLength(100)]
        public string XCustomerName { get; set; }
        [Column("X_SalesmanCode")]
        [StringLength(50)]
        public string XSalesmanCode { get; set; }
        [Column("X_CustPONo")]
        [StringLength(50)]
        public string XCustPono { get; set; }
        [Column("N_FreightAmt", TypeName = "money")]
        public decimal? NFreightAmt { get; set; }
        [Column("N_TaxAmt", TypeName = "money")]
        public decimal? NTaxAmt { get; set; }
        [Column("N_DriverID")]
        public int? NDriverId { get; set; }
        [Column("X_DriverName")]
        [StringLength(100)]
        public string XDriverName { get; set; }
        [Column("N_TruckID")]
        public int? NTruckId { get; set; }
    }
}
