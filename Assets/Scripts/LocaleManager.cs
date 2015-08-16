#pragma warning disable 169

using UnityEngine;
using System.Collections.Generic;

public class LocaleManager : MonoBehaviour
{
	static LocaleManager m_Instance;

	public string m_CurrentLanguage;

	[SerializeField]
	TextAsset[] m_Translations;

	IDictionary<string, string> m_CurrentDictionary;

	IDictionary<string, string> m_GlobalSubstitutions = new Dictionary<string, string>();

	public static string GetText( string id, IDictionary<string, string> substitutions = null )
	{
		return m_Instance.GetTextImpl( id, substitutions ?? new Dictionary<string, string>() );
	}

	public static void SetSubstitution( string key, string value )
	{
		m_Instance.m_GlobalSubstitutions[key] = value;
	}

	public void Awake()
	{
		DontDestroyOnLoad( gameObject );
		m_Instance = this;

		FillDictionary();
	}

	string GetTextImpl( string id, IDictionary<string, string> substitutions )
	{
		string text = string.Format ("ID NOT FOUND: {0}", id);

		if (m_CurrentDictionary.ContainsKey (id)) {
			text = m_CurrentDictionary [id];

			foreach (KeyValuePair<string,string> substitution in substitutions) {
				text = text.Replace (string.Format("{{{0}}}",substitution.Key), substitution.Value);

			}
		}

		return text;
	}

	void FillDictionary()
	{
		m_CurrentDictionary = new Dictionary<string, string>();

		foreach ( var translationFile in m_Translations )
		{
			if ( translationFile.name != m_CurrentLanguage )
				continue;

			foreach ( var line in translationFile.text.Split( new [] {"\n","\r\n"}, System.StringSplitOptions.RemoveEmptyEntries ) )
			{
				var tokens = line.Split( new[]{ '=' }, 2 );
				m_CurrentDictionary[tokens[0]] = tokens[1];
			}
		}
	}
}
