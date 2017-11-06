using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HM.Eleven.QQPlugins;


namespace HM.Eleven.QQPlugins.Test
{
    public partial class Form1 : Form
    {
        public ChatController cc;
        delegate void sendStringDelegate(string str);

        public Form1()
        {
            InitializeComponent();
            init();
        }

        private void init()
        {
            cc = new ChatController();
            cc.outputQQEvent = new ChatController.sendQQChatMessage(printOutput);
            //cc.outputEvent = new ChatController.sendChatMessageDelegate(printOutput);
            cc.start();
        }

        private void printOutput(QQInfo info)
        {
            if (textBox1.InvokeRequired)
            {
                ChatController.sendQQChatMessage printEvent = new ChatController.sendQQChatMessage(printOutput);
                Invoke(printEvent, (object)info);
            }
            else
            {
                textBox1.AppendText("\r\nHM_11 : " + info.info + "\r\n");
            }
        }

        private void printInput(string str)
        {
            if (textBox1.InvokeRequired)
            {
                sendStringDelegate printEvent = new sendStringDelegate(printInput);
                Invoke(printEvent, (object)str);
            }
            else
            {
                textBox1.AppendText("\r\nUser : " + str + "\r\n");
            }
        }

        private void getInput()
        {
            string inputStr = textBox2.Text;
            textBox2.Text = "";
            cc.input(new QQInfo(inputStr, 0));
            printInput(inputStr);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            getInput();
            textBox2.Focus();
        }

        private void textBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                getInput();
            }
        }

        private void textBox1_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                textBox2.Text = "";
            }
        }

        private void Form1_Shown(object sender, EventArgs e)
        {
            textBox2.Focus();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Environment.Exit(0);
        }
    }
}
