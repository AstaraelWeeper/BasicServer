using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace BasicServer
{
    class JSONMessage
    {
        [JsonProperty("messageType")]
        public string MessageType { get; set; }
        [JsonProperty("messageBody")]
        public string MessageBody { get; set; }  
    }
}
