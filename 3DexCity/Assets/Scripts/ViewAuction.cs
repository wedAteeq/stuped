using UnityEngine;
using UnityEngine.UI;
using Sfs2X;
using Sfs2X.Util;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Entities.Data;


public class ViewAuction : MonoBehaviour
{

    //----------------------------------------------------------
    // Private properties (Connection part)
    //----------------------------------------------------------
   
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
	private static ViewAuction instance;
	public static ViewAuction Instance
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


    public void OnViewNotificationsButtonclicked()
	{//on view admin notification button clicked, sent 
		//view notitifications request to server
		//store the frame that notitifications values will be displayed on
        Transverser.itemPrefab3 = itemPrefab;
		//store the panel that notitifications will be displayed on
        Transverser.itemPrefab3Parent = Parent;
		NetworkManager.Instance.GetNotifications();//send request to server
	}//end OnViewNotificationsButtonclicked

	public void ViewMyNotifications(ISFSObject objIn)
	{   //this function will get notifications from server and display it to the member
		ISFSArray AuctionReq = objIn.GetSFSArray("AuctionRequests");//get auction requests
		//get membership in auction requests
        ISFSArray MembershipReq = objIn.GetSFSArray("membershipRequests");
        ISFSArray CreditCard = objIn.GetSFSArray("CreditCard");//get cridit cards
        Debug.Log(AuctionReq.Size());
        Debug.Log(MembershipReq.Size());
		//to store auction requests
        Transverser.AuctionReq = AuctionReq;
		Transverser.MembershipReq = MembershipReq;//to store membership  requests
		Transverser.CreditCard = CreditCard;		//to store cridit cards
        Debug.Log(CreditCard.Size());
        Debug.Log("Display Notifications");
		//display form that will show the notifications
        NotificationForm.gameObject.SetActive(true);
		//class that will deal with notification form contents
        ScrollableAdminNotificationsPanel m = new ScrollableAdminNotificationsPanel();
		if (display > 0)//to allaw request notification from server each time
            m.Start();
        display++;
	}//end ViewMyNotifications
		
}