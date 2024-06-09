using ServerCommonModule.Repository.Interfaces;
using ServerCommonModule.Repository;
using ServerCommonModule.Database.Interfaces;
using ServerCommonModule.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserDomain.ControlModule.Interfaces;
using UserDomain.model;
using UserDomain.Model;
using Npgsql;
using UserDomain.user_image;

namespace UserDomain.ControlModule
{
    public class UserManager : IUserManager
    {
        private IRepositoryFactory fact;
        private IRepositoryManager<PrimeUser> primeUserManager;
        private IRepositoryManager<ArchiveUser> archiveUserManager;
        private IRepositoryManager<UserImage> userImageManager;
        private IDbUtility dbUtility;
        private IDbUtilityFactory dbUtilityFactory;
        private ThumbnailGenerator thumbnailGenerator = new ThumbnailGenerator();

        private PrimeUserCollection primeUsers = new PrimeUserCollection();
        private ArchiveUserCollection archiveUsers = new ArchiveUserCollection();
        private UserImageCollection userImages = new UserImageCollection();

        /// <summary>
        /// Constructor for local dev host. DB wont exist on sub site so wont work. Plus forced to postgress
        /// whereas lower envParams will use either or
        /// </summary>
        public UserManager()
        {
            IEnvironmentalParameters envParms = new EnvironmentalParameters();
            envParms.ConnectionString = "Host=localhost;Username=postgres;Password=modena;Database=UserDb";
            envParms.DatabaseType = "PostgreSQL";
            dbUtilityFactory = new PgUtilityFactory(envParms, null);

            fact = new RepositoryFactory(dbUtilityFactory, envParms);


        }

        /// <summary>
        /// COnstrcutor for web application
        /// </summary>
        /// <param name="envParams"></param>
        public UserManager(IEnvironmentalParameters envParams, IDbUtilityFactory dbUtilityFactory)
        {
            dbUtilityFactory = new PgUtilityFactory(envParams, null);

            fact = new RepositoryFactory(dbUtilityFactory, envParams);
        }

        #region Load Methods
        /// <summary>
        /// Defaults to auto reload but if contents are expected to be stable then the reload flag
        /// can be adjusted so the cached value is used preventing db overhead.
        /// </summary>
        /// <param name="reload"></param>
        /// <returns></returns>
        private async Task<PrimeUserCollection> LoadCollection(bool reload)
        {
            if (reload == true || primeUsers?.Count == 0)
            {
                primeUsers = new PrimeUserCollection();
                primeUserManager = fact.Get(primeUsers);
                await primeUserManager.LoadCollection();
            }

            if (primeUsers == null) primeUsers = new PrimeUserCollection();

            return primeUsers;
        }

        private async Task<ArchiveUserCollection> LoadArchiveCollection(bool reload)
        {
            if (reload == true || archiveUsers?.Count == 0)
            {
                archiveUsers = new ArchiveUserCollection();
                archiveUserManager = fact.Get(archiveUsers);
                await primeUserManager.LoadCollection();
            }

            if (archiveUsers == null) archiveUsers = new ArchiveUserCollection();

            return archiveUsers;
        }


        private async Task<UserImageCollection> LoadUserImageCollection(bool reload)
        {
            if (reload == true || userImages?.Count == 0)
            {
                userImages = new UserImageCollection();
                userImageManager = fact.Get(userImages);
                await userImageManager.LoadCollection();
            }

            if (userImages == null) userImages = new UserImageCollection();

            return userImages;
        }
        #endregion Load Methods



        public async Task<string> CreateUser(PrimeUser newUser, string? thumbnail = "", bool reload = true)
        {
            string status = System.String.Empty;
            PrimeUserCollection primeUsers = await LoadCollection(reload);
            ArchiveUserCollection? archiveUsers = await LoadArchiveCollection(reload);
            // uniqueness check first

            List<PrimeUser> duplicates = primeUsers.Where(x => (x.Code == newUser.Code || x.Tag == newUser.Tag) && x.Id != newUser.Id).ToList();
            List<ArchiveUser> archivedDuplicates = archiveUsers.Where(x => (x.Code == newUser.Code || x.Tag == newUser.Tag) && x.Id != newUser.Id).ToList();

            if (duplicates.Count > 0) status = Properties.strings.DuplicateUserTagStatus;
            else if (archivedDuplicates.Count > 0) status = Properties.strings.DuplicateArchivedUserTagStatus;
            else
            {
                newUser.ThumbnailUrl = thumbnailGenerator.CreateNewThumbnail(newUser.Tag);
                primeUsers.Add(newUser);
                await primeUserManager.InsertSingleItem(newUser);
            }


            return status;
        }

        public async Task<string> ArchiveUser(PrimeUser archiveThisUser, bool reload = true)
        {
            string status = System.String.Empty;
            PrimeUserCollection primeUsers = await LoadCollection(reload);
            ArchiveUserCollection archiveUsers = await LoadArchiveCollection(reload);
            // uniqueness check first
            List<ArchiveUser> archivedDuplicates = archiveUsers.Where(x => (x.Code == archiveThisUser.Code || x.Tag == archiveThisUser.Tag || x.Id == archiveThisUser.Id)).ToList();

            if (archivedDuplicates.Count > 0) status = Properties.strings.DuplicateArchivedUserTagStatus;
            else
            {
                // add to archive table first - do so BEFORE deleting from main table in case archived creation fails, otherwise data integrity is broken
                ArchiveUser newArchiveUser = new ArchiveUser(archiveThisUser);
                archiveUsers.Add(newArchiveUser);
                await archiveUserManager.InsertSingleItem(newArchiveUser);


                // now can delete from main
                await DeleteUser(archiveThisUser);

            }


            return status;
        }

        public async Task<string> DeleteUser(PrimeUser deleteUser, bool reload = true)
        {
            string status = System.String.Empty;
            PrimeUserCollection primeUsers = await LoadCollection(reload);

            primeUsers.Remove(deleteUser);
            await primeUserManager.DeleteSingleItem(deleteUser);

            return status;
        }

        public async Task<string> UpdateUser(PrimeUser updateUser, bool reload = true)
        {
            string status = System.String.Empty;
            PrimeUserCollection primeUsers = await LoadCollection(reload);
            ArchiveUserCollection archiveUsers = await LoadArchiveCollection(reload);
            // uniqueness check first
            List<PrimeUser> duplicates = primeUsers.Where(x => (x.Code == updateUser.Code || x.Tag == updateUser.Tag) && x.Id != updateUser.Id).ToList();
            List<ArchiveUser> archivedDuplicates = archiveUsers.Where(x => (x.Code == updateUser.Code || x.Tag == updateUser.Tag) && x.Id != updateUser.Id).ToList();

            if (duplicates.Count > 0) status = Properties.strings.DuplicateUserTagStatus;
            else if (archivedDuplicates.Count > 0) status = Properties.strings.DuplicateArchivedUserTagStatus;
            else await primeUserManager.UpdateSingleItem(updateUser);

            return status;
        }

        public async Task<PrimeUser> GetUserFromGuid(Guid guid, bool reload = true)
        {
            PrimeUserCollection primeUsers = await LoadCollection(reload);
            return primeUsers.FindById(guid);
        }

        public async Task<PrimeUserCollection> GetAllActiveUsers(bool reload = true)
        {
            PrimeUserCollection primeUsers = await LoadCollection(reload);
            return primeUsers;
        }

        public async Task<PrimeUser?> GetUserFromTag(string tag, bool reload = true)
        {
            PrimeUserCollection primeUsers = await LoadCollection(reload);
            return primeUsers.Where(x => x.Tag == tag).FirstOrDefault();
        }

        public async Task<PrimeUser?> GetUserFromCode(string code, bool reload = true)
        {
            PrimeUserCollection primeUsers = await LoadCollection(reload);
            return primeUsers.Where(x => x.Code == code).FirstOrDefault();
        }

        public async Task<ArchiveUser?> GetArchiveUserFromCode(string code, bool reload = true)
        {
            ArchiveUserCollection archiveUsers = await LoadArchiveCollection(reload);
            return archiveUsers.Where(x => x.Code == code).FirstOrDefault();
        }

        public async Task<List<UserImage>> GetUserImages(Guid guid, bool reload = true)
        {
            List<UserImage> userImagesFound = new();

            UserImageCollection userImages = await LoadUserImageCollection(reload);
            return userImages.Where(x => x.Id == guid).ToList();
        }

        public async Task<string> SaveUserImage(UserImage newImage, bool reload = true)
        {
            string status = System.String.Empty;
            UserImageCollection userImagesAll = await LoadUserImageCollection(reload);

            // uniqueness check first
            List<UserImage> duplicates = userImagesAll.Where(x => (x.Id == newImage.Id && x.ImageType == newImage.ImageType)).ToList();

            if (duplicates.Count > 0)
            {
                // replace current
                await userImageManager.UpdateSingleItem(newImage);
            }
            else
            {
                await userImageManager.InsertSingleItem(newImage);
            }


            return status;
        }

        public async Task<string> DeleteUserImage(UserImage newImage, bool reload = true)
        {
            string status = System.String.Empty;
            UserImageCollection userImages = await LoadUserImageCollection(reload);

            userImages.Remove(newImage);
            await userImageManager.DeleteSingleItem(newImage);

            return status;
        }

        public async Task<bool> GenerateTestUsers()
        {
            string commandText = $"CALL public.create_test_users()";

            IDbUtility dbUtility = dbUtilityFactory.Get();

            await dbUtility.ExecuteNonQuery(dbUtilityFactory.Get().GetBaseConnection(), commandText, null);

            return true;
        }

        public async Task<PrimeUser?> GetNextUserFromDirection(Guid guid, string direction, bool reload = true)
        {
            PrimeUserCollection primeUsers = await LoadCollection(reload);


            PrimeUser current = primeUsers.FindById(guid);

            if (primeUsers.Count > 0)
            {
                if (current != null)
                {
                    // default return if all else fails

                    switch (direction.ToLower())
                    {
                        case "first":
                            return primeUsers.OrderBy(x => x.Code).First();
                        case "last":
                            return primeUsers.OrderBy(x => x.Code).Last();
                        case "previous":
                            List<PrimeUser> allUsers = primeUsers.OrderBy(x => x.Code).ToList();
                            for (int a = 0; a < allUsers.Count; a++)
                            {
                                if (allUsers[a].Id == current.Id && a > 0)
                                {
                                    return allUsers[a - 1];
                                }
                            }
                            return current;

                        case "next":
                            List<PrimeUser> allUsers2 = primeUsers.OrderBy(x => x.Code).ToList();
                            for (int a = 0; a < allUsers2.Count; a++)
                            {
                                if (allUsers2[a].Id == current.Id && a + 1 < allUsers2.Count)
                                {
                                    return allUsers2[a + 1];
                                }

                            }
                            return current;
                    }
                    return primeUsers.First();
                }
                else
                {
                    switch (direction.ToLower())
                    {
                        case "first":
                        case "previous":
                            return primeUsers.First();
                        case "next":
                        case "last":
                            return primeUsers.Last();
                    }
                }
            }
            else
            {
                return primeUsers.OrderBy(x => x.Code).First();
            }
            return null;


        }
    }
}
