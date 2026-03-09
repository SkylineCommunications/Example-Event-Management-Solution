namespace ExampleEventManager_ConfigureUDAPIToken
{
	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.Utils.InteractiveAutomationScript;

	internal class TokenDialog : Dialog
	{
		public TokenDialog(IEngine engine, string secret) : base(engine)
		{
			Title = "Token";

			AddWidget(new TextBox(secret), 0, 0, 1, 2);
			AddWidget(new Label("For security reasons, this secret will only be shown now. You will not be able to retrieve it again when you have closed this window."), 1, 0, 1, 2);
			AddWidget(CloseButton, 3, 0, 1, 2);
		}

		public Button CloseButton { get; } = new Button("Close")
		{
			Style = ButtonStyle.CallToAction,
		};
	}
}
