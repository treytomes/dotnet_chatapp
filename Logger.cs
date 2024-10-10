using Amazon.BedrockRuntime.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace chatapp
{
	internal static class Logger
	{
		public static void Log(ConverseStreamMetadataEvent e)
		{
			Console.WriteLine("ConverseStreamMetadataEvent:");
			Console.WriteLine($"\tMetrics: {e.Metrics}");
			Console.WriteLine($"\tTrace: {e.Trace}");
			Console.WriteLine($"\tUsage: {e.Usage}");
		}
	}
}
