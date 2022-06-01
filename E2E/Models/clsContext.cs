using E2E.Models.Tables;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace E2E.Models
{
    public class clsContext : DbContext
    {
        public clsContext() : base("strContext")
        {
        }

        public DbSet<EForm_Files> EForm_Files { get; set; }
        public DbSet<EForm_Galleries> EForm_Galleries { get; set; }
        public DbSet<EForms> EForms { get; set; }

        public DbSet<Master_Departments> Master_Departments { get; set; }
        public DbSet<Master_Divisions> Master_Divisions { get; set; }
        public DbSet<Master_Grades> Master_Grades { get; set; }
        public DbSet<Master_LineWorks> Master_LineWorks { get; set; }
        public DbSet<Master_Plants> Master_Plants { get; set; }
        public DbSet<Master_Processes> Master_Processes { get; set; }
        public DbSet<Master_Sections> Master_Sections { get; set; }
        public DbSet<ServiceChangeDueDate> ServiceChangeDueDates { get; set; }
        public DbSet<ServiceCommentFiles> ServiceCommentFiles { get; set; }
        public DbSet<ServiceComments> ServiceComments { get; set; }
        public DbSet<ServiceFiles> ServiceFiles { get; set; }
        public DbSet<Services> Services { get; set; }
        public DbSet<System_Authorize> System_Authorizes { get; set; }
        public DbSet<System_Configurations> System_Configurations { get; set; }
        public DbSet<System_DueDateStatuses> System_DueDateStatuses { get; set; }
        public DbSet<System_Prefix_EN> System_Prefix_ENs { get; set; }
        public DbSet<System_Prefix_TH> System_Prefix_THs { get; set; }
        public DbSet<System_Priorities> System_Priorities { get; set; }
        public DbSet<System_Roles> System_Roles { get; set; }
        public DbSet<System_Statuses> System_Statuses { get; set; }

        public DbSet<TopicComments> TopicComments { get; set; }
        public DbSet<TopicFiles> TopicFiles { get; set; }
        public DbSet<TopicGalleries> TopicGalleries { get; set; }
        public DbSet<Topics> Topics { get; set; }
        public DbSet<UserDetails> UserDetails { get; set; }
        public DbSet<Users> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}