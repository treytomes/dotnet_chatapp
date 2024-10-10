using Amazon.BedrockRuntime;
using Amazon.BedrockRuntime.Model;
using Amazon.Runtime.Documents;

namespace chatapp
{
	internal class Conversation
	{
		public readonly List<Message> messages = new List<Message>();

		private void AddContent(ConversationRole role, ContentBlock newContent)
		{
			var lastMessage = messages.LastOrDefault();
			if (lastMessage?.Role == role)
			{
				lastMessage.Content.Add(newContent);
			}
			else
			{
				messages.Add(new Message
				{
					Role = role,
					Content = new List<ContentBlock> { newContent }
				});
			}
		}

		private void AddUserContent(ContentBlock newContent)
		{
			AddContent(ConversationRole.User, newContent);
		}

		private void AddAssistantContent(ContentBlock newContent)
		{
			AddContent(ConversationRole.Assistant, newContent);
		}

		public void AddUserMessage(string message)
		{
			var newContent = new ContentBlock { Text = message ?? " " };
			AddUserContent(newContent);
		}

		public void AddToolResult(ToolResultBlock toolResult)
		{
			var newContent = new ContentBlock
			{
				ToolResult = toolResult
			};
			AddUserContent(newContent);
		}

		public void AddAssistantMessage(string message)
		{
			var newContent = new ContentBlock
			{
				Text = message ?? " "
			};
			AddAssistantContent(newContent);
		}

		public void AddToolUse(ToolUseBlock toolUse)
		{
			var newContent = new ContentBlock
			{
				ToolUse = toolUse
			};
			AddAssistantContent(newContent);
		}
	}
}
