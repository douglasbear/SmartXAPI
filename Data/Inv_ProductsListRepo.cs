using System;
using System.Collections.Generic;
using System.Linq;
using SmartxAPI.Models;

namespace SmartxAPI.Data
{
    public class Inv_ProductsListRepo : IInvProductsListRepo
    {
        private readonly SmartxContext _context;

        public Inv_ProductsListRepo(SmartxContext context)
        {
            _context = context;
        }
        public IEnumerable<VwInvItemSearch> GetAllItems(int? nCompanyID)
        {
            //return _context.AccCompany.ToList();
            return _context.VwInvItemSearch
            .Where(V => V.NCompanyId==nCompanyID)
            .ToList();
        }


    }

    public interface IInvProductsListRepo
    {
        IEnumerable<VwInvItemSearch> GetAllItems(int? nCompanyID);
    }
}