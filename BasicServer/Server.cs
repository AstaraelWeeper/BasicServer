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
            VideoPlayerControls
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
                else
                {
                    JsonReturn = "video action failed";
                }

                return JsonReturn;

            }
        }

        public string VideoPlayerControls(string message)
        {
            string stringReturnMessage = "";
            string testJSONlist = "{\"info\":[{\"start\":\"0\",\"end\":\"5\",\"ELR\":\"0\"},{\"start\":\"5\",\"end\":\"10\",\"ELR\":\"20\"},{\"start\":\"10\",\"end\":\"15\",\"ELR\":\"99\"}]}";

            if (videoDisplay != null)
            {

                if (message.Contains("Stop"))
                {
                    if (videoDisplay != null)
                    {
                        stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"Stopped\"}";
                    }
                    else
                    {
                        stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"No Video Player\"}";
                    }
                    return stringReturnMessage;
                }
                else if (message.Contains("Pause"))
                {
                    if (videoDisplay != null)
                    {
                        stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"Paused\"}";
                    }
                    else
                    {
                        stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"No Video Player\"}";
                    }
                    return stringReturnMessage;
                }
                else if (message.Contains("Play"))
                {
                    if (videoDisplay != null)
                    {
                        stringReturnMessage = "{\"messageType\":\"List\",\"messageBody\":" + testJSONlist + "}";
                       
                    }
                    else
                    {
                        //stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"No Video Player\"}";
                        videoDisplay = new VideoDisplay();
                        stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"Playing\"}";
                    }
                    return stringReturnMessage;
                }
                else if (message.Contains("Play Forwards"))
                {
                    if (videoDisplay != null)
                    {
                        stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"Playing Forwards\"}";

                    }
                    else
                    {
                        stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"No Video Player\"}";
                    }
                    return stringReturnMessage;
                }
                else if (message.Contains("Play Backwards"))
                {
                    if (videoDisplay != null)
                    {
                        stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"Playing Backwards\"}";

                    }
                    else
                    {
                        stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"No Video Player\"}";
                    }
                    return stringReturnMessage;
                }
                else if (message.Contains("Step Forwards"))
                {
                    if (videoDisplay != null)
                    {
                        stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"Stepped Forwards\"}";

                    }
                    else
                    {
                        stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"No Video Player\"}";
                    }
                    return stringReturnMessage;
                }
                else if (message.Contains("Step Backwards"))
                {
                    if (videoDisplay != null)
                    {
                        stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"Stepped Backwards\"}";

                    }
                    else
                    {
                        stringReturnMessage = "{\"messageType\":\"VideoPlayer\",\"messageBody\":\"No Video Player\"}";
                    }
                    return stringReturnMessage;
                }
            }
            return stringReturnMessage;
        }

    }
}
