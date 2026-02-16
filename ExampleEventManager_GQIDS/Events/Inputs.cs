namespace ExampleEventManager_GQIDs.Events
{
	using System;

	using Skyline.DataMiner.Analytics.GenericInterface;

	internal class Inputs
	{
		private readonly GQIStringArgument _filterRequest = new GQIStringArgument("FilterRequest")
		{
			IsRequired = false,
		};

		public string FilterRequest { get; private set; } = String.Empty;

		internal GQIArgument[] GetArguments() => new GQIArgument[]
		{
			_filterRequest,
		};

		internal void Process(OnArgumentsProcessedInputArgs args)
		{
			if (args.TryGetArgumentValue(_filterRequest, out string filterrequest))
			{
				FilterRequest = filterrequest;
			}

		}

		internal bool Validate() => true;
	}
}
