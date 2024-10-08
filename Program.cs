using Amazon;
using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using chatapp;
using System.Text;

// Set the model ID, e.g., Claude 3 Haiku.
const string MODEL_ID = "anthropic.claude-3-haiku-20240307-v1:0";
const string AWS_PROFILE_NAME = "sandbox";

var inferenceConfig = new InferenceConfiguration()
{
	MaxTokens = 512,
	Temperature = 0.5F,
	TopP = 0.9F
};

var conversation = new Conversation();

ConverseStreamRequest CreateRequest(Conversation conversation, string userMessage)
{
	conversation.AddUserMessage(userMessage);
	return new ConverseStreamRequest
	{
		ModelId = MODEL_ID,
		Messages = conversation.messages,
		InferenceConfig = inferenceConfig
	};
}

// Create a Bedrock Runtime client in the AWS Region you want to use.
var client = Authentication.GetBedrockClient(AWS_PROFILE_NAME);

// Q&A loop
while (true)
{
	Console.Write("> ");
	var userMessage = Console.ReadLine();
	if (userMessage == null)
	{
		continue;
	}

	// Create a request with the model ID, the user message, and an inference configuration.
	var request = CreateRequest(conversation, userMessage);

	try
	{
		// Send the request to the Bedrock Runtime and wait for the result.
		var response = await client.ConverseStreamAsync(request);

		// Extract and print the streamed response text in real-time.
		var sb = new StringBuilder();
		foreach (var chunk in response.Stream.AsEnumerable().OfType<ContentBlockDeltaEvent>())
		{
			sb.Append(chunk.Delta.Text);
			Console.Write(chunk.Delta.Text);
		}
		conversation.AddAssistantMessage(sb.ToString());
	}
	catch (AmazonBedrockRuntimeException e)
	{
		Console.WriteLine($"ERROR: Can't invoke '{MODEL_ID}'. Reason: {e.Message}");
		throw;
	}

	Console.WriteLine();
}
