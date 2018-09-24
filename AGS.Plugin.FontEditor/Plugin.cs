using System.Collections.Generic;
using AGS.Types;

namespace AGS.Plugin.FontEditor
{
	public class Component: IEditorComponent
	{
		private const string COMPONENT_ID = "FontEditorComponent";
		private const string CONTROL_ID_CONTEXT_MENU_OPTION = "FontEditorOptionClick";
		private const string CONTROL_ID_ROOT_NODE = "FontEditorRoot";
		private const string CONTROL_ID_MAIN_MENU_OPTION = "FontEditorMainMenuOptionClick";
		private const string NEW_MAIN_MENU_ID = "FontEditorMenu";

		private IAGSEditor _editor;
		private ContentDocument _pane;
		private FontEditor LocalFontEditor;

		public Component(IAGSEditor editor)
		{
			_editor = editor;
			_editor.GUIController.RegisterIcon("FontEditorIcon", Properties.Resources.FontEditor);
			_editor.GUIController.ProjectTree.AddTreeRoot(this, CONTROL_ID_ROOT_NODE, "FontEditor", "FontEditorIcon");

			LocalFontEditor = new FontEditor(editor);
			_pane = new ContentDocument(LocalFontEditor, "FontEditor", this);
		}
		string IEditorComponent.ComponentID
		{
			get { return COMPONENT_ID; }
		}
		IList<MenuCommand> IEditorComponent.GetContextMenu(string controlID)
		{
			List<MenuCommand> contextMenu = new List<MenuCommand>();
			contextMenu.Add(new MenuCommand(CONTROL_ID_CONTEXT_MENU_OPTION, "FontEditor context menu option"));
			return contextMenu;
		}

		void IEditorComponent.CommandClick(string controlID)
		{
			if ( controlID == CONTROL_ID_CONTEXT_MENU_OPTION )
			{
				_editor.GUIController.ShowMessage("You clicked the context menu option!", MessageBoxIconType.Information);
			}
			else if ( controlID == CONTROL_ID_MAIN_MENU_OPTION )
			{
				_editor.GUIController.ShowMessage("You clicked the main menu option!", MessageBoxIconType.Information);
			}
			else if ( controlID == CONTROL_ID_ROOT_NODE )
			{
				_editor.GUIController.AddOrShowPane(_pane);
			}
		}

		void IEditorComponent.PropertyChanged(string propertyName, object oldValue)
		{
		}

		void IEditorComponent.BeforeSaveGame()
		{
		}
		void IEditorComponent.RefreshDataFromGame()
		{
			// A new game has been loaded, so remove the existing pane
			_editor.GUIController.RemovePaneIfExists(_pane);

			if ( _editor.CurrentGame.Fonts.Count > 0 )
			{
				LocalFontEditor.FontList.Clear();
				LocalFontEditor.FontView.Nodes.Clear();

				foreach ( AGS.Types.Font font in _editor.CurrentGame.Fonts )
				{
					LocalFontEditor.AddFontToList(_editor.CurrentGame.DirectoryPath, font.WFNFileName, font.Name);
				}
			}
		}
		void IEditorComponent.GameSettingsChanged()
		{
		}
		void IEditorComponent.ToXml(System.Xml.XmlTextWriter writer)
		{
			//writer.WriteElementString("TextBoxContents", ((FontEditor)_pane.Control).TextBoxContents);
		}
		void IEditorComponent.FromXml(System.Xml.XmlNode node)
		{
			if ( node == null )
			{
				// node will be null if loading a 2.72 game or if
				// the game hasn't used this plugin before

				//((FontEditor)_pane.Control).TextBoxContents = "Default text";
			}
			else
			{
				//((FontEditor)_pane.Control).TextBoxContents = node.SelectSingleNode("TextBoxContents").InnerText;
			}
		}
		void IEditorComponent.EditorShutdown()
		{
		}
	}
}
