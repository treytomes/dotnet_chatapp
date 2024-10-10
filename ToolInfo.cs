using Amazon.BedrockRuntime.Model;

namespace chatapp
{
	internal class ToolInfo
	{
		private Func<object, object> invoke;

		private ToolInfo(ToolSpecification toolSpec, Type requestType, Func<object, object> invoke)
		{
			Tool = new Tool()
			{
				ToolSpec = toolSpec
			};
			RequestType = requestType;
			this.invoke = invoke;
		}

		public static ToolInfo Create<TRequest, TResponse>(Func<TRequest, TResponse> invoke)
		{
			Func<object, object> invokeWrapper = (object request) => invoke((TRequest)request)!;
			return new ToolInfo(ToolHelpers.GenerateToolSpec<TRequest>(), typeof(TRequest), invokeWrapper));
		}

		public string Name
		{
			get
			{
				return Tool.ToolSpec.Name;
			}
		}

		public Tool Tool { get; private set; }

		public Type RequestType { get; private set; }

		public object Invoke(object request)
		{
			return invoke(request);
		}
	}
}
