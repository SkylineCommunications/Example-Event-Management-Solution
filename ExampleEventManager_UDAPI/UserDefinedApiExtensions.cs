namespace ExampleEventManager_UDAPI
{
	using System;
	using System.Runtime.CompilerServices;
	using Microsoft.Extensions.DependencyInjection;
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.SDM;
	using Skyline.DataMiner.SDM.UserDefinedApi;
	using Skyline.DataMiner.SDM.UserDefinedApi.DI;
    using Skyline.DataMiner.Utils.Examples.EventManagement.ApiHelpers;
    using Skyline.DataMiner.Utils.Examples.EventManagement.Models;

    internal static class UserDefinedApiExtensions
	{
		public static UserDefinedApi.UserDefinedApiBuilder AddServices(this UserDefinedApi.UserDefinedApiBuilder builder)
		{
			if (builder is null)
			{
				throw new ArgumentNullException(nameof(builder));
			}

			// This ensures that static constructors are called and exposers are registered.
			RuntimeHelpers.RunClassConstructor(typeof(EventExposers).TypeHandle);
			RuntimeHelpers.RunClassConstructor(typeof(EventExposers.Languages).TypeHandle);
			// Register repositories.
			return builder
 					.AddRepository<Event, IBulkRepository<Event>>(
 						sp => sp.GetRequiredService<IAccessor<IEngine>>().Value.GetEventApiHelper().Events);
		}

		private static IEventApiHelper GetEventApiHelper(this IEngine engine)
		{
			if (engine is null)
			{
				throw new ArgumentNullException(nameof(engine), "Engine cannot be null.");
			}

			return new EventApiHelper(engine.GetUserConnection());
		}
	}
}
