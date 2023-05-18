using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net.Mail;
using System.Text;

namespace RabbitMQTestAPI.Library
{
    public class RabbitMQConsumer
    {
        private readonly string _exchangeName;
        private readonly string _queueName;
        private readonly string _routingKey;

        public RabbitMQConsumer(string exchangeName, string queueName, string routingKey)
        {
            _exchangeName = exchangeName;
            _queueName = queueName;
            _routingKey = routingKey;
        }

        public void StartConsuming()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" }; // set the host name accordingly

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare(_exchangeName, ExchangeType.Direct);
                channel.QueueDeclare(_queueName, true, false, false, null);
                channel.QueueBind(_queueName, _exchangeName, _routingKey, null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    byte[] body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);

                    var mail = Mail.Deserialize(message); 

                    // handle the message based on its type and send it using the specified method
                    switch (mail.Type)
                    {
                        case MessageType.SMTP:
                            SendUsingSMTP(mail);
                            break;
                        // add other message types as per your requirements
                        default:
                            Console.WriteLine("Invalid message type.");
                            break;
                    }

                    channel.BasicAck(ea.DeliveryTag, false); // Acknowledge the message
                };

                channel.BasicConsume(_queueName, false, consumer);

                Console.WriteLine("Consumer started. Waiting for messages...");
                Console.ReadLine(); 
            }
        }

        private void SendUsingSMTP(Mail mail)
        {
            try
            {
                var smtpClient = new SmtpClient("smtp.gmail.com"); // replace with your SMTP server details, e.g. gmail

                smtpClient.EnableSsl = true;

                smtpClient.Credentials = new System.Net.NetworkCredential("username@gmail.com", "password");  // replace with your SMTP credentials

                // create MailMessage object
                var message = new MailMessage()
                {
                    From = new MailAddress("username@gmail.com"), // replace with the sender email address
                    Subject = mail.Subject,
                    Body = mail.Body,
                };

                message.To.Add(mail.Recipient); // add recipient(s)

                // send the mail
                smtpClient.Send(message);

                Console.WriteLine($"Mail sent via SMTP: \"{mail.Subject}\" to {mail.Recipient} at {DateTime.Now}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send mail via SMTP: \"{mail.Subject}\" to {mail.Recipient} at {DateTime.Now}");
            }
        }


    }

}
