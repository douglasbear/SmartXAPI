using System;
using System.Collections.Generic;
using System.Linq;
using SmartxAPI.Models;

namespace SmartxAPI.Data
{
    public class Acc_SalesQuotationRepo : IAcc_SalesQuotationRepo
    {
        private readonly SmartxContext _context;

        public Acc_SalesQuotationRepo(SmartxContext context)
        {
            _context = context;
        }


        public IEnumerable<VwInvSalesQuotationNoSearch> GetSalesQuotationList(int? nCompanyId,int nFnYearId)
        {
            return _context.VwInvSalesQuotationNoSearch
            .Where(V => V.NCompanyId==nCompanyId && V.NFnYearId==nFnYearId)
            .ToList();
        }

    }

    public interface IAcc_SalesQuotationRepo
    {
        IEnumerable<VwInvSalesQuotationNoSearch> GetSalesQuotationList(int? nCompanyId,int nFnYearId);
    }
}