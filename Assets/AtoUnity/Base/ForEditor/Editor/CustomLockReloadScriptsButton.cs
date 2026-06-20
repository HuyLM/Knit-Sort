using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
namespace AtoGame.Base
{
	[InitializeOnLoad]
	public static class CustomLockReloadScriptsButton
	{
		private static ScriptableObject _toolbar;
		private static bool isToggled = false;
		private static bool needLock = false;

		static CustomLockReloadScriptsButton()
		{
			EditorApplication.delayCall += AddToolbarButton;
			AssemblyReloadEvents.afterAssemblyReload += AssemblyReloadEvents_afterAssemblyReload;
		}

		private static void AssemblyReloadEvents_afterAssemblyReload()
		{
			if(needLock)
			{
				isToggled = true;
				//EditorApplication.LockReloadAssemblies();
			}

		}

		private static void AddToolbarButton()
		{
			if (_toolbar != null) return;

			Assembly editorAssembly = typeof(Editor).Assembly;
			Type toolbarType = editorAssembly.GetType("UnityEditor.Toolbar");

			UnityEngine.Object[] toolbars = Resources.FindObjectsOfTypeAll(toolbarType);
			_toolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;
			if (_toolbar == null) return;

			// Lấy rootVisualElement của Toolbar
			FieldInfo rootField = toolbarType.GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
			VisualElement root = rootField.GetValue(_toolbar) as VisualElement;

			if (root == null) return;

			RegisterCallback("ToolbarZoneRightAlign", () =>
			{
				GUILayout.FlexibleSpace();
				GUI.backgroundColor = isToggled ? Color.red : Color.white;
				if (GUILayout.Button(isToggled ? "Locked" : "Unlock", GUILayout.Width(55)))
				{
					isToggled = !isToggled;
					if(isToggled)
					{
						EditorApplication.LockReloadAssemblies();
						Debug.Log("LockReloadAssemblies");
					}
					else
					{
						needLock = true;
						EditorApplication.UnlockReloadAssemblies();
						Debug.Log("UnlockReloadAssemblies");
					}
				}
			});

			void RegisterCallback(string rootName, Action onGUI)
			{
				var toolbarZone = root.Q(rootName);
				if (toolbarZone != null)
				{
					var parent = new VisualElement()
					{
						style =
						{
							flexGrow = 0,
							flexDirection = FlexDirection.Row,
						}
					};
					var container = new IMGUIContainer();
					container.onGUIHandler += () => { onGUI?.Invoke(); };
					parent.Add(container);
					toolbarZone.Add(parent);
				}
			}
		}
	}
}
