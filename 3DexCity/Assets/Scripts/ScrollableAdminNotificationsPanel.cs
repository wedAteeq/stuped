using UnityEngine;
using UnityEngine.UI;
using Sfs2X.Entities.Data;
using UnityEngine.EventSystems;
using Sfs2X;
using Sfs2X.Util;
using Sfs2X.Core;
using Sfs2X.Requests;

public class ScrollableAdminNotificationsPanel : MonoBehaviour
{

    // Use this for initialization
   
    private ISFSArray AuctionReq, MembershipReq, CreditCard;
    private int itemCount, columnCount = 1;
    private string decision, username, request, AuctionID;

    public GameObject itemPrefab;
    public GameObject itemPrefab1Parent;
	private static ScrollableAdminNotificationsPanel instance;
	public static ScrollableAdminNotificationsPanel Instance
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

    public void Start()
    {
        AuctionReq = Transverser.AuctionReq;
        MembershipReq = Transverser.MembershipReq;
        CreditCard = Transverser.CreditCard;
        // Debug.Log(AuctionReq.Size());
        //Debug.Log(MembershipReq.Size());
        // Debug.Log(CreditCard.Size());
        if (AuctionReq != null && AuctionReq.Size() > 0 || MembershipReq != null && MembershipReq.Size() > 0)
        {

            itemCount = AuctionReq.Size() + MembershipReq.Size();

            itemPrefab = Transverser.itemPrefab3;
            itemPrefab1Parent = Transverser.itemPrefab3Parent;

            itemPrefab.gameObject.SetActive(true);
            RectTransform rowRectTransform = itemPrefab.GetComponent<RectTransform>();
            RectTransform containerRectTransform = itemPrefab1Parent.GetComponent<RectTransform>();

            //calculate the width and height of each child item.
            float width = containerRectTransform.rect.width / columnCount;
            float ratio = width / rowRectTransform.rect.width;

            float height = rowRectTransform.rect.height * ratio;

            int rowCount = itemCount / columnCount;

            if (itemCount % rowCount > 0)
                rowCount++;

            //adjust the height of the container so that it will just barely fit all its children
            float scrollHeight = height * rowCount;
            containerRectTransform.offsetMin = new Vector2(containerRectTransform.offsetMin.x, -scrollHeight / 2);
            containerRectTransform.offsetMax = new Vector2(containerRectTransform.offsetMax.x, scrollHeight / 2);

            int j = 0;
            int i = 0, DBIndex = 0;
            string request = "auction";
            for (; i < itemCount + 1; i++)
            {
                //this is used instead of a double for loop because itemCount may not fit perfectly into the rows/columns
                if (i % columnCount == 0)
                    j++;

                //create a new item, name it, and set the parent
                GameObject newItem = Instantiate(itemPrefab) as GameObject;
                newItem.name = itemPrefab1Parent.name + " item at (" + i + "," + j + ")";
                newItem.transform.parent = itemPrefab1Parent.transform;

                //move and size the new item
                RectTransform rectTransform = newItem.GetComponent<RectTransform>();

                float x = -containerRectTransform.rect.width / 2 + width * (i % columnCount);
                float y = containerRectTransform.rect.height / 2 - height * j;
                rectTransform.offsetMin = new Vector2(x, y);

                x = rectTransform.offsetMin.x + width;
                y = rectTransform.offsetMin.y + height;
                rectTransform.offsetMax = new Vector2(x, y);

                GameObject Message, AcceptButton, RejectButton;
                Text textMessage;
                if (i == AuctionReq.Size())
                {
                    DBIndex = 0;
                    request = "membership";
                }

                if (i == 0)
                    Message = GameObject.Find("msg");
                else
                    if (i == AuctionReq.Size())
                    Message = GameObject.Find("msg" + (i - 1));
                else
                    Message = GameObject.Find("msg" + (DBIndex - 1));


                textMessage = Message.GetComponent<Text>();
                textMessage.name = "msg" + DBIndex;
                if (i < itemCount)
                {
                    if (i < AuctionReq.Size())
                    {
                        textMessage.text = AuctionReq.GetSFSObject(i).GetUtfString("name") + " requested an auction?\nusername: " + AuctionReq.GetSFSObject(i).GetUtfString("username") + "\nCredit Card: " + Creditcard(AuctionReq.GetSFSObject(i).GetUtfString("username"));
                        Debug.Log(AuctionReq.GetSFSObject(i).GetUtfString("name"));
                    }
                    else
                    {
                        textMessage.text = MembershipReq.GetSFSObject(DBIndex).GetUtfString("username") + " requested membership in an auction ? " + "\nAuction ID: " + MembershipReq.GetSFSObject(DBIndex).GetUtfString("Auction_ID") + "\nCredit Card: " + Creditcard(MembershipReq.GetSFSObject(DBIndex).GetUtfString("username"));
                        Debug.Log(MembershipReq.GetSFSObject(DBIndex).GetUtfString("username"));
                    }
                }
                Button AccButton, RejButton;

                if (i == 0)
                    AcceptButton = GameObject.Find("Accept");
                else
                if (i == AuctionReq.Size())
                    AcceptButton = GameObject.Find("Accept_auction " + (i - 1));
                else
                    AcceptButton = GameObject.Find("Accept_" + request + " " + (DBIndex - 1));


                AccButton = AcceptButton.GetComponent<Button>();
                AccButton.name = "Accept_" + request + " " + DBIndex;

                if (i == 0)
                    RejectButton = GameObject.Find("Reject");
                else
                if (i == AuctionReq.Size())
                    RejectButton = GameObject.Find("Reject_auction " + (i - 1));
                else
                    RejectButton = GameObject.Find("Reject_" + request + " " + (DBIndex - 1));

                RejButton = RejectButton.GetComponent<Button>();
                RejButton.name = "Reject_" + request + " " + DBIndex;

                if (i == 0)
                    newItem.SetActive(false);
                DBIndex++;
            }

            GameObject MSG = GameObject.Find("msg" + (DBIndex - 1));
            Text textMSG = MSG.GetComponent<Text>();
            textMSG.name = "msg";

            GameObject AcceptBut = GameObject.Find("Accept_" + request + " " + (DBIndex - 1));
            Button AButton = AcceptBut.GetComponent<Button>();
            AButton.name = "Accept";

            GameObject RejectBut = GameObject.Find("Reject_" + request + " " + (DBIndex - 1)); ;
            Button RButton = RejectBut.GetComponent<Button>();
            RButton.name = "Reject";

        }

    }


   
    public void OnDecisionButtonClicked()
	{   //on accept/reject request reservation or membership in auction button clicked,
		//send the request to server
		//to get clicked button name (accept/reject)
        string name = EventSystem.current.currentSelectedGameObject.name;
		//get "_" index
        int UderScoreIndex = name.IndexOf("_");
		//get Decision
        string DecisionButtonName = name.Substring(0, UderScoreIndex);
		DecisionButtonName = DecisionButtonName.ToLower();//convert it to lower letters
		//get type of request (reserv/membership) in auction and index of the request in DB
		request = name.Substring(UderScoreIndex + 1);
        int SpaceIndex = request.IndexOf(" ");//get index of space
		string DBIndex = request.Substring(SpaceIndex + 1);//get the request index in DB
		int index = int.Parse(DBIndex);//parse it to integer
		request = request.Substring(0, SpaceIndex);//get type of request (reserve/membership) in auction
        Debug.Log(" index " + index + " decision: " + DecisionButtonName + " request " + request);
        decision = DecisionButtonName;
        if (request == "auction")
		{   		//get username of the requster
            username = Transverser.AuctionReq.GetSFSObject(index).GetUtfString("username");
			//get requested auction id
            AuctionID = Transverser.AuctionReq.GetSFSObject(index).GetUtfString("Auction_ID");
        }
        else
		{		//get username of the requster
            username = Transverser.MembershipReq.GetSFSObject(index).GetUtfString("username");
			//get requested auction id
            AuctionID = Transverser.MembershipReq.GetSFSObject(index).GetUtfString("Auction_ID");
        }
        Debug.Log("username: " + username + " AuctionID: " + AuctionID);
        ISFSObject objOut = new SFSObject();
        objOut.PutUtfString("username", username);//set username
		objOut.PutUtfString("decision", decision);//set decision (accept/reject)
		objOut.PutUtfString("Auction_ID", AuctionID);//set requested auction id
		objOut.PutUtfString("request", request);//set request type (reserve/membership)
        Debug.Log("username: " + username + " decision: " + decision);
		//send the reserve/membership decision request to server
 		NetworkManager.Instance.AuctionDecision(objOut);
    }



   
	public void AuctionDecision(ISFSObject objIn)
	{
		string result = objIn.GetUtfString("Result");

        if (result == "Successful")
            Debug.Log("Successful");
        else
            Debug.Log("error");

    }//end extension

   
    private string Creditcard(string username)
    {
        string creditCardNum = "";
        for (int i = 0; i < CreditCard.Size(); i++)
        {
            if (CreditCard.GetSFSObject(i).GetUtfString("username") == username)
            {
                creditCardNum = CreditCard.GetSFSObject(i).GetUtfString("number");
                break;
            }
        }
        return creditCardNum;
    }
}