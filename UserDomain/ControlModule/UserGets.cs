using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserDomain.model;
using UserDomain.Model;

namespace UserDomain.ControlModule
{
    public interface IUserGets
    {
        public Task<PrimeUser> GetFromGuid(Guid guid, bool reload = true);
        public Task<PrimeUserCollection> GetAll(bool reload = true);

        public Task<PrimeUser?> GetFromTag(string tag, bool reload = true);

        public Task<PrimeUser?> GetFromCode(string code, bool reload = true);
    }
    public class UserGets
    {
    }
}
