using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WechatConn.App_Start
{
    public class RequestXML
    {
        private String toUserName = String.Empty;

        /// <summary>
        /// 公众号
        /// </summary>
        public String ToUserName
        {
            get { return toUserName; }
            set { toUserName = value; }
        }

        private String fromUserName = "";

        /// <summary>
        /// 发送方微信号
        /// </summary>
        public String FromUserName
        {
            get { return fromUserName; }
            set { fromUserName = value; }
        }

        private String createTime = String.Empty;

        /// <summary>
        /// 创建时间
        /// </summary>
        public String CreateTime
        {
            get { return createTime; }
            set { createTime = value; }
        }

        private String msgType = String.Empty;

        /// <summary>
        /// 信息类型 地理位置:location,文本消息:text,消息类型:image
        /// </summary>
        public String MsgType
        {
            get { return msgType; }
            set { msgType = value; }
        }

        private String content = String.Empty;

        /// <summary>
        /// 信息内容
        /// </summary>
        public String Content
        {
            get { return content; }
            set { content = value; }
        }

        private String location_X = String.Empty;

        /// <summary>
        /// 地理位置纬度
        /// </summary>
        public String Location_X
        {
            get { return location_X; }
            set { location_X = value; }
        }

        private String location_Y = String.Empty;

        /// <summary>
        /// 地理位置经度
        /// </summary>
        public String Location_Y
        {
            get { return location_Y; }
            set { location_Y = value; }
        }

        private String scale = String.Empty;

        /// <summary>
        /// 地图缩放大小
        /// </summary>
        public String Scale
        {
            get { return scale; }
            set { scale = value; }
        }

        private String mapInfo = String.Empty;

        /// <summary>
        /// 地理位置信息
        /// </summary>
        public String MapInfo
        {
            get { return mapInfo; }
            set { mapInfo = value; }
        }

        private String picUrl = String.Empty;

        /// <summary>
        /// 图片链接
        /// </summary>
        public String PicUrl
        {
            get { return picUrl; }
            set { picUrl = value; }
        }

        private String _event;
        /// <summary>
        /// 事件类型
        /// </summary>
        public String Event
        {
            get { return _event; }
            set { _event = value; }
        }

        private String eventKey;
        /// <summary>
        /// 事件key
        /// </summary>
        public String EventKey
        {
            get { return eventKey; }
            set { eventKey = value; }
        }
    }
}