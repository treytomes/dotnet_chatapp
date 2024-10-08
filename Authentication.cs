using Amazon;
using Amazon.BedrockRuntime;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;

namespace chatapp
{
	internal static class Authentication
	{
		private static readonly RegionEndpoint Region = RegionEndpoint.USEast1;

		public static AWSCredentials LoginToAWS(string profileName)
		{
			var chain = new CredentialProfileStoreChain();
			if (!chain.TryGetAWSCredentials(profileName, out AWSCredentials awsCredentials))
			{
				throw new Exception("Failed to create AWSCredentials object.");
			}
			return awsCredentials;
		}

		public static AmazonBedrockRuntimeClient GetBedrockClient(string profileName)
		{
			var creds = LoginToAWS(profileName);
			return new AmazonBedrockRuntimeClient(creds, Region);
		}
	}
}
