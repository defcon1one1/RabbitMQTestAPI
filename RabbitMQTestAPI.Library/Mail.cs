using System.Text.Json;

namespace RabbitMQTestAPI.Library
{

    public enum MessageType
    {
        SMTP,
        // add other types as per your requirements
    }

    public class Mail
    {
        public string Recipient { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public MessageType Type { get; set; }

        public string Serialize()
        {
            return JsonSerializer.Serialize(this);
        }

        public static Mail Deserialize(string json)
        {
            return JsonSerializer.Deserialize<Mail>(json);
        }
    }

}
