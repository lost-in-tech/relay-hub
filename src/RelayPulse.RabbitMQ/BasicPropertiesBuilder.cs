using RabbitMQ.Client;
using RelayPulse.Core;

namespace RelayPulse.RabbitMQ;

internal sealed class BasicPropertiesBuilder(IUniqueId uniqueId)
{
    public IBasicProperties Build<T>(IModel channel, Message<T> msg)
    {
        var prop = channel.CreateBasicProperties();

        prop.ContentEncoding = "utf-8";

        if (!string.IsNullOrWhiteSpace(msg.Type))
        {
            prop.Type = msg.Type;
        }

        if (!string.IsNullOrWhiteSpace(msg.AppId))
        {
            prop.AppId = msg.AppId;
        }

        if (!string.IsNullOrWhiteSpace(msg.UserId))
        {
            prop.UserId = msg.UserId;
        }

        if (!string.IsNullOrWhiteSpace(msg.Cid))
        {
            prop.CorrelationId = msg.Cid;
        }

        prop.MessageId = msg.Id ?? uniqueId.New().ToString();

        if (msg.Headers.Count > 0)
        {
            prop.Headers ??= new Dictionary<string, object>();

            foreach (var header in msg.Headers)
            {
                prop.Headers[header.Key] = header.Value;
            }
        }

        return prop;
    }
}