namespace ExampleEventManager_GQIDs.Languages
{
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	using GQI_Shared;
	using Skyline.DataMiner.Analytics.GenericInterface;
    using Skyline.DataMiner.Analytics.GenericInterface.Operators;
    using Skyline.DataMiner.Net.Jobs;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.SDM;
    using Skyline.DataMiner.Utils.Examples.EventManager.ApiHelpers;
    using Skyline.DataMiner.Utils.Examples.EventManager.Models;
    using SLDataGateway.API.Querying;

	/// <summary>
	/// Represents a data source.
	/// See: https://aka.dataminer.services/gqi-external-data-source for a complete example.
	/// </summary>
	[GQIMetaData(Name = "Event.Get Languages")]
	public sealed class GetLanguages : IGQIDataSource
 		, IGQIOnInit
 		, IGQIInputArguments
 		, IGQIOnPrepareFetch
	{
		private GQIDMS _dms;
		private Columns _columns;
		private Inputs _inputs;
		private EventApiHelper _eventApiHelper;
		private IGQILogger _logger;
		private IGQISortOperator _sortOperator;
		private GQIPageEnumerator _pageEnumerator;

		public OnInitOutputArgs OnInit(OnInitInputArgs args)
		{
			_dms = args.DMS;
			_columns = new Columns();
			_inputs = new Inputs();
			_logger = args.Logger;	
			_eventApiHelper = new EventApiHelper(_dms.GetConnection());
			return default;
		}

		public GQIArgument[] GetInputArguments()
		{
			return _inputs.GetArguments();
		}

		public OnArgumentsProcessedOutputArgs OnArgumentsProcessed(OnArgumentsProcessedInputArgs args)
		{
			_inputs.Process(args);
			return default;
		}

		public GQIColumn[] GetColumns()
		{
			return _columns.GetColumns();
		}

		public OnPrepareFetchOutputArgs OnPrepareFetch(OnPrepareFetchInputArgs args)
		{
			if(_inputs.Validate() == false)
			{
				_logger.Error($"Invalid input: Identifier must be a valid GUID. Provided value: {_inputs.Identifier}");
				_pageEnumerator = new GQIPageEnumerator(new List<GQIRow>());
				return default;
			}

			var filter = new ANDFilterElement<Event>(EventExposers.Identifier.Equal(_inputs.Identifier)).ToQuery();
			foreach (var sortField in _sortOperator?.Fields ?? Enumerable.Empty<IGQISortField>())
			{
				filter = _columns.ApplySorting(filter, sortField);
			}

			// Use SelectMany to flatten the IEnumerable<IEnumerator<GQIRow>> to IEnumerable<GQIRow>
			_pageEnumerator = new GQIPageEnumerator(
 				_eventApiHelper.Events
 					.ReadPaged(filter, 100)
 					.SelectMany(page => page.SelectMany(CreateGQIRows)));

			return default;
		}

		public GQIPage GetNextPage(GetNextPageInputArgs args)
		{
			return _pageEnumerator.GetNextPage(100);
		}

		private IEnumerable<GQIRow> CreateGQIRows(Event empEvent)
		{

			foreach (var package in empEvent.Languages)
			{
				yield return _columns.CreateGQIRow(package);
			}
		}
	}
}
