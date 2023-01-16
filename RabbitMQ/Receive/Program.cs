using System;
using RabbitMQ.Client;
using System.Text;
using RabbitMQ;
using RabbitMQ.Client.Events;

namespace program
{
    class program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory() {HostName = "localhost"};

            using(var connection = factory.CreateConnection())
            {
                using(var channel = connection.CreateModel())
                {
                   channel.QueueDeclare(queue:"hola", durable:false, exclusive:false, autoDelete:false, arguments:null);

                   var consumer = new EventingBasicConsumer(channel);

                   consumer.Received += (model,ea) =>
                   {
                     var body = ea.Body.ToArray();
                     var message = Encoding.UTF8.GetString(body);

                     Console.WriteLine($"[x] Received {message}");
                   };

                   channel.BasicConsume(queue:"hola", autoAck:true, consumer:consumer);

                   Console.WriteLine("Presiona cualquier letra para salir...");
                   Console.ReadLine();

                }
            }
        }
        
    }
}
