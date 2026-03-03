namespace ExampleEventManager_UploadFile.View
{
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;
    using System.Collections.Generic;

    public class UploadFileDialog : Dialog
    {
        private FileSelector _fileSelector;

        private Button _okButton;

        private Button _cancelButton;
        
        public UploadFileDialog(IEngine engine) : base(engine)
        {

            _fileSelector = new FileSelector()
            {
                AllowedFileNameExtensions = new List<string> { ".docx" },
                AllowMultipleFiles = false,
                MaxFileSizeInBytes = 15 * 1024 * 1024, // 15 MB
                Tooltip = "Please upload a .docx file smaller than 15 MB.",
                PlaceHolder = "Please upload an event form document.",
            };

            _okButton = new Button()
            {
                Text = "Ok",
                Style = ButtonStyle.CallToAction
            };

            _cancelButton = new Button()
            {
                Text = "Cancel",
                Style = ButtonStyle.None
            };

            Title = "Upload file";
            AddWidget(_fileSelector, 0, 0, 1, 3);
            AddWidget(_okButton, 2, 2);
            AddWidget(_cancelButton, 2, 0);
        }

        public FileSelector FileSelector { get { return _fileSelector; } }

        public Button OkButton { get { return _okButton; } }

        public Button CancelButton { get { return _cancelButton; } }
    }
}
