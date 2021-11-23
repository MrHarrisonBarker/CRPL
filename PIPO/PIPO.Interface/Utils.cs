using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace PIPO.Interface;

public static class Utils
{
    public static CompiledContract LoadContract(string name)
    {
        var json = JObject.Load(new JsonTextReader(new StreamReader(File.OpenRead($"./{name}.json"))));
        return new CompiledContract()
        {
            Abi = json["abi"].ToString(),
            ByteCode = json["bytecode"].ToString()
        };
    }
}