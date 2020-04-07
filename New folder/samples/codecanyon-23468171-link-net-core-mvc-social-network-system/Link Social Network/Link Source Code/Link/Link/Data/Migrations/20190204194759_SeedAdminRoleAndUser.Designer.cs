/**********************************************************************************************************************
 * Link  is a Social Network for enable distributed user to involve, collaborate and communicate . 
 * Copyright(C) 2019 Indie Sudio. All rights reserved.
 * https://indiestd.com/
 * info@indiestd.com
 *********************************************************************************************************************/
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace Link.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20190204194759_SeedAdminRoleAndUser")]
    partial class SeedAdminRoleAndUser
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.2.1-servicing-10028")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Link.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AccessFailedCount");

                    b.Property<byte[]>("Avatar");

                    b.Property<DateTime?>("BirthDate");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Description");

                    b.Property<string>("Email")
                        .HasMaxLength(256);

                    b.Property<bool>("EmailConfirmed");

                    b.Property<int?>("GenderId");

                    b.Property<string>("Interests");

                    b.Property<bool>("LockoutEnabled");

                    b.Property<DateTimeOffset?>("LockoutEnd");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256);

                    b.Property<int>("NumberOfFollwer");

                    b.Property<int>("NumberOfViewer");

                    b.Property<string>("PasswordHash");

                    b.Property<string>("PhoneNumber");

                    b.Property<bool>("PhoneNumberConfirmed");

                    b.Property<DateTime>("RegistrationDate")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("SecurityStamp");

                    b.Property<string>("SocialMedia");

                    b.Property<bool>("TwoFactorEnabled");

                    b.Property<string>("UserName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("GenderId");

                    b.HasIndex("NormalizedEmail")
                        .HasName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers");
                });

            modelBuilder.Entity("Link.Models.CommentModels.Comment", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CommentGroupTypeId");

                    b.Property<string>("Content");

                    b.Property<DateTime>("Created");

                    b.Property<bool>("CreatedByAdmin");

                    b.Property<bool>("CreatedByCurrentUser");

                    b.Property<string>("Creator");

                    b.Property<byte[]>("File");

                    b.Property<string>("FileMimeType");

                    b.Property<string>("FileURL");

                    b.Property<string>("GroupId");

                    b.Property<bool>("IsNew");

                    b.Property<DateTime>("Modified");

                    b.Property<string>("Parent");

                    b.Property<string>("Pings");

                    b.Property<string>("Root");

                    b.Property<int>("UpvoteCount");

                    b.Property<bool>("UserHasUpvoted");

                    b.Property<string>("UserVoted");

                    b.HasKey("Id");

                    b.HasIndex("CommentGroupTypeId");

                    b.ToTable("Comment");
                });

            modelBuilder.Entity("Link.Models.CommentModels.CommentGroupType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("CommentGroupType");
                });

            modelBuilder.Entity("Link.Models.CommentModels.CommentUserVote", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CommentId");

                    b.Property<string>("UserId");

                    b.Property<int>("VoteTypeId");

                    b.HasKey("Id");

                    b.HasIndex("VoteTypeId");

                    b.ToTable("CommentUserVote");
                });

            modelBuilder.Entity("Link.Models.CommentModels.VoteType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("VoteType");
                });

            modelBuilder.Entity("Link.Models.ConnectionModels.Connection", b =>
                {
                    b.Property<string>("ConnectionID")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("UserAgent");

                    b.Property<string>("UserId");

                    b.HasKey("ConnectionID");

                    b.ToTable("Connections");
                });

            modelBuilder.Entity("Link.Models.EmailLogsModels.EmailLogsInfo", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("CreatedDate");

                    b.Property<int>("EmailTypeId");

                    b.Property<string>("From");

                    b.Property<bool>("IsSent");

                    b.Property<string>("Message");

                    b.Property<string>("Subject");

                    b.Property<string>("To");

                    b.HasKey("Id");

                    b.HasIndex("EmailTypeId");

                    b.ToTable("EmailLogsInfo");
                });

            modelBuilder.Entity("Link.Models.EmailLogsModels.EmailType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("EmailType");
                });

            modelBuilder.Entity("Link.Models.GroupModels.Group", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("Created");

                    b.Property<string>("Creator");

                    b.Property<string>("Description");

                    b.Property<int>("GroupTypeId");

                    b.Property<string>("LastUpdatedBy");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255);

                    b.Property<int>("NumberOfMember");

                    b.Property<int>("NumberOfViewer");

                    b.Property<byte[]>("Photo");

                    b.Property<DateTime>("Updated");

                    b.HasKey("Id");

                    b.HasIndex("GroupTypeId");

                    b.ToTable("Group");
                });

            modelBuilder.Entity("Link.Models.GroupModels.GroupMember", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("GroupId");

                    b.Property<bool>("IsAdmin");

                    b.Property<DateTime>("JoinDate");

                    b.Property<string>("MemberId");

                    b.HasKey("Id");

                    b.ToTable("GroupMember");
                });

            modelBuilder.Entity("Link.Models.GroupModels.GroupMemberRequest", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<string>("FromId");

                    b.Property<string>("GroupId");

                    b.Property<DateTime>("RequestDate");

                    b.Property<string>("ToEmail");

                    b.HasKey("Id");

                    b.ToTable("GroupMemberRequest");
                });

            modelBuilder.Entity("Link.Models.GroupModels.GroupType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("GroupType");
                });

            modelBuilder.Entity("Link.Models.GroupModels.GroupViewers", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("GroupId");

                    b.Property<string>("ViewersId");

                    b.HasKey("Id");

                    b.ToTable("GroupViewers");
                });

            modelBuilder.Entity("Link.Models.NotificationModels.Notification", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CommentId");

                    b.Property<string>("Content");

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("FromUserId");

                    b.Property<string>("GroupId");

                    b.Property<bool>("IsSeen");

                    b.Property<int>("NotificationTypeId");

                    b.Property<string>("Title");

                    b.Property<string>("ToUserId");

                    b.HasKey("Id");

                    b.HasIndex("NotificationTypeId");

                    b.ToTable("Notification");
                });

            modelBuilder.Entity("Link.Models.NotificationModels.NotificationType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("NotificationType");
                });

            modelBuilder.Entity("Link.Models.TransactionModels.CommentTransaction", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("CommentId");

                    b.Property<string>("CommentRoot");

                    b.Property<int>("CommentTransactionTypeId");

                    b.Property<string>("Data");

                    b.Property<string>("GroupId");

                    b.Property<DateTime>("TimeStamp");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("CommentTransactionTypeId");

                    b.ToTable("CommentTransaction");
                });

            modelBuilder.Entity("Link.Models.TransactionModels.CommentTransactionType", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("CommentTransactionType");
                });

            modelBuilder.Entity("Link.Models.UserLogsModels.UserAction", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("UserAction");
                });

            modelBuilder.Entity("Link.Models.UserLogsModels.UserLogs", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("ActionDate");

                    b.Property<byte>("UserActionId");

                    b.Property<int?>("UserActionId1");

                    b.Property<string>("UserAgent");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.HasIndex("UserActionId1");

                    b.ToTable("UserLogs");
                });

            modelBuilder.Entity("Link.Models.UserProfileModels.Followers", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("FollowDate");

                    b.Property<string>("FollowersId");

                    b.Property<bool>("IsSeen");

                    b.Property<DateTime>("SeenDate");

                    b.Property<string>("UserId");

                    b.HasKey("Id");

                    b.ToTable("Followers");
                });

            modelBuilder.Entity("Link.Models.UserProfileModels.Gender", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Lang");

                    b.Property<string>("Name");

                    b.HasKey("Id");

                    b.ToTable("Gender");
                });

            modelBuilder.Entity("Link.Models.UserProfileModels.Viewers", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("UserId");

                    b.Property<string>("ViewersId");

                    b.HasKey("Id");

                    b.ToTable("Viewers");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken();

                    b.Property<string>("Name")
                        .HasMaxLength(256);

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("RoleId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("ClaimType");

                    b.Property<string>("ClaimValue");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider");

                    b.Property<string>("ProviderKey");

                    b.Property<string>("ProviderDisplayName");

                    b.Property<string>("UserId")
                        .IsRequired();

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("RoleId");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId");

                    b.Property<string>("LoginProvider");

                    b.Property<string>("Name");

                    b.Property<string>("Value");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens");
                });

            modelBuilder.Entity("Link.Models.ApplicationUser", b =>
                {
                    b.HasOne("Link.Models.UserProfileModels.Gender", "Gender")
                        .WithMany()
                        .HasForeignKey("GenderId");
                });

            modelBuilder.Entity("Link.Models.CommentModels.Comment", b =>
                {
                    b.HasOne("Link.Models.CommentModels.CommentGroupType", "CommentGroupType")
                        .WithMany()
                        .HasForeignKey("CommentGroupTypeId");
                });

            modelBuilder.Entity("Link.Models.CommentModels.CommentUserVote", b =>
                {
                    b.HasOne("Link.Models.CommentModels.VoteType", "VoteType")
                        .WithMany()
                        .HasForeignKey("VoteTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Link.Models.EmailLogsModels.EmailLogsInfo", b =>
                {
                    b.HasOne("Link.Models.EmailLogsModels.EmailType", "EmailType")
                        .WithMany()
                        .HasForeignKey("EmailTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Link.Models.GroupModels.Group", b =>
                {
                    b.HasOne("Link.Models.GroupModels.GroupType", "GroupType")
                        .WithMany()
                        .HasForeignKey("GroupTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Link.Models.NotificationModels.Notification", b =>
                {
                    b.HasOne("Link.Models.NotificationModels.NotificationType", "NotificationType")
                        .WithMany()
                        .HasForeignKey("NotificationTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Link.Models.TransactionModels.CommentTransaction", b =>
                {
                    b.HasOne("Link.Models.TransactionModels.CommentTransactionType", "CommentTransactionType")
                        .WithMany()
                        .HasForeignKey("CommentTransactionTypeId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Link.Models.UserLogsModels.UserLogs", b =>
                {
                    b.HasOne("Link.Models.UserLogsModels.UserAction", "UserAction")
                        .WithMany()
                        .HasForeignKey("UserActionId1");
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("Link.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("Link.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Link.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("Link.Models.ApplicationUser")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
