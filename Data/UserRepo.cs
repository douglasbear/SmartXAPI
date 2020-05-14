using System;
using System.Collections.Generic;
using System.Linq;
using SmartxAPI.Models;

namespace SmartxAPI.Data
{
    public class UserRepo : IUserRepo
    {
        private readonly SmartxContext _context;

        public UserRepo(SmartxContext context)
        {
            _context = context;
        }

        public void CreateUser(SecUser cust)
        {
            if(cust == null)
            {
                throw new ArgumentNullException(nameof(cust));
            }

            _context.SecUser.Add(cust);
        }

        public void DeleteUser(SecUser cust)
        {
            if(cust == null)
            {
                throw new ArgumentNullException(nameof(cust));
            }
            _context.SecUser.Remove(cust);
        }

        public IEnumerable<SecUser> GetAllUsers()
        {
            return _context.SecUser.ToList();
        }

        public SecUser GetUserById(int id)
        {
            return _context.SecUser.FirstOrDefault(p => p.NUserId == id);
        }

        public bool SaveChanges()
        {
            return (_context.SaveChanges() >= 0);
        }

        public void UpdateUser(SecUser cust)
        {
            //Nothing
        }

    
    }

    public interface IUserRepo
    {
        bool SaveChanges();
        IEnumerable<SecUser> GetAllUsers();
        SecUser GetUserById(int id);
        void CreateUser(SecUser cust);
        void UpdateUser(SecUser cust);
        void DeleteUser(SecUser cust);
    }
}