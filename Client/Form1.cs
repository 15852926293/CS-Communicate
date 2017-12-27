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

namespace Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        public static Socket gloSocket;
        //public Socket connect()
        public void connect()
        {
            //string host = GetIpAddress();
            string host = textBox1.Text;
            if (host == "")
            {
                throw new Exception("Get host IPV4 Address Failed");
            }

            int port = Convert.ToInt32(textBox2.Text);

            IPAddress ip = IPAddress.Parse(host);
            IPEndPoint ipe = new IPEndPoint(ip, port);

            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            clientSocket.Connect(ipe);
            richTextBox1.AppendText(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n");
            richTextBox1.AppendText("Connect to server success!\n" + "\n\n");
            gloSocket = clientSocket;

            Thread recth = new Thread(recMsg);
            recth.IsBackground = true;
            recth.Start();
        }

        public void recMsg()
        {
            while (true)
            {
                //receive message
                string recMsg = "";
                byte[] recBytes = new byte[4096];
                int bytes = gloSocket.Receive(recBytes, recBytes.Length, 0);
                recMsg += Encoding.ASCII.GetString(recBytes, 0, bytes);
                richTextBox1.AppendText("From Server:" + "\n");
                richTextBox1.AppendText(DateTime.Now.ToString("yyy-MM-dd HH:mm:ss") + "\n");
                richTextBox1.AppendText(recMsg + "\n\n");
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Thread connth = new Thread(connect);
            connth.IsBackground = true;
            connth.Start();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            string sendMsg = richTextBox2.Text;
            richTextBox1.AppendText("To Server:" + "\n");
            richTextBox1.AppendText(DateTime.Now.ToString("yyy-MM-dd HH:mm:ss") + "\n");
            richTextBox1.AppendText(sendMsg + "\n\n");
            byte[] sendBytes = Encoding.ASCII.GetBytes(sendMsg);
            gloSocket.Send(sendBytes);
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
