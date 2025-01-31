using RabbitMQ.Client;
using RelayPulse.Core;

namespace RelayPulse.RabbitMQ.Subscribers;

internal sealed class QueueSettingsValidator
{
    public void Validate(IQueueSettings settings)
    {
        if (settings.Queues == null || settings.Queues.Length == 0)
        {
            throw new RelayPulseException("No queues settings provided");
        }

        foreach (var queue in settings.Queues)
        {
           Validate(settings, queue);
        }
    }

    private static readonly string[] ExchangeTypesSupported = [RabbitMQ.ExchangeTypesSupported.Fanout,
        RabbitMQ.ExchangeTypesSupported.Direct,
        RabbitMQ.ExchangeTypesSupported.Topic,
        RabbitMQ.ExchangeTypesSupported.Headers];
    private void Validate(IQueueSettings settings, QueueSettings queue)
    {
        var exchangeName = queue.Exchange.EmptyAlternative(settings.DefaultExchange);

        if (string.IsNullOrWhiteSpace(exchangeName))
        {
            throw new RelayPulseException(
                $"Exchange name cannot be empty. Provide an exchange name for the queue {queue.Name}");
        }

        var exchangeType = queue.ExchangeType.TryPickNonEmpty(settings.DefaultExchangeType);

        if (string.IsNullOrWhiteSpace(exchangeType))
        {
            throw new RelayPulseException($"Exchange type cannot be empty. Provide an exchange type for queue {queue.Name}");
        }

        if (ExchangeTypesSupported.All(xc => xc != exchangeType))
        {
            throw new RelayPulseException(
                $"Exchange type not valid. Provide a valid exchange type value. Supported values are {string.Join(", ",ExchangeTypesSupported)}");
        }

        if (exchangeType.IsSame(ExchangeType.Fanout))
        {
            if (queue.Bindings is { Length: > 0 })
            {
                throw new RelayPulseException($"Bindings not supported for exchange type {exchangeType}");
            }
        }

        if (!queue.DeadLetterDisabled)
        {
            var deadLetterExchangeType =
                queue.DeadLetterExchangeType.EmptyAlternative(settings.DefaultDeadLetterExchangeType ?? RabbitMQ.ExchangeTypesSupported.Direct);

            if (deadLetterExchangeType != RabbitMQ.ExchangeTypesSupported.Direct
                && deadLetterExchangeType != RabbitMQ.ExchangeTypesSupported.Topic)
            {
                throw new RelayPulseException(
                    $"Only direct or topic is supported for dead letter exchange type");
            }
            
        
            if (!queue.RetryDisabled)
            {
                var retryExchangeType =
                    queue.RetryExchangeType.EmptyAlternative(settings.DefaultRetryExchangeType ?? RabbitMQ.ExchangeTypesSupported.Direct);

                if (retryExchangeType != RabbitMQ.ExchangeTypesSupported.Direct
                    && retryExchangeType != RabbitMQ.ExchangeTypesSupported.Topic)
                {
                    throw new RelayPulseException(
                        $"Only direct or topic is supported for retry exchange type");
                }
            }
        }
    }
}