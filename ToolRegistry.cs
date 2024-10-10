using Amazon.BedrockRuntime.Model;
using Amazon.BedrockRuntime;
using Amazon.Runtime.Documents;

namespace chatapp
{
	internal class ToolRegistry
	{
		private List<ToolInfo> toolInfos = new List<ToolInfo>();

		public List<Tool> Tools
		{
			get
			{
				return toolInfos.Select(x => x.Tool).ToList();
			}
		}

		public void Register<TToolRequest, TToolResponse>(Func<TToolRequest, TToolResponse> invoke)
		{
			toolInfos.Add(ToolInfo.Create(invoke));
		}

		public ToolUseBlock ParseToolUse(string toolName, string toolUseId, string payload)
		{
			foreach (var toolInfo in toolInfos)
			{
				if (toolInfo.Name == toolName)
				{
					var toolData = ToolHelpers.ParseToolRequest(payload, toolInfo.RequestType);
					var toolResponse = new ToolUseBlock
					{
						ToolUseId = toolUseId,
						Name = toolName,
						Input = Document.FromObject(toolData),
					};
					return toolResponse;
				}
			}

			throw new Exception($"Unknown tool: {toolName}, {payload}");
		}

		public ToolResultBlock InvokeTool(string toolName, string toolUseId, string payload)
		{
			foreach (var toolInfo in toolInfos)
			{
				if (toolInfo.Name == toolName)
				{
					var toolData = ToolHelpers.ParseToolRequest(payload, toolInfo.RequestType);
					var toolResult = toolInfo.Invoke(toolData);
					//Console.WriteLine($"toolData: {toolData}");
					var toolResponse = new ToolResultBlock
					{
						ToolUseId = toolUseId,
						Status = ToolResultStatus.Success,
						Content = new List<ToolResultContentBlock>()
						{
							new ToolResultContentBlock
							{
								Json = Document.FromObject(toolResult)
							}
						}
					};
					return toolResponse;
				}
			}

			throw new Exception($"Unknown tool: {toolName}, {payload}");
		}
	}
}
