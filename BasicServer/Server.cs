using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading;
using System.IO;

namespace BasicServer
{
    public partial class Server : Form
    {
        private AsynchronousSocketListener listener;
        private Thread ioThread;
        private AsynchronousSocketListener.ScreenWriterDelegate screenWriterDelegate;
        public delegate string VideoFormActionDelegate(VideoAction action, string message);
        private VideoFormActionDelegate videoFormActionDelegate;
        string storedMessagesFilePath = @"C:\Users\Public\BasicServerLog.txt";
        List<string> MessageLog = new List<string>();
        VideoDisplay videoDisplay = new VideoDisplay();
        public Server()
        {
            InitializeComponent();
            if (!File.Exists(storedMessagesFilePath))
            {
                File.Create(storedMessagesFilePath).Dispose();
            }
            screenWriterDelegate = new AsynchronousSocketListener.ScreenWriterDelegate(WriteToScreen);
            videoFormActionDelegate = new VideoFormActionDelegate(PerformVideoAction);
            LoadWifiListener();
        }

        public void WriteToScreen(string message)
        {
            if (txtServer.InvokeRequired)
            {
                Invoke(screenWriterDelegate, message);
            }
            else
            {
                txtServer.Text += "\r\n" + message;
                MessageLog.Add("\r\n" + message);
                System.IO.File.WriteAllLines(storedMessagesFilePath, MessageLog);
            }
        }

        void LoadWifiListener()
        {
            listener = new AsynchronousSocketListener(screenWriterDelegate, videoFormActionDelegate);
            ioThread = new Thread(new ThreadStart(listener.StartListening));
            ioThread.SetApartmentState(ApartmentState.STA);
            ioThread.Start();
            WriteToScreen("Socket server listening");
        }

        public enum VideoAction
        {
            InitialisePlayers,
            VideoPlayerControls,
            Sessions
        }

        public string PerformVideoAction(VideoAction action, string message)
        {
            string JsonReturn = "failed at PVA";

            if (txtServer.InvokeRequired)
            {
                return Invoke(videoFormActionDelegate, action, message).ToString();
            }

            else
            {
                if (action == VideoAction.InitialisePlayers)
                {
                }
                else if (action == VideoAction.VideoPlayerControls)
                {
                    JsonReturn = VideoPlayerControls(message);
                }
                else if (action == VideoAction.Sessions)
                {
                    JsonReturn = SessionsHandler(message);
                }
                else
                {
                    JsonReturn = "video action failed";
                }

                return JsonReturn;

            }
        }

        public string SessionsHandler(string message)
        {
            string stringReturnMessage = "";
            string testJSONlist = "{\"sessions\":[{\"name\":\"A\",\"date\":\"01/01/2010\",\"list\":\"1\"},{\"name\":\"B\",\"date\":\"05/11/2013\",\"list\":\"2\"},{\"name\":\"C\",\"date\":\"15/04/2015\",\"list\":\"3\"}]}";
            string testLocation_MediaJSON1 = File.ReadAllText("C:\\Users\\Rachel Griffiths\\Documents\\Testing\\jsondetail.txt");
            string testLocation_MediaJSON2 = File.ReadAllText("C:\\Users\\Rachel Griffiths\\Documents\\Testing\\jsondetail2.txt");
            string testLocation_MediaJSON3 = File.ReadAllText("C:\\Users\\Rachel Griffiths\\Documents\\Testing\\jsondetail3.txt");

            if (message.Contains("Load"))
            {
                //needs to return type: Sessions, Body: Name, Date, List
                stringReturnMessage = "{\"messageType\":\"Sessions\",\"messageBody\":" + testJSONlist + "}";
            }
            else//will be sending one session to return location and media details for
            {
                if (message == "0")
                {
                    stringReturnMessage = "{\"messageType\":\"Detail\",\"messageBody\":" + testLocation_MediaJSON1 + "}";
                }
                else if (message == "1")
                {
                    stringReturnMessage = "{\"messageType\":\"Detail\",\"messageBody\":" + testLocation_MediaJSON2 + "}";
                }
                else if (message == "2")
                {
                    stringReturnMessage = "{\"messageType\":\"Detail\",\"messageBody\":" + testLocation_MediaJSON3 + "}";
                }
            }
            return stringReturnMessage;
        }
        public string VideoPlayerControls(string message)
        {
            string stringReturnMessage = "";

            if (videoDisplay != null)
            {

                if (message.Contains("Stop"))
                {
                    stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"Stopped\"}";
                    return stringReturnMessage;
                }
                else if (message.Contains("Pause"))
                {

                    stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"Paused\"}";

                    return stringReturnMessage;
                }
                else if (message.Contains("Play"))
                {

                    stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"Playing\"}";

                    return stringReturnMessage;
                }
                else if (message.Contains("Play Forwards"))
                {

                    stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"Playing Forwards\"}";

                }
                else if (message.Contains("Play Backwards"))
                {

                    stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"Playing Backwards\"}";


                }
                else if (message.Contains("Step Forwards"))
                {

                    stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"Stepped Forwards\"}";

                    return stringReturnMessage;
                }
                else if (message.Contains("Step Backwards"))
                {
                    stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"Stepped Backwards\"}";

                    return stringReturnMessage;
                }
            }
            stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"No Video Player\"}";
            return stringReturnMessage;
        }

    }
}
