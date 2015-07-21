using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BasicServer
{
    public class AsynchronousSocketListener
    {
        public string JsonReturn;
        // Thread signal.
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        private Server.VideoFormActionDelegate _videoFormActionDelegate;
        public delegate void ScreenWriterDelegate(string input);

        public AsynchronousSocketListener(ScreenWriterDelegate screenWriterCall, Server.VideoFormActionDelegate videoFormActionDelegate)
        {
            _screenWriterCall = screenWriterCall;
            _videoFormActionDelegate = videoFormActionDelegate;
        }

        private ScreenWriterDelegate _screenWriterCall;
        public void StartListening()
        {
            // Data buffer for incoming data.
            byte[] bytes = new Byte[1024];

            // Establish the local endpoint for the socket.
            // The DNS name of the computer

            IPAddress[] ipAddresses = Dns.GetHostAddresses(Dns.GetHostName());
            IPAddress ipAddress = ipAddresses[0];
            foreach (IPAddress IPA in ipAddresses)
            {

                if (IPA.AddressFamily.ToString() == "InterNetwork")
                {

                    ipAddress = IPA;
                    break;
                }
            }
            IPEndPoint localEndPoint = new IPEndPoint(ipAddress, 15000); //port defined here

            // Create a TCP/IP socket.
            using (Socket listener = new Socket(AddressFamily.InterNetwork,
                SocketType.Stream, ProtocolType.Tcp))
            {
                // Bind the socket to the local endpoint and listen for incoming connections.
                try
                {
                    listener.Bind(localEndPoint);
                    listener.Listen(100);

                    while (true)
                    {
                        // Set the event to nonsignaled state.
                        allDone.Reset();

                        // Start an asynchronous socket to listen for connections.
                        _screenWriterCall("WiFi server listening...");
                        listener.BeginAccept(
                            new AsyncCallback(AcceptCallback),
                            listener);

                        // Wait until a connection is made before continuing.
                        allDone.WaitOne();

                    }

                }
                catch (Exception e)
                {
                    _screenWriterCall(e.ToString());
                }
            }

            _screenWriterCall("\nPress ENTER to continue...");
        }

        public void AcceptCallback(IAsyncResult ar)
        {
            // Signal the main thread to continue.
            allDone.Set();

            // Get the socket that handles the client request.
            Socket listener = (Socket)ar.AsyncState;
            Socket handler = listener.EndAccept(ar);

            // Create the state object.
            StateObject state = new StateObject();
            state.workSocket = handler;
            handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                new AsyncCallback(ReadCallback), state);
        }

        public void ReadCallback(IAsyncResult ar)
        {
            String content = String.Empty;

            // Retrieve the state object and the handler socket
            // from the asynchronous state object.
            StateObject state = (StateObject)ar.AsyncState;
            Socket handler = state.workSocket;

            // Read data from the client socket. 
            int bytesRead = handler.EndReceive(ar);

            if (bytesRead > 0)
            {
                // There  might be more data, so store the data received so far.
                state.sb.Append(Encoding.ASCII.GetString(
                    state.buffer, 0, bytesRead));

                // Check for end-of-file tag. If it is not there, read 
                // more data.
                content = state.sb.ToString().Remove(0, 2);
                //   if (content.IndexOf("<EOF>") > -1)   //these blocks are commented out as we are not using an EOF check.
                //   {
                // All the data has been read from the 
                // client. Display it on the console.
                string message = "Read " + content.Length + "bytes from wifi socket. Data = " + content;
                _screenWriterCall(message);
                //callJSONParse    
                ParseJson parseJson = new ParseJson(_videoFormActionDelegate);
                JsonReturn = parseJson.InitialParsing(content); //parse message
                Send(handler, JsonReturn);
             

                //   }
                /*   else
                   {
                       // Not all data received. Get more.
                       handler.BeginReceive(state.buffer, 0, StateObject.BufferSize, 0,
                       new AsyncCallback(ReadCallback), state);
                   } */
            }
        }

        private void Send(Socket handler, String data)
        {
            // Convert the string data to byte data using ASCII encoding.
            byte[] byteData = Encoding.ASCII.GetBytes(data);
            _screenWriterCall("Sending: " + data);

            // Begin sending the data to the remote device.
            handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), handler);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            {
                // Retrieve the socket from the state object.
                Socket handler = (Socket)ar.AsyncState;

                // Complete sending the data to the remote device.
                int bytesSent = handler.EndSend(ar);
                _screenWriterCall(String.Format("Sent {0} bytes to client.", bytesSent));

                handler.Shutdown(SocketShutdown.Both);
                handler.Close();


            }
            catch (Exception e)
            {
                _screenWriterCall(e.ToString());
            }
        }
    }
}
