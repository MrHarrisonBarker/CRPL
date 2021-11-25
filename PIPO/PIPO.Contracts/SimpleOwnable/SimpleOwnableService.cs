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
using PIPO.Contracts.SimpleOwnable.ContractDefinition;

namespace PIPO.Contracts.SimpleOwnable
{
    public partial class SimpleOwnableService
    {
        public static Task<TransactionReceipt> DeployContractAndWaitForReceiptAsync(Nethereum.Web3.Web3 web3, SimpleOwnableDeployment simpleOwnableDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            return web3.Eth.GetContractDeploymentHandler<SimpleOwnableDeployment>().SendRequestAndWaitForReceiptAsync(simpleOwnableDeployment, cancellationTokenSource);
        }

        public static Task<string> DeployContractAsync(Nethereum.Web3.Web3 web3, SimpleOwnableDeployment simpleOwnableDeployment)
        {
            return web3.Eth.GetContractDeploymentHandler<SimpleOwnableDeployment>().SendRequestAsync(simpleOwnableDeployment);
        }

        public static async Task<SimpleOwnableService> DeployContractAndGetServiceAsync(Nethereum.Web3.Web3 web3, SimpleOwnableDeployment simpleOwnableDeployment, CancellationTokenSource cancellationTokenSource = null)
        {
            var receipt = await DeployContractAndWaitForReceiptAsync(web3, simpleOwnableDeployment, cancellationTokenSource);
            return new SimpleOwnableService(web3, receipt.ContractAddress);
        }

        protected Nethereum.Web3.Web3 Web3{ get; }

        public ContractHandler ContractHandler { get; }

        public SimpleOwnableService(Nethereum.Web3.Web3 web3, string contractAddress)
        {
            Web3 = web3;
            ContractHandler = web3.Eth.GetContractHandler(contractAddress);
        }

        public Task<string> OwnerQueryAsync(OwnerFunction ownerFunction, BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerFunction, string>(ownerFunction, blockParameter);
        }

        
        public Task<string> OwnerQueryAsync(BlockParameter blockParameter = null)
        {
            return ContractHandler.QueryAsync<OwnerFunction, string>(null, blockParameter);
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
