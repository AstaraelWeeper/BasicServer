using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Reflection;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;




namespace BasicServer
{
    class ParseJson
    {
        //PowerpointHandler powerpointHandler = new PowerpointHandler();
        List<string> history = new List<string>();
        string storedHistoryFilePath = @"C:\Users\Public\MissionManagerHistory.txt";

        private Server.VideoFormActionDelegate _videoFormActionDelegate;


        public ParseJson(Server.VideoFormActionDelegate videoFormActionDelegate)
        {
            _videoFormActionDelegate = videoFormActionDelegate;

            if (!File.Exists(storedHistoryFilePath))
            {
                File.Create(storedHistoryFilePath).Dispose();
            }
        }


        public string InitialParsing(string JsonIn) //return JSON
        {
            string bluetoothConn = "CONNECTION_ACTIVE_BLUETOOTH";
            string wifiConn = "CONNECTION_ACTIVE_WIFI";
            JsonIn = JsonIn.TrimEnd('\0');
            if (JsonIn.Contains(bluetoothConn))
            {
                if (JsonIn.Length > bluetoothConn.Length)
                {
                    JsonIn.Replace(bluetoothConn, "");
                    string jsonReturn = InitialParsing(JsonIn);
                    return jsonReturn;
                }

                else return JsonIn; //send back the incoming message
            }
            else if (JsonIn.Contains(wifiConn))
            {
                if (JsonIn.Length > wifiConn.Length)
                {
                    JsonIn.Replace(wifiConn, "");
                    string jsonReturn = InitialParsing(JsonIn);
                    return jsonReturn;
                }
                else return JsonIn; //send back the incoming message
            }
            else
            {
                List<string> initialList = new List<string>();
                JsonIn = JsonIn.TrimEnd('\0');
                if (string.IsNullOrEmpty(JsonIn))
                {
                    return "";
                }
                else
                {

                    var JSONMessage = JsonConvert.DeserializeObject<JSONMessage>(JsonIn);
                    PropertyInfo[] JsonProperties = JSONMessage.GetType().GetProperties();//get the properties of the class

                    foreach (var prop in JsonProperties)
                    {
                        string value = prop.GetValue(JSONMessage, null) as string; //get property value
                        initialList.Add(value);
                    }

                    string JsonReturn = ParseJsonMethod(initialList);
                    return JsonReturn;
                }
            }
        }



        public string ParseJsonMethod(List<string> JsonMessage) //will need to return JSON
        {
            string JsonReturn = "failed at ParseJson";
            List<string> list = new List<string>();


            if (JsonMessage[0] == "LaunchVideo")
            {
                
                JsonReturn = _videoFormActionDelegate(Server.VideoAction.InitialisePlayers, JsonMessage[1]);
                return JsonReturn;
            }

            else if (JsonMessage[0] == "VideoPlayer")
            {
                JsonReturn = _videoFormActionDelegate(Server.VideoAction.VideoPlayerControls, JsonMessage[1]);
                return JsonReturn;
            }

            else if (JsonMessage[0] == "Sessions")
            {
                JsonReturn = _videoFormActionDelegate(Server.VideoAction.Sessions, JsonMessage[1]);
                return JsonReturn;
            }

            else if (JsonMessage[0] == "Media")
            {
                JsonReturn = _videoFormActionDelegate(Server.VideoAction.Media, JsonMessage[1]);
                return JsonReturn;
            }
            
            return JsonReturn;

        }

        

    }
}

