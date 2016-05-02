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

public class MemLog : MonoBehaviour {
	private static MemLog instance;
	private SmartFox sfs;

	//----------------------------------------------------------
	// Private properties (Connection part)
	//----------------------------------------------------------
	private string ServerIP = "127.0.0.1";// Default host
	private int defaultTcpPort = 9933;// Default TCP port
	private int defaultWsPort = 8888;			// Default WebSocket port
	private string ZoneName = "3DexCityZone";
	private int ServerPort = 0;

	private string username;
	private string password;

	//----------------------------------------------------------
	// UI elements
	//----------------------------------------------------------
	public InputField UserName;
	public InputField Password;
	public Text TextMessage;
	public Transform ActivateAccount;
	public Transform Login;
	public Transform Home;
	public Transform logount;


	// Use this for initialization
	void Start () {
		Application.runInBackground = true;
		enableInterface(true);
//		TextMessage.text = "";
//		UserName.text = "";
//		Password.text = "";
	}
	
	// Update is called once per frame
	void Update () {
		// As Unity is not thread safe, we process the queued up callbacks on every frame
		if (sfs != null)
			sfs.ProcessEvents();
	}

	//this the first step to let user log in using SFS, in this step we 
	public void OnLoginButtonClicked()
	{
		username = UserName.text;
		password = Password.text;
		if (username == "" || username == " " || password == "" || password == " ")
			TextMessage.text = "Missing to fill required value";
		else
		{  // Enable interface
			enableInterface(false);
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
			//register event handlers
			sfs.ThreadSafeMode = true;
			sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
			sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
			sfs.AddEventListener(SFSEvent.LOGIN, OnLogin);
			sfs.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
			sfs.AddEventListener(SFSEvent.UDP_INIT, OnUdpInit);
			sfs.AddEventListener(SFSEvent.ROOM_JOIN, OnRoomJoin);
			sfs.AddEventListener(SFSEvent.ROOM_JOIN_ERROR, OnRoomJoinError);
			sfs.Connect(ServerIP, ServerPort);
		}

	}//end

	//-----------------------------------------------------------------------ON Button Clicked

	public void OnLogOutButtonClicked()
	{//on logoutbuton clicked this function called
		// Disconnect from server
		if (sfs != null && sfs.IsConnected)
		{
			sfs.Disconnect();
		}
	}

	//-----------------------------------------------------------------------Reset

	private void reset()
	{
		// Remove SFS2X listeners
		sfs.RemoveAllEventListeners();
		// Enable interface
		enableInterface(true);
	}

	//-----------------------------------------------------------------------OnConnection
	private void OnConnection(BaseEvent evt)
	{
		if ((bool)evt.Params["success"])
		{
			// Login
			Debug.Log("Successfully Connected!");

			password = PasswordUtil.MD5Password(password);//to incrypt the password
			//added by wed to anable networking
			SmartFoxConnection.Connection = sfs;
			sfs.Send(new LoginRequest(username, password, ZoneName));
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
	private void OnRoomJoin(BaseEvent evt) {
		// Remove SFS2X listeners and re-enable interface before moving to the main game scene
		//this room called city,and it seplify the MMO concepts,
		MMORoom room =(MMORoom) evt.Params["room"];
		if (room.Name == "city") {
			int AdminIndex = username.IndexOf ("n");//check to specify the interace(admin or member)
			string admin = username.Substring (0, AdminIndex + 1);
			NetworkManager.Instance.StartWorking ();
			if (admin.Equals ("Admin"))
				UserInterfaces.Instance.OnAdminLoginButtonAtLoginPage ();
			else
				UserInterfaces.Instance.OnLoginButtonAtLoginPage ();
			Login.gameObject.SetActive (false);//desable the lodin interace
		}
	}

	//to send the stream packets of positions updates
	private void OnUdpInit(BaseEvent evt) {
		Debug.Log ("OnUdpInit");
		if (!(bool)evt.Params ["success"]) {
			// Disconnect
			sfs.Disconnect ();
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
			UserInterfaces.Instance.onconirmLogOut ();
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
			UserName.text = "";
			Password.text = "";
			enableInterface(true);

			Login.gameObject.SetActive(false);
			ActivateAccount.gameObject.SetActive(true);
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
	private void enableInterface(bool enable)
	{
//		UserName.interactable = enable;
	//	Password.interactable = enable;
	//	TextMessage.text = "";
	}
}
