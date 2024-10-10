using Amazon.BedrockRuntime;

namespace chatapp
{
	internal class PromptResult
	{
		public PromptResult(StopReason stopReason, string text, string toolName, string toolUseId, string tool)
		{
			StopReason = stopReason;
			Text = text;
			ToolName = toolName;
			ToolUseId = toolUseId;
			Tool = tool;
		}

		public StopReason StopReason { get; set; }
		public string Text { get; set; }
		public string ToolName { get; set; }
		public string ToolUseId { get; set; }
		public string Tool { get; set; }
	}
}
