using Amazon.SQS;
using Amazon.SQS.Model;
using System;
using System.Threading.Tasks;

public class SqsService
{
    private readonly AmazonSQSClient _sqsClient;
    private readonly string _queueUrl = "https://sqs.us-east-1.amazonaws.com/541962714297/success-queue";

    public SqsService()
    {
        _sqsClient = new AmazonSQSClient();
    }

    public async Task<string?> FetchMessageAsync()
    {
        var request = new ReceiveMessageRequest
        {
            QueueUrl = _queueUrl,
            MaxNumberOfMessages = 1,
            WaitTimeSeconds = 5 // Enable long polling
        };

        var response = await _sqsClient.ReceiveMessageAsync(request);

        if (response.Messages.Count > 0)
        {
            Console.WriteLine($"Found {response.Messages.Count()} messages in the queue");
            var message = response.Messages[0];
            Console.WriteLine($"Message fetched, now deleting it from the queue");
            // Delete the message after processing
            await _sqsClient.DeleteMessageAsync(new DeleteMessageRequest
            {
                QueueUrl = _queueUrl,
                ReceiptHandle = message.ReceiptHandle
            });

            return message.Body;
        }

        return null;
    }
}
