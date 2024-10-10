using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace chatapp
{
	[Description("Answer questions about the changes made to the latest version of the .NET framework.")]
	[DisplayName("dotnet_info")]
	internal class DotNetInfoRequest
	{
		[Description("The root question that is needing to be answered.")]
		[Required]
		public required string question { get; set; }
	}
}
