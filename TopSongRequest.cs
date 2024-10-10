using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace chatapp
{
	[Description("Get the most popular song played on a radio station.")]
	[DisplayName("top_song")]
	internal class TopSongRequest
	{
		[Description("The call sign for the radio station for which you want the most popular song. Example calls signs are WZPZ and WKRP.")]
		[Required]
		public required string sign { get; set; }
	}
}
