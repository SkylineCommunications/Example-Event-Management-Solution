namespace ExampleEventManager_UploadFile.View
{
	using System.Collections.Generic;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	internal class UploadFileDialog : Dialog
	{
		public UploadFileDialog(IEngine engine) : base(engine)
		{
			Title = "Upload file";
			AddWidget(FileSelector, 0, 0, 1, 3);
			AddWidget(OkButton, 2, 2);
			AddWidget(CancelButton, 2, 0);
		}

		public FileSelector FileSelector { get; } = new FileSelector()
		{
			AllowedFileNameExtensions = new List<string> { ".docx" },
			AllowMultipleFiles = false,
			MaxFileSizeInBytes = 15 * 1024 * 1024, // 15 MB
			Tooltip = "Please upload a .docx file smaller than 15 MB.",
			PlaceHolder = "Please upload an event form document.",
		};

		public Button OkButton { get; } = new Button()
		{
			Text = "Ok",
			Style = ButtonStyle.CallToAction
		};

		public Button CancelButton { get; } = new Button()
		{
			Text = "Cancel",
			Style = ButtonStyle.None
		};
	}
}
