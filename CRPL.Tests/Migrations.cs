using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using CRPL.Data.ContractDeployment;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using NUnit.Framework;

namespace CRPL.Tests;

[TestFixture]
public class Migrations
{
    private ContractContext Context;

    private static DbConnection CreateInMemoryDatabase()
    {
        var connection = new SqliteConnection("Filename=:memory:");

        connection.Open();

        return connection;
    }
    
    [SetUp]
    public async Task Setup()
    {
        var options = new DbContextOptionsBuilder<ContractContext>()
            .UseSqlite(CreateInMemoryDatabase())
            .Options;

        Context = new ContractContext(options);
    }

    [Test]
    public async Task Should_Migrate_And_Seed()
    {
        await Context.Database.EnsureDeletedAsync();
        var migrator = Context.Database.GetService<IMigrator>();
        var migrations = Context.Database.GetMigrations();
        var migrationCount = (await Context.Database.GetPendingMigrationsAsync()).Count();
        var migrationCounter = migrationCount;
        foreach (var migration in migrations)
        {
            await migrator.MigrateAsync(migration);
            (await Context.Database.GetPendingMigrationsAsync()).Count().Should().Be(-- migrationCounter);
        }
        (await Context.Database.GetAppliedMigrationsAsync()).Count().Should().Be(migrationCount);
    }
}