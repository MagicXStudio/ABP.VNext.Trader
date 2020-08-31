using DotNetty.Codecs;
using DotNetty.Handlers.Logging;
using DotNetty.Transport.Bootstrapping;
using DotNetty.Transport.Channels;
using DotNetty.Transport.Channels.Sockets;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Abp.VNext.Hello.XNetty
{
    public class ClientBootstrap
    {
        public static IPAddress Host => IPAddress.Parse("106.13.130.51");

        public static int Port => int.Parse("443");

        private readonly static Lazy<ClientBootstrap> lazy = new Lazy<ClientBootstrap>(() => new ClientBootstrap(), true);
        private readonly Bootstrap bootstrap = new Bootstrap();
        public static ClientBootstrap Client => lazy.Value;
        private ClientBootstrap()
        {
            DisConnected += TradeClientBootstrap_DisConnected;
            RetryConnect += TradeClientBootstrap_RetryConnectAsync;
        }
        private void TradeClientBootstrap_DisConnected(object sender, DisConnectedArgs e)
        {
        }
        private async void TradeClientBootstrap_RetryConnectAsync(object sender, DisConnectedArgs e)
        {
            await InitBootstrapAsync().ContinueWith(x =>
            {
            });
        }

        public IChannel ChatChannel { get; private set; }

        public MultithreadEventLoopGroup Group => new MultithreadEventLoopGroup();

        /// <summary>
        /// 异步初始化交易TCP连接
        /// </summary>
        public async Task<Bootstrap> InitBootstrapAsync()
        {
            Bootstrap bootstrap = new Bootstrap();
            bootstrap
                .Group(Group)
                .Channel<TcpSocketChannel>()
                 .Option(ChannelOption.AllowHalfClosure, false)
                   .Option(ChannelOption.AutoRead, true)
                   .Option(ChannelOption.ConnectTimeout, TimeSpan.FromSeconds(10))
                   .Option(ChannelOption.SoTimeout, 10)
                   .Option(ChannelOption.SoLinger, 1)
                   .Option(ChannelOption.IpTos, 1024)
                   .Option(ChannelOption.SoBacklog, 10240)
                   .Option(ChannelOption.SoKeepalive, false)
                   .Option(ChannelOption.SoRcvbuf, 10240)
                   .Option(ChannelOption.SoReuseaddr, true)
                   .Option(ChannelOption.SoReuseport, true)
                   .Option(ChannelOption.SoSndbuf, 1024)
                   .Option(ChannelOption.TcpNodelay, true)
                   .Option(ChannelOption.WriteSpinCount, 10)
                .Handler(new ActionChannelInitializer<ISocketChannel>(channel =>
                {
                    IChannelPipeline pipeline = channel.Pipeline;
                    // pipeline.AddLast(new LoggingHandler());

                    // pipeline.AddLast("framing-Encoder", new LengthFieldPrepender(4));
                    // pipeline.AddLast("framing-Decoder", new LengthFieldBasedFrameDecoder(int.MaxValue, 0, 4, 0, 4));
                    // pipeline.AddLast(new DelimiterBasedFrameDecoder(8192, Delimiters.LineDelimiter()));

                    pipeline.AddLast("Framing-String-Encoder", new StringEncoder(Encoding.UTF8));
                    pipeline.AddLast("Framing-String-Decoder", new StringDecoder(Encoding.UTF8));

                    pipeline.AddLast("TradeClientHandler", new ClientHandler(WhenChannelActive));
                }));
            try
            {
                ChatChannel = await bootstrap.ConnectAsync(new IPEndPoint(Host, Port));
            }
            catch (Exception ex)
            {
                await InitBootstrapAsync().ContinueWith(x =>
                {
                    if (x.IsFaulted)
                    {
                        string msg = $"无法连接到服务器({ex.Message}{Environment.NewLine}{ex.InnerException.Message})";
                        if (x.Exception != null)
                        {
                            CoreDispatcher.Dispatcher.ExceptionLog(x.Exception);
                        }
                    }
                    else if (x.IsCompleted)
                    {
                        if (ChatChannel.Active)
                        {
                        }
                    }
                });
            }
            return bootstrap;
        }
        /// <summary>
        /// 当激活通道后，开始定时发送心跳
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public string WhenChannelActive(IChannelHandlerContext context)
        {

            return context.Name;
        }

        /// <summary>
        /// 发送消息
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public async Task SendAsync(object message)
        {
            string json = message.ToString();
            RequestCommand requestCmd = message as RequestCommand;
            Tuple<int, int> cmd = Tuple.Create(requestCmd.Scope, requestCmd.Cmd);
            try
            {
                if (ChatChannel == null || ChatChannel.Active == false)
                {
                    await InitBootstrapAsync().ContinueWith(x =>
                    {
                        if (x.IsFaulted)
                        {
                            if (x.Exception != null)
                            {
                                x.Exception.Handle((e) =>
                                {
                                    CoreDispatcher.Dispatcher.ExceptionLog(e);
                                    return true;
                                });
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                await CoreDispatcher.Dispatcher.ExceptionLog(ex);
            }
            finally
            {
                if (ChatChannel != null && ChatChannel.Active == true)
                {
                    await ChatChannel.WriteAndFlushAsync(json).ContinueWith(x =>
                    {
                        if (x.IsCompleted)
                        {
                        }
                    });
                }
            }
        }

        public event EventHandler<DisConnectedArgs> DisConnected;
        public event EventHandler<DisConnectedArgs> RetryConnect;

        /// <summary>
        /// 与服务端失联后，触发
        /// </summary>
        /// <param name="context"></param>
        public void OnChannelInactiveHandler(IChannelHandlerContext context)
        {
            DisConnectedArgs args = new DisConnectedArgs();
            args.EndPoint = context.Channel.RemoteAddress;
            args.ContextId = context.Channel.Id.AsLongText();
            DisConnected?.Invoke(this, args);
            RetryConnect(this, args);
        }
        public override string ToString() => JsonConvert.SerializeObject(new { Group });
    }

}
