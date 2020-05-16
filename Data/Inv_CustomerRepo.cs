using System;
using System.Collections.Generic;
using System.Linq;
using SmartxAPI.Models;

namespace SmartxAPI.Data
{
    public class Inv_CustomerRepo : IInv_CustomerRepo
    {
        private readonly SmartxContext _context;

        public Inv_CustomerRepo(SmartxContext context)
        {
            _context = context;
        }

        public void CreateCustomer(InvCustomer cust)
        {
            if(cust == null)
            {
                throw new ArgumentNullException(nameof(cust));
            }

            _context.InvCustomer.Add(cust);
        }

        public void DeleteCustomer(InvCustomer cust)
        {
            if(cust == null)
            {
                throw new ArgumentNullException(nameof(cust));
            }
            _context.InvCustomer.Remove(cust);
        }

        public IEnumerable<InvCustomer> GetAllCustomers()
        {
            return _context.InvCustomer.ToList();
        }

        public InvCustomer GetCustomerById(int id)
        {
            return _context.InvCustomer.FirstOrDefault(p => p.NCustomerId == id);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void UpdateCustomer(InvCustomer cust)
        {
            //Nothing
        }

    
    }

    public interface IInv_CustomerRepo
    {
        bool SaveChanges();
        IEnumerable<InvCustomer> GetAllCustomers();
        InvCustomer GetCustomerById(int id);
        void CreateCustomer(InvCustomer cust);
        void UpdateCustomer(InvCustomer cust);
        void DeleteCustomer(InvCustomer cust);
    }
}