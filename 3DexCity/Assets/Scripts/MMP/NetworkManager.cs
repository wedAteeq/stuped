
using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;
using Sfs2X.Requests;
using Sfs2X.Logging;
using Sfs2X.Requests.MMO;
using Sfs2X.Util;

// The Neywork manager sends the messages to server and handles the response.
public class NetworkManager : MonoBehaviour
{

	public GameObject[] playerModels;//this will hold 2 male and female to spwan players
	//private PlayerController localPlayerController;
	private GameObject localPlayer; //this will hold the actual player
	private Dictionary<int, GameObject> remotePlayers = new Dictionary<int, GameObject>();
	private Dictionary<int, NetworkTransformReceiver> recipients = new Dictionary<int, NetworkTransformReceiver>();
	private Dictionary<string, bool> rooms = new Dictionary<string, bool>();
	private bool running = false;
	private string aucname="";
	private string Request1,Request2,userDecision;
	private static NetworkManager instance;
	public static NetworkManager Instance {
		get {
			return instance;
		}
	}
	
	private SmartFox smartFox;  // The reference to SFS client
	
	void Awake() {
		instance = this;	
	}
	

			
	// This is needed to handle server events in queued mode
	void FixedUpdate() {
		if (!running) return;
		smartFox.ProcessEvents();
		if (smartFox != null) {
			smartFox.ProcessEvents();
		}
	}
	/**
	 * This is where we receive events about people in proximity (AoI).
	 * We get two lists, one of new users that have entered the AoI and one with users that have left our proximity area.
	 */
	public void OnProximityListUpdate(BaseEvent evt)
	{
		var addedUsers = (List<User>) evt.Params["addedUsers"];
		var removedUsers = (List<User>) evt.Params["removedUsers"];

		// Handle all new Users
		foreach (User user in addedUsers)
		{
			SpawnRemotePlayer (
				(SFSUser) user, 
				Convert.ToInt16(user.GetVariable ("g").Value), 
				new Vector3(user.AOIEntryPoint.FloatX, 5.36f, user.AOIEntryPoint.FloatZ),
				new Vector3(0, (float) user.GetVariable("r").GetDoubleValue() , 0)
			);		
		}

		// Handle removed users
		foreach (User user in removedUsers)
		{
			RemoveRemotePlayer((SFSUser) user);
		}
	}
		
	/**
	 * When user variable is updated on any client within the AoI, then this event is received.
	 * This is where most of the game logic for this example is contained.
	 */
	public void OnUserVariableUpdate(BaseEvent evt) {//this method will leston to every change occure in server side
		#if UNITY_WSA && !UNITY_EDITOR
		List<string> changedVars = (List<string>)evt.Params["changedVars"];
		#else
		ArrayList changedVars = (ArrayList)evt.Params["changedVars"];
		#endif
		SFSUser user = (SFSUser)evt.Params["user"];//get user from sfs
		if (user == smartFox.MySelf) {//spwanLocalPlayer if does not exist
			if (localPlayer == null) {
				NetworkTransform transform = NetworkTransform.FromUserVariables 
					((float)user.GetVariable ("x").GetDoubleValue (), 5.36f,
						(float)user.GetVariable ("z").GetDoubleValue (), 0, 
						(float)user.GetVariable ("r").GetDoubleValue (), 0, Convert.ToDouble (user.GetVariable ("t").Value));	
			//	SpawnLocalPlayer (Convert.ToInt16(user.GetVariable ("g").Value), transform);
			}return;
		}
		if (recipients.ContainsKey(user.Id)){
            // Check if the remote user changed his position or rotation
            NetworkTransformReceiver recipent = recipients[user.Id];
            if (recipent != null){
                if (changedVars.Contains("x") ||  changedVars.Contains("z") || changedVars.Contains("r")) {
			// Move the character to a new position...
			NetworkTransform transform= NetworkTransform.FromUserVariables
						((float)user.GetVariable("x").GetDoubleValue(), 
							5.36f, (float)user.GetVariable("z").GetDoubleValue(),
							0, (float)user.GetVariable("r").GetDoubleValue(), 0, Convert.ToDouble(user.GetVariable("t").Value));		
				recipent.ReceiveTransform (transform);
					/*
                if (changedVars.Contains("m"))
                    remotePlayers[user.Id].GetComponent<enamy>().moveAvatar((float)user.GetVariable("m").GetDoubleValue());
                else if (changedVars.Contains("l"))
                    remotePlayers[user.Id].GetComponent<enamy>().turnLeft();
                else if (changedVars.Contains("r"))
                    remotePlayers[user.Id].GetComponent<enamy>().turnRigt();
                    */
            }
		// Remote client selected new model?
		if (changedVars.Contains("model")) {
			SpawnRemotePlayer(user, user.GetVariable("model").GetIntValue(),  
						remotePlayers[user.Id].transform.position, remotePlayers[user.Id].transform.localEulerAngles);
		}
            }
        }
	}
	/// <summary>
	///  Send a request to get typ of player
	/// </summary>
	public void StartWorking()
	{
		smartFox = SmartFoxConnection.Connection;
		UnsubscribeDelegates ();
		running = true;
		SubscribeDelegates ();
		TimeManager.Instance.Init ();
		InitRooms ();
		//init Rooms

	}
	private void InitRooms()
	{
		//rooms.Add ("RR1", true);
		//rooms.Add ("RR2", true);
		rooms.Add ("RR3", true);
		rooms.Add ("RR4", true);
		rooms.Add ("YR1", true);
		rooms.Add ("YR2", true);
		rooms.Add ("YR3", true);
		rooms.Add ("YR4", true);
		rooms.Add ("GR1", true);
		rooms.Add ("GR2", true);
		rooms.Add ("GR3", true);
		rooms.Add ("GR4", true);
		rooms.Add ("BR1", true);
		rooms.Add ("BR2", true);
		rooms.Add ("BR3", true);
		rooms.Add ("BR4", true);
	}
	private void SubscribeDelegates() {
		smartFox.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
		smartFox.AddEventListener(SFSEvent.USER_VARIABLES_UPDATE, OnUserVariableUpdate);
		smartFox.AddEventListener(SFSEvent.PROXIMITY_LIST_UPDATE, OnProximityListUpdate);
		smartFox.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
	}
	private void UnsubscribeDelegates() {
		if (smartFox != null) {
			smartFox.RemoveAllEventListeners ();
		}
	}
	//----------------------------------------------------------
	// Private player helper methods
	//----------------------------------------------------------
	//this MethodAccessException recive SystemInfo of PlayerPrefs from server side codes
	private void SpawnRemotePlayer(SFSUser user, int numModel,  Vector3 pos, Vector3 rot) {
		// See if there already exists a model so we can destroy it first
		if (remotePlayers.ContainsKey(user.Id) && remotePlayers[user.Id] != null) {
			Destroy(remotePlayers[user.Id]);
			remotePlayers.Remove(user.Id);
		}
		// Lets spawn our remote player model
		GameObject remotePlayer = GameObject.Instantiate(playerModels[1]) as GameObject;
		remotePlayer.transform.position = pos;
		remotePlayer.transform.localEulerAngles = rot;
		Debug.Log ("inside spwan remote player");
		remotePlayer.AddComponent<NetworkTransformInterpolation>();
		remotePlayer.AddComponent<NetworkTransformReceiver>();

		recipients.Add(user.Id, remotePlayer.GetComponent<NetworkTransformReceiver> ());
		remotePlayers.Add(user.Id, remotePlayer);
	}

	private void RemoveRemotePlayer(SFSUser user) {
		if (user == smartFox.MySelf) return;

		if (remotePlayers.ContainsKey(user.Id)) {
			Destroy(remotePlayers[user.Id]);
			remotePlayers.Remove(user.Id);
		}
		if (recipients.ContainsKey(user.Id) && recipients[user.Id] != null) {
			Destroy(recipients[user.Id]);
			recipients.Remove(user.Id);
		}
	}

	/// <summary>
	/// When connection is lost we load the login scene
	/// </summary>
	private void OnConnectionLost(BaseEvent evt) {
		UnsubscribeDelegates();
		//change view
	}


	private void SpawnLocalPlayer(int numModel, NetworkTransform trans) {
		Vector3 pos;
		Vector3 rot;
		// See if there already exists a model - if so, take its pos+rot before destroying it
		if (localPlayer != null) {
			pos = localPlayer.transform.position;
			rot = localPlayer.transform.localEulerAngles;
			//	Camera.main.transform.parent = null;
			Destroy(localPlayer);
		} else {
			pos =trans.Position;
			rot = trans.AngleRotationFPS;
		}
		// Lets spawn our local player model
		localPlayer = GameObject.Instantiate(playerModels[0]) as GameObject;
		localPlayer.transform.position = pos;
		localPlayer.transform.localEulerAngles = rot;
		localPlayer.AddComponent<NetworkTransformSender>();
		NetworkTransformSender.Instance.StartSendTransform();
		AddMsnLesteners ();
	}

	///<summary>
	/// This methods to manage chatting/////////////////////////////////////////////////////////////////////////////////////
	/// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// </summary>
	private void AddMsnLesteners()
	{
		// Add SFS2X buddy-related event listeners
		// NOTE: for simplicity, most buddy-related events cause the whole
		// buddylist in the interface to be recreated from scratch, also if those
		// events are caused by the current user himself. A more refined approach should
		// update the specific items to which the event refers.
		smartFox.AddEventListener(SFSBuddyEvent.BUDDY_LIST_INIT, OnBuddyListInit);
		smartFox.AddEventListener(SFSBuddyEvent.BUDDY_ERROR, OnBuddyError);
		smartFox.AddEventListener(SFSBuddyEvent.BUDDY_ONLINE_STATE_UPDATE, OnBuddyListUpdate);
		smartFox.AddEventListener(SFSBuddyEvent.BUDDY_VARIABLES_UPDATE, OnBuddyListUpdate);
		smartFox.AddEventListener(SFSBuddyEvent.BUDDY_ADD, OnBuddyListUpdate);
		smartFox.AddEventListener(SFSBuddyEvent.BUDDY_REMOVE, OnBuddyListUpdate);
		smartFox.AddEventListener(SFSBuddyEvent.BUDDY_BLOCK, OnBuddyListUpdate);
		smartFox.AddEventListener(SFSBuddyEvent.BUDDY_MESSAGE, OnBuddyMessage);
		smartFox.Send(new Sfs2X.Requests.Buddylist.InitBuddyListRequest());

	}
	private void OnBuddyListInit(BaseEvent evt) {
		BuddyMessenger.Instance.OnBuddyListInit (evt);
	}
	private void OnBuddyError(BaseEvent evt) {
		Debug.LogError("The following error occurred in the buddy list system: " + (string)evt.Params["errorMessage"]);
	}
	private void OnBuddyListUpdate(BaseEvent evt) {
		BuddyMessenger.Instance.OnBuddyListUpdate (evt);
	}
	private void OnBuddyMessage(BaseEvent evt) {
		BuddyMessenger.Instance.OnBuddyMessage (evt);
	}
	public void sendOnlineRequest (bool x)
	{
		smartFox.Send(new Sfs2X.Requests.Buddylist.GoOnlineRequest(x));

	}

	public void SendSetDetails (List<BuddyVariable> buddyVars)
	{
		smartFox.Send(new Sfs2X.Requests.Buddylist.SetBuddyVariablesRequest(buddyVars));
	}

	public void SendAddBuddy (string name)
	{
		smartFox.Send(new Sfs2X.Requests.Buddylist.AddBuddyRequest(name));
	}
	public Buddy GetBuddyByName (string name)
	{
		return smartFox.BuddyManager.GetBuddyByName(name);
	}
	public void SendMessagetoBuddy (string name, string mess, ISFSObject param)
	{
		Buddy buddy=smartFox.BuddyManager.GetBuddyByName(name);
		smartFox.Send(new Sfs2X.Requests.Buddylist.BuddyMessageRequest(mess, buddy,param));
	}
	public void SendBlockBuddy (string name)
	{
		bool isBlocked = smartFox.BuddyManager.GetBuddyByName(name).IsBlocked;
		smartFox.Send(new Sfs2X.Requests.Buddylist.BlockBuddyRequest(name, !isBlocked));
	}

	public void SendRemoveBuddy (string name)
	{
		smartFox.Send(new Sfs2X.Requests.Buddylist.RemoveBuddyRequest(name));
	}
	public List<Buddy> GetBuddyList()
	{
		return smartFox.BuddyManager.BuddyList;
	}
	public BuddyVariable GetMyVariable(string varName)
	{
		return smartFox.BuddyManager.GetMyVariable(varName);
	}
	public string getMyName()
	{
		return smartFox.MySelf.Name;
	}
	public List<string> getBuddyStates()
	{
		return smartFox.BuddyManager.BuddyStates;
	}
	public string getMyStates()
	{
		return smartFox.BuddyManager.MyState;
	}
	public bool getMyOnlineState()
	{
		return smartFox.BuddyManager.MyOnlineState;
	}
	///<summary>
	///End Of manage chatting///////////////////////////////////////////////////////////////////////////////////////////////
	/// ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	/// </summary>

	/// <summary>
	/// Send local transform to the server
	/// </summary>
	/// <param name="ntransform">
	/// A <see cref="NetworkTransform"/>
	/// </param>
	public void SendTransform(NetworkTransform ntransform) {
		
		if (localPlayer != null  ) {
			Debug.Log ("SendTransform");
			ISFSObject trans = new SFSObject ();
			ntransform.ToSFSObject (trans);
			Debug.Log (smartFox.LastJoinedRoom.Name);
			smartFox.Send(new ExtensionRequest ("control.tr",trans, smartFox.LastJoinedRoom));		
		}
	}

	/// <summary>
	/// Request the current server time. Used for time synchronization
	/// </summary>	
	public void TimeSyncRequest() {
		Debug.Log ("TimeSyncRequest() ");
		ExtensionRequest request = new ExtensionRequest("control.ti", new SFSObject(), smartFox.LastJoinedRoom);
		smartFox.Send(request);
	}
	public void SendBid(double price, int id) {
		ISFSObject bidding = new SFSObject ();
		bidding.PutInt ("id", id);
		bidding.PutDouble ("bid", price);
		ExtensionRequest request = new ExtensionRequest("auction.bidding",bidding, smartFox.LastJoinedRoom);
		smartFox.Send(request);
	}
	private void OnApplicationQuit() {
		UnsubscribeDelegates();
	}
		


	public void sendReserveAuction( ISFSObject data)
	{
		ExtensionRequest request = new ExtensionRequest("auction.reserve", data, smartFox.LastJoinedRoom);
		smartFox.Send(request);

	}

	public void sendRequestMemberShip( ISFSObject data)
	{
		ExtensionRequest request = new ExtensionRequest("auction.membership", data, smartFox.LastJoinedRoom);
		smartFox.Send(request);

	}

	public void sendGetSlots()
	{
	ExtensionRequest request = new ExtensionRequest("auction.slots", new SFSObject(), smartFox.LastJoinedRoom);
		smartFox.Send(request);

	}


	public void GetAccountInfo(ISFSObject data,String userRequest)
    {
        ExtensionRequest request = new ExtensionRequest("ViewAccount.view", data, smartFox.LastJoinedRoom);
		Request1=userRequest;
        smartFox.Send(request);
    }

	public void DeleteAccount(ISFSObject data,String userRequest)
	{
		ExtensionRequest request = new ExtensionRequest("ViewAccount.delete", data, smartFox.LastJoinedRoom);
		Request2=userRequest;
		smartFox.Send(request);
	}

	public void UpdateAccount(ISFSObject data)
	{
		ExtensionRequest request = new ExtensionRequest("ViewAccount.edit", data, smartFox.LastJoinedRoom);
		smartFox.Send(request);
	}

	public void CreateRoom(ISFSObject data)
	{
		ExtensionRequest request = new ExtensionRequest("Room.create", data, smartFox.LastJoinedRoom);
		smartFox.Send(request);
	}

	public void DeleteRoom(ISFSObject data)
	{
		ExtensionRequest request = new ExtensionRequest("Room.delete", data, smartFox.LastJoinedRoom);
		smartFox.Send(request);
	}

	public void RequestAccessRoom(ISFSObject data)
	{
		ExtensionRequest request = new ExtensionRequest("Room.accessRequest", data, smartFox.LastJoinedRoom);
		smartFox.Send(request);
	}

	public void GetNotifications(ISFSObject data)
	{
		ExtensionRequest request = new ExtensionRequest("Room.getNotifications", data, smartFox.LastJoinedRoom);
		smartFox.Send(request);
	}

	public void GetNotifications()
	{
		ExtensionRequest request = new ExtensionRequest("Room.getAuctions", new SFSObject(), smartFox.LastJoinedRoom);
		smartFox.Send(request);
	}

	public void GetFriendsList(ISFSObject data)
	{
		ExtensionRequest request = new ExtensionRequest("Room.getFriends", data, smartFox.LastJoinedRoom);
		smartFox.Send(request);
	}
	public void GetAuctions()
	{
		ExtensionRequest request = new ExtensionRequest("auction.get",  new SFSObject(), smartFox.LastJoinedRoom);
		smartFox.Send(request);
	}

	public void Decision(ISFSObject data,string decision)
	{   userDecision = decision;
		ExtensionRequest request = new ExtensionRequest("Room.decision", data, smartFox.LastJoinedRoom);
		smartFox.Send(request);
	}

	public void AuctionDecision(ISFSObject data)
	{
		ExtensionRequest request = new ExtensionRequest("Room.auction", data, smartFox.LastJoinedRoom);
		smartFox.Send(request);
	}

	public void AddNewAdmin(ISFSObject data)
	{
		ExtensionRequest request = new ExtensionRequest("Room.addAdmin", data, smartFox.LastJoinedRoom);
		smartFox.Send(request);
	}
    /////////////////////////////////////////////////////////////////////ON Extension Response
    /// 
    ///
    // This method handles all the responses from the server
    private void OnExtensionResponse(BaseEvent evt) {
		try {
			string cmd = (string)evt.Params["cmd"];
			ISFSObject dt = (SFSObject)evt.Params["params"];

			if (cmd == "time") {//spwan Player
			HandleServerTime(dt);
			}
			else if (cmd == "slots") {
				ManageAuction.Instance.SetSlots(dt, true);
			}
			else if (cmd == "rooms") {
				HandleRooms(dt);
			}
			else if (cmd == "slotef") {
				
				ManageAuction.Instance.SetSlots(dt, false);
			}
			else if (cmd == "ReserveS") {
				ManageAuction.Instance.ReserveSuccess();
			}
			else if(cmd == "ViewAccount") {

				ISFSArray useraccountinfo = dt.GetSFSArray("account");
				string username = useraccountinfo.GetSFSObject(0).GetUtfString("username");

				int AdminIndex = username.IndexOf("n");
					string admin = username.Substring(0, AdminIndex + 1);

				      if (admin.Equals("Admin"))
					     manageAdminAccount.Instance.ViewAdminAccount(dt);
				else if (Request1=="")
					     manageMemberAccount.Instance.ViewMemberAccount(dt);
				else if (Request1=="AdminRequest")
					ViewMemberAccount.Instance.ViewAccountInfo(dt);
				}
			else if (cmd == "DeleteAccount") {
				string result = dt.GetUtfString("DeleteResult");
				int dotIndex = result.IndexOf(".");
				string admin = result.Substring(dotIndex+1);
				admin = admin.Substring(0, result.IndexOf("n")+1);
				if (admin.Equals("Admin"))
					manageAdminAccount.Instance.DeleteAccount(dt);
				else if (Request2=="")
					manageMemberAccount.Instance.DeleteAccount(dt);
				else if (Request2=="AdminRequest")
					ViewMemberAccount.Instance.DeleteAccount(dt);
			}
			else if (cmd == "UpdateAccount") {
				string result = dt.GetUtfString("UpdateResult");
				int dotIndex = result.IndexOf(".");
				string admin = result.Substring(dotIndex+1);
				admin = admin.Substring(0, result.IndexOf("n")+1);
				if (admin.Equals("Admin"))
					manageAdminAccount.Instance.UpdateAccount(dt);
				else
					manageMemberAccount.Instance.UpdateAccount(dt);

			}else if (cmd == "CreateRoom") {
				manageMemberAccount.Instance.createRoom(dt);
			}else if (cmd == "DeleteRoom") {
				manageMemberAccount.Instance.deleteRoom(dt);
			}else if (cmd == "RequestAccessRoom") {
				RequestAccess.Instance.RequestAccessPRoom(dt);
			}else if (cmd == "ViewNotifications") {
				ViewNotifications.Instance.ViewMyNotifications(dt);
			}else if (cmd == "ViewFriends") {
				ViewFriends.Instance.ViewMyFriends(dt);
			}else if (cmd == "ViewAuctionRequests") {
				ViewAuction.Instance.ViewMyNotifications(dt);
			}else if (cmd == "Decision") {
				if (userDecision=="AccessRoom")
				ScrollableNotificationsPanel.Instance.AccessDecision(dt);
				else 
					ScrollableFriendsPanel.Instance.DeleteDecision(dt);
			}else if (cmd == "AuctionDecision") {
				ScrollableAdminNotificationsPanel.Instance.AuctionDecision(dt);
			}else if (cmd == "AddAdmin") {
				AddAdmin.Instance.AddNewAdminResult(dt);
			}

			//Auction manager
			else if (cmd == "items") {//we need first to create the items before openning the auction
				HandleInitItems(dt);
			}
			else if (cmd == "dur") {//this makes 3 things: 1/ make auction room true 2/send notification to the user
				HandleStartAuction(dt);
			}
			else if (cmd == "end") {//1/ disable all bid buttons 2/ show end notification 3/destroy items 4/ let user go out 5/make auction room false
				AddAdmin.Instance.AddNewAdminResult(dt);
			}
			else if (cmd == "bidding") {//1/ update current price of item
				Bidding(dt);
			}
			else if (cmd == "usa") {//you can start your auction
				//contains items info
				HandleInitItems(dt);
				//contains duration to end
				HandleStartAuction(dt);
			}
			else if (cmd == "auctions") {//you can start your auction
				Debug.Log("I recive Auctions");
				ManageAuction.Instance.ReciveAuctions(dt);
			}
		}
		catch (Exception e) {
			Debug.Log("Exception handling response: "+e.Message+" >>> "+e.StackTrace);
		}
	}
	private void HandleServerTime(ISFSObject dt )
	{
		long time = dt.GetLong ("t");
		TimeManager.Instance.Synchronize (Convert.ToDouble(time));
	}

	private void HandleInitItems(ISFSObject dt )
	{
		ManageItems.Instance.InitItemsPositions ();
		int size = dt.GetInt ("s");
		for (int i=0;i<size;i++)
		{
			ManageItems.Instance.spwanItem (i,dt.GetDouble(i+""));
		}
	}
	private void HandleRooms(ISFSObject dt )
	{
		int size = dt.GetInt ("s");
		int id = 0;
		for (int i = 0; i < size; i++) {
			id = dt.GetInt (i + "");
			string rID = getRoomromId (id);
			rooms.Remove (rID);
		}

		if (dt.ContainsKey ("a")) {
			rooms.Add ("a",true);
		}
		
			
	}
	private string getRoomromId(int id)
	{
		switch (id) {
		case 1: return "RR1";
		case 2:return "RR2";
		case 3:return "RR3";
		case 4:return "RR4";
		case 5:return "BR1";
		case 6:return "BR2";
		case 7:return "BR3";
		case 8:return "BR4";
		case 9:return "YR1";
		case 10:return "YR2";
		case 11:return "YR3";
		case 12:return "YR4";
		case 13:return "GR1";
		case 14:return "GR2";
		case 15:return "GR3";
		case 16:return "GR4";
		}
		return "";
	}
	private void HandleStartAuction(ISFSObject dt )
	{
		//1 open Auction Room
		rooms.Add("a",true);
		//2 show notification
		UserInterfaces.Instance.setStartAuction(dt.GetUtfString("o"),dt.GetUtfString("n"),dt.GetInt("d"));
		UserInterfaces.Instance.OnStartAuction ();
		//3 in future add: show timer
	}
	private void Bidding(ISFSObject dt)
	{
		ManageItems.Instance.updatePrice (dt.GetInt("id"),dt.GetDouble("p"));
	}

	public bool EnterRoom(String id)
	{
		return rooms.ContainsKey (id);
	}

}
