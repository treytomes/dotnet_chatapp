using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using System.Text;

namespace chatapp
{
	internal class ConversationalContext
	{
		private readonly InferenceConfiguration inferenceConfig = new InferenceConfiguration()
		{
			MaxTokens = 512,
			Temperature = 0.5f,
			TopP = 0.9f
		};

		private readonly ToolRegistry Tools = new ToolRegistry();
		private readonly Conversation conversation = new Conversation();
		private readonly AmazonBedrockRuntimeClient client = Authentication.GetBedrockClient(Settings.AWS_PROFILE_NAME);

		public void RegisterTool<TToolRequest, TToolResponse>(Func<TToolRequest, TToolResponse> invoke)
		{
			Tools.Register(invoke);
		}

		public void AddUserMessage(string userMessage)
		{
			conversation.AddUserMessage(userMessage);
		}

		public async Task<PromptResult> GetResponse(string userMessage = "", bool echo = true)
		{
			// Extract and print the streamed response text in real-time.
			var sbText = new StringBuilder();
			var toolName = string.Empty;
			var toolUseId = string.Empty;
			var sbTool = new StringBuilder();
			var stopReason = StopReason.End_turn;

			// Create a request with the model ID, the user message, and an inference configuration.
			var request = CreateRequest(conversation, userMessage);

			try
			{
				// Send the request to the Bedrock Runtime and wait for the result.
				var response = await client.ConverseStreamAsync(request);

				// e.g. What is the most popular song on WZPZ?
				foreach (var chunk in response.Stream.AsEnumerable())
				{
					switch (chunk)
					{
						case ContentBlockDeltaEvent e0:
							if (e0.Delta.ToolUse != null)
							{
								sbTool.Append(e0.Delta.ToolUse.Input);
							}
							if (e0.Delta.Text != null)
							{
								sbText.Append(e0.Delta.Text);
								if (echo)
								{
									Console.Write(e0.Delta.Text);
								}
							}
							//Console.WriteLine("Content block delta:");
							//Console.WriteLine("\tTool use: {0}", e.Delta.ToolUse.Input);
							//Console.WriteLine("\tText: {0}", e.Delta.Text);
							break;
						case MessageStartEvent e1:
							//Console.WriteLine($"Message start: {e1.Role}");
							break;
						case MessageStopEvent e2:
							stopReason = e2.StopReason;
							//Console.WriteLine("Message stop reason: {0}", e.StopReason);
							//Console.WriteLine("\tAdditional fields: {0}", e.AdditionalModelResponseFields);
							break;
						case ContentBlockStartEvent e3:
							//Console.WriteLine("Content block start:");
							//Console.WriteLine($"\tBlock index: {e.ContentBlockIndex}");
							if (e3.Start.ToolUse != null)
							{
								//Console.WriteLine($"Tool use id: {e.Start.ToolUse.ToolUseId}");
								//Console.WriteLine($"Tool name: {e.Start.ToolUse.Name}");
								toolName = e3.Start.ToolUse.Name;
								toolUseId = e3.Start.ToolUse.ToolUseId;
							}
							break;
						case ContentBlockStopEvent e4:
							//Console.WriteLine("Content block stop:");
							//Console.WriteLine($"\tBlock index: {e.ContentBlockIndex}");
							break;
						case ConverseStreamMetadataEvent e5:
							//Logger.Log(chunk as ConverseStreamMetadataEvent);
							break;
						default:
							throw new Exception($"Unknown chunk type: {chunk.GetType().Name}");
					}
				}
				Console.WriteLine();

				//Console.WriteLine($"stopReason: {stopReason}");
				if (sbText.Length > 0)
				{
					conversation.AddAssistantMessage(sbText.ToString());
				}
				if (sbTool.Length > 0)
				{
					//Console.WriteLine($"tool: {toolName} = {sbTool}");
					conversation.AddToolUse(Tools.ParseToolUse(toolName, toolUseId, sbTool.ToString()));
					var toolResult = Tools.InvokeTool(toolName, toolUseId, sbTool.ToString());
					conversation.AddToolResult(toolResult);

					// Shove the tool result into the conversation and keep going.
					return await GetResponse();
				}
			}
			catch (AmazonBedrockRuntimeException e)
			{
				Console.WriteLine($"ERROR: Can't invoke '{Settings.MODEL_ID}'. Reason: {e.Message}");
				throw;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
			}

			Console.WriteLine();

			return new PromptResult(stopReason, sbText.ToString(), toolName, toolUseId, sbTool.ToString());
		}

		private ConverseStreamRequest CreateRequest(Conversation conversation, string userMessage = "")
		{
			if (userMessage.Length > 0)
			{
				conversation.AddUserMessage(userMessage);
			}
			return new ConverseStreamRequest
			{
				ModelId = Settings.MODEL_ID,
				Messages = conversation.messages,
				InferenceConfig = inferenceConfig,
				ToolConfig = new ToolConfiguration()
				{
					ToolChoice = new ToolChoice()
					{
						//Any = new AnyToolChoice(),
						Auto = new AutoToolChoice(),
						//Tool = new SpecificToolChoice()
						//{
						//	Name = "top_song"
						//}
					},
					Tools = Tools.Tools
				}
			};
		}
	}
}
