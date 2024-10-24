﻿using System;
using System.IO;
using System.IO.MemoryMappedFiles;
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
                for (int i = 0; i < 4; i++)
                {
                    if (Directory.Exists(Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, th2)) + "/export"))
                    {
                        curdirect = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, th2)) + "/export";
                        break;
                    }
                    else
                    {
                        th2 += @"..\";
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
                string args = data.Substring(3).Split('/')[0];
                var tcpEndPoint = new IPEndPoint(IPAddress.Parse(args), Convert.ToInt32("4307"));
                var tcpSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                tcpSocket.Connect(tcpEndPoint);
                socket = tcpSocket;
                string a = Socet(data);
                if (a == "-cn")
                {
                    Console.WriteLine("Error connect");
                    outData("-cn");
                    return;
                }
                Console.WriteLine("Connected");
                outData(a);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error connect");
                Console.WriteLine(e);
                outData("-cn");
            }
        }
        private static Timer inDataTimer = new Timer(100);
        private static Socket socket;
        private static string curdirect = "";
        private static bool conected = false;

        static string Socet(string str)
        {
            try
            {
                var data = Encoding.UTF8.GetBytes(str);
                socket.Send(data);
                var buffer = new byte[512];
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
                    if(indata.Substring(0, 3) == "~cn")
                    {
                        Connect(indata);
                    }
                    else
                    {
                        outData("-cn");
                    }
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
