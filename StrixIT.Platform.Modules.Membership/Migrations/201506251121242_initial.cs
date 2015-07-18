//------------------------------------------------------------------------------
// <auto-generated>
//     This code was not generated by a tool. but for stylecop suppression.
// </auto-generated>
//------------------------------------------------------------------------------
namespace StrixIT.Platform.Modules.Membership.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Applications",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 250),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Groups",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 250),
                        UsePermissions = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        GroupId = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 250),
                        Description = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Groups", t => t.GroupId, cascadeDelete: true)
                .Index(t => t.GroupId);
            
            CreateTable(
                "dbo.GroupsInRoles",
                c => new
                    {
                        GroupId = c.Guid(nullable: false),
                        RoleId = c.Guid(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(),
                        MaxNumberOfUsers = c.Int(),
                        CurrentNumberOfUsers = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.GroupId, t.RoleId })
                .ForeignKey("dbo.Roles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.Groups", t => t.GroupId)
                .Index(t => t.GroupId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.Permissions",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        ApplicationId = c.Guid(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Applications", t => t.ApplicationId)
                .Index(t => t.ApplicationId);
            
            CreateTable(
                "dbo.UserProfileFields",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        FieldType = c.Int(nullable: false),
                        Name = c.String(nullable: false, maxLength: 100),
                        Section = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserProfileValues",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        UserId = c.Guid(nullable: false),
                        Culture = c.String(nullable: false, maxLength: 5),
                        CustomFieldId = c.Guid(nullable: false),
                        NumberValue = c.Double(),
                        StringValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.UserProfileFields", t => t.CustomFieldId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.CustomFieldId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(maxLength: 250),
                        Email = c.String(nullable: false, maxLength: 250),
                        PreferredCulture = c.String(nullable: false, maxLength: 5),
                        DateAcceptedTerms = c.DateTime(),
                        LockedOut = c.Boolean(nullable: false),
                        Approved = c.Boolean(nullable: false),
                        Password = c.String(nullable: false, maxLength: 256),
                        VerificationId = c.Guid(),
                        VerificationWindowStart = c.DateTime(),
                        LastLoginDate = c.DateTime(),
                        FailedPasswordAttemptCount = c.Int(nullable: false),
                        FailedPasswordAttemptWindowStart = c.DateTime(),
                        RegistrationComment = c.String(maxLength: 500),
                        Session = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UsersInRoles",
                c => new
                    {
                        GroupRoleGroupId = c.Guid(nullable: false),
                        GroupRoleRoleId = c.Guid(nullable: false),
                        UserId = c.Guid(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(),
                    })
                .PrimaryKey(t => new { t.GroupRoleGroupId, t.GroupRoleRoleId, t.UserId })
                .ForeignKey("dbo.GroupsInRoles", t => new { t.GroupRoleGroupId, t.GroupRoleRoleId })
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => new { t.GroupRoleGroupId, t.GroupRoleRoleId })
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.RolePermissions",
                c => new
                    {
                        RoleId = c.Guid(nullable: false),
                        PermissionId = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.RoleId, t.PermissionId })
                .ForeignKey("dbo.Roles", t => t.RoleId, cascadeDelete: true)
                .ForeignKey("dbo.Permissions", t => t.PermissionId, cascadeDelete: true)
                .Index(t => t.RoleId)
                .Index(t => t.PermissionId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UsersInRoles", "UserId", "dbo.Users");
            DropForeignKey("dbo.UsersInRoles", new[] { "GroupRoleGroupId", "GroupRoleRoleId" }, "dbo.GroupsInRoles");
            DropForeignKey("dbo.UserProfileValues", "UserId", "dbo.Users");
            DropForeignKey("dbo.UserProfileValues", "CustomFieldId", "dbo.UserProfileFields");
            DropForeignKey("dbo.GroupsInRoles", "GroupId", "dbo.Groups");
            DropForeignKey("dbo.RolePermissions", "PermissionId", "dbo.Permissions");
            DropForeignKey("dbo.RolePermissions", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.Permissions", "ApplicationId", "dbo.Applications");
            DropForeignKey("dbo.GroupsInRoles", "RoleId", "dbo.Roles");
            DropForeignKey("dbo.Roles", "GroupId", "dbo.Groups");
            DropIndex("dbo.RolePermissions", new[] { "PermissionId" });
            DropIndex("dbo.RolePermissions", new[] { "RoleId" });
            DropIndex("dbo.UsersInRoles", new[] { "UserId" });
            DropIndex("dbo.UsersInRoles", new[] { "GroupRoleGroupId", "GroupRoleRoleId" });
            DropIndex("dbo.UserProfileValues", new[] { "CustomFieldId" });
            DropIndex("dbo.UserProfileValues", new[] { "UserId" });
            DropIndex("dbo.Permissions", new[] { "ApplicationId" });
            DropIndex("dbo.GroupsInRoles", new[] { "RoleId" });
            DropIndex("dbo.GroupsInRoles", new[] { "GroupId" });
            DropIndex("dbo.Roles", new[] { "GroupId" });
            DropTable("dbo.RolePermissions");
            DropTable("dbo.UsersInRoles");
            DropTable("dbo.Users");
            DropTable("dbo.UserProfileValues");
            DropTable("dbo.UserProfileFields");
            DropTable("dbo.Permissions");
            DropTable("dbo.GroupsInRoles");
            DropTable("dbo.Roles");
            DropTable("dbo.Groups");
            DropTable("dbo.Applications");
        }
    }
}
