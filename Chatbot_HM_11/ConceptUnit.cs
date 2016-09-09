using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chatbot_HM_11
{
    public class ConceptUnit
    {
        public Word sub;
        public List<Word> asub;
        public Word pred;
        public List<Word> apred;
        public Word obj;
        public List<Word> aobj;
        public int id;

        public ConceptUnit()
        {
            id = 0;
            sub = new Word("", "");
            pred = new Word("", "");
            obj = new Word("", "");
            asub = new List<Word>();
            apred = new List<Word>();
            aobj = new List<Word>();
        }

        public ConceptUnit(Word sub, List<Word> asub, Word pred, List<Word> apred, Word obj, List<Word> aobj, int id)
        {
            this.id = id;
            this.sub = sub;
            this.pred = pred;
            this.obj = obj;
            this.asub = asub;
            this.apred = apred;
            this.aobj = aobj;
        }

        public ConceptUnit(ConceptUnit u)
        {
            if (u == null) return;
            sub = u.sub;
            pred = u.pred;
            obj = u.obj;
            asub = new List<Word>(u.asub.ToArray());
            apred =new List<Word>( u.apred.ToArray());
            aobj = new List<Word>(u.aobj.ToArray());
        }

        public string toString()
        {
            string res = "";
            foreach (var a in asub) res += a.word;
            res += sub.word;
            foreach (var a in apred) res += a.word;
            res += pred.word;
            foreach (var a in aobj) res += a.word;
            res += obj.word;
            return res;
        }
    }
}
