
namespace ExampleEventManager_DownloadFile
{
	using System;
	using System.Linq;

	using Newtonsoft.Json.Linq;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Utils.SecureCoding.SecureSerialization.Json.Newtonsoft;

	internal class ScriptContext
	{
		public ScriptContext(IEngine engine)
		{
			Engine = engine;

			FilePath = GetScriptParam("filePath").Single();
		}

		public IEngine Engine { get; }

		public string FilePath { get; }

		private string[] GetScriptParam(string name)
		{
			var rawValue = Engine.GetScriptParam(name).Value;
			if (String.IsNullOrEmpty(rawValue))
			{
				throw new ArgumentException($"Script Param '{name}' cannot be left empty.");
			}

			if (IsJsonArray(rawValue))
			{
				return SecureNewtonsoftDeserialization.DeserializeObject<string[]>(rawValue);
			}
			else
			{
				return new[] { rawValue };
			}
		}

		private static bool IsJsonArray(string json)
		{
			try
			{
				JArray.Parse(json);
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}

