/*
***********************************************
*  Copyright (c), Skyline Communications NV.  *
***********************************************

Revision History:

DATE		VERSION		AUTHOR			COMMENTS

16/02/2026	1.0.0.1		, Skyline	Initial version
****************************************************************************
*/

namespace ExampleEventManagerGQIDS
{
	using System;
	using Skyline.DataMiner.Analytics.GenericInterface;

	/// <summary>
	/// Represents a data source.
	/// See: https://aka.dataminer.services/gqi-external-data-source for a complete example.
	/// </summary>
	[GQIMetaData(Name = "ExampleEventManager_GQIDS")]
	public sealed class ExampleEventManagerGQIDS : IGQIDataSource
	{
		public GQIColumn[] GetColumns()
		{
			// Define data source columns
			// See: https://aka.dataminer.services/igqidatasource-getcolumns
			return Array.Empty<GQIColumn>();
		}

		public GQIPage GetNextPage(GetNextPageInputArgs args)
		{
			// Define data source rows
			// See: https://aka.dataminer.services/igqidatasource-getnextpage
			return new GQIPage(Array.Empty<GQIRow>())
			{
				HasNextPage = false,
			};
		}
	}
}
