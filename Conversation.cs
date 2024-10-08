using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;

namespace chatapp
{
	internal class Conversation
	{
		public readonly List<Message> messages = new List<Message>();

		public void AddUserMessage(string message)
		{
			messages.Add(new Message
			{
				Role = ConversationRole.User,
				Content = new List<ContentBlock> { new ContentBlock { Text = message } }
			});
		}

		public void AddAssistantMessage(string message)
		{
			messages.Add(new Message
			{
				Role = ConversationRole.Assistant,
				Content = new List<ContentBlock> { new ContentBlock { Text = message } }
			});
		}
	}
}
