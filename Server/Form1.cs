using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Server
{
    public partial class Form1 : Form
    {
        public Form1()
        {            
            InitializeComponent();

            string host = GetIpAddress();
            textBox1.Text = host;
            if (host == "")
            {
                throw new Exception("Get host IPV4 Address Failed");
            }
        }

        public static string GetIpAddress()
        {
            string hostName = Dns.GetHostName();   //获取本机名
            IPHostEntry localhost = Dns.GetHostEntry(hostName);
            foreach (IPAddress ipa in localhost.AddressList)
            {
                if (ipa.AddressFamily == AddressFamily.InterNetwork)
                    return ipa.ToString();
            }
            return "";
        }

        public static Socket gloSocket;
        //public Socket connect()
        public void connect()
        {
            //string host = GetIpAddress();
            //textBox1.Text = "127.0.0.1";
            string host = textBox1.Text;
            int port = Convert.ToInt32(textBox2.Text);

            IPAddress ip = IPAddress.Parse(host);
            IPEndPoint ipe = new IPEndPoint(ip, port);

            Socket sSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sSocket.Bind(ipe);
            sSocket.Listen(10);
            richTextBox1.AppendText("Waitting for client connect...\n");

            //receive message
            Socket serverSocket = sSocket.Accept();
            richTextBox1.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n");
            richTextBox1.AppendText("Connection established!\n" + "Client Info: " + serverSocket.RemoteEndPoint.ToString() + "\n\n");
            gloSocket = serverSocket;
            //return serverSocket;
            Thread recth = new Thread(recMsg);
            recth.IsBackground = true;
            recth.Start();
        }

        public void recMsg()
        {
            while (true)
            {
                string recMsg = "";
                byte[] recbyte = new byte[4096];
                int bytes = gloSocket.Receive(recbyte, recbyte.Length, 0);
                recMsg += Encoding.ASCII.GetString(recbyte, 0, bytes);
                richTextBox1.AppendText("From Client:" + "\n");
                richTextBox1.AppendText(DateTime.Now.ToString("yyy-MM-dd HH:mm:ss") + "\n");
                richTextBox1.AppendText(recMsg + "\n\n");
                if (recMsg == "bye")
                {
                    break;
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            Thread connth = new Thread(connect);
            connth.IsBackground = true;
            connth.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string sendMsg = richTextBox2.Text;
            richTextBox1.AppendText("To Client:" + "\n");
            richTextBox1.AppendText(DateTime.Now.ToString("yyy-MM-dd HH:mm:ss") + "\n");
            richTextBox1.AppendText(sendMsg + "\n\n");
            byte[] sendByte = Encoding.ASCII.GetBytes(sendMsg);
            gloSocket.Send(sendByte, sendByte.Length, 0);
            richTextBox2.Clear();
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            richTextBox1.SelectionStart = richTextBox1.Text.Length; //Set the current caret position at the end
            richTextBox1.ScrollToCaret(); //Now scroll it automatically
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
            richTextBox2.SelectionStart = richTextBox2.Text.Length; //Set the current caret position at the end
            richTextBox2.ScrollToCaret(); //Now scroll it automatically
        }

        private void richTextBox2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter && e.KeyCode == Keys.Control)//判断回车键
            {
                this.button2_Click(sender, e);//触发按钮事件
            }
        }
    }
}
