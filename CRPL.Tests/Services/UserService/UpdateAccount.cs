using System.Linq;
using System.Threading.Tasks;
using CRPL.Data.Account;
using CRPL.Data.Account.InputModels;
using CRPL.Tests.Factories;
using FluentAssertions;
using NUnit.Framework;

namespace CRPL.Tests.Services.UserService;

[TestFixture]
public class UpdateAccount
{
    [Test]
    public async Task Should_Update_UserAccount()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var userService = new UserServiceFactory().Create(context);

            var status = await userService.UpdateAccount(TestConstants.TestAccountIds[UserAccount.AccountStatus.Incomplete], new AccountInputModel
            {
                RegisteredJurisdiction = "USA",
                DateOfBirth = new UserAccount.DOB
                {
                    Day = 16,
                    Month = 1,
                    Year = 1974
                }
            });

            status.UserAccount.Status.Should().Be(UserAccount.AccountStatus.Incomplete);
            status.UserAccount.RegisteredJurisdiction.Should().BeEquivalentTo("USA");
            status.UserAccount.DateOfBirth.Day.Should().Be(16);
            status.UserAccount.DateOfBirth.Month.Should().Be(1);
            status.UserAccount.DateOfBirth.Year.Should().Be(1974);

            var userAccount = context.UserAccounts.First(x => x.Id == TestConstants.TestAccountIds[UserAccount.AccountStatus.Incomplete]);
            
            userAccount.RegisteredJurisdiction.Should().BeEquivalentTo("USA");
            userAccount.DateOfBirth.Day.Should().Be(16);
            userAccount.DateOfBirth.Month.Should().Be(1);
            userAccount.DateOfBirth.Year.Should().Be(1974);
        }
    }

    [Test]
    public async Task Should_Update_Status_When_Complete()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var userService = new UserServiceFactory().Create(context);

            var status = await userService.UpdateAccount(TestConstants.TestAccountIds[UserAccount.AccountStatus.Incomplete], new AccountInputModel
            {
                Email = "test@test.co.uk",
                PhoneNumber = "1",
                RegisteredJurisdiction = "USA",
                DateOfBirth = new UserAccount.DOB
                {
                    Day = 16,
                    Month = 1,
                    Year = 1974
                },

            });

            status.PartialFields.Count().Should().Be(0);
            status.UserAccount.Status.Should().Be(UserAccount.AccountStatus.Complete);

            var userAccount = context.UserAccounts.First(x => x.Id == TestConstants.TestAccountIds[UserAccount.AccountStatus.Incomplete]);

            userAccount.Status.Should().Be(UserAccount.AccountStatus.Complete);
        }
    }

    [Test]
    public async Task Should_Return_Partials()
    {
        await using (var context = new TestDbApplicationContextFactory().CreateContext())
        {
            var userService = new UserServiceFactory().Create(context);
            var status = await userService.UpdateAccount(TestConstants.TestAccountIds[UserAccount.AccountStatus.Incomplete], new AccountInputModel());

            status.PartialFields.Count().Should().BePositive();
            status.PartialFields.First(x => x.Field == "RegisteredJurisdiction").Type.Should().BeEquivalentTo("string");
        }
    }
}