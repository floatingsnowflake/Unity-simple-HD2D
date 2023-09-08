using System.Collections;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEditor;
using TWC.editor;
using TWC.Utilities;

[CustomEditor(typeof (TWCReadme))]
public class TWCReadmeEditor : Editor
{
	
	TWCReadme readme;
	
	GUIStyle LinkStyle { get { return m_LinkStyle; } }
	[SerializeField] GUIStyle m_LinkStyle;
	
	GUIStyle TitleStyle { get { return m_TitleStyle; } }
	[SerializeField] GUIStyle m_TitleStyle;
	
	GUIStyle BodyStyle { get { return m_BodyStyle; } }
	[SerializeField] GUIStyle m_BodyStyle;
	
	Texture2D logo;
	Vector2 scrollPosition;
	
	void OnEnable()
	{
		readme = (TWCReadme)target;
		
		
		m_BodyStyle = new GUIStyle(EditorStyles.label);
		m_BodyStyle.wordWrap = true;
		m_BodyStyle.fontSize = 14;
		m_BodyStyle.richText = true;
		
		m_TitleStyle = new GUIStyle(m_BodyStyle);
		m_TitleStyle.fontSize = 22;

		m_LinkStyle = new GUIStyle(m_BodyStyle);
		m_LinkStyle.wordWrap = false;
		// Match selection color which works nicely for both light and dark skins
		m_LinkStyle.normal.textColor = new Color (0x00/255f, 0x78/255f, 0xDA/255f, 1f);
		m_LinkStyle.stretchWidth = false;
		
		logo = EditorUtilities.LoadIcon("twcLogo.png");

		LoadChangelog();	
	}
	
	
	
	public override void OnInspectorGUI()
	{
		GUILayout.Label(logo);
		
		GUILayout.Label("Readme", m_TitleStyle);
		GUILayout.Label(readme.version, m_BodyStyle);
		GUILayout.Label(readme.text, m_BodyStyle);
		
		
		
	
		EditorUtilities.DrawUILine(Color.black, 1);
		
		if (GUILayout.Button("Getting Started"))
		{
			Application.OpenURL(readme.gettingStartedLink);
		}
		
		if (GUILayout.Button("Getting Started Video"))
		{
			Application.OpenURL(readme.videoGettingStartedLink);
		}
		
		if (GUILayout.Button("Documentation"))
		{
			Application.OpenURL(readme.documentationLink);
		}
			
		if (GUILayout.Button("FAQ"))
		{
			Application.OpenURL(readme.faqLink);
		}
		
		
		EditorUtilities.DrawUILine(Color.black, 1);
		
		
		if (GUILayout.Button("Website"))
		{
			Application.OpenURL(readme.websiteLink);
		}

		
		if (GUILayout.Button("Asset-Store"))
		{
			Application.OpenURL(readme.assetStoreLink);
		}
		
		if (GUILayout.Button("Support"))
		{
			Application.OpenURL(readme.emailLink);
		}
		
		EditorUtilities.DrawUILine(Color.black, 1);
		
		GUILayout.Label("<b>More by doorfortyfour</b>", m_BodyStyle);
		GUILayout.Label("Assets:");
		
		if (GUILayout.Button("Databox"))
		{
			Application.OpenURL(readme.databoxLink);
		}
		
		if (GUILayout.Button("FlowReactor"))
		{
			Application.OpenURL(readme.flowreactorLink);
		}
		
		GUILayout.Label("Games:");
		if (GUILayout.Button("MarZ: Tactical Base Defense"))
		{
			Application.OpenURL(readme.marzLink);
		}
		
		EditorUtilities.DrawUILine(Color.black, 1);
		
		GUILayout.Label("<b>Changelog</b>", m_BodyStyle);
		Rect _lastRect = new Rect(0,0,0,0);
		if (Event.current.type == EventType.Repaint)
		{
			_lastRect = GUILayoutUtility.GetLastRect();
		}
		
		using (var scrollView = new EditorGUILayout.ScrollViewScope(scrollPosition, GUILayout.Width(_lastRect.width), GUILayout.Height(100)))
		{
			scrollPosition = scrollView.scrollPosition;
			GUILayout.TextArea(readme.changelog, GUILayout.ExpandHeight(true));
		}
		
	}
	
	void LoadChangelog()
	{
		string path = TWC.editor.EditorUtilities.GetRelativeResPath() + "/Changelog.txt";
		
		//Read the text from directly from the test.txt file
		System.IO.StreamReader reader = new System.IO.StreamReader(path);
		readme.changelog = reader.ReadToEnd();
		
		readme.version = System.IO.File.ReadLines(path).First();
		
		reader.Close();
	}
}
