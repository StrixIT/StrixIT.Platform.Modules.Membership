//-----------------------------------------------------------------------
// <copyright file="MembershipDataSource.cs" company="StrixIT">
//     Author: R.G. Schurgers MA MSc. Copyright (c) StrixIT. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Data.Entity;
using System.Linq;
using StrixIT.Platform.Core;

namespace StrixIT.Platform.Modules.Membership
{
    internal class MembershipDataSource : EntityFrameworkDataSource, IMembershipDataSource
    {
        public MembershipDataSource() : base(MembershipConstants.MEMBERSHIP)
        {
            this.Configuration.ValidateOnSaveEnabled = false;
            this.Configuration.LazyLoadingEnabled = false;
        }

        #region DbSets

        public DbSet<Application> Applications { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<UserSecurity> UserSecurity { get; set; }

        public DbSet<UserSessionStorage> UserSessions { get; set; }

        public DbSet<Group> Groups { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Permission> Permissions { get; set; }

        public DbSet<UserInRole> UsersInRoles { get; set; }

        public DbSet<GroupInRole> GroupsInRoles { get; set; }

        public DbSet<UserProfileField> UserProfileFields { get; set; }

        public DbSet<UserProfileValue> UserProfileFieldValues { get; set; }

        #endregion

        public IQueryable<T> Query<T>(string includes) where T : class
        {
            return this.Query<T>().Include(includes);
        }

        public T Find<T>(object[] keys) where T : class
        {
            return this.Set<T>().Find(keys);
        }

        #region Model Builder

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            if (modelBuilder == null)
            {
                throw new ArgumentNullException("modelBuilder");
            }

            modelBuilder.Entity<User>().HasMany(u => u.ProfileValues).WithRequired(p => p.User).WillCascadeOnDelete(false);
            modelBuilder.Entity<User>().ToTable("Users");
            modelBuilder.Entity<Group>().HasMany(g => g.Roles).WithRequired().HasForeignKey(r => r.GroupId);
            modelBuilder.Entity<Group>().HasMany(g => g.Roles).WithRequired(r => r.Group).WillCascadeOnDelete(false);
            modelBuilder.Entity<Role>().HasMany(r => r.Permissions).WithMany(p => p.Roles).Map(ma => ma.MapLeftKey("RoleId").MapRightKey("PermissionId").ToTable("RolePermissions"));
            modelBuilder.Entity<UserSecurity>().HasRequired(s => s.User).WithRequiredPrincipal();
            modelBuilder.Entity<UserSecurity>().ToTable("Users");
            modelBuilder.Entity<UserSessionStorage>().HasRequired(s => s.User).WithRequiredPrincipal();
            modelBuilder.Entity<UserSessionStorage>().HasRequired(s => s.UserSecurity).WithRequiredPrincipal();
            modelBuilder.Entity<UserSessionStorage>().ToTable("Users");
            modelBuilder.Entity<UserInRole>().HasKey(ur => new { ur.GroupRoleGroupId, ur.GroupRoleRoleId, ur.UserId }).ToTable("UsersInRoles");
            modelBuilder.Entity<UserInRole>().HasRequired(u => u.GroupRole).WithMany().WillCascadeOnDelete(false);
            modelBuilder.Entity<GroupInRole>().HasKey(gr => new { gr.GroupId, gr.RoleId }).ToTable("GroupsInRoles");
            modelBuilder.Entity<Permission>().HasRequired(p => p.Application).WithMany().WillCascadeOnDelete(false);

            base.OnModelCreating(modelBuilder);
        }

        #endregion
    }
}