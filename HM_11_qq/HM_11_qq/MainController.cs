using Flexlive.CQP.Framework;
using System;
using HM_11_qq.Helper;
using HM_11_qq.Struct;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel;

namespace HM_11_qq
{
    public class MainController : CQAppAbstract
    {
        public ChatController cc;

        /// <summary>
        /// 应用初始化，所有应用初始化信息请在此处添加代码。
        /// </summary>
        public override void Initialize()
        {
            // 初始化
            cc = new ChatController();
            cc.info = new ChatInfo();
            cc.concepts = new List<Sentence>();
            //cc.info = IOHelper.getInfoFromJson("chatbotInfo.json");
            //cc.concepts = IOHelper.getConceptsFromJson("chatbotConcepts.json");
            if (cc.concepts == null) cc.concepts = new List<Sentence>();
            if (cc.info == null) cc.info = new ChatInfo();
            cc.outputEvent = new ChatController.sendChatMessageDelegate(printOutput);
            cc.specials = IOHelper.readSpecialAnswerFromFile("special.txt");
            cc.start();
        }

        private void printOutput(string str)
        {
            long qqnum=287859992;
            //截断输出内容，分段输出，防止其超过插件限制的字数
            int maxlen = 500;
            for (int i = 0; i <= str.Length / maxlen; i++)
            {
                string tmpstr = "";
                if (str.Length <= i * maxlen + maxlen)
                {
                    tmpstr = str.Substring(i * maxlen, str.Length - i * maxlen);
                }
                else
                {
                    tmpstr = str.Substring(i * maxlen, maxlen);
                }
                CQ.发送私聊消息(qqnum, tmpstr);
            }
                
        }

        /// <summary>
        /// Type=21 私聊消息。
        /// </summary>
        /// <param name="subType">子类型，11/来自好友 1/来自在线状态 2/来自群 3/来自讨论组。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromQQ">来源QQ。</param>
        /// <param name="msg">消息内容。</param>
        /// <param name="font">字体。</param>
        public override void PrivateMessage(int subType, int sendTime, long fromQQ, string msg, int font)
        {
            // 处理私聊消息。
            if (cc == null)
            {
                CQ.发送私聊消息(fromQQ, "正在初始化。");
                try
                {
                    Initialize();
                }
                catch (Exception e)
                {
                    CQ.发送私聊消息(fromQQ, "初始化失败：" + e.Message);
                }
            }
            else
            {
                try
                {
                    cc.input(msg);
                }
                catch (Exception e)
                {
                    CQ.发送私聊消息(fromQQ, "处理失败：" + e.Message);
                }
                
            }
            
            //CQ.发送私聊消息(fromQQ, "hi我是苦瓜酱~你发的消息是：" + msg);
        }

        /// <summary>
        /// Type=2 群消息。
        /// </summary>
        /// <param name="subType">子类型，目前固定为1。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromGroup">来源群号。</param>
        /// <param name="fromQQ">来源QQ。</param>
        /// <param name="fromAnonymous">来源匿名者。</param>
        /// <param name="msg">消息内容。</param>
        /// <param name="font">字体。</param>
        public override void GroupMessage(int subType, int sendTime, long fromGroup, long fromQQ, string fromAnonymous, string msg, int font)
        {
            // 处理群消息。

            var groupMember = CQ.取群成员信息(fromGroup, fromQQ);

            //CQ.发送群消息(fromGroup, String.Format("{0} 你的群名片：{1}， 入群时间：{2}， 最后发言：{3}。", CQ.CQ码_At(fromQQ),
            //    groupMember.名片, groupMember.加群时间, groupMember.最后发言));
        }

        /// <summary>
        /// Type=4 讨论组消息。
        /// </summary>
        /// <param name="subType">子类型，目前固定为1。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromDiscuss">来源讨论组。</param>
        /// <param name="fromQQ">来源QQ。</param>
        /// <param name="msg">消息内容。</param>
        /// <param name="font">字体。</param>
        public override void DiscussMessage(int subType, int sendTime, long fromDiscuss, long fromQQ, string msg, int font)
        {
            // 处理讨论组消息。
        }

        /// <summary>
        /// Type=11 群文件上传事件。
        /// </summary>
        /// <param name="subType">子类型，目前固定为1。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromGroup">来源群号。</param>
        /// <param name="fromQQ">来源QQ。</param>
        /// <param name="file">上传文件信息。</param>
        public override void GroupUpload(int subType, int sendTime, long fromGroup, long fromQQ, string file)
        {
            // 处理群文件上传事件。
        }

        /// <summary>
        /// Type=101 群事件-管理员变动。
        /// </summary>
        /// <param name="subType">子类型，1/被取消管理员 2/被设置管理员。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromGroup">来源群号。</param>
        /// <param name="beingOperateQQ">被操作QQ。</param>
        public override void GroupAdmin(int subType, int sendTime, long fromGroup, long beingOperateQQ)
        {
            // 处理群事件-管理员变动。
        }

        /// <summary>
        /// Type=102 群事件-群成员减少。
        /// </summary>
        /// <param name="subType">子类型，1/群员离开 2/群员被踢 3/自己(即登录号)被踢。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromGroup">来源群。</param>
        /// <param name="fromQQ">来源QQ。</param>
        /// <param name="beingOperateQQ">被操作QQ。</param>
        public override void GroupMemberDecrease(int subType, int sendTime, long fromGroup, long fromQQ, long beingOperateQQ)
        {
            // 处理群事件-群成员减少。
        }

        /// <summary>
        /// Type=103 群事件-群成员增加。
        /// </summary>
        /// <param name="subType">子类型，1/管理员已同意 2/管理员邀请。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromGroup">来源群。</param>
        /// <param name="fromQQ">来源QQ。</param>
        /// <param name="beingOperateQQ">被操作QQ。</param>
        public override void GroupMemberIncrease(int subType, int sendTime, long fromGroup, long fromQQ, long beingOperateQQ)
        {
            // 处理群事件-群成员增加。
        }

        /// <summary>
        /// Type=201 好友事件-好友已添加。
        /// </summary>
        /// <param name="subType">子类型，目前固定为1。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromQQ">来源QQ。</param>
        public override void FriendAdded(int subType, int sendTime, long fromQQ)
        {
            // 处理好友事件-好友已添加。
        }

        /// <summary>
        /// Type=301 请求-好友添加。
        /// </summary>
        /// <param name="subType">子类型，目前固定为1。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromQQ">来源QQ。</param>
        /// <param name="msg">附言。</param>
        /// <param name="responseFlag">反馈标识(处理请求用)。</param>
        public override void RequestAddFriend(int subType, int sendTime, long fromQQ, string msg, string responseFlag)
        {
            // 处理请求-好友添加。
        }

        /// <summary>
        /// Type=302 请求-群添加。
        /// </summary>
        /// <param name="subType">子类型，目前固定为1。</param>
        /// <param name="sendTime">发送时间(时间戳)。</param>
        /// <param name="fromGroup">来源群号。</param>
        /// <param name="fromQQ">来源QQ。</param>
        /// <param name="msg">附言。</param>
        /// <param name="responseFlag">反馈标识(处理请求用)。</param>
        public override void RequestAddGroup(int subType, int sendTime, long fromGroup, long fromQQ, string msg, string responseFlag)
        {
            // 处理请求-群添加。

            CQ.置群添加请求(responseFlag, 1, 1);
        }
    }
}
