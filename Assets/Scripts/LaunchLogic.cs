#pragma warning disable 169

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LaunchLogic : MonoBehaviour
{
	[SerializeField]
	InputField m_Username;

	[SerializeField]
	Toggle m_NetPlayToggle;

	[SerializeField]
	InputField m_HostName;

	[SerializeField]
	InputField m_Port;

	[SerializeField]
	Toggle m_ServerToggle;

	[SerializeField]
	Popup m_Popup;

	[SerializeField]
	Network m_Network;

	public void OnUsernameChanged()
	{
		LocaleManager.SetSubstitution( "USER", m_Username.text );
	}

	public void OnLaunch()
	{
		if ( m_NetPlayToggle.isOn == false )
			Application.LoadLevel( "Play" );
		else if ( m_ServerToggle.isOn )
			StartServer();
		else
			StartClient();
	}

	void StartServer()
	{
		m_Popup.Show( "SERVER_TITLE", "WAIT_FOR_CLIENT", new Dictionary<string,string> () {
			{ "PORT", m_Port.text }
		});
		m_Network.StartServer( int.Parse( m_Port.text ) );
	}

	void StartClient()
	{
		m_Popup.Show( "CLIENT_TITLE", "WAIT_FOR_SERVER", new Dictionary<string, string>() {
			{ "HOSTNAME", m_HostName.text },
			{ "PORT", m_Port.text }
		});
		m_Network.StartClient( m_HostName.text, int.Parse( m_Port.text ) );
	}
}
