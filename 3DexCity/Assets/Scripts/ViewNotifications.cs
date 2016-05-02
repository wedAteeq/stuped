using UnityEngine;
using UnityEngine.UI;
using Sfs2X;
using Sfs2X.Util;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Entities.Data;


public class ViewNotifications : MonoBehaviour
{

    //----------------------------------------------------------
    // Private properties (Connection part)
    //----------------------------------------------------------
  
    private string Room_ID;
    private ISFSArray useraccountinfo;
    private int display = 0;
    //----------------------------------------------------------
    // UI elements
    //----------------------------------------------------------
    public Transform NotificationForm;
    public GameObject itemPrefab;
    public GameObject Parent;


    //----------------------------------------------------------
    // Unity calback methods
    //----------------------------------------------------------
	private static ViewNotifications instance;
	public static ViewNotifications Instance
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
  
    // Update is called once per frame
  

    //----------------------------------------------------------
    // Public interface methods for UI
    //----------------------------------------------------------

    public void OnViewNotificationsButtonclicked()
	{//on view member notification button clicked, sent 
		//view notitifications request to server
		//store the frame that notitifications values will be displayed on
		Transverser.itemPrefab1 = itemPrefab;
		//store the panel that notitifications will be displayed on
		Transverser.itemPrefab1Parent = Parent;
		Room_ID = Transverser.MyRoomID;//get user room ID
        ISFSObject objOut = new SFSObject();
        objOut.PutUtfString("Room_ID", Room_ID);
		NetworkManager.Instance.GetNotifications(objOut);//send request to server
	}//end OnViewNotificationsButtonclicked



	public void ViewMyNotifications(ISFSObject objIn)
	{   //this function will get notifications from server and display it to the member
		useraccountinfo = objIn.GetSFSArray("Notifications");
        Transverser.userinfo = null;
        Transverser.userinfo = useraccountinfo;//to store notifications 
        Transverser.RoomID = Room_ID;//to store room ID
       // Debug.Log("Display Notifications");
		//display form that will show the notifications
        NotificationForm.gameObject.SetActive(true);
		//class that will deal with notification form contents
        ScrollableNotificationsPanel m = new ScrollableNotificationsPanel();
        if (display > 0)//to allaw request notification from server each time
            m.Start();
        display++;
	}//end viewMyNotifications

   
}