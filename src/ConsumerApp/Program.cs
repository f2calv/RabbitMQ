﻿using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;
//modified from;
//https://github.com/rabbitmq/rabbitmq-tutorials/blob/master/dotnet/Receive/Receive.cs
class Receive
{
    static async Task Main()
    {
        Console.Title = AppDomain.CurrentDomain.FriendlyName;
        Console.WriteLine("Waiting for RabbitMQ to fully start...");
        await Task.Delay(15_000);//delay startup
        var factory = new ConnectionFactory() { HostName = "rabbitmq" };
        using (var connection = factory.CreateConnection())
        using (var channel = connection.CreateModel())
        {
            channel.QueueDeclare(queue: "hello", durable: false, exclusive: false, autoDelete: false, arguments: null);

            Console.WriteLine(" [*] Waiting for messages.");

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                Console.WriteLine(" [x] Received {0}", message);
            };
            while (true)
            {
                channel.BasicConsume(queue: "hello", autoAck: true, consumer: consumer);
                await Task.Delay(5_000);//delay
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }
    }
}