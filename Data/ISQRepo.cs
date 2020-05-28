using System;
using System.Collections.Generic;
using System.Linq;
using SmartxAPI.Models;

namespace SmartxAPI.Data
{
    public class ISQRepo : IISQRepo
    {
        private readonly SmartxContext _context;

        public ISQRepo(SmartxContext context)
        {
            _context = context;
        }


        public IEnumerable<VwInvSalesQuotationNoSearch> GetSalesQuotationList(int? nCompanyId,int nFnYearId)
        {
            return _context.VwInvSalesQuotationNoSearch
            .Where(V => V.NCompanyId==nCompanyId && V.NFnYearId==nFnYearId)
            .ToList();
        }

        public dynamic GetData(){
            
            return null;
        }

        
    }

    public interface IISQRepo
    {
        IEnumerable<VwInvSalesQuotationNoSearch> GetSalesQuotationList(int? nCompanyId,int nFnYearId);
        dynamic GetData();   
    }
}