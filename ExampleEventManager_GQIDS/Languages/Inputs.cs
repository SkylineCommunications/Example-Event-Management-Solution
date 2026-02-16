namespace ExampleEventManager_GQIDs.Languages
{
	using System;

	using Skyline.DataMiner.Analytics.GenericInterface;

	internal class Inputs
	{
		private readonly GQIStringArgument _IdentiferArg = new GQIStringArgument("Identifier")
		{
			IsRequired = true,
		};

		public string Identifier { get; private set; } = String.Empty;

		internal GQIArgument[] GetArguments() => new GQIArgument[]
		{
 			_IdentiferArg,
		};

		internal void Process(OnArgumentsProcessedInputArgs args)
		{
			if (args.TryGetArgumentValue(_IdentiferArg, out string identifier))
			{
				Identifier = identifier;
			}
		}

		internal bool Validate() => Guid.TryParse(Identifier, out Guid result);
	}
}
