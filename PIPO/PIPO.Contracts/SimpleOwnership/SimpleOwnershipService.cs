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
using PIPO.Contracts.SimpleOwnership.ContractDefinition;

namespace PIPO.Contracts.SimpleOwnership
{
    public partial class SimpleOwnershipService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.Web3 web3, SimpleOwnershipDeployment simpleOwnershipDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<SimpleOwnershipDeployment>().SendRequestAndWaitForReceiptAsync(simpleOwnershipDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.Web3 web3, SimpleOwnershipDeployment simpleOwnershipDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<SimpleOwnershipDeployment>().SendRequestAsync(simpleOwnershipDeployment);
        }

        public static async Task<SimpleOwnershipService> DeployContractAndGetServiceAsync(Nethereum.Web3.Web3 web3, SimpleOwnershipDeployment simpleOwnershipDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, simpleOwnershipDeployment, cancellationTokenSource);
            return new SimpleOwnershipService(web3, receipt.ContractAddress);
        }

        protected Nethereum.Web3.Web3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public SimpleOwnershipService(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public Task<BigInteger> NumberOfOwnershipsQueryAsync(NumberOfOwnershipsFunction numberOfOwnershipsFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<NumberOfOwnershipsFunction, BigInteger>(numberOfOwnershipsFunction, blockParameter);
        }

        
        public Task<BigInteger> NumberOfOwnershipsQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<NumberOfOwnershipsFunction, BigInteger>(null, blockParameter);
        }

        public Task<string> OwnerQueryAsync(OwnerFunction ownerFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerFunction, string>(ownerFunction, blockParameter);
        }

        
        public Task<string> OwnerQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerFunction, string>(null, blockParameter);
        }

        public Task<string> SimpleMintRequestAsync(SimpleMintFunction simpleMintFunction)
        {
             return ContractHandler.SendRequestAsync(simpleMintFunction);
        }

        public Task<TransactionReceipt> SimpleMintRequestAndWaitForReceiptAsync(SimpleMintFunction simpleMintFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(simpleMintFunction, cancellationToken);
        }

        public Task<string> SimpleMintRequestAsync(string receiver)
        {
            var simpleMintFunction = new SimpleMintFunction();
                simpleMintFunction.Receiver = receiver;
            
             return ContractHandler.SendRequestAsync(simpleMintFunction);
        }

        public Task<TransactionReceipt> SimpleMintRequestAndWaitForReceiptAsync(string receiver, CancellationTokenSource cancellationToken = null)
        {
            var simpleMintFunction = new SimpleMintFunction();
                simpleMintFunction.Receiver = receiver;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(simpleMintFunction, cancellationToken);
        }

        public Task<string> TransferOwnerRequestAsync(TransferOwnerFunction transferOwnerFunction)
        {
             return ContractHandler.SendRequestAsync(transferOwnerFunction);
        }

        public Task<TransactionReceipt> TransferOwnerRequestAndWaitForReceiptAsync(TransferOwnerFunction transferOwnerFunction, CancellationTokenSource cancellationToken = null)
        {
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferOwnerFunction, cancellationToken);
        }

        public Task<string> TransferOwnerRequestAsync(string to)
        {
            var transferOwnerFunction = new TransferOwnerFunction();
                transferOwnerFunction.To = to;
            
             return ContractHandler.SendRequestAsync(transferOwnerFunction);
        }

        public Task<TransactionReceipt> TransferOwnerRequestAndWaitForReceiptAsync(string to, CancellationTokenSource cancellationToken = null)
        {
            var transferOwnerFunction = new TransferOwnerFunction();
                transferOwnerFunction.To = to;
            
             return ContractHandler.SendRequestAndWaitForReceiptAsync(transferOwnerFunction, cancellationToken);
        }
    }
}
