namespace ExampleEventManager_DownloadFile
{
	using System.IO;
	using System.Runtime.Remoting.Contexts;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	/// <summary>
	/// Represents a dialog that displays a button for downloading a file.
	/// </summary>
	internal class DownloadButtonDialog : Dialog
	{
		/// <summary>
		/// Initializes a new instance of the DownloadButtonDialog class, configuring the download button and dialog
		/// title based on the provided engine parameters.
		/// </summary>
		/// <param name="engine">The engine used to retrieve script parameters for configuring the dialog.</param>
		public DownloadButtonDialog(IEngine engine) : base(engine)
		{
			var context = new ScriptContext(engine);
			string fileName = Path.GetFileName(context.FilePath);

			DownloadButton.DownloadedFileName = fileName;
			DownloadButton.RemoteFilePath = context.FilePath;

			Title = "Download file";

			AddWidget(DownloadButton, 1, 0);
		}

		/// <summary>
		/// Gets the download button control.
		/// </summary>
		public DownloadButton DownloadButton { get; } = new DownloadButton
		{
			StartDownloadImmediately = false,
			Style = ButtonStyle.CallToAction,
			Text = "Download",
		};
	}
}
