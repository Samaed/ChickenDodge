#pragma warning disable 169

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class Popup : MonoBehaviour
{
	[SerializeField]
	MonoBehaviour[] m_ComponentsToDisable;

	[SerializeField]
	Text m_PopupTitle;

	[SerializeField]
	Text m_PopupMessage;

	public void Show( string title, string message, Dictionary<string, string> substitutions = null )
	{
		gameObject.SetActive( true );
		m_PopupTitle.text = LocaleManager.GetText( title );
		m_PopupMessage.text = LocaleManager.GetText( message , substitutions );

		foreach ( var comp in m_ComponentsToDisable )
			comp.enabled = false;
	}

	public void Hide()
	{
		gameObject.SetActive( false );

		foreach ( var comp in m_ComponentsToDisable )
			comp.enabled = true;
	}
}
