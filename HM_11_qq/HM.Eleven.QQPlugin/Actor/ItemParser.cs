using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HM.Eleven.QQPlugins.Struct;
using System.IO;

namespace HM.Eleven.QQPlugins.Actor
{
    class ItemParser
    {
        public string dataDir = @"./Data/";
        Dictionary<string, Link> link1;
        Dictionary<string, Link> link2;
        Dictionary<string,Area> areas;


        //public ItemParser(string dir = ".")
        //{
        //    dataDir = string.Format("{0}/Data/", dir);
        //    init();
        //}

        //public string author = "";

        //private int nowindex = 1;
        //private DateTime lastdate;
        //private string getId()
        //{
        //    string id = "";

        //    DateTime nowdate = DateTime.Now;
        //    if (nowdate > lastdate)
        //    {
        //        lastdate = nowdate;
        //        nowindex = 1;
        //    }
        //    id = string.Format("{0}-{1}-{2}", lastdate.ToString("yyyyMMddHHmmss"), author, nowindex++);

        //    return id;
        //}

        //private int existLink(string str1, string str2, string type = "")
        //{
        //    int res = -1;

        //    if (items.ContainsKey(str1) && items.ContainsKey(str2))
        //    {
        //        var item1list = items[str1];
        //        var item2list = items[str2];
        //        foreach (var item1 in item1list)
        //        {
        //            foreach (var item2 in item2list)
        //            {
        //                string idid = string.Format("{0},{1}", item1.id, item2.id);
        //                if (links.ContainsKey(idid))
        //                {
        //                    foreach (var link in links[idid])
        //                    {
        //                        if (string.IsNullOrWhiteSpace(type) || link.type == type)
        //                        {
        //                            return 1;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    return res;
        //}

        //private bool typeIsFather(string type)
        //{
        //    bool result = false;
        //    string[] words = { "是", "属于" };
        //    foreach (var w in words)
        //    {
        //        if (type == w) return true;
        //    }
        //    return result;
        //}

        //private bool typeIsProperty(string type)
        //{
        //    bool result = false;
        //    string[] words = { "有" };
        //    foreach (var w in words)
        //    {
        //        if (type == w) return true;
        //    }
        //    return result;
        //}

        //private List<Item> getProperty(Item item)
        //{
        //    List<Item> res = new List<Item>();

        //    foreach (var p in links.Keys.Where(x => x.StartsWith(item.id)).Select(x => links[x]))
        //    {
        //        foreach (var pp in p)
        //        {
        //            if (typeIsProperty(pp.type))
        //            {
        //                res.Add(itemsById[pp.id2]);
        //                //break;
        //            }
        //        }
        //    }

        //    return res;
        //}

        //private int existProperty(Item item1, string str2)
        //{
        //    var tmp = getProperty(item1);
        //    foreach (var t in tmp)
        //    {
        //        if (t.content == str2) return 1;
        //    }
        //    return -1;
        //}

        //private List<Item> getFathers(Item item)
        //{
        //    List<Item> res = new List<Item>();
        //    foreach (var l in links.Keys.Where(x => x.StartsWith(item.id)).Select(x => links[x]))
        //    {
        //        foreach (var linklist in l)
        //        {
        //            if (typeIsFather(linklist.type))
        //            {
        //                string id2 = linklist.id2;

        //                Item targetitem = itemsById[id2];
        //                if (id2 == item.id) break;
        //                if (res.Where(x => x.id == targetitem.id).Count() <= 0) res.Add(targetitem);
        //                break;
        //            }
        //        }
        //    }
        //    foreach (var r in res)
        //    {
        //        var tmp = getFathers(r);
        //        foreach (var t in tmp)
        //        {
        //            if (res.Where(x => x.id == t.id).Count() <= 0) res.Add(t);
        //        }
        //    }
        //    return res;
        //}


        //private Link findLink(Item item1, Item item2, string type)
        //{
        //    string idid = string.Format("{0},{1}", item1.id, item2.id);
        //    if (links.ContainsKey(idid))
        //    {
        //        foreach (var link in links[idid])
        //        {
        //            if (link.type == type)
        //            {
        //                return link;
        //            }
        //        }
        //    }
        //    return addLink(item1.id, item2.id, type);
        //}

        //private Item findItem(string content, string author = "")
        //{
        //    if (string.IsNullOrWhiteSpace(author))
        //    {
        //        author = this.author;
        //    }
        //    if (items.ContainsKey(content))
        //    {
        //        foreach (var item in items[content])
        //        {
        //            if (item.author == author)
        //            {
        //                // find exist item
        //                return item;
        //            }
        //        }
        //    }
        //    return addItem(content);
        //}

        //private static string removeBlanks(string ori)
        //{
        //    string[] blanks = { "\t", " ", "　", "\r" };
        //    string res = ori;
        //    foreach (var b in blanks) res = res.Replace(b, string.Empty);
        //    return res;
        //}

        //private static string[] spaceCut(string str)
        //{
        //    List<string> res = new List<string>();

        //    string[] keys = str.Trim(new char[] { '\r', '\n', '\t', ' ', '的' }).Split(' ');
        //    foreach (var k in keys)
        //    {
        //        if (!string.IsNullOrWhiteSpace(k)) res.Add(k);
        //    }


        //    return res.ToArray();
        //}

        //private static string[] naiveCut(string str)
        //{
        //    List<string> res = new List<string>();

        //    string[] keys = str.Split(new char[] { '的', '是', '有' });
        //    foreach (var k in keys)
        //    {
        //        if (!string.IsNullOrWhiteSpace(k)) res.Add(k);
        //    }

        //    return res.ToArray();
        //}

        //private static string[] cut(string str)
        //{
        //    List<string> res = new List<string>();

        //    List<string> tmp = new List<string>(spaceCut(str));
        //    foreach (var t in tmp)
        //    {
        //        string[] tmp1 = naiveCut(t);
        //        foreach (var tt in tmp1) res.Add(tt);
        //    }

        //    return res.ToArray();
        //}

        //public string input(string str)
        //{
        //    string[] keys = cut(str);
        //    if (keys.Length == 3)
        //    {
        //        //三元组
        //        string key1 = keys[0];
        //        string key2 = keys[2];
        //        string link1 = keys[1];
        //        findLink(findItem(key1), findItem(key2), link1);
        //    }
        //    return "ok";
        //}


        //private Item addItem(string content)
        //{
        //    string id = getId();
        //    Item item = new Item(id, content);
        //    if (!items.ContainsKey(content)) items[content] = new List<Item>();
        //    items[content].Add(item);
        //    this.itemsById[id] = item;
        //    return item;
        //}

        //private Link addLink(string id1, string id2, string type)
        //{
        //    string date = DateTime.Now.ToString("yyyyMMddHHmmss");
        //    Link link = new Link(id1, id2, type, date, author);
        //    string idid = string.Format("{0},{1}", id1, id2);
        //    if (!links.ContainsKey(idid)) links[idid] = new List<Link>();
        //    links[idid].Add(link);
        //    return link;
        //}

        //public void save()
        //{
        //    Dictionary<string, string> savedatas = new Dictionary<string, string>();
        //    Dictionary<string, string> savelinks = new Dictionary<string, string>();
        //    foreach (var itemlist in items)
        //    {
        //        foreach (var item in itemlist.Value)
        //        {
        //            string res = string.Format("{0}-{1}\r\n", item.id, item.content.Replace("\r", "\\r").Replace("\n", "\\n"));
        //            if (!savedatas.ContainsKey(item.author)) savedatas[item.author] = "";
        //            savedatas[item.author] += res;
        //        }
        //    }
        //    foreach (var linklist in links)
        //    {
        //        string[] ids = linklist.Key.Split(',');
        //        foreach (var link in linklist.Value)
        //        {
        //            string res = string.Format("{0},{1},{2},{3},{4}\r\n", ids[0], ids[1], link.type, link.date.ToString("yyyyMMddHHmmss"), link.author);
        //            if (!savelinks.ContainsKey(link.author)) savelinks[link.author] = "";
        //            savelinks[link.author] += res;
        //        }


        //    }

        //    foreach (var data in savedatas)
        //    {
        //        using (FileStream fs = File.Open(string.Format("{0}/{1}.data.txt", dataDir, data.Key), FileMode.Create))
        //        {
        //            using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8))
        //            {
        //                w.Write(data.Value);
        //            }

        //        }
        //    }
        //    foreach (var link in savelinks)
        //    {
        //        using (FileStream fs = File.Open(string.Format("{0}/{1}.link.txt", dataDir, link.Key), FileMode.Create))
        //        {
        //            using (StreamWriter w = new StreamWriter(fs, Encoding.UTF8))
        //            {
        //                w.Write(link.Value);
        //            }
        //        }
        //    }
        //}

        //private void init()
        //{

        //    this.items = new Dictionary<string, List<Item>>();
        //    this.itemsById = new Dictionary<string, Item>();
        //    this.links = new Dictionary<string, List<Link>>();
        //    string[] datafiles = Directory.GetFiles(dataDir, "*.data.txt");
        //    foreach (var file in datafiles)
        //    {
        //        /***
        //         * datafile format:
        //         * 
        //         * 20171016184502-author1-13-AABAadada!!?!a
        //         * 20171016185559-author1-14-asdadada131411123aa23
        //         * 
        //         * 
        //         ***/

        //        try
        //        {
        //            string dataname = file.Replace(".data.txt", "");
        //            string linkpath = string.Format("{0}/{1}.link.txt", dataDir, dataname);
        //            string[] linkdata = File.ReadAllLines(linkpath);
        //            string[] strdata = File.ReadAllLines(file);
        //            foreach (var line in strdata)
        //            {
        //                string[] tmp = line.Split('-');

        //                if (tmp.Length >= 3)
        //                {
        //                    string id = "";
        //                    string content = "";
        //                    id = string.Format("{0}-{1}-{2}", tmp[0], tmp[1], tmp[2]);
        //                    for (int i = 3; i < tmp.Length; i++)
        //                    {
        //                        content += i;
        //                    }
        //                    content = content.Replace("\\r", "\r").Replace("\\n", "\n");
        //                    Item item = new Item(id, content);
        //                    if (!this.items.ContainsKey(content))
        //                    {
        //                        // new
        //                        this.items[content] = new List<Item>();
        //                    }
        //                    this.items[content].Add(item);
        //                    this.itemsById[id] = item;
        //                }

        //            }

        //            /***
        //             * linkfile format:
        //             * 
        //             * 20171016184502-author1-13,20171016185559-author1-14,包含,20171020191919,author1
        //             * 20171017184502-author2-231,20171022010401-author2-146,例如,20171020191919,author2
        //             * 
        //             * 
        //             ***/

        //            foreach (var line in linkdata)
        //            {
        //                string[] tmp = line.Split(',');
        //                if (tmp.Length == 5)
        //                {
        //                    string id1 = tmp[0];
        //                    string id2 = tmp[1];
        //                    string type = tmp[2];
        //                    string date = tmp[3];
        //                    string author = tmp[4];
        //                    string idid = string.Format("{0},{1}", id1, id2);
        //                    if (!links.ContainsKey(idid))
        //                    {
        //                        links[idid] = new List<Link>();
        //                    }
        //                    links[idid].Add(new Link(id1, id2, type, date, author));
        //                }
        //            }
        //        }
        //        catch (Exception e)
        //        {

        //        }

        //    }
        //}

        public static string removeUnText(string ori)
        {
            string res = ori;
            res = res.Replace(" ", "");
            res = res.Replace("&nbsp;", " ");
            res = res.Replace("<br>", "\r\n");
            res = res.Replace("<br />", "\r\n");
            res = res.Replace("&quot;", "\"");
            res = res.Replace("&gt;", ">");
            res = res.Replace("&lt;", "<");
            res = res.Replace("&amp;", "&");
            return res;
        }

        public static string removeBlank(string ori,bool ignoreNewline=false)
        {
            string blanks = " \t"; 
            if (!ignoreNewline) blanks += "\r\n"; 

            StringBuilder sb = new StringBuilder();
            
            foreach(var c in ori)
            {
                if (!blanks.Contains(c)) sb.Append(c);
            }
            return sb.ToString();
        }

        public static string[] splitSentence(string str)
        {
            string[] splits = { " ", "\t", "\n", "\r", ",", ".", "?", " ", "!", ";", ":", "，", "。", "”", "“", "‘", "’", "：", "；", "？", "！", "、", "（", "）", "(", ")", "\"", "'", "—", "《", "》", "【", "】", "…" };
            return str.Split(splits, StringSplitOptions.RemoveEmptyEntries);
        }
    }
}
