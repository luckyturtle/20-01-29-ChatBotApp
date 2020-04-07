/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/

using Link.Models;
using Link.Models.CommentModels;
using Link.Models.ConnectionModels;
using Link.Models.EmailLogsModels;
using Link.Models.GroupModels;
using Link.Models.NotificationModels;
using Link.Models.TransactionModels;
using Link.Models.UserLogsModels;
using Link.Models.UserProfileModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Link.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<Comment> Comment { get; set; }
        public DbSet<CommentUserVote> CommentUserVote { get; set; }
        public DbSet<VoteType> VoteType { get; set; }

        public DbSet<Gender> Gender { get; set; }

        public DbSet<Followers> Followers { get; set; }

        public DbSet<Viewers> Viewers { get; set; }

        public DbSet<CommentGroupType> CommentGroupType { get; set; }

        public DbSet<Group> Group { get; set; }
        public DbSet<GroupType> GroupType { get; set; }

        public DbSet<GroupMember> GroupMember { get; set; }
        public DbSet<GroupViewers> GroupViewers { get; internal set; }

        public DbSet<EmailLogsInfo> EmailLogsInfo { get; set; }
        public DbSet<EmailType> EmailType { get; set; }

        public DbSet<GroupMemberRequest> GroupMemberRequest { get; set; }

        public DbSet<Connection> Connections { get; set; }

        public DbSet<Notification> Notification { get; set; }

        public DbSet<NotificationType> NotificationType { get; set; }

        public DbSet<CommentTransaction> CommentTransaction { get; set; }

        public DbSet<CommentTransactionType> CommentTransactionType { get; set; }

        public DbSet<UserAction> UserAction { get; set; }

        public DbSet<UserLogs> UserLogs { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);

        }
    }
}
