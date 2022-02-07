using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using Nethereum.Hex.HexTypes;
using Nethereum.ABI.FunctionEncoding.Attributes;
using Nethereum.Web3;
using Nethereum.RPC.Eth.DTOs;
using Nethereum.Contracts.CQS;
using Nethereum.Contracts.ContractHandlers;
using Nethereum.Contracts;
using System.Threading;
using CRPL.Contracts.Standard.ContractDefinition;
using CRPL.Contracts.Structs;

namespace CRPL.Contracts.Standard
{
    public partial class StandardService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.Web3 web3, StandardDeployment standardDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<StandardDeployment>().SendRequestAndWaitForReceiptAsync(standardDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.Web3 web3, StandardDeployment standardDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<StandardDeployment>().SendRequestAsync(standardDeployment);
        }

        public static async Task<StandardService> DeployContractAndGetServiceAsync(Nethereum.Web3.Web3 web3, StandardDeployment standardDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, standardDeployment, cancellationTokenSource);
            return new StandardService(web3, receipt.ContractAddress);
        }

        protected Nethereum.Web3.Web3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public StandardService(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public Task<string> ApproveManagerRequestAsync(ApproveManagerFunction approveManagerFunction)
        {
             return ContractHandler.SendRequestAsync(approveManagerFunction);
        }

        public Task<TransactionReceipt> ApproveManagerRequestAndWaitForReceiptAsync(ApproveManagerFunction approveManagerFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(approveManagerFunction, cancellationToken);
        }

        public Task<string> ApproveManagerRequestAsync(string manager, bool hasApproval)
        {
            var approveManagerFunction = new ApproveManagerFunction();
                approveManagerFunction.Manager = manager;
                approveManagerFunction.HasApproval = hasApproval;
            
             return ContractHandler.SendRequestAsync(approveManagerFunction);
        }

        public Task<TransactionReceipt> ApproveManagerRequestAndWaitForReceiptAsync(string manager, bool hasApproval, CancellationTokenSource cancellationToken = null)
        {
            var approveManagerFunction = new ApproveManagerFunction();
                approveManagerFunction.Manager = manager;
                approveManagerFunction.HasApproval = hasApproval;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(approveManagerFunction, cancellationToken);
        }

        public Task<string> ApproveOneRequestAsync(ApproveOneFunction approveOneFunction)
        {
             return ContractHandler.SendRequestAsync(approveOneFunction);
        }

        public Task<TransactionReceipt> ApproveOneRequestAndWaitForReceiptAsync(ApproveOneFunction approveOneFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(approveOneFunction, cancellationToken);
        }

        public Task<string> ApproveOneRequestAsync(BigInteger rightId, string approved)
        {
            var approveOneFunction = new ApproveOneFunction();
                approveOneFunction.RightId = rightId;
                approveOneFunction.Approved = approved;
            
             return ContractHandler.SendRequestAsync(approveOneFunction);
        }

        public Task<TransactionReceipt> ApproveOneRequestAndWaitForReceiptAsync(BigInteger rightId, string approved, CancellationTokenSource cancellationToken = null)
        {
            var approveOneFunction = new ApproveOneFunction();
                approveOneFunction.RightId = rightId;
                approveOneFunction.Approved = approved;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(approveOneFunction, cancellationToken);
        }

        public Task<string> BindRestructureRequestAsync(BindRestructureFunction bindRestructureFunction)
        {
             return ContractHandler.SendRequestAsync(bindRestructureFunction);
        }

        public Task<TransactionReceipt> BindRestructureRequestAndWaitForReceiptAsync(BindRestructureFunction bindRestructureFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(bindRestructureFunction, cancellationToken);
        }

        public Task<string> BindRestructureRequestAsync(BigInteger rightId, bool accepted)
        {
            var bindRestructureFunction = new BindRestructureFunction();
                bindRestructureFunction.RightId = rightId;
                bindRestructureFunction.Accepted = accepted;
            
             return ContractHandler.SendRequestAsync(bindRestructureFunction);
        }

        public Task<TransactionReceipt> BindRestructureRequestAndWaitForReceiptAsync(BigInteger rightId, bool accepted, CancellationTokenSource cancellationToken = null)
        {
            var bindRestructureFunction = new BindRestructureFunction();
                bindRestructureFunction.RightId = rightId;
                bindRestructureFunction.Accepted = accepted;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(bindRestructureFunction, cancellationToken);
        }

        public Task<BigInteger> ExpiresOnQueryAsync(ExpiresOnFunction expiresOnFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<ExpiresOnFunction, BigInteger>(expiresOnFunction, blockParameter);
        }

        
        public Task<BigInteger> ExpiresOnQueryAsync(BigInteger rightId, BlockParameter blockParameter = null)
        {
            var expiresOnFunction = new ExpiresOnFunction();
                expiresOnFunction.RightId = rightId;
            
            return ContractHandler.QueryAsync<ExpiresOnFunction, BigInteger>(expiresOnFunction, blockParameter);
        }

        public Task<string> GetApprovedQueryAsync(GetApprovedFunction getApprovedFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<GetApprovedFunction, string>(getApprovedFunction, blockParameter);
        }

        
        public Task<string> GetApprovedQueryAsync(BigInteger rightId, BlockParameter blockParameter = null)
        {
            var getApprovedFunction = new GetApprovedFunction();
                getApprovedFunction.RightId = rightId;
            
            return ContractHandler.QueryAsync<GetApprovedFunction, string>(getApprovedFunction, blockParameter);
        }

        public Task<bool> IsManagerQueryAsync(IsManagerFunction isManagerFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<IsManagerFunction, bool>(isManagerFunction, blockParameter);
        }

        
        public Task<bool> IsManagerQueryAsync(string client, string manager, BlockParameter blockParameter = null)
        {
            var isManagerFunction = new IsManagerFunction();
                isManagerFunction.Client = client;
                isManagerFunction.Manager = manager;
            
            return ContractHandler.QueryAsync<IsManagerFunction, bool>(isManagerFunction, blockParameter);
        }

        public Task<string> LegalDefinitionQueryAsync(LegalDefinitionFunction legalDefinitionFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<LegalDefinitionFunction, string>(legalDefinitionFunction, blockParameter);
        }

        
        public Task<string> LegalDefinitionQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<LegalDefinitionFunction, string>(null, blockParameter);
        }

        public Task<string> LegalMetaQueryAsync(LegalMetaFunction legalMetaFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<LegalMetaFunction, string>(legalMetaFunction, blockParameter);
        }

        
        public Task<string> LegalMetaQueryAsync(BigInteger rightId, BlockParameter blockParameter = null)
        {
            var legalMetaFunction = new LegalMetaFunction();
                legalMetaFunction.RightId = rightId;
            
            return ContractHandler.QueryAsync<LegalMetaFunction, string>(legalMetaFunction, blockParameter);
        }

        public Task<OwnershipOfOutputDTO> OwnershipOfQueryAsync(OwnershipOfFunction ownershipOfFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<OwnershipOfFunction, OwnershipOfOutputDTO>(ownershipOfFunction, blockParameter);
        }

        public Task<OwnershipOfOutputDTO> OwnershipOfQueryAsync(BigInteger rightId, BlockParameter blockParameter = null)
        {
            var ownershipOfFunction = new OwnershipOfFunction();
                ownershipOfFunction.RightId = rightId;
            
            return ContractHandler.QueryDeserializingToObjectAsync<OwnershipOfFunction, OwnershipOfOutputDTO>(ownershipOfFunction, blockParameter);
        }

        public Task<BigInteger> PortfolioSizeQueryAsync(PortfolioSizeFunction portfolioSizeFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<PortfolioSizeFunction, BigInteger>(portfolioSizeFunction, blockParameter);
        }

        
        public Task<BigInteger> PortfolioSizeQueryAsync(string owner, BlockParameter blockParameter = null)
        {
            var portfolioSizeFunction = new PortfolioSizeFunction();
                portfolioSizeFunction.Owner = owner;
            
            return ContractHandler.QueryAsync<PortfolioSizeFunction, BigInteger>(portfolioSizeFunction, blockParameter);
        }

        public Task<ProposalOutputDTO> ProposalQueryAsync(ProposalFunction proposalFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryDeserializingToObjectAsync<ProposalFunction, ProposalOutputDTO>(proposalFunction, blockParameter);
        }

        public Task<ProposalOutputDTO> ProposalQueryAsync(BigInteger rightId, BlockParameter blockParameter = null)
        {
            var proposalFunction = new ProposalFunction();
                proposalFunction.RightId = rightId;
            
            return ContractHandler.QueryDeserializingToObjectAsync<ProposalFunction, ProposalOutputDTO>(proposalFunction, blockParameter);
        }

        public Task<string> ProposeRestructureRequestAsync(ProposeRestructureFunction proposeRestructureFunction)
        {
             return ContractHandler.SendRequestAsync(proposeRestructureFunction);
        }

        public Task<TransactionReceipt> ProposeRestructureRequestAndWaitForReceiptAsync(ProposeRestructureFunction proposeRestructureFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(proposeRestructureFunction, cancellationToken);
        }

        public Task<string> ProposeRestructureRequestAsync(BigInteger rightId, List<OwnershipStake> restructured)
        {
            var proposeRestructureFunction = new ProposeRestructureFunction();
                proposeRestructureFunction.RightId = rightId;
                proposeRestructureFunction.Restructured = restructured;
            
             return ContractHandler.SendRequestAsync(proposeRestructureFunction);
        }

        public Task<TransactionReceipt> ProposeRestructureRequestAndWaitForReceiptAsync(BigInteger rightId, List<OwnershipStake> restructured, CancellationTokenSource cancellationToken = null)
        {
            var proposeRestructureFunction = new ProposeRestructureFunction();
                proposeRestructureFunction.RightId = rightId;
                proposeRestructureFunction.Restructured = restructured;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(proposeRestructureFunction, cancellationToken);
        }

        public Task<string> RegisterRequestAsync(RegisterFunction registerFunction)
        {
             return ContractHandler.SendRequestAsync(registerFunction);
        }

        public Task<TransactionReceipt> RegisterRequestAndWaitForReceiptAsync(RegisterFunction registerFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(registerFunction, cancellationToken);
        }

        public Task<string> RegisterRequestAsync(List<OwnershipStake> to)
        {
            var registerFunction = new RegisterFunction();
                registerFunction.To = to;
            
             return ContractHandler.SendRequestAsync(registerFunction);
        }

        public Task<TransactionReceipt> RegisterRequestAndWaitForReceiptAsync(List<OwnershipStake> to, CancellationTokenSource cancellationToken = null)
        {
            var registerFunction = new RegisterFunction();
                registerFunction.To = to;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(registerFunction, cancellationToken);
        }

        public Task<BigInteger> RegisterTimeQueryAsync(RegisterTimeFunction registerTimeFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<RegisterTimeFunction, BigInteger>(registerTimeFunction, blockParameter);
        }

        
        public Task<BigInteger> RegisterTimeQueryAsync(BigInteger rightId, BlockParameter blockParameter = null)
        {
            var registerTimeFunction = new RegisterTimeFunction();
                registerTimeFunction.RightId = rightId;
            
            return ContractHandler.QueryAsync<RegisterTimeFunction, BigInteger>(registerTimeFunction, blockParameter);
        }

        public Task<string> RegisterWithMetaRequestAsync(RegisterWithMetaFunction registerWithMetaFunction)
        {
             return ContractHandler.SendRequestAsync(registerWithMetaFunction);
        }

        public Task<TransactionReceipt> RegisterWithMetaRequestAndWaitForReceiptAsync(RegisterWithMetaFunction registerWithMetaFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(registerWithMetaFunction, cancellationToken);
        }

        public Task<string> RegisterWithMetaRequestAsync(List<OwnershipStake> to, Meta def)
        {
            var registerWithMetaFunction = new RegisterWithMetaFunction();
                registerWithMetaFunction.To = to;
                registerWithMetaFunction.Def = def;
            
             return ContractHandler.SendRequestAsync(registerWithMetaFunction);
        }

        public Task<TransactionReceipt> RegisterWithMetaRequestAndWaitForReceiptAsync(List<OwnershipStake> to, Meta def, CancellationTokenSource cancellationToken = null)
        {
            var registerWithMetaFunction = new RegisterWithMetaFunction();
                registerWithMetaFunction.To = to;
                registerWithMetaFunction.Def = def;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(registerWithMetaFunction, cancellationToken);
        }

        public Task<string> TitleQueryAsync(TitleFunction titleFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<TitleFunction, string>(titleFunction, blockParameter);
        }

        
        public Task<string> TitleQueryAsync(BigInteger rightId, BlockParameter blockParameter = null)
        {
            var titleFunction = new TitleFunction();
                titleFunction.RightId = rightId;
            
            return ContractHandler.QueryAsync<TitleFunction, string>(titleFunction, blockParameter);
        }

        public Task<string> WorkHashQueryAsync(WorkHashFunction workHashFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<WorkHashFunction, string>(workHashFunction, blockParameter);
        }

        
        public Task<string> WorkHashQueryAsync(BigInteger rightId, BlockParameter blockParameter = null)
        {
            var workHashFunction = new WorkHashFunction();
                workHashFunction.RightId = rightId;
            
            return ContractHandler.QueryAsync<WorkHashFunction, string>(workHashFunction, blockParameter);
        }

        public Task<string> WorkURIQueryAsync(WorkURIFunction workURIFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<WorkURIFunction, string>(workURIFunction, blockParameter);
        }

        
        public Task<string> WorkURIQueryAsync(BigInteger rightId, BlockParameter blockParameter = null)
        {
            var workURIFunction = new WorkURIFunction();
                workURIFunction.RightId = rightId;
            
            return ContractHandler.QueryAsync<WorkURIFunction, string>(workURIFunction, blockParameter);
        }
    }
}
