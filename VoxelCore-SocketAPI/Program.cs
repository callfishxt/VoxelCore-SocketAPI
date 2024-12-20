using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Timers;
using Timer = System.Timers.Timer;

namespace SocketAPI
{
    class Program
    {
        //out and in from server
        static void Main(string[] args)
        {
            try
            {
                string th2 = @"";
                string path = "./";
                for (int i = 0; i < 4; i++)
                {   
                    path = "./" + th2;
                    if (Directory.Exists(path + "export"))
                    {
                        curdirect = path + "export";
                        break;
                    }
                    else
                    {
                        th2 += @"../";
                    }
                }
                if(curdirect == "")
                {
                    Console.WriteLine("Directory /export not exist");
                    Console.ReadLine();
                    return;
                }

                if (File.Exists(curdirect + "/outData")) File.Delete(curdirect + "/outData");
                if (File.Exists(curdirect + "/inData")) File.Delete(curdirect + "/inData");
                inDataTimer.Elapsed += inData;
                inDataTimer.Start();
                Console.WriteLine("Started");
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
            smd();
        }
        static void smd()
        {
            Console.ReadLine();
            smd();
        }
        static void Connect(string data)
        {
            try
            {
                string[] args = data.Split('/');
                buffersuze = int.Parse(args[2]);
                if (File.Exists(curdirect + "/outData")) File.Delete(curdirect + "/outData");
                if (File.Exists(curdirect + "/inData")) File.Delete(curdirect + "/inData");
                var tcpEndPoint = new IPEndPoint(IPAddress.Parse(args[0]), Convert.ToInt32(args[1]));
                var tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                tcpSocket.Connect(tcpEndPoint);
                socket = tcpSocket;
                string a = Socet(data.Substring(args[0].Length + args[1].Length + args[2].Length + 3));
                if (a == "-cn")
                {
                    Console.WriteLine("Error connect server");
                    outData("-cn");
                    return;
                }
                conected = true;
                Console.WriteLine("Connected");
                outData(a);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error connect program");
                Console.WriteLine(e);
                outData("-cn");
            }
        }
        private static Timer inDataTimer = new Timer(100);
        private static Socket socket;
        private static string curdirect = "";
        private static bool conected = false;
        private static int buffersuze;
        static string Socet(string str)
        {
            try
            {
                var data = Encoding.UTF8.GetBytes(str);
                socket.Send(data);
                var buffer = new byte[buffersuze];
                var size = 0;
                var answer = new StringBuilder();
                do
                {
                    size = socket.Receive(buffer);
                    answer.Append(Encoding.UTF8.GetString(buffer, 0, size));
                }
                while (socket.Available > 0);
                return answer + "";
            }
            catch
            {
                Console.WriteLine("Disconected");
                conected = false;
                return "-cn";
            }
        }
        //Data
        static void inData(Object source, ElapsedEventArgs e)
        {
            if(File.Exists(curdirect + "/inData"))
            {
                string indata = File.ReadAllText(curdirect + "/inData");
                File.Delete(curdirect + "/inData");
                if (conected)
                {
                    outData(Socet(indata));
                }
                else
                {
                    Connect(indata);
                }
            }
        }
        static void outData(string outdata)
        {
            if (!File.Exists(curdirect + "/outData"))
            {
                File.WriteAllText(curdirect + "/outData", outdata);
            }
            else
            {
                Console.WriteLine("out Data fauled");
            }
        }
    }
}
