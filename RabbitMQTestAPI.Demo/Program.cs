using RabbitMQTestAPI.Library;

class Program
{
    static void Main(string[] args)
    {
        var producer = new RabbitMQProducer("mailExchange", "mailRoutingKey");
        var consumer = new RabbitMQConsumer("mailExchange", "mailQueue", "mailRoutingKey");

        var message = new Mail
        {
            Recipient = "janbzdyl@gmail.com",
            Subject = "Hello",
            Body = "This is a test mail",
            Type = MessageType.SMTP
        };

        producer.SendMessage(message);
        consumer.StartConsuming();
    }
}
