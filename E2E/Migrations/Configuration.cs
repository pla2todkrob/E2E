namespace E2E.Migrations
{
    using E2E.Models;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<E2E.Models.ClsContext>
    {
        protected override void Seed(E2E.Models.ClsContext context)
        {
            ClsDefaultSystem.Generate();
        }

        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }
    }
}
