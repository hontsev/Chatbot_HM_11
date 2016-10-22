using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using HM_11_qq;
using HM_11_qq.Helper;
using HM_11_qq.Struct;

namespace TestForm
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
            cc.outputEvent = new ChatController.sendChatMessageDelegate(printOutput);
            cc.start();
        }

        private void printOutput(string str)
        {
            if (textBox1.InvokeRequired)
            {
                sendStringDelegate printEvent = new sendStringDelegate(printOutput);
                Invoke(printEvent, (object)str);
            }
            else
            {
                textBox1.AppendText("HM_11:" + str + "\r\n");
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
                textBox1.AppendText("User:" + str + "\r\n");
            }
        }

        private void getInput()
        {
            string inputStr = textBox2.Text;
            textBox2.Text = "";
            cc.input(inputStr);
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
    }
}
