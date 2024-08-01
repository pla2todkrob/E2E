using E2E.Models.Tables;
using System;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace E2E.Models
{
    public class ClsContext : DbContext
    {
        public ClsContext() : base(ConfigurationManager.AppSettings["NameConn"])
        {
        }

        public DbSet<BusinessCardFiles> BusinessCardFiles { get; set; }

        public DbSet<BusinessCards> BusinessCards { get; set; }

        public DbSet<ChatBotAnswer> ChatBotAnswers { get; set; }

        public DbSet<ChatBotQuestion> ChatBotQuestions { get; set; }

        public DbSet<ChatBot> ChatBots { get; set; }

        public DbSet<ChatBotUploadHistory> ChatBotUploadHistories { get; set; }

        public DbSet<ChatGPTHistory> ChatGPTHistories { get; set; }

        public DbSet<ChatGPT> ChatGPTs { get; set; }

        public DbSet<EForm_Files> EForm_Files { get; set; }

        public DbSet<EForm_Galleries> EForm_Galleries { get; set; }

        public DbSet<EForms> EForms { get; set; }

        public DbSet<Log_BusinessCards> Log_BusinessCards { get; set; }

        public DbSet<Log_DbChange> Log_DbChanges { get; set; }

        public DbSet<Log_DbDelete> Log_DbDeletes { get; set; }

        public DbSet<Log_Exception> Log_Exceptions { get; set; }

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

        public DbSet<PlantDetail> PlantDetails { get; set; }

        public DbSet<SatisfactionDetails> SatisfactionDetails { get; set; }

        public DbSet<SatisfactionDetails_BusinessCards> SatisfactionDetails_BusinessCards { get; set; }

        public DbSet<Satisfactions> Satisfactions { get; set; }

        public DbSet<Satisfactions_BusinessCards> Satisfactions_BusinessCards { get; set; }

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

        public DbSet<System_Program> System_Programs { get; set; }

        public DbSet<System_Roles> System_Roles { get; set; }

        public DbSet<System_Statuses> System_Statuses { get; set; }

        public DbSet<TopicComments> TopicComments { get; set; }

        public DbSet<TopicFiles> TopicFiles { get; set; }

        public DbSet<TopicGalleries> TopicGalleries { get; set; }

        public DbSet<Topics> Topics { get; set; }

        public DbSet<TopicSections> TopicSections { get; set; }

        public DbSet<TopicView> TopicView { get; set; }

        public DbSet<UserDetails> UserDetails { get; set; }

        public DbSet<Users> Users { get; set; }

        public DbSet<UserUploadHistory> UserUploadHistories { get; set; }

        public DbSet<WorkRootDocuments> WorkRootDocuments { get; set; }

        public DbSet<WorkRoots> WorkRoots { get; set; }

        protected Guid GetUserId(DbPropertyValues propertyValues)
        {
            try
            {
                Guid res = Guid.Empty;
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    res = Guid.Parse(HttpContext.Current.User.Identity.Name);
                }
                else
                {
                    string propUserId = propertyValues.PropertyNames.Where(w => w.Contains("User_Id")).FirstOrDefault();
                    if (!string.IsNullOrEmpty(propUserId))
                    {
                        res = Guid.Parse(propertyValues[propUserId].ToString());
                    }
                }
                return res;
            }
            catch (Exception)
            {
                throw;
            }
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserDetails>()
                .HasRequired(ud => ud.Users)
                .WithOptional(u => u.UserDetails)
                .WillCascadeOnDelete(true);
        }

        public override int SaveChanges()
        {
            try
            {
                var modifiedEntries = ChangeTracker.Entries().Where(e => e.State == EntityState.Modified);
                foreach (var entry in modifiedEntries)
                {
                    var tableName = entry.Entity.GetType().Name;
                    var originalValues = entry.OriginalValues;
                    var currentValues = entry.CurrentValues;
                    Guid userId = GetUserId(originalValues);

                    foreach (var propName in originalValues.PropertyNames)
                    {
                        if (propName.ToLower().Contains("update"))
                        {
                            var propType = entry.Entity.GetType().GetProperty(propName).PropertyType;
                            if (propType == typeof(DateTime))
                            {
                                continue;
                            }
                        }

                        var originalValue = originalValues[propName];
                        var currentValue = currentValues[propName];
                        if (!Equals(originalValue, currentValue))
                        {
                            var log = new Log_DbChange()
                            {
                                TableName = tableName,
                                ColumnName = propName,
                                OriginalValue = originalValue?.ToString(),
                                CurrentValue = currentValue?.ToString(),
                                User_Id = userId,
                                IP_Address = HttpContext.Current.Request.UserHostAddress,
                            };
                            Log_DbChanges.Add(log);
                        }
                    }
                }

                //Handle deletions
                var deletedEntries = ChangeTracker
                .Entries()
                .Where(e => e.State == EntityState.Deleted);

                foreach (var entry in deletedEntries)
                {
                    Guid userId = GetUserId(entry.OriginalValues);
                    var entityType = entry.Entity.GetType();
                    var tableName = entityType.Name;
                    var primaryKey = entityType.GetProperties().Where(p => p.GetCustomAttributes(typeof(KeyAttribute), false).Count() > 0).First();
                    var keyValues = entry.OriginalValues[primaryKey.Name].ToString();
                    var log = new Log_DbDelete()
                    {
                        TableName = tableName,
                        KeyValues = keyValues,
                        User_Id = userId,
                        IP_Address = HttpContext.Current.Request.UserHostAddress,
                    };
                    Log_DbDeletes.Add(log);
                }
                return base.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
