
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Nethereum.JsonRpc.Client;
using Nethereum.Web3;

namespace CRPL.Tests.Mocks;

public static class MockWebUtils
{
    public static Dictionary<string, object> DefaultMappings = new()
    {
        { "eth_estimateGas", new Nethereum.Hex.HexTypes.HexBigInteger(BigInteger.One) },
        { "eth_call", "" },
        { "eth_sendTransaction", "" }
    };

    public static Dictionary<string, object> FromDefault(Dictionary<string, object> extra)
    {
        var mappings = DefaultMappings;
        return mappings.Concat(extra).ToDictionary(pair => pair.Key, pair => pair.Value);
    }
}

public class MockWeb3
{
    public readonly Web3 DummyWeb3;
    private readonly MockClient DummyClient;

    public MockWeb3(Dictionary<string, object>? returnMappings)
    {
        if (returnMappings != null) DummyClient = new MockClient(returnMappings);
        else DummyClient = new MockClient(MockWebUtils.DefaultMappings);
        DummyWeb3 = new Web3(DummyClient);
    }

    private class MockClient : IClient
    {
        private object DummyReturn;
        private Dictionary<string, object> ReturnMapping;

        public MockClient(Dictionary<string, object> returnMapping)
        {
            ReturnMapping = returnMapping;
        }

        public Task SendRequestAsync(RpcRequest request, string route = null)
        {
            throw new System.NotImplementedException();
        }

        public Task SendRequestAsync(string method, string route = null, params object[] paramList)
        {
            throw new System.NotImplementedException();
        }

        public RequestInterceptor OverridingRequestInterceptor { get; set; }

        public Task<T> SendRequestAsync<T>(RpcRequest request, string route = null)
        {
            if (ReturnMapping[request.Method].GetType().BaseType == typeof(Exception))
            {
                throw (Exception)ReturnMapping[request.Method];
            }
            return Task.FromResult((T)ReturnMapping[request.Method]);
        }

        public Task<T> SendRequestAsync<T>(string method, string route = null, params object[] paramList)
        {
            throw new System.NotImplementedException();
        }
    }
}