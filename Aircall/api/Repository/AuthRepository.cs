using api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace api.Repository
{
    public class AuthRepository : IDisposable
    {
        private Aircall_DBEntities1 _ctx;
        public AuthRepository()
        {
            _ctx = new Aircall_DBEntities1();
        }

        public Client FindUser(string userName, string password)
        {
            var user = _ctx.Clients.Where(x => x.Email == userName && x.Password == password).FirstOrDefault();

            return user;
        }

        public void Dispose()
        {
            _ctx.Dispose();
           // _userManager.Dispose();

        }
    }
}