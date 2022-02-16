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
using CRPL.Web.Services.Background.VerificationPipeline;
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
    private readonly IVerificationQueue VerificationQueue;

    public RegistrationService(
        ILogger<UserService> logger,
        ApplicationContext context,
        IMapper mapper,
        IBlockchainConnection blockchainConnection,
        IContractRepository contractRepository,
        IVerificationQueue verificationQueue)
    {
        Logger = logger;
        Context = context;
        Mapper = mapper;
        BlockchainConnection = blockchainConnection;
        ContractRepository = contractRepository;
        VerificationQueue = verificationQueue;
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
            Hash = application.WorkHash,
            Title = application.Title,
            Status = RegisteredWorkStatus.ProcessingVerification
        };

        Context.RegisteredWorks.Add(registeredWork);
        Context.SaveChanges();
        
        VerificationQueue.QueueWork(registeredWork.Id);

        return registeredWork;
    }

    public async Task<RegisteredWork> CompleteRegistration(Guid applicationId)
    {
        Logger.LogInformation("Completing registration for {Id}", applicationId);

        var application = await Context.CopyrightRegistrationApplications
            .Include(x => x.AssociatedWork).FirstOrDefaultAsync(x => x.Id == applicationId);
        if (application == null) throw new ApplicationNotFoundException(applicationId);
        if (application.AssociatedWork == null) throw new Exception("There is no work associated with this application!");
        if (application.AssociatedWork.Status != RegisteredWorkStatus.Verified) throw new WorkNotVerifiedException();
        if (application.Status != ApplicationStatus.Submitted) throw new Exception("Application not in correct state!");

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
                WorkType = application.WorkType.ToString(),
                Protections = application.Protections
            }
        };
        
        try
        {
            var transactionId = await new Contracts.Copyright.CopyrightService(BlockchainConnection.Web3(), ContractRepository.DeployedContract(CopyrightContract.Copyright).Address)
                .RegisterWithMetaRequestAsync(register);

            Context.Update(application);
            application.AssociatedWork.RegisteredTransactionId = transactionId;
            application.AssociatedWork.Status = RegisteredWorkStatus.SentToChain;
            application.TransactionId = transactionId;
            
            Logger.LogInformation("sent register transaction at {Id}", transactionId);

            await Context.SaveChangesAsync();

            return application.AssociatedWork;
        }
        catch (Exception e)
        {
            application.Status = ApplicationStatus.Failed;
            application.AssociatedWork.Status = RegisteredWorkStatus.Rejected;
            await Context.SaveChangesAsync();
            
            Logger.LogError(e, "Exception thrown when sending register transaction");
            throw;
        }
    }
}