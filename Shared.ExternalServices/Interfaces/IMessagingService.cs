using Shared.ExternalServices.DTOs;

namespace Shared.ExternalServices.Interfaces
{
    public interface IMessagingService
    {
        /// <summary>
        /// To Publish a message to Azure Service Bus Topic
        /// </summary>
        /// <param name="message">The message Object to Publish</param>
        /// <param name="subscriberName">The name of the Subscriber that can be used to read Published Topic messages.</param>
        /// <returns></returns>
        Task PublishTopicMessage(dynamic message, string subscriberName);

        /// <summary>
        /// To Publish a list of messages to Azure Service Bus
        /// </summary>
        /// <param name="messages">The List of messages to publish. These messages must be in a Json Serialized form</param>
        /// <param name="subscriberName">The name of the Subscriber that can be used to read Published Topic messages.</param>
        /// <returns></returns>
        Task PublishTopicMessage(List<string> messages, string subscriberName);
    }
}
