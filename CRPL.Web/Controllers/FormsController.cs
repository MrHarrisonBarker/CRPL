using System.Net;
using CRPL.Data.Applications;
using CRPL.Data.Applications.DataModels;
using CRPL.Data.Applications.InputModels;
using CRPL.Data.Applications.ViewModels;
using CRPL.Web.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CRPL.Web.Controllers;

[ApiController]
[Route("[controller]")]
public class FormsController : ControllerBase
{
    private readonly ILogger<FormsController> Logger;
    private readonly IFormsService FormsService;
    private readonly IDisputeService DisputeService;

    public FormsController(ILogger<FormsController> logger, IFormsService formsService, IDisputeService disputeService)
    {
        Logger = logger;
        FormsService = formsService;
        DisputeService = disputeService;
    }

    [HttpGet("{id}")]
    public async Task<ApplicationViewModel> Get([FromRoute] Guid id)
    {
        try
        {
            return await FormsService.GetApplication(id);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when getting application");
            throw;
        }
    }

    [HttpGet("users/{id}")]
    public async Task<List<ApplicationViewModel>> GetMy(Guid id)
    {
        try
        {
            return await FormsService.GetMyApplications(id);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when getting application");
            throw;
        }
    }

    [HttpPost("copyright/registration")]
    public async Task<CopyrightRegistrationViewModel> UpdateCopyrightRegistration(CopyrightRegistrationInputModel inputModel)
    {
        try
        {
            return await FormsService.Update<CopyrightRegistrationViewModel>(inputModel);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when updating copyright registration application");
            throw;
        }
    }

    [HttpPost("copyright/ownership")]
    public async Task<OwnershipRestructureViewModel> UpdateOwnershipStructure(OwnershipRestructureInputModel inputModel)
    {
        try
        {
            return await FormsService.Update<OwnershipRestructureViewModel>(inputModel);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when updating ownership application");
            throw;
        }
    }

    [HttpPost("copyright/dispute")]
    public async Task<DisputeViewModel> UpdateDispute(DisputeInputModel inputModel)
    {
        try
        {
            return await FormsService.Update<DisputeViewModel>(inputModel);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when updating copyright registration application");
            throw;
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Cancel(Guid id)
    {
        try
        {
            await FormsService.CancelApplication(id);
            return StatusCode(200);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when canceling application");
            throw;
        }
    }

    [HttpPost("copyright/submit/registration/{id}")]
    public async Task<CopyrightRegistrationViewModel> SubmitCopyrightRegistration(Guid id)
    {
        try
        {
            return await FormsService.Submit<CopyrightRegistrationApplication, CopyrightRegistrationViewModel>(id);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when submitting copyright registration application");
            throw;
        }
    }

    [HttpPost("copyright/submit/ownership/{id}")]
    public async Task<OwnershipRestructureViewModel> SubmitOwnershipRestructure(Guid id)
    {
        try
        {
            return await FormsService.Submit<OwnershipRestructureApplication, OwnershipRestructureViewModel>(id);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when submitting ownership restructure application");
            throw;
        }
    }

    [HttpPost("copyright/submit/dispute/{id}")]
    public async Task<DisputeViewModel> SubmitDispute(Guid id)
    {
        try
        {
            return await FormsService.Submit<DisputeApplication, DisputeViewModel>(id);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when submitting dispute application");
            throw;
        }
    }

    [HttpPost("copyright/resolve/dispute")]
    public async Task<DisputeViewModel> ResolveDispute([FromBody] ResolveDisputeInputModel inputModel)
    {
        try
        {
            return inputModel.Accept ? await DisputeService.AcceptRecourse(inputModel.DisputeId, inputModel.Message) : await DisputeService.RejectRecourse(inputModel.DisputeId, inputModel.Message);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when resolving dispute application");
            throw;
        }
    }

    [HttpPost("copyright/record/payment/dispute/{id}")]
    public async Task RecordPayment(Guid id, string transaction)
    {
        try
        {
            await DisputeService.RecordPaymentAndResolve(id, transaction);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when recording payment for dispute resolve");
            throw;
        }
    }

    [HttpDelete("user/{id}")]
    public async Task<DeleteAccountViewModel> DeleteUser(Guid id)
    {
        try
        {
            var application = await FormsService.Update<DeleteAccountViewModel>(new DeleteAccountInputModel()
            {
                AccountId = id
            });

            return await FormsService.Submit<DeleteAccountApplication, DeleteAccountViewModel>(application.Id);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when deleting user {Id}", id);
            throw;
        }
    }

    [HttpPatch("wallet/{id}/to/{address}")]
    public async Task<WalletTransferViewModel> WalletTransfer(Guid id, string address)
    {
        try
        {
            var application = await FormsService.Update<WalletTransferViewModel>(new WalletTransferInputModel()
            {
                UserId = id,
                WalletAddress = address
            });
            
            return await FormsService.Submit<WalletTransferApplication, WalletTransferViewModel>(application.Id);
        }
        catch (Exception e)
        {
            Logger.LogError(e, "Exception thrown when transferring {Id}'s wallet to {Address}", id, address);
            throw;
        }
    }
}