namespace ExampleEventManager_GQIDs.Events
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Skyline.DataMiner.Analytics.GenericInterface;
	using Skyline.DataMiner.Analytics.GenericInterface.Operators;
    using Skyline.DataMiner.Learning.EventManagement.Models;
    using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.SDM;
    using SLDataGateway.API.Querying;
	using SLDataGateway.API.Types.Querying;

	internal class Columns
	{
		private readonly Dictionary<GQIColumn, FieldExposer> _columnMap = new Dictionary<GQIColumn, FieldExposer>
		{
			[new GQIStringColumn("Identifier")] = EventExposers.Identifier,
			[new GQIStringColumn("Name")] = EventExposers.Name,
			[new GQIStringColumn("Description")] = EventExposers.Description,
			[new GQIDateTimeColumn("Start")] = EventExposers.Start,
			[new GQIDateTimeColumn("End")] = EventExposers.End,
			[new GQIIntColumn("Type")] = EventExposers.Type,
			[new GQIIntColumn("Status")] = EventExposers.Status,
		};

		internal GQIColumn[] GetColumns() => _columnMap.Keys.ToArray();

		internal IQuery<Event> ApplySorting(FilterElement<Event> filter, IGQISortField sortField)
		{
			return ApplySorting(filter.ToQuery(), sortField);
		}

		internal IQuery<Event> ApplySorting(IQuery<Event> query, IGQISortField sortField)
		{
			if (query is null)
			{
				throw new ArgumentNullException(nameof(query));
			}

			if (sortField is null)
			{
				return query;
			}

			SortOrder sortDirection;
			switch (sortField.Direction)
			{
				case GQISortDirection.Ascending:
					sortDirection = SortOrder.Ascending;
					break;

				case GQISortDirection.Descending:
					sortDirection = SortOrder.Descending;
					break;

				default:
					throw new NotSupportedException($"The sort direction '{sortField.Direction}' is not supported.");
			}

			var exposer = _columnMap.FirstOrDefault(map => sortField.Column.Equals(map.Key)).Value;
			if (exposer is null)
			{
				return query;
			}

			var orderByElement = OrderByElementFactory.Create(exposer, sortDirection);
			if (!query.Order.Elements.Any())
			{
				return query.WithOrder(
					OrderBy.Default.SingleConcat(orderByElement));
			}
			else
			{
				return query.WithOrder(
					query.Order.SingleConcat(orderByElement));
			}
		}
	}
}
