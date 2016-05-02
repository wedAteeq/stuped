using UnityEngine;
using UnityEngine.UI;
using Sfs2X;
using Sfs2X.Util;
using UnityEngine;
using UnityEngine.UI;
using Sfs2X;
using Sfs2X.Util;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Entities.Data;


public class ViewFriends : MonoBehaviour
{

    //----------------------------------------------------------
    // Private properties (Connection part)
    //----------------------------------------------------------

    private string Room_ID;//user;
    private ISFSArray Friends;
    private int display = 0;
    private string user;

    //----------------------------------------------------------
    // UI elements
    //----------------------------------------------------------
    public Transform friendsForm;

    public GameObject itemPrefab;
    public GameObject Parent;

	private static ViewFriends instance;
	public static ViewFriends Instance
	{
		get
		{
			return instance;
		}
	}
	void Awake()
	{
		instance = this;
	}
  
    //----------------------------------------------------------
    // Public interface methods for UI
    //----------------------------------------------------------

    public void OnViewFrendsButtonclicked()
	{//on view friends button clicked, sent 
		//view friends request to server
		//store the frame that a friend info will be displayed on
        Transverser.itemPrefab2 = itemPrefab;
		//store the panel that All friends will be displayed on
        Transverser.itemPrefab2Parent = Parent;
		Room_ID = Transverser.MyRoomID;//get user room ID
        ISFSObject objOut = new SFSObject();
        objOut.PutUtfString("Room_ID", Room_ID);
        objOut.PutUtfString("username", "");
		NetworkManager.Instance.GetFriendsList(objOut);//send request to server
	}//end OnViewFrendsButtonclicked

   

	public void ViewMyFriends(ISFSObject objIn)
	{   //this function will get notifications from server and display it to the member
        Friends = objIn.GetSFSArray("Friends");
       // Transverser.Friends = null;
        Transverser.Friends = Friends;//to store friends
        Transverser.RoomID = Room_ID;//to store user roomID
       // Debug.Log("Display friends");
		//display form that will show the Friend
        friendsForm.gameObject.SetActive(true);
		//class that will allow user to manage his/her Frinds
        ScrollableFriendsPanel m = new ScrollableFriendsPanel();
		if (display > 0)//to allow request Friends list from server each time
            m.Start();
        display++;
	}//end ViewMyFriends


   
}