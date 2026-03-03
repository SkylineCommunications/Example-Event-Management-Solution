namespace ExampleEventManager_GQIDs.Events
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using GQI_Shared;
    using Skyline.DataMiner.Analytics.GenericInterface;
    using Skyline.DataMiner.Analytics.GenericInterface.Operators;
    using Skyline.DataMiner.Learning.EventManagement.ApiHelpers;
    using Skyline.DataMiner.Learning.EventManagement.Models;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM.UserDefinedApi.OData;
    using SLDataGateway.API.Querying;

    /// <summary>
    /// Represents a data source.
    /// See: https://aka.dataminer.services/gqi-external-data-source for a complete example.
    /// </summary>
    [GQIMetaData(Name = "Events.Get Events")]
    public sealed class GetEvents : IGQIDataSource
         , IGQIOnInit
        , IGQIInputArguments
         , IGQIOptimizableDataSource
         , IGQIOnPrepareFetch
    {
        private GQIDMS _dms;
        private Columns _columns;
        private EventApiHelper _eventApiHelper;
        private IGQIUpdater _updater;
        private IGQILogger _logger;
        private Inputs _inputs;
        private IGQISortOperator _sortOperator;
        private GQIPageEnumerator _pageEnumerator;

        /// <summary>
        /// Initializes internal components and dependencies using the provided input arguments.
        /// </summary>
        /// <param name="args">Input arguments containing required services and configuration.</param>
        /// <returns>An output argument object representing the result of the initialization.</returns>
        public OnInitOutputArgs OnInit(OnInitInputArgs args)
        {
            _dms = args.DMS;
            _logger = args.Logger;
            _columns = new Columns();
            _inputs = new Inputs();
            _eventApiHelper = new EventApiHelper(_dms.GetConnection());
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
        /// Optimizes the query node by handling sort operators or appending the next operator.
        /// </summary>
        /// <param name="currentNode">The current query node to optimize.</param>
        /// <param name="nextOperator">The next core operator to apply.</param>
        /// <returns>The optimized query node.</returns>
        public IGQIQueryNode Optimize(IGQIDataSourceNode currentNode, IGQICoreOperator nextOperator)
        {
            if (nextOperator.IsSortOperator(out var sortOperator))
            {
                _sortOperator = sortOperator;
                return currentNode;
            }

            return currentNode.Append(nextOperator);
        }

        /// <summary>
        /// Prepares and executes a fetch operation for events, applying filters and sorting as specified.
        /// </summary>
        /// <param name="args">Input arguments containing filter and sorting information.</param>
        /// <returns>An output argument containing the results of the fetch operation.</returns>
        public OnPrepareFetchOutputArgs OnPrepareFetch(OnPrepareFetchInputArgs args)
        {
            var filter = new TRUEFilterElement<Event>().ToQuery();

            if (_inputs.FilterRequest != String.Empty)
            {
                // This ensures that static constructors are called and exposers are registered.
                RuntimeHelpers.RunClassConstructor(typeof(EventExposers).TypeHandle);
                RuntimeHelpers.RunClassConstructor(typeof(EventExposers.Languages).TypeHandle);
                var translator = new ODataSdmTranslator<Event>();
                filter = translator.TranslateFilter(_inputs.FilterRequest).ToQuery();
            }

            foreach (var sortField in _sortOperator?.Fields ?? Enumerable.Empty<IGQISortField>())
            {
                filter = _columns.ApplySorting(filter, sortField);
            }

            _pageEnumerator = new GQIPageEnumerator(_eventApiHelper.Events
                .ReadPaged(filter, 100)
                .SelectMany(page => page.Select(CreateGQIRow)));

            return default;
        }

        /// <summary>
        /// Retrieves the next page of results from the enumerator.
        /// </summary>
        /// <param name="args">Input arguments for retrieving the next page.</param>
        /// <returns>The next page of results.</returns>
        public GQIPage GetNextPage(GetNextPageInputArgs args)
        {
            return _pageEnumerator.GetNextPage(100);
        }

        private GQIRow CreateGQIRow(Event empEvent)
        {
            return new GQIRow(empEvent.Identifier, new[]
            {
                 new GQICell { Value = empEvent.Identifier },
                 new GQICell { Value = empEvent.Name },
                new GQICell { Value = empEvent.Description },
                new GQICell { Value = empEvent.Start.ToUniversalTime() },
                new GQICell { Value = empEvent.End.ToUniversalTime() },
                new GQICell { Value = (int) empEvent.Type, DisplayValue = empEvent.Type.ToString() },
                new GQICell { Value = (int) empEvent.Status, DisplayValue = empEvent.Status.ToString() },
             });
        }

        /// <summary>
        /// Retrieves the collection of input arguments.
        /// </summary>
        /// <returns>An array of input arguments.</returns>
        public GQIArgument[] GetInputArguments()
        {
            return _inputs.GetArguments();
        }

        /// <summary>
        /// Processes the specified input arguments and returns the result.
        /// </summary>
        /// <param name="args">The input arguments to process.</param>
        /// <returns>The result of processing the input arguments.</returns>
        public OnArgumentsProcessedOutputArgs OnArgumentsProcessed(OnArgumentsProcessedInputArgs args)
        {
            _inputs.Process(args);
            return default;
        }
    }
}
