using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Trader.Client.Infrastucture;
using Trader.Domain.Algorithms;

namespace Trader.Client.Views
{
    /// <summary>
    ///多线程代码中的11个常见的问题 http://www.uml.org.cn/c%2B%2B/200902034.asp
    /// </summary>
    public class AbpViewer : ReactiveObject
    {
        public CountdownEvent Countdown { get; } = new CountdownEvent(100);

        public AbpViewer()
        {
            Parallel.For(1, 100, x =>
            {
                Task.Delay(1000);
                Counter = x;
            });
            DecrementAsync = async (x) =>
            {
                long result = await x;
                Counter += result;
            };
            InitRabbitMQ();
            Pipe = new Pipe(PipeOptions.Default);
        }
        long counter;
        public long Counter
        {
            get => counter;
            set => this.RaiseAndSetIfChanged(ref counter, value);
        }

        string text;
        public string Text
        {
            get => text;
            set => this.RaiseAndSetIfChanged(ref text, value);
        }
        private Pipe Pipe { get; }

        private Action<Task<long>> DecrementAsync { get; set; }

        public Command HelloCommand => new Command(() =>
                                                 {
                                                     Thread task = new Thread(() =>
                                                     {
                                                         Task.Delay(1000);
                                                         Counter = Interlocked.Increment(ref counter);
                                                         Counter = Interlocked.Read(ref counter);
                                                         Counter = Interlocked.Add(ref counter, Thread.CurrentThread.ManagedThreadId);
                                                         Counter = Interlocked.Decrement(ref counter);
                                                     });
                                                     task.Name = "T-Countdown";
                                                     task.Start();
                                                 });
        public Command VolatileCommand => new Command(() =>
        {
            Counter--;
            DecrementAsync.Invoke(Task.FromResult(Counter));
        });
        public Command ExchangeCommand => new Command(async () =>
        {
            Interlocked.MemoryBarrier();
            Counter = await Fibonacci.FibAsync(26);
            Counter = Interlocked.Exchange(ref counter, Environment.TickCount);
        });


        private void InitRabbitMQ()
        {
            ConnectionFactory factory = new ConnectionFactory()
            {
                HostName = "47.98.226.195",
                UserName = "admin",
                Password = "zxcvbnm",
                VirtualHost = "/"
            };
            IConnection connection = factory.CreateConnection();
            IModel channel = connection.CreateModel();
            channel.ExchangeDeclare(
                exchange: "cap.default.router",
                type: "topic",
                durable: true);
            channel.QueueDeclare(
                                 queue: "Abp.VNext.Hello.Cap-Queue.v1",
                                 durable: true,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: new Dictionary<string, object>() { { "x-message-ttl", 864000000 } });

            channel.QueueBind(
            queue: "Abp.VNext.Hello.Cap-Queue.v1",
            exchange: "cap.default.router",
            routingKey: "Now"
            );
            EventingBasicConsumer consumer = new EventingBasicConsumer(channel);
            string consumerTag = channel.BasicConsume(
                                          queue: "Abp.VNext.Hello.Cap-Queue.v1",
                                          autoAck: false,
                                          consumer: consumer
                                          );

            consumer.Received += (model, args) =>
            {
                ReadOnlyMemory<byte> body = args.Body;
                string message = Encoding.UTF8.GetString(body.ToArray());
                Text = message;
                channel.BasicAck(args.DeliveryTag, false);
            };
        }
    }
}
