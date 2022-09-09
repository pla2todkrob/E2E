namespace E2E.Migrations
{
    using E2E.Models;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<E2E.Models.clsContext>
    {
        protected override void Seed(E2E.Models.clsContext context)
        {
            clsDefaultSystem.Generate();
        }

        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }
    }
}