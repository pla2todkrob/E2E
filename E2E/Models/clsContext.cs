using E2E.Models.Tables;
using System.Configuration;
using System.Data.Entity;

namespace E2E.Models
{
    public class clsContext : DbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public clsContext() : base(ConfigurationManager.AppSettings["NameConn"])
        {
        }

        public DbSet<EForm_Files> EForm_Files { get; set; }
        public DbSet<EForm_Galleries> EForm_Galleries { get; set; }
        public DbSet<EForms> EForms { get; set; }
        public DbSet<Log_Login> Log_Logins { get; set; }
        public DbSet<Log_SendEmail> Log_SendEmails { get; set; }
        public DbSet<Log_SendEmailTo> Log_SendEmailTos { get; set; }
        public DbSet<Manuals> Manuals { get; set; }
        public DbSet<Master_Categories> Master_Categories { get; set; }
        public DbSet<Master_Departments> Master_Departments { get; set; }
        public DbSet<Master_Divisions> Master_Divisions { get; set; }
        public DbSet<Master_Documents> Master_Documents { get; set; }
        public DbSet<Master_DocumentVersions> Master_DocumentVersions { get; set; }
        public DbSet<Master_Grades> Master_Grades { get; set; }
        public DbSet<Master_InquiryTopics> Master_InquiryTopics { get; set; }
        public DbSet<Master_LineWorks> Master_LineWorks { get; set; }
        public DbSet<Master_Plants> Master_Plants { get; set; }
        public DbSet<Master_Processes> Master_Processes { get; set; }
        public DbSet<Master_Sections> Master_Sections { get; set; }
        public DbSet<SatisfactionDetails> SatisfactionDetails { get; set; }
        public DbSet<Satisfactions> Satisfactions { get; set; }
        public DbSet<ServiceChangeDueDate> ServiceChangeDueDates { get; set; }
        public DbSet<ServiceCommentFiles> ServiceCommentFiles { get; set; }
        public DbSet<ServiceComments> ServiceComments { get; set; }
        public DbSet<ServiceDocuments> ServiceDocuments { get; set; }
        public DbSet<ServiceFiles> ServiceFiles { get; set; }
        public DbSet<Services> Services { get; set; }
        public DbSet<ServiceTeams> ServiceTeams { get; set; }
        public DbSet<System_Authorize> System_Authorizes { get; set; }
        public DbSet<System_Configurations> System_Configurations { get; set; }
        public DbSet<System_DueDateStatuses> System_DueDateStatuses { get; set; }
        public DbSet<System_Language> System_Language { get; set; }
        public DbSet<System_ManualType> System_ManualType { get; set; }
        public DbSet<System_Prefix_EN> System_Prefix_ENs { get; set; }
        public DbSet<System_Prefix_TH> System_Prefix_THs { get; set; }
        public DbSet<System_Priorities> System_Priorities { get; set; }
        public DbSet<System_Roles> System_Roles { get; set; }
        public DbSet<System_Statuses> System_Statuses { get; set; }

        public DbSet<TopicComments> TopicComments { get; set; }
        public DbSet<TopicFiles> TopicFiles { get; set; }
        public DbSet<TopicGalleries> TopicGalleries { get; set; }
        public DbSet<Topics> Topics { get; set; }
        public DbSet<TopicSections> TopicSections { get; set; }
        public DbSet<UserDetails> UserDetails { get; set; }
        public DbSet<Users> Users { get; set; }
        public DbSet<UserUploadHistories> UserUploadHistories { get; set; }
        public DbSet<WorkRootDocuments> WorkRootDocuments { get; set; }
        public DbSet<WorkRoots> WorkRoots { get; set; }
    }
}
