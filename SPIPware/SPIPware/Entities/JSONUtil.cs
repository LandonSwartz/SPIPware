using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SPIPware.Entities
{
    class JSONUtil
    {
      
        // utility to serialize json
        public static void SaveJSON(dynamic data, string filePath)
        {
            File.WriteAllText(filePath, JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented));
        }

        // utility to deserialize json
        public static T LoadJSON<T>(string filePath)
        {
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
        }
    }
}
