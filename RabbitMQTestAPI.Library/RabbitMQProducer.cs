using RabbitMQ.Client;
using System.Text;

namespace RabbitMQTestAPI.Library
{
    public class RabbitMQProducer
    {
        private readonly string _exchangeName;
        private readonly string _routingKey;

        public RabbitMQProducer(string exchangeName, string routingKey)
        {
            _exchangeName = exchangeName;
            _routingKey = routingKey;
        }

        public void SendMessage(Mail mail)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" }; // set the host name accordingly
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct);

                byte[] message = Encoding.UTF8.GetBytes(mail.Serialize()); // serialize the message body

                channel.BasicPublish(exchange: _exchangeName, routingKey: _routingKey, basicProperties: null, body: message);

                Console.WriteLine("Message sent to RabbitMQ.");
            }
        }
    }
}