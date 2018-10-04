using System.Collections.Generic;
using ApiClientShared.Enums;
using Microsoft.EntityFrameworkCore;

namespace DbEntity.Models.Entities.Context
{
    public class CvManagementInMemoryDbContext : CvManagementDbContext
    {
        #region Constructor

        public CvManagementInMemoryDbContext(DbContextOptions options) : base(options)
        {
        }

        #endregion
    }
}