using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management.Automation;
using WebSocketSharp;
using WebSocketSharp.Server;
using PowerShellCommands;
using System.Net;
using System.Net.Sockets;

namespace RAT_Socket
{
    public class PSCommand : WebSocketBehavior
    {
        protected override void OnMessage(MessageEventArgs e)
        {
            Console.Clear();
            Console.WriteLine("Command received: " + e.Data +"\nInterpreting...");
            
            PowerShellSession ps = new PowerShellSession();
            List<string> splitdata = ps.SanitizeInput(e.Data);
            if (splitdata.Count > 0)
            {
                switch (splitdata[0])
                {
                    case "newuser":
                        {
                            splitdata.RemoveAt(0);
                            BroadcastMessage("Creating new user.");
                            ps.PSCreateUser(splitdata);
                            break;
                        }
                    case "edituser":
                        {
                            splitdata.RemoveAt(0);
                            BroadcastMessage("Editing user.");
                            ps.PSEditUser(splitdata);
                            break;
                        }
                    case "deleteuser":
                        {
                            splitdata.RemoveAt(0);
                            Send("Deleting user.");
                            BroadcastMessage("Deleting user.");
                            ps.PSDeleteUser(splitdata[0]);
                            break;
                        }
                    case "getuser":
                        {
                            splitdata.RemoveAt(0);
                            BroadcastMessage(ps.PSGetUser());
                            break;
                        }
                    default:
                        {
                            Send("Unknown command! " + splitdata[0]);
                            Console.WriteLine("Unknown command! " + splitdata[0]);
                            break;
                        }

                }
            }
            else
            {
                BroadcastMessage("Cannot resolve command!\n Message: " + e.Data);
            }
        }
        protected override void OnOpen()
        {
            BroadcastMessage("Connected.");
        }
        public void BroadcastMessage(string msgs)
        {
            Console.WriteLine(msgs);
            Send(msgs);
        }
    }
    internal class WebSocket
    {
        public static string LocalHostIP()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            return "error";
        }
        static void Main(string[] args)
        {
            string host = LocalHostIP();
            WebSocketServer wss = new WebSocketServer("ws://" + host + ":1337");
            wss.AddWebSocketService<PSCommand>("/PSCommand");
            wss.Start();
            Console.WriteLine("Server listening. Server IP: "+ host);

            Console.ReadKey();
            wss.Stop();
        }

    }
}
