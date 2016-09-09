using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Chatbot_HM_11
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
            cc=new ChatController();
            cc.info = IOController.getInfoFromJson("chatbotInfo.json");
            cc.concepts = IOController.getConceptsFromJson("chatbotConcepts.json");
            if (cc.concepts == null) cc.concepts = new List<ConceptUnit>();
            if (cc.info == null) cc.info = new ChatInfo();
            cc.outputEvent = new ChatController.sendChatMessageDelegate(printOutput);
            cc.specials = IOController.readSpecialAnswerFromFile("special.txt");
            cc.start();
        }

        private void printOutput(string str)
        {
            if (textBox2.InvokeRequired)
            {
                sendStringDelegate printEvent = new sendStringDelegate(printOutput);
                Invoke(printEvent, (object)str);
            }
            else
            {
                textBox2.AppendText("HM_11:" + str + "\r\n");
            }
        }

        private void printInput(string str)
        {
            if (textBox2.InvokeRequired)
            {
                sendStringDelegate printEvent = new sendStringDelegate(printInput);
                Invoke(printEvent, (object)str);
            }
            else
            {
                textBox2.AppendText("User:" + str + "\r\n");
            }
        }

        private void getInput()
        {
            string inputStr = textBox1.Text;
            textBox1.Text = "";
            cc.input(inputStr);
            printInput(inputStr);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            getInput();
            textBox1.Focus();
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
                textBox1.Text = "";
            }
        }
    }
}
