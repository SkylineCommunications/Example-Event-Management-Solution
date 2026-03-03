namespace ExampleEventManager_GQIDs.Languages
{
	using System.Collections.Generic;
	using System.Linq;
	using GQI_Shared;
	using Skyline.DataMiner.Analytics.GenericInterface;
    using Skyline.DataMiner.Analytics.GenericInterface.Operators;
    using Skyline.DataMiner.Learning.EventManagement.ApiHelpers;
    using Skyline.DataMiner.Learning.EventManagement.Models;
    using Skyline.DataMiner.Net.Jobs;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.SDM;
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

		/// <summary>
		/// Initializes internal components and dependencies using the provided input arguments.
		/// </summary>
		/// <param name="args">Input arguments containing configuration and dependencies.</param>
		/// <returns>An output argument object representing the result of the initialization.</returns>
		public OnInitOutputArgs OnInit(OnInitInputArgs args)
		{
			_dms = args.DMS;
			_columns = new Columns();
			_inputs = new Inputs();
			_logger = args.Logger;	
			_eventApiHelper = new EventApiHelper(_dms.GetConnection());
			return default;
		}

		/// <summary>
		/// Retrieves the collection of input arguments.
		/// </summary>
		/// <returns>An array of GQIArgument representing the input arguments.</returns>
		public GQIArgument[] GetInputArguments()
		{
			return _inputs.GetArguments();
		}

		/// <summary>
		/// Processes the provided arguments and returns the result.
		/// </summary>
		/// <param name="args">The input arguments to process.</param>
		/// <returns>The result of processing the input arguments.</returns>
		public OnArgumentsProcessedOutputArgs OnArgumentsProcessed(OnArgumentsProcessedInputArgs args)
		{
			_inputs.Process(args);
			return default;
		}

		/// <summary>
		/// Retrieves the collection of columns associated with the current instance.
		/// </summary>
		/// <returns>An array of GQIColumn objects representing the columns.</returns>
		public GQIColumn[] GetColumns()
		{
			return _columns.GetColumns();
		}

		/// <summary>
		/// Prepares and fetches event data based on input arguments, applying validation, filtering, and sorting.
		/// </summary>
		/// <param name="args">Input arguments containing event identifier and related parameters.</param>
		/// <returns>An output argument containing the result of the fetch operation.</returns>
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

		/// <summary>
		/// Retrieves the next page of results from the enumerator.
		/// </summary>
		/// <param name="args">Input arguments for retrieving the next page.</param>
		/// <returns>The next page of results as a GQIPage object.</returns>
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
