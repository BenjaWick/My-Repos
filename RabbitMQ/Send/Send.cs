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
                    channel.QueueDeclare(queue: "hola", durable:false, exclusive:false, autoDelete:false, arguments:null);

                    var message = @"{'Tittle':'Prueba 1','Description':'01','ChannelId':'UCpjuL28buAOG0tSVcmMuwFg','Video':'Pio.mp4'}";//new {Tittle ="", Description ="", ChannelId = "UCpjuL28buAOG0tSVcmMuwFg", Video = "Pio.mp4" };//"Pio.mp4";
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "", routingKey: "hola", basicProperties:null, body:body);
                    Console.WriteLine($"[x] Sent {message}");
                }
            }

            Console.WriteLine("Presiona cualquier tecla para salir...");
            Console.ReadLine();
        }
    }
}