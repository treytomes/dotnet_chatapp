namespace chatapp
{
	internal class TopSongResponse
	{
		public TopSongResponse(string song, string artist)
		{
			Song = song;
			Artist = artist;
		}

		public string Song { get; set; }
		public string Artist { get; set; }
	}
}
