using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserDomain.model;
using UserDomain.Model;

namespace UserDomain.ControlModule.Interfaces
{
    public interface IUserManager
    {
        public Task<string> CreateUser(PrimeUser newUser, string? thumbnail = "", bool reload = true);

        public Task<string> ArchiveUser(PrimeUser newUser, bool reload = true);

        public Task<string> UpdateUser(PrimeUser newUser, bool reload = true);

        public Task<string> DeleteUser(PrimeUser deleteUser, bool reload = true);

        public Task<PrimeUser> GetUserFromGuid(Guid guid, bool reload = true);
        public Task<PrimeUserCollection> GetAllActiveUsers(bool reload = true);

        public Task<PrimeUser?> GetUserFromTag(string tag, bool reload = true);

        public Task<PrimeUser?> GetUserFromCode(string code, bool reload = true);

        public Task<PrimeUser?> GetNextUserFromDirection(Guid id, string direction, bool reload = true);

        public Task<ArchiveUser?> GetArchiveUserFromCode(string code, bool reload = true);

        public Task<List<UserImage>> GetUserImages(Guid guid, bool reload = true);

        public Task<string> SaveUserImage(UserImage newImage, bool reload = true);

        public Task<string> DeleteUserImage(UserImage newImage, bool reload = true);
        public Task<bool> GenerateTestUsers();
    }
}
