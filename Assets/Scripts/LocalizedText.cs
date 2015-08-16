#pragma warning disable 169

using UnityEngine;

[RequireComponent( typeof( UnityEngine.UI.Text ) )]
public class LocalizedText : MonoBehaviour
{
	[SerializeField]
	string m_ID;

	public void Start()
	{
		gameObject.GetComponent<UnityEngine.UI.Text>().text = LocaleManager.GetText( m_ID );
	}
}
