using UnityEngine;
using System.Collections;
using UnityEngine.UI;
//using UnityEditor;

using Sfs2X;
using Sfs2X.Util;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Requests;
using Sfs2X.Logging;

public class MLog : MonoBehaviour {
	private static MLog instance;
	private SmartFox sfs;

	//----------------------------------------------------------
	// Private properties (Connection part)
	//----------------------------------------------------------
	private string ServerIP = "127.0.0.1";// Default host
	private int defaultTcpPort = 9933;// Default TCP port
	private int defaultWsPort = 8888;			// Default WebSocket port
	private string ZoneName = "3DexCityZone";
	private int ServerPort = 0;

	public Text TextMessage;


	//----------------------------------------------------------
	// UI elements
	//----------------------------------------------------------

	public Transform Home;
	public Transform logount;

	// Use this for initialization
	void Start () {
		Application.runInBackground = true;
	}

	// Update is called once per frame
	void Update () {
		// As Unity is not thread safe, we process the queued up callbacks on every frame
		if (sfs != null)
			sfs.ProcessEvents();
	}


	public void OnTourButtonClicked()
	{
			#if UNITY_WEBGL
			{
			sfs = new SmartFox(UseWebSocket.WS);
			ServerPort = defaultWsPort;
			}
			#else
			{
				sfs = new SmartFox();
				ServerPort = defaultTcpPort;
			}
			#endif

			sfs.ThreadSafeMode = true;

			sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
			sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
			sfs.AddEventListener(SFSEvent.LOGIN, OnLogin);
			sfs.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
			sfs.AddEventListener(SFSEvent.UDP_INIT, OnUdpInit);
			sfs.Connect(ServerIP, ServerPort);


	}//end

	//-----------------------------------------------------------------------ON Button Clicked

	public void OnLogOutButtonClicked()
	{
		// Disconnect from server
		if (sfs != null && sfs.IsConnected)
		{
			sfs.Disconnect();
		}
		logount.gameObject.SetActive(false);
		Home.gameObject.SetActive(true);
//		AdminView.gameObject.SetActive(false);
//		MemberView.gameObject.SetActive(false);
	}

	//-----------------------------------------------------------------------Reset

	private void reset()
	{
		// Remove SFS2X listeners
		sfs.RemoveAllEventListeners();
	}

	//-----------------------------------------------------------------------OnConnection
	private void OnConnection(BaseEvent evt)
	{
		if ((bool)evt.Params["success"])
		{
			// Login
			Debug.Log("Successfully Connected!");

			//added by wed to anable networking
			SmartFoxConnection.Connection = sfs;
			sfs.Send(new LoginRequest("", "", ZoneName));
		}
		else
		{
			Debug.Log("Connection Failed!");
			// Remove SFS2X listeners and re-enable interface
			reset();
			// Show error message
			TextMessage.text = "Connection Failed!";
		}
	}
	//-----------------------------------------------------------------------OnLogin
	private void OnLogin(BaseEvent evt)
	{
		Debug.Log("Logged In: " + evt.Params["user"]);
	   //enable visiter view


		if (sfs.RoomManager.ContainsRoom ("city")) {
			{
				Debug.Log ("inside true availble room");

				sfs.Send (new JoinRoomRequest ("city"));
				Debug.Log ("after inside true availble room");
				sfs.InitUDP();

			}
		} else {
			Debug.Log ("inside false availble room");
		}

	}
	//-----------------------------------------------------------------------OnRoomJoin


	//-----------------------------------------------------------------------Network methods

	private void OnUdpInit(BaseEvent evt) {
		reset ();
		if (!(bool)evt.Params ["success"]) {
			// Disconnect
			sfs.Disconnect();
			// Show error message
			TextMessage.text = "UDP initialization failed: " + (string)evt.Params ["errorMessage"];
		}
	}

	//------------------------------------------------------------------------------ON Error
	private void OnConnectionLost(BaseEvent evt)
	{
		string reason = (string)evt.Params["reason"];

		if (reason != ClientDisconnectionReason.MANUAL)
		{
			// Show error message
			Debug.Log("Connection was lost; reason is: " + reason);
		}
	}//end

	private void OnLoginError(BaseEvent evt)
	{    // Show error message
		string message = (string)evt.Params["errorMessage"];
		string msg = "Login failed: " + message;

		Debug.Log(msg);
		if (message == "Your account has not been activated yet!")
		{
			TextMessage.text = "";
//			UserName.text = "";
//			Password.text = "";
//			enableInterface(true);

//			Login.gameObject.SetActive(false);
//			ActivateAccount.gameObject.SetActive(true);

		}
		else
		{
			// Disconnect
//			EditorUtility.DisplayDialog("Waring Message", "         Wrong password / username", "ok");
			sfs.Disconnect();

			// Remove SFS2X listeners and re-enable interface
			reset();
		}
	}
	private void OnRoomJoinError(BaseEvent evt) {
		// Show error message
		Debug.Log("Failed Joined Room");
	}


}
