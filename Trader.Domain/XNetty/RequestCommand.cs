﻿using Newtonsoft.Json;
using System;
using System.Text;

namespace Abp.VNext.Hello.XNetty
{
    public class RequestCommand
    {
        /// <summary>
        /// I表示IOS, A表示Android, PC表示PC, H5表示移动端, D表示桌面程序
        /// </summary>
        public string ConnectionId { get; set; }

        /// <summary>
        /// 请求编号（36位的UUID字符串，或其他唯一的字符串）
        /// </summary>
        public string RequestNo
        {
            get => Environment.TickCount.ToString();//80856671
        }

        public string Message { get; set; }

        /// <summary>
        /// 模块ID
        /// </summary>
        public int Scope { get; set; }

        /// <summary>
        /// 指令ID
        /// </summary>
        /// 
        public int Cmd { get; set; }

    }


    /// <summary>
    /// 请求指令
    /// </summary>
    /// <typeparam name="T"></typeparam>

    public class RequestCommand<T> : RequestCommand
    {
        public static string Serialize(RequestCommand<T> requestCommand) => JsonConvert.SerializeObject(requestCommand);

        public static byte[] EncodeRequestCommand(RequestCommand<T> requestCommand)
        {
            string cmd = Serialize(requestCommand);
            byte[] messageBytes = Encoding.UTF8.GetBytes(cmd);
            return messageBytes;
        }

        /// <summary>
        /// 请求实体对象
        /// </summary>

        public T Data { get; set; } = default(T);
        public override string ToString() => Serialize(this);
        public void SetModule(Tuple<int, int> tuple)
        {
            Scope = tuple.Item1;
            Cmd = tuple.Item2;
        }
    }

}
