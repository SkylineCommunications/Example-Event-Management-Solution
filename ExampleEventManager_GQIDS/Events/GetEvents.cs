namespace ExampleEventManager_GQIDs.Events
{
    using System;
    using System.Linq;
    using System.Runtime.CompilerServices;
    using GQI_Shared;
    using Skyline.DataMiner.Analytics.GenericInterface;
    using Skyline.DataMiner.Analytics.GenericInterface.Operators;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
    using Skyline.DataMiner.SDM.UserDefinedApi.OData;
    using Skyline.DataMiner.Utils.Examples.EventManagement.ApiHelpers;
    using Skyline.DataMiner.Utils.Examples.EventManagement.Models;
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

        public OnInitOutputArgs OnInit(OnInitInputArgs args)
        {
            _dms = args.DMS;
            _logger = args.Logger;
            _columns = new Columns();
            _inputs = new Inputs();
            _eventApiHelper = new EventApiHelper(_dms.GetConnection());
            return default;
        }

        public GQIColumn[] GetColumns()
        {
            return _columns.GetColumns();
        }

        public IGQIQueryNode Optimize(IGQIDataSourceNode currentNode, IGQICoreOperator nextOperator)
        {
            if (nextOperator.IsSortOperator(out var sortOperator))
            {
                _sortOperator = sortOperator;
                return currentNode;
            }

            return currentNode.Append(nextOperator);
        }

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

        public GQIArgument[] GetInputArguments()
        {
            return _inputs.GetArguments();
        }

        public OnArgumentsProcessedOutputArgs OnArgumentsProcessed(OnArgumentsProcessedInputArgs args)
        {
            _inputs.Process(args);
            return default;
        }
    }
}
