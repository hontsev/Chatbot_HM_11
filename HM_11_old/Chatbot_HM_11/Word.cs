﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatbot_HM_11
{
    public class Word
    {
        public string word;
        public string type;

        public Word()
        {
            word = "";
            type = "";
        }
        public Word(string word, string type)
        {
            this.word = word;
            this.type = type;
        }
        public Word(LtpWord w)
        {
            this.word = w.cont;
            this.type = w.pos;
        }
    }
}
