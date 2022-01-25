using System;
using System.Collections.Generic;
using CRPL.Data;
using CRPL.Data.Account;
using CRPL.Data.BlockchainUtils;
using Nethereum.Web3.Accounts;

namespace CRPL.Tests;

public class TestConstants
{
    public static string TestAccountId = "0x12890d2cce102216644c59dae5baed380d84830c";
    public static string PrivateKey = "0xb5b1870957d373ef0eeffecc6e4812c0fd08f554b37b233526acc331bf1544f7";

    public static BlockchainConnection PrivateTestConnection() => new BlockchainConnection(LawsOfNature.ChainUrl, new Account(PrivateKey, LawsOfNature.ChainId));

    public static Dictionary<UserAccount.AccountStatus, Guid> TestAccountIds = new()
    {
        { UserAccount.AccountStatus.Incomplete, new Guid("D67B16A9-2E44-4A14-9169-0AE8FED2203C") },
        { UserAccount.AccountStatus.Complete, new Guid("73C4FF17-1EF8-483C-BCDB-9A6191888F04") },
        { UserAccount.AccountStatus.Created, new Guid("8E9C6FB8-A8D7-459F-A39C-B06E68FE4E03") }
    };
}