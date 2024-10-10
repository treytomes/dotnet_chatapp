using Amazon.BedrockRuntime.Model;
using Amazon.Runtime.Documents;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Dynamic;
using System.Reflection;
using System.Text.Json;
using System.Text;

namespace chatapp
{
	internal static class ToolHelpers
	{
		public static ToolSpecification GenerateToolSpec<TRequest>()
		{
			var requestType = typeof(TRequest);

			var propertyInfos = requestType.GetProperties();
			var requiredProperties = propertyInfos.Where(x => x.GetCustomAttribute<RequiredAttribute>() != null).Select(x => x.Name).ToArray();
			var properties = propertyInfos.Select(x => new
			{
				name = x.Name,
				type = x.PropertyType.Name,
				description = x.GetCustomAttribute<DescriptionAttribute>()!.Description,
			});

			dynamic propertyBag = new ExpandoObject();
			var propSet = (IDictionary<string, object>)propertyBag;
			foreach (var property in properties)
			{
				propSet[property.name] = new
				{
					property.type,
					property.description,
				};
			}

			return new ToolSpecification
			{
				Name = requestType.GetCustomAttribute<DisplayNameAttribute>()!.DisplayName,
				Description = requestType.GetCustomAttribute<DescriptionAttribute>()!.Description,
				InputSchema = new ToolInputSchema()
				{
					Json = Document.FromObject(new
					{
						type = "object",
						properties = propertyBag,
						required = requiredProperties,
					})
				},
			};
		}

		public static object ParseToolRequest(string toolPayload, Type payloadType)
		{
			using (var stream = new MemoryStream(Encoding.ASCII.GetBytes(toolPayload)))
			{
				var toolData = JsonSerializer.Deserialize(stream, payloadType);
				if (toolData == null)
				{
					throw new Exception("Unable to deserialize tool payload.");
				}
				return toolData;
			}
		}

		public static TToolPayload ParseToolRequest<TToolPayload>(string toolPayload)
		{
			return (TToolPayload)ParseToolRequest(toolPayload, typeof(TToolPayload));
		}
	}
}
