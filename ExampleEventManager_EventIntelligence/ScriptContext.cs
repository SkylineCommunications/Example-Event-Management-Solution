namespace ExampleEventManager_EventIntelligence
{
	using System;
	using System.Collections.Generic;
	using System.Linq;

	using Newtonsoft.Json.Linq;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Utils.SecureCoding.SecureSerialization.Json.Newtonsoft;

	internal class ScriptContext
	{
		public ScriptContext(IEngine engine)
		{
			Engine = engine;

			ModelIdentifiers = GetScriptParam("ModelIdentifiers");
			Prompt = GetScriptParam("Prompt").Single();
		}

		public IEngine Engine { get; }

		public List<string> ModelIdentifiers { get; }

		public string Prompt { get; }

		private List<string> GetScriptParam(string name)
		{
			var rawValue = Engine.GetScriptParam(name).Value;
			if (String.IsNullOrEmpty(rawValue))
			{
				throw new ArgumentException($"Script Param '{name}' cannot be left empty.");
			}

			if (IsJsonArray(rawValue))
			{
				return SecureNewtonsoftDeserialization.DeserializeObject<List<string>>(rawValue);
			}
			else
			{
				return new List<string> { rawValue };
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
