namespace ExampleEventManager_UploadFile.Controller
{
    using ExampleEventManager_UploadFile.View;
    using Skyline.DataMiner.Automation;
    using Skyline.DataMiner.Net.Helper;
    using Skyline.DataMiner.Utils.InteractiveAutomationScript;
    using System;
    using System.IO;


    /// <summary>
    /// Provides functionality to handle file uploads and manage related dialogs within the DataMiner environment.
    /// </summary>
    public class UploadController
    {
        private const string DEFAULTFOLDER = @"C:\Skyline DataMiner\Documents\DMA_COMMON_DOCUMENTS\Example Event Management\UploadedFiles\";

        private InteractiveController _controller;
        private IEngine _engine;
        private UploadFileDialog _dialog;

        /// <summary>
        /// Initializes a new instance of the UploadController class with the specified engine.
        /// </summary>
        /// <param name="engine">The engine used to initialize the controller and dialog.</param>
        public UploadController(IEngine engine)
        {
            _engine = engine;
            _controller = new InteractiveController(engine);

            _dialog = new UploadFileDialog(engine);

            _dialog.OkButton.Pressed += OkButton_Pressed;


            _controller.ShowDialog(_dialog);
        }

        private void OkButton_Pressed(object sender, EventArgs e)
        {
            try
            {
                string fileName = string.Empty;

                _dialog.FileSelector.UploadedFilePaths.ForEach(path =>
                {
                    if (File.Exists(Path.Combine(DEFAULTFOLDER, Path.GetFileName(path))))
                    {
                        File.Delete(Path.Combine(DEFAULTFOLDER, Path.GetFileName(path)));
                    }
                });

                _dialog.FileSelector.CopyUploadedFiles(DEFAULTFOLDER);

                _dialog.FileSelector.UploadedFilePaths.ForEach(path =>
                {
                    fileName = Path.GetFileName(path);
                    TryDeleteEventFolder(path);
                });
               
                if (!String.IsNullOrWhiteSpace(fileName))
                {
                    _engine.AddOrUpdateScriptOutput("DOCUMENTFILEPATH", Path.Combine(DEFAULTFOLDER, fileName));
                    _engine.Log("File uploaded successfully: " + fileName);
                }

            }
            catch (Exception ex)
            {
                _engine.Log("Error during file upload: " + ex.Message);
            }

            _engine.ExitSuccess("File upload process completed.");
        }

        private void CancelButton_Pressed(object sender, EventArgs e)
        {
            _dialog.FileSelector.UploadedFilePaths.ForEach(path => TryDeleteEventFolder(path));
            _engine.ExitSuccess("Upload canceled by user.");
        }

        private bool TryDeleteEventFolder(string fullFilePath)
        {
            try
            {
                DirectoryInfo fileDirectory = new DirectoryInfo(Path.GetDirectoryName(fullFilePath));
                DirectoryInfo eventFolder = fileDirectory?.Parent;

                if (eventFolder != null && eventFolder.Exists)
                {
                    eventFolder.Delete(true);
                    return true;
                }
            }
            catch (Exception ex)
            {
                // Log exception here if needed
               _engine.Log($"Error deleting event folder ({fullFilePath}: {ex.Message}");
            }

            return false;
        }
    }
}
