using ApiClientShared.Enums;
using DbEntity.Models.Entities;
using DbEntity.Models.Entities.Context;

namespace DbEntity.Extensions
{
    public static class CvManagementInMemoryDbContextExtensions
    {
        #region Methods

        /// <summary>
        ///     Seed data to in-memory database.
        /// </summary>
        /// <param name="dbContext"></param>
        public static void Seed(this CvManagementInMemoryDbContext dbContext)
        {
            AddUsers(dbContext);
            AddUserDescriptions(dbContext);
            AddProjects(dbContext);
        }

        /// <summary>
        ///     Add users to user table.
        /// </summary>
        /// <param name="dbContext"></param>
        private static void AddUsers(CvManagementInMemoryDbContext dbContext)
        {
            dbContext.Users.Add(new User(1, "admin@gmail.com", "5773961b8fb0e85fa14aec3681647c7d", "Linh", "Nguyen",
                "https://via.placeholder.com/512x512", 0, UserRoles.Admin, UserStatuses.Active));
            dbContext.Users.Add(new User(2, "user@gmail.com", "5773961b8fb0e85fa14aec3681647c7d", "Hang", "Nguyen",
                "https://via.placeholder.com/512x512", 0, UserRoles.Ordinary, UserStatuses.Active));
            dbContext.Users.Add(new User(3, "lightalakanzam@gmail.com", "5773961b8fb0e85fa14aec3681647c7d", "Lan",
                "Nguyen", "https://via.placeholder.com/512x512", 0, UserRoles.Ordinary, UserStatuses.Active));
            dbContext.Users.Add(new User(4, "redplane_dt@yahoo.com.vn", "5773961b8fb0e85fa14aec3681647c7d", "Ha",
                "Nguyen", "https://via.placeholder.com/512x512", 0, UserRoles.Ordinary, UserStatuses.Active));

            dbContext.SaveChanges();
        }

        /// <summary>
        ///     Add user description to user table.
        /// </summary>
        /// <param name="dbContext"></param>
        private static void AddUserDescriptions(CvManagementInMemoryDbContext dbContext)
        {
            dbContext.UserDescriptions.Add(new UserDescription(1, 1, "DESCRIPTION_01"));
            dbContext.UserDescriptions.Add(new UserDescription(2, 1, "DESCRIPTION_02"));
            dbContext.UserDescriptions.Add(new UserDescription(3, 1, "DESCRIPTION_03"));

            dbContext.UserDescriptions.Add(new UserDescription(4, 1, "DESCRIPTION_01"));
            dbContext.UserDescriptions.Add(new UserDescription(5, 1, "DESCRIPTION_02"));
            dbContext.UserDescriptions.Add(new UserDescription(6, 1, "DESCRIPTION_03"));

            dbContext.UserDescriptions.Add(new UserDescription(7, 1, "DESCRIPTION_01"));
            dbContext.UserDescriptions.Add(new UserDescription(8, 1, "DESCRIPTION_02"));
            dbContext.UserDescriptions.Add(new UserDescription(9, 1, "DESCRIPTION_03"));

            dbContext.SaveChanges();
        }

        /// <summary>
        ///     Add project to in-memory database context.
        /// </summary>
        /// <param name="dbContext"></param>
        private static void AddProjects(CvManagementInMemoryDbContext dbContext)
        {
            dbContext.Projects.Add(new Project(1, 1, "PROJECT_01", "PROJECT_DESCRIPTION_01", 0, 0));
            dbContext.Projects.Add(new Project(2, 1, "PROJECT_02", "PROJECT_DESCRIPTION_02", 0, 0));
            dbContext.Projects.Add(new Project(3, 1, "PROJECT_03", "PROJECT_DESCRIPTION_03", 0, 0));

            dbContext.Projects.Add(new Project(4, 2, "PROJECT_01", "PROJECT_DESCRIPTION_01", 0, 0));
            dbContext.Projects.Add(new Project(5, 2, "PROJECT_02", "PROJECT_DESCRIPTION_02", 0, 0));
            dbContext.Projects.Add(new Project(6, 2, "PROJECT_03", "PROJECT_DESCRIPTION_03", 0, 0));

            dbContext.Projects.Add(new Project(7, 3, "PROJECT_01", "PROJECT_DESCRIPTION_01", 0, 0));
            dbContext.Projects.Add(new Project(8, 3, "PROJECT_02", "PROJECT_DESCRIPTION_02", 0, 0));
            dbContext.Projects.Add(new Project(9, 3, "PROJECT_03", "PROJECT_DESCRIPTION_03", 0, 0));

            dbContext.Projects.Add(new Project(10, 4, "PROJECT_01", "PROJECT_DESCRIPTION_01", 0, 0));
            dbContext.Projects.Add(new Project(11, 4, "PROJECT_02", "PROJECT_DESCRIPTION_02", 0, 0));
            dbContext.Projects.Add(new Project(12, 4, "PROJECT_03", "PROJECT_DESCRIPTION_03", 0, 0));

            dbContext.SaveChanges();
        }
    }

    #endregion
}