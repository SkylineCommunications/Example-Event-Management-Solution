namespace ExampleEventManager_UDAPI.Controllers
{
	using System;
	using System.Linq;
	using Microsoft.Extensions.Logging;
	using Skyline.DataMiner.Net.Jobs;
	using Skyline.DataMiner.Net.Messages.SLDataGateway;
	using Skyline.DataMiner.SDM;
	using Skyline.DataMiner.SDM.UserDefinedApi;
	using Skyline.DataMiner.SDM.UserDefinedApi.OData;
	using Skyline.DataMiner.SDM.UserDefinedApi.OData.Exceptions;
    using Skyline.DataMiner.Utils.Examples.EventManager.Models;

    [ApiController]
	[Route("eventmanager/events")]
	public class EventsController : ControllerBase
	{
		private readonly ILogger<EventsController> _logger;
		private readonly IRepository<Event> _repository;
		private readonly ODataSdmTranslator<Event> _translator;

		public EventsController(
			ILogger<EventsController> logger,
			IRepository<Event> repository)
		{
			_logger = logger ?? throw new ArgumentNullException(nameof(logger));
			_repository = repository ?? throw new ArgumentNullException(nameof(repository));
			_translator = new ODataSdmTranslator<Event>();
		}

		/// <summary>
		/// Retrieves a collection of <see cref="Event"/> objects based on the specified filter and order.
		/// </summary>
		/// <param name="filter">
		/// An OData filter string used to filter the results. If not specified, all items are returned.
		/// </param>
		/// <param name="orderby">
		/// An OData orderby string used to order the results. You can specify multiple properties, e.g. "Start desc, Name asc". Default is "Start".
		/// </param>
		/// <returns>
		/// An <see cref="IApiResult"/> containing the filtered and ordered collection of <see cref="Event"/> objects,
		/// or an error result if the request is invalid or an exception occurs.
		/// </returns>
		/// <example>
		/// Retrieve all Events ordered by start ascending (default)
		/// GET /empoweverevents/events
		///
		/// Retrieve Events with a filter and custom order
		/// GET  /empoweverevents/events?filter=contains(Name,'AB')&orderby=Identifier asc
		///
		/// Retrieve Events with a filter only
		/// GET /empoweverevents/events?filter=Identifier eq '123e4567-e89b-12d3-a456-426614174000'
		/// </example>
		[HttpGet]
		public IApiResult Read(
 			[FromQuery] string filter = "",
 			[FromQuery] string orderby = "Start")
		{
			try
			{
				var query = _translator.Translate(filter, orderby);
				var result = _repository.Read(query);
				return Ok(result);
			}
			catch (ODataParseException ex)
			{
				_logger.LogError(ex, ex.Message);
				return BadRequest(new Error
				{
					Title = ex.GetType().Name,
					Details = ex.Message,
				});
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
				return StatusCode(500, new Error
				{
					Title = ex.GetType().Name,
					Details = ex.Message,
				});
			}
		}


		/// <summary>
		/// Creates a new <see cref="Event"/> object in the repository.
		/// </summary>
		/// <param name="model">
		/// The <see cref="Event"/> object to be created. Must be provided in the request body.
		/// </param>
		/// <returns>
		/// An <see cref="IApiResult"/> with status code 201 (Created) containing the created <see cref="Event"/> object,
		/// or an error result if the creation fails or validation errors occur.
		/// </returns>
		/// <example>
		/// Create a new Event
		/// POST /empoweverevents/events
		/// Content-Type: application/json
		///
		/// {
		///   "Identifier": "123e4567-e89b-12d3-a456-426614174000",
		///   "Name": "Empower 2026",
		///   ...
		/// }
		/// </example>
		[HttpPost]
		public IApiResult Create(
 			[FromBody] Event model)
		{
			try
			{
				var createdModel = _repository.Create(model);
				return StatusCode(201, createdModel);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
				return StatusCode(500, new Error
				{
					Title = ex.GetType().Name,
					Details = ex.Message,
				});
			}
		}

		/// <summary>
		/// Updates an existing <see cref="Event"/> object or creates it if it does not exist.
		/// </summary>
		/// <param name="model">
		/// The <see cref="Event"/> object to be updated or created. Must be provided in the request body.
		/// The Identifier property is used to determine if the model already exists.
		/// </param>
		/// <returns>
		/// An <see cref="IApiResult"/> with status code 200 (OK) containing the updated <see cref="Event"/> object if it existed,
		/// or status code 201 (Created) containing the newly created object if it did not exist,
		/// or an error result if the operation fails or validation errors occur.
		/// </returns>
		/// <example>
		/// Update an existing event or create if it does not exist
		/// PUT /empoweverevents/events
		/// Content-Type: application/json
		///
		/// {
		///   "Identifier": "123e4567-e89b-12d3-a456-426614174000",
		///   "Name": "Empower 2026",
		///   ...
		/// }
		/// </example>
		[HttpPut]
		public IApiResult Update(
 			[FromBody] Event model)
		{
			try
			{
				var filter = EventExposers.Identifier.Equal(model.Identifier);
				var exists = _repository.Count(filter) != 0;
				if (exists)
				{
					var updatedModel = _repository.Update(model);
					return Ok(updatedModel);
				}
				else
				{
					var createdModel = _repository.Create(model);
					return StatusCode(201, createdModel);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
				return StatusCode(500, new Error
				{
					Title = ex.GetType().Name,
					Details = ex.Message,
				});
			}
		}


		/// <summary>
		/// Deletes one or more <see cref="Event"/> objects based on the specified model identifier.
		/// </summary>
		/// <param name="modelId">
		/// The identifier of the model to delete.
		/// </param>
		/// <returns>
		/// An <see cref="IApiResult"/> with status code 204 (No Content) if the deletion was successful or if no matching models were found,
		/// or an error result if an exception occurs during the deletion process.
		/// </returns>
		/// <example>
		/// Delete an event by identifier
		/// DELETE /definitionofdone/dods?modelId=123e4567-e89b-12d3-a456-426614174000
		/// </example>
		[HttpDelete]
		public IApiResult Delete(
 			[FromQuery] string modelId)
		{
			try
			{
				FilterElement<Event> filter = EventExposers.Identifier.Equal(modelId);

				var modelRegistration = _repository.Read(filter).ToArray();
				if (modelRegistration.Length == 0)
				{
					return StatusCode(204);
				}

				// Should be fine since we expect the modelId to be unique by either identifier or name.
				// And if so we don't expect a lot of entries to be deleted at once.
				foreach (var model in modelRegistration)
				{
					_repository.Delete(model);
				}

				return StatusCode(204);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, ex.Message);
				return StatusCode(500, new Error
				{
					Title = ex.GetType().Name,
					Details = ex.Message,
				});
			}
		}
	}
}
