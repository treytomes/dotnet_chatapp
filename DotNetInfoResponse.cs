namespace chatapp
{
	internal class DotNetInfoResponse
	{
		public DotNetInfoResponse(IEnumerable<VectorTextResponse> responses)
		{
			this.responses = responses.ToArray();
		}

		public VectorTextResponse[] responses { get; }
	}
}
