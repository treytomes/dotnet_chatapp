namespace chatapp
{
	internal class VectorTextResponse
	{
		public VectorTextResponse(string text, string metadata)
		{
			this.text = text;
			this.metadata = metadata;
		}

		public string text { get; set; }
		public string metadata { get; set; }
	}
}
