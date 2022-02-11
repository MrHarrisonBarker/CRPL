using System.Numerics;
using System.Text;
using AutoMapper;
using CRPL.Contracts.Copyright.ContractDefinition;
using CRPL.Contracts.Structs;
using CRPL.Data.Account;
using CRPL.Data.Applications;
using CRPL.Data.Applications.ViewModels;
using CRPL.Data.BlockchainUtils;
using CRPL.Data.ContractDeployment;
using CRPL.Web.Exceptions;
using CRPL.Web.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CRPL.Web.Services;

public class RegistrationService : IRegistrationService
{
    private readonly ILogger<UserService> Logger;
    private readonly ApplicationContext Context;
    private readonly IMapper Mapper;
    private readonly IBlockchainConnection BlockchainConnection;
    private readonly IContractRepository ContractRepository;

    public RegistrationService(ILogger<UserService> logger, ApplicationContext context, IMapper mapper, IBlockchainConnection blockchainConnection, IContractRepository contractRepository)
    {
        Logger = logger;
        Context = context;
        Mapper = mapper;
        BlockchainConnection = blockchainConnection;
        ContractRepository = contractRepository;
    }

    public RegisteredWork StartRegistration(CopyrightRegistrationApplication application)
    {
        Logger.LogInformation("Started a copyright registration {Id}", application.Id);

        var registeredWork = new RegisteredWork()
        {
            AssociatedApplication = new List<Application>()
            {
                application
            },
            UserWorks = application.AssociatedUsers.Select(x => new UserWork()
            {
                UserAccount = x.UserAccount
            }).ToList(),
            Hash = application.WorkHash
        };

        Context.RegisteredWorks.Add(registeredWork);
        Context.SaveChanges();

        return registeredWork;
    }

    public async Task<RegisteredWork> CompleteRegistration(Guid applicationId)
    {
        Logger.LogInformation("Completing registration for {Id}", applicationId);

        var application = await Context.CopyrightRegistrationApplications
            .Include(x => x.AssociatedWork).FirstOrDefaultAsync(x => x.Id == applicationId);
        if (application == null) throw new ApplicationNotFoundException(applicationId);
        if (application.AssociatedWork == null) throw new Exception("There is no work associated with this application!");

        var handler = BlockchainConnection.Web3().Eth.GetContractTransactionHandler<RegisterWithMetaFunction>();
        var register = new RegisterWithMetaFunction
        {
            To = application.OwnershipStakes.Decode().Select(x => Mapper.Map<OwnershipStakeContract>(x)).ToList(),
            Def = new Meta
            {
                Expires = new BigInteger((DateTime.Now.AddYears(application.YearsExpire) - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds),
                Title = application.Title,
                Registered = new BigInteger((DateTime.Now - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds),
                LegalMeta = application.Legal,
                WorkHash = Encoding.UTF8.GetString(application.WorkHash),
                WorkUri = application.WorkUri,
                Protections = application.Protections
            }
        };

        // var estimate = await handler.EstimateGasAsync(ContractRepository.DeployedContract(CopyrightContract.Copyright).Address, register);

        try
        {
            var transactionId = await new Contracts.Copyright.CopyrightService(BlockchainConnection.Web3(), ContractRepository.DeployedContract(CopyrightContract.Copyright).Address)
                .RegisterWithMetaRequestAsync(register);

            Context.Update(application);
            application.AssociatedWork.RegisteredTransactionId = transactionId;
            application.TransactionId = transactionId;
            
            Logger.LogInformation("sent register transaction at {Id}", transactionId);

            await Context.SaveChangesAsync();

            return application.AssociatedWork;
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when sending register transaction");
            throw;
        }
    }
}