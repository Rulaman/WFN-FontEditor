using AGS.Types;

namespace AGS.Plugin.FontEditor
{
	[RequiredAGSVersion("3.0.0.0")]
	public class PluginLoader: IAGSEditorPlugin
	{
		public PluginLoader(IAGSEditor editor)
		{
			editor.AddComponent(new Component(editor));
		}

		public void Dispose()
		{
		}
	}
}
