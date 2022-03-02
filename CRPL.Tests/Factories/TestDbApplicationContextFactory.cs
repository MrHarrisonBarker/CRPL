using System;
using System.Collections.Generic;
using System.Data.Common;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace CRPL.Tests.Factories;

public class TestDbApplicationContextFactory : IDisposable
{
    private DbConnection Connection;
    public ApplicationContext Context { get; }

    private DbContextOptions<ApplicationContext> CreateOptions()
    {
        return new DbContextOptionsBuilder<ApplicationContext>()
            .EnableSensitiveDataLogging()
            .UseSqlite(Connection).Options;
    }

    public TestDbApplicationContextFactory(IReadOnlyCollection<RegisteredWork>? registeredWorks = null, IReadOnlyCollection<Application>? applications = null, IReadOnlyCollection<UserAccount>? userAccounts = null)
    {
        Connection = new SqliteConnection("DataSource=:memory:;Foreign Keys=False");
        Connection.Open();

        Context = new ApplicationContext(CreateOptions());
        Context.Database.EnsureCreated();
        
        if (userAccounts != null) Context.UserAccounts.AddRange(userAccounts);
        if (registeredWorks != null) Context.RegisteredWorks.AddRange(registeredWorks);
        if (applications != null) Context.Applications.AddRange(applications);
        Context.SaveChanges();
    }

    public void Dispose()
    {
        Context.Database.EnsureDeleted();
        Connection.Close();
        Connection.Dispose();
        Connection = default;
    }
}