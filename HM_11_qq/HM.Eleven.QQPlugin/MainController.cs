using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newbe.CQP.Framework;
using Newbe.CQP.Framework.Extensions;
using HM.Eleven.QQPlugins.Struct;
using HM.Eleven.QQPlugins.Helper;

namespace HM.Eleven.QQPlugins
{
    public class MainController : Newbe.CQP.Framework.PluginBase
    {
        public ChatController cc;
        public MainController(ICoolQApi coolQApi) : base(coolQApi)
        {
            cc = new ChatController();

            cc.outputQQEvent = new ChatController.sendQQChatMessage(printOutput);
            //cc.outputEvent = new ChatController.sendChatMessageDelegate(printOutput);
            cc.start();
        }
        public override string AppId => "HM.Eleven.QQPlugins";

        public override int ProcessPrivateMessage(int subType, int sendTime, long fromQQ, string msg, int font)
        {
            //使用CoolQApi将信息回发给发送者
            //CoolQApi.SendPrivateMsg(fromQQ, msg);
            // var res= CoolApiExtensions.GetGroupMemberList(CoolQApi ,318224623);
            //CoolQApi.SendPrivateMsg(fromQQ,  res.SourceString);
            //foreach (var v in res.Model)
            //{
            //    CoolQApi.SendPrivateMsg(fromQQ, v.Number + v.NickName + v.Level);

            //}
            //printOutput(new QQInfo("(log)" + msg, 287859992));
            cc.input(new QQInfo(msg, fromQQ));
            //printOutput(msg);
            //CoolQApi.SendPrivateMsg(fromQQ, subType+msg[0].ToString());
            
            return base.ProcessPrivateMessage(subType, sendTime, fromQQ, msg, font);
        }

        public override int ProcessGroupMessage(int subType, int sendTime, long fromGroup, long fromQq, string fromAnonymous, string msg, int font)
        {
            //CoolQApi.SendGroupMsg(fromGroup, msg);
            //cc.input(msg, fromGroup);
            //printOutput(new QQInfo("(log)" + msg, 287859992));
            cc.input(new QQInfo(msg, fromGroup,true));
            return base.ProcessGroupMessage(subType, sendTime, fromGroup, fromQq, fromAnonymous, msg, font);
        }

        private void printOutput(QQInfo info)
        {
            long qqnum = info.qq;
            //截断输出内容，分段输出，防止其超过插件限制的字数
            int maxlen = 50;
            if (info.info.Length > maxlen) info.info = info.info.Substring(0, maxlen) + "...";
            if (info.isgroup) CoolQApi.SendGroupMsg(qqnum, info.info);
            else CoolQApi.SendPrivateMsg(qqnum, info.info);

            //for (int i = 0; i <= info.info.Length / maxlen; i++)
            //{
            //    string tmpstr = "";
            //    if (info.info.Length <= i * maxlen + maxlen)
            //    {
            //        tmpstr = info.info.Substring(i * maxlen, info.info.Length - i * maxlen);
            //    }
            //    else
            //    {
            //        tmpstr = info.info.Substring(i * maxlen, maxlen);
            //    }
            //    if (info.isgroup) CoolQApi.SendGroupMsg(qqnum, tmpstr);
            //    else CoolQApi.SendPrivateMsg(qqnum, tmpstr);
            //}

        }
    }
}
