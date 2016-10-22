using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HM_11_qq.Struct
{
    public class ChatInfo
    {

        public int conceptIndex;

        public int loopWaitTime;
        public int loopOutputTime;
        public double like;
        public double excitement;
        public double interest;
        public int waitTime;
        public int maxWaitTime;



        public ChatInfo()
        {
            loopWaitTime = 10;
            loopOutputTime = 400;
            conceptIndex = 0;

            like = 50;
            excitement = 50;
            interest = 50;

            waitTime = 10;
            maxWaitTime = 10;
        }
    }
}
