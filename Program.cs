using Build5Nines.SharpVector;
using Build5Nines.SharpVector.Data;
using System.Net;
using System.Text.RegularExpressions;

// SharpVector Examples: https://github.com/Build5Nines/SharpVector

namespace chatapp
{
	internal class Program
	{
		private ConversationalContext context = new ConversationalContext();
		private BasicMemoryVectorDatabase dotNetInfoDb = new BasicMemoryVectorDatabase();

		internal Program()
		{
			context.RegisterTool((TopSongRequest request) => new TopSongResponse("My Fav Song", "The Awesome Songers"));

			context.RegisterTool((DotNetInfoRequest request) =>
			{
				const int PAGE_COUNT = 5;

				Console.WriteLine($"Searching for .NET info on: {request.question}");

				// Perform a Vector Search
				var result = dotNetInfoDb.Search(request.question, pageCount: PAGE_COUNT);

				//if (!result.IsEmpty)
				//{
				//	Console.WriteLine("Similar Text Found:");
				//	foreach (var item in result.Texts)
				//	{
				//		Console.WriteLine("{0} - {1} - {2}", item.VectorComparison, item.Metadata, item.Text);
				//	}
				//}

				if (result.IsEmpty)
				{
					return new DotNetInfoResponse(new[]
					{
						new VectorTextResponse("I don't know.", string.Empty)
					});
				}
				else
				{
					return new DotNetInfoResponse(result.Texts.Select(x => new VectorTextResponse(x.Text, x.Metadata ?? string.Empty)));
				}
			});
		}

		static async Task Main()
		{
			await (new Program()).Start();
		}

		private async Task Initialize()
		{
			// Download a document and add all of its contents to our .NET info db.
			using (HttpClient client = new())
			{
				// It's loaded full of yucky JavaScript! :-(
				var document = await client.GetStringAsync("https://devblogs.microsoft.com/dotnet/performance_improvements_in_net_7");
				document = WebUtility.HtmlDecode(Regex.Replace(document, @"<[^>]+>|&nbsp;", ""));
				
				// It's a big document; loading it all as one big text blob misses the point of vectorizing the thing.
				//vectorDb.AddText(s);

				/// Paragraph Chunking
				var loader = new TextDataLoader<int, string>(dotNetInfoDb);
				loader.AddDocument(document, new TextChunkingOptions<string>
				{
					Method = TextChunkingMethod.Paragraph,
					RetrieveMetadata = (chunk) => {
						// add some basic metadata since this can't be null
						return "{ chuckSize: \"" + chunk.Length + "\" }";
					}
				});
			}
		}

		private async Task Start()
		{
			await Initialize();

			//var input = "What has changed in .NET 7?";
			//var documents = new[]
			//{
			//	"What is an amphibian?",
			//	"Cos'è un anfibio?",
			//	"A frog is an amphibian.",
			//	"Frogs, toads, and salamanders are all examples.",
			//	"Amphibians are four-limbed and ectothermic vertebrates of the class Amphibia.",
			//	"They are four-limbed and ectothermic vertebrates.",
			//	"A frog is green.",
			//	"A tree is green.",
			//	"It's not easy bein' green.",
			//	"A dog is a mammal.",
			//	"A dog is a man's best friend.",
			//	"You ain't never had a friend like me.",
			//	"Rachel, Monica, Phoebe, Joey, Chandler, Ross",
			//};

			// Index the documents
			//foreach (var doc in documents)
			//{
			//	vectorDb.AddText(doc);
			//}

			//context.AddUserMessage("You are an AI assistant that helps people find information.");

			//// Download a document and add all of its contents to our chat
			//using (HttpClient client = new())
			//{
			//	string s = await client.GetStringAsync("https://devblogs.microsoft.com/dotnet/performance_improvements_in_net_7");
			//	s = WebUtility.HtmlDecode(Regex.Replace(s, @"<[^>]+>|&nbsp;", ""));
			//	context.AddUserMessage("Here's some additional information: " + s); // uh oh!
			//	//context.AddUserMessage("Generative Engine Optimization (GEO) is the act of optimizing web content for consumption by a large language model.");
			//}

			while (true)
			{
				var userMessage = GetUserMessage();
				var response = await context.GetResponse(userMessage);
			}
		}

		private string GetUserMessage()
		{
			while (true)
			{
				Console.Write("> ");
				var userMessage = Console.ReadLine();
				if (userMessage != null)
				{
					return userMessage;
				}
			}
		}
	}
}
