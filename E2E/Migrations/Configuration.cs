namespace E2E.Migrations
{
    using E2E.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<E2E.Models.clsContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = true;
        }

        protected override void Seed(E2E.Models.clsContext context)
        {
            clsDefaultSystem.Generate();
        }
    }
}