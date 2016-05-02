using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using Sfs2X;
using Sfs2X.Logging;
using Sfs2X.Util;
using Sfs2X.Core;
using Sfs2X.Entities;
using Sfs2X.Entities.Data;
using Sfs2X.Entities.Variables;

public class BuddyMessenger : MonoBehaviour {

	private static BuddyMessenger instance;
	public static BuddyMessenger Instance {
		get {
			return instance;
		}
	}
		

	void Awake() {
		instance = this;	
	}
		
	//----------------------------------------------------------
	// UI elements
	//----------------------------------------------------------

	// User details panel components
	public Animator userPanelAnim;
	public Text loggedInText;
	public Toggle onlineToggle;
	public InputField nickInput;
	public InputField moodInput;
	public Text stateButtonLabel;
	public RectTransform stateDropDown;
	public GameObject stateItemPrefab;

	// Bubby list panel components
	public Animator buddiesPanelAnim;
	public InputField buddyInput;
	public RectTransform buddyListContent;
	public GameObject buddyListItemPrefab;
	public Sprite IconAvailable;
	public Sprite IconAway;
	public Sprite IconOccupied;
	public Sprite IconOffline;
	public Sprite IconBlocked;
	public Sprite IconBlock;
	public Sprite IconUnblock;

	// Chat panel components
	public RectTransform chatPanelsContainer;
	public GameObject chatPanelPrefab;

	//----------------------------------------------------------
	// Private properties
	//----------------------------------------------------------

	private string currentState = "Available";

	private static string BUDDYVAR_AGE = SFSBuddyVariable.OFFLINE_PREFIX + "age";
	private static string BUDDYVAR_MOOD = "mood";


	//----------------------------------------------------------
	// Public interface methods for UI
	//----------------------------------------------------------



	/**
	 * Changes the currently selected state in user panel.
	 */
	public void OnStateItemClick(string stateValue) {
		currentState = stateValue;
		stateButtonLabel.text = currentState;
		stateDropDown.gameObject.SetActive(false);
	}

	/**
	 * Makes current user go online/offline in the buddy list system.
	 */
	public void OnOnlineToggleChange(bool isChecked) {
		NetworkManager.Instance.sendOnlineRequest (isChecked);
	}

	/**
	 * Sets the current user details in the buddy system.
	 * This can be done if the current user is online in the buddy system only.
	 */
	public void OnSetDetailsButtonClick() {
		List<BuddyVariable> buddyVars = new List<BuddyVariable>();
		buddyVars.Add(new SFSBuddyVariable(ReservedBuddyVariables.BV_NICKNAME, nickInput.text));
		buddyVars.Add(new SFSBuddyVariable(BUDDYVAR_MOOD, moodInput.text));
		buddyVars.Add(new SFSBuddyVariable(ReservedBuddyVariables.BV_STATE, currentState));
		NetworkManager.Instance.SendSetDetails (buddyVars);
	}

	/**
	 * Adds a buddy to the current user's buddy list.
	 */
	public void OnAddBuddyButtonClick() {
		if (buddyInput.text != "") {
			NetworkManager.Instance.SendAddBuddy (buddyInput.text);
			buddyInput.text = "";
		}
	}

	/**
	 * Start a chat with a buddy.
	 */
	public void OnChatBuddyButtonClick(string buddyName) {
		// Check if panel is already open; if yes bring it to front
		Transform panel = chatPanelsContainer.Find(buddyName);

		if (panel == null) {
			GameObject newChatPanel = Instantiate(chatPanelPrefab) as GameObject;
			ChatPanel chatPanel = newChatPanel.GetComponent<ChatPanel>();

			chatPanel.buddy = NetworkManager.Instance.GetBuddyByName (buddyName);
			chatPanel.closeButton.onClick.AddListener(() => OnChatCloseButtonClick(buddyName));
			chatPanel.sendButton.onClick.AddListener(() => OnSendMessageButtonClick(buddyName));

			newChatPanel.transform.SetParent(chatPanelsContainer, false);

		} else {
			panel.SetAsLastSibling();

		}
	}

	/**
	 * Sends a chat message to a buddy.
	 */
	public void OnSendMessageButtonClick(string buddyName) {
		// Get panel
		Transform panel = chatPanelsContainer.Find(buddyName);
		
		if (panel != null) {
			ChatPanel chatPanel = panel.GetComponent<ChatPanel>();

			string message = chatPanel.messageInput.text;

			// Add a custom parameter containing the recipient name,
			// so that we are able to write messages in the proper chat tab
			ISFSObject _params = new SFSObject();
			_params.PutUtfString("recipient", buddyName);
			NetworkManager.Instance.SendMessagetoBuddy (buddyName,message,_params);
			chatPanel.messageInput.text = "";
		}
	}

	/**
	 * Destroys a chat panel.
	 */
	public void OnChatCloseButtonClick(string panelName) {
		Transform panel = chatPanelsContainer.Find(panelName);
		if (panel != null)
			UnityEngine.Object.Destroy(panel.gameObject);
	}
	
	/**
	 * Blocks/unblocks a buddy.
	 */
	public void OnBlockBuddyButtonClick(string buddyName) {
		NetworkManager.Instance.SendBlockBuddy (buddyName);
	}
	
	/**
	 * Removes a user from the buddy list.
	 */
	public void OnRemoveBuddyButtonClick(string buddyName) {
		NetworkManager.Instance.SendRemoveBuddy (buddyName);
	}


	//----------------------------------------------------------
	// SmartFoxServer event listeners
	//----------------------------------------------------------

	private void OnConnection(BaseEvent evt) {
		if ((bool)evt.Params["success"]) {
			// Login
		//	sfs.Send(new Sfs2X.Requests.LoginRequest(nameInput.text));
		} else {
			// Remove SFS2X listeners and re-enable interface
		//	reset();

			// Show error message
		}
	}
	
	private void OnConnectionLost(BaseEvent evt) {
		// Show login panel
		//loginPanelAnim.SetBool("loggedIn", false);

		// Hide user and buddies panels
		userPanelAnim.SetBool("loggedIn", false);
		userPanelAnim.SetBool("panelOpen", false);
		buddiesPanelAnim.SetBool("loggedIn", false);
		buddiesPanelAnim.SetBool("panelOpen", false);

		// Remove SFS2X listeners and re-enable interface
		string reason = (string) evt.Params["reason"];

		if (reason != ClientDisconnectionReason.MANUAL) {
			// Show error message
		}
	}
	
	private void OnLogin(BaseEvent evt) {
		User user = (User) evt.Params["user"];

		// Hide login panel
		//loginPanelAnim.SetBool("loggedIn", true);

		// Show user and buddies panel tabs
		userPanelAnim.SetBool("loggedIn", true);
		buddiesPanelAnim.SetBool("loggedIn", true);

		// Set "Logged in as" text
		loggedInText.text = "Logged in as " + user.Name;

		// Initialize buddy list system
		//sfs.Send(new Sfs2X.Requests.Buddylist.InitBuddyListRequest());
	}

	/**
	 * Initializes interface when buddy list data is received.
	 */
	public void OnBuddyListInit(BaseEvent evt) {
		// Populate list of buddies
		OnBuddyListUpdate(evt);
		
		// Set current user details as buddy

		// Name
		nickInput.text = NetworkManager.Instance.getMyName();

		List<string> buddystates = NetworkManager.Instance.getBuddyStates ();
		// States
		foreach (string state in buddystates) {
			string stateValue = state;
			GameObject newDropDownItem = Instantiate(stateItemPrefab) as GameObject;
			BuddyStateItemButton stateItem = newDropDownItem.GetComponent<BuddyStateItemButton>();
			stateItem.stateValue = stateValue;
			stateItem.label.text = stateValue;
			
			stateItem.button.onClick.AddListener(() => OnStateItemClick(stateValue));
			
			newDropDownItem.transform.SetParent(stateDropDown, false);
			// Set current state
			if (NetworkManager.Instance.getMyStates() == state) {
				OnStateItemClick(state);
			}
		}

		// Online
		onlineToggle.isOn = NetworkManager.Instance.getMyOnlineState();
		
		// Buddy variables
		BuddyVariable age =NetworkManager.Instance.GetMyVariable(BUDDYVAR_AGE);

		BuddyVariable mood =NetworkManager.Instance.GetMyVariable(BUDDYVAR_MOOD);
		moodInput.text = ((mood != null && !mood.IsNull()) ? mood.GetStringValue() : "");
	}

	/**
	 * Populates the buddy list.
	 */
	public void OnBuddyListUpdate(BaseEvent evt) {
		// Remove current list content
		for (int i = buddyListContent.childCount - 1; i >= 0; --i) {
			GameObject.Destroy(buddyListContent.GetChild(i).gameObject);
		}
		buddyListContent.DetachChildren();
		List<Buddy> buddies = NetworkManager.Instance.GetBuddyList ();
		// Recreate list content
		foreach (Buddy buddy in buddies) {
			GameObject newListItem = Instantiate(buddyListItemPrefab) as GameObject;

			BuddyListItem buddylistItem = newListItem.GetComponent<BuddyListItem>();

			// Nickname
			buddylistItem.mainLabel.text = (buddy.NickName != null && buddy.NickName != "") ? buddy.NickName : buddy.Name;

			// Age
			BuddyVariable age = buddy.GetVariable(BuddyMessenger.BUDDYVAR_AGE);
			buddylistItem.mainLabel.text += (age != null && !age.IsNull()) ? " (" + age.GetIntValue() + " yo)" : "";

			// Mood
			BuddyVariable mood = buddy.GetVariable(BuddyMessenger.BUDDYVAR_MOOD);
			buddylistItem.moodLabel.text = (mood != null && !mood.IsNull()) ? mood.GetStringValue() : "";

			// Icon
			if (buddy.IsBlocked) {
				buddylistItem.stateIcon.sprite = IconBlocked;
				buddylistItem.chatButton.interactable = false;
				buddylistItem.blockButton.transform.GetChild(0).GetComponentInChildren<Image>().sprite = IconUnblock;
			}
			else
			{
				buddylistItem.blockButton.transform.GetChild(0).GetComponentInChildren<Image>().sprite = IconBlock;

				if (!buddy.IsOnline) {
					buddylistItem.stateIcon.sprite = IconOffline;
					buddylistItem.chatButton.interactable = false;
				}
				else {
					string state = buddy.State;
					
					if (state == "Available")
						buddylistItem.stateIcon.sprite = IconAvailable;
					else if (state == "Away")
						buddylistItem.stateIcon.sprite = IconAway;
					else if (state == "Occupied")
						buddylistItem.stateIcon.sprite = IconOccupied;
				}
			}

			// Buttons
			string buddyName = buddy.Name; // Required or the listeners will always receive the last buddy name
			buddylistItem.removeButton.onClick.AddListener(() => OnRemoveBuddyButtonClick(buddyName));
			buddylistItem.blockButton.onClick.AddListener(() => OnBlockBuddyButtonClick(buddyName));
			buddylistItem.chatButton.onClick.AddListener(() => OnChatBuddyButtonClick(buddyName));

			buddylistItem.buddyName = buddyName;

			// Add item to list
			newListItem.transform.SetParent(buddyListContent, false);

			// Also update chat panel if open
			Transform panel = chatPanelsContainer.Find(buddyName);
			
			if (panel != null) {
				ChatPanel chatPanel = panel.GetComponent<ChatPanel>();
				chatPanel.buddy = buddy;
			}
		}
	}

	/**
	 * Handles messages receive from buddies.
	 */
	public void OnBuddyMessage(BaseEvent evt) {
		bool isItMe = (bool)evt.Params["isItMe"];
		Buddy sender = (Buddy)evt.Params["buddy"];
		string message = (string)evt.Params["message"];

		Buddy buddy;
		if (isItMe)
		{
			string buddyName = (evt.Params["data"] as ISFSObject).GetUtfString("recipient");
			buddy = NetworkManager.Instance.GetBuddyByName (buddyName);
		}
		else
			buddy = sender;

		if (buddy != null) {
			// Open panel if needed
			OnChatBuddyButtonClick(buddy.Name);

			// Print message
			Transform panel = chatPanelsContainer.Find(buddy.Name);
			ChatPanel chatPanel = panel.GetComponent<ChatPanel>();
			chatPanel.addMessage("<b>" + (isItMe ? "You" : buddy.Name) + ":</b> " + message);
		}
	}
}