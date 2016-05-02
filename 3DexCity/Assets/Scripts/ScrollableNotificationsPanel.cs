using UnityEngine;
using UnityEngine.UI;
using Sfs2X.Entities.Data;
using UnityEngine.EventSystems;
using Sfs2X;
using Sfs2X.Util;
using Sfs2X.Core;
using Sfs2X.Requests;

public class ScrollableNotificationsPanel : MonoBehaviour
{

   
    private ISFSArray useraccountinfo;
    private int itemCount, columnCount = 1;
    private string decision, Room_ID, username;

    public GameObject itemPrefab;
    public GameObject itemPrefab1Parent;
	private static ScrollableNotificationsPanel instance;
	public static ScrollableNotificationsPanel Instance
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
        useraccountinfo = Transverser.userinfo;

        if (useraccountinfo != null && useraccountinfo.Size() > 0)
        {

            itemCount = useraccountinfo.Size();

            itemPrefab = Transverser.itemPrefab1;
            itemPrefab1Parent = Transverser.itemPrefab1Parent;

            itemPrefab.gameObject.SetActive(true);
            RectTransform rowRectTransform = itemPrefab.GetComponent<RectTransform>();
            RectTransform containerRectTransform = itemPrefab1Parent.GetComponent<RectTransform>();

            //calculate the width and height of each child item.
            float width = containerRectTransform.rect.width / columnCount;
            float ratio = width / rowRectTransform.rect.width;

            float height = rowRectTransform.rect.height * ratio ;

            int rowCount = itemCount / columnCount;

            if (itemCount % rowCount > 0)
                rowCount++;

            //adjust the height of the container so that it will just barely fit all its children
            float scrollHeight = height * rowCount;
            containerRectTransform.offsetMin = new Vector2(containerRectTransform.offsetMin.x, -scrollHeight / 2);
            containerRectTransform.offsetMax = new Vector2(containerRectTransform.offsetMax.x, scrollHeight / 2);

            int j = 0;
            int i;

            for (i = 0; i < itemCount + 1; i++)
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

                if (i == 0)
                    Message = GameObject.Find("Message");
                else
                    Message = GameObject.Find("Message" + (i - 1));


                textMessage = Message.GetComponent<Text>();
                textMessage.name = "Message" + i;
                if (i < itemCount)
                {
                    textMessage.text = useraccountinfo.GetSFSObject(i).GetUtfString("name") + " requested to friend you?";
                    Debug.Log(useraccountinfo.GetSFSObject(i).GetUtfString("name"));
                }
                Button AccButton, RejButton;

                if (i == 0)
                    AcceptButton = GameObject.Find("Accept ");
                else
                    AcceptButton = GameObject.Find("Accept " + (i - 1));


                AccButton = AcceptButton.GetComponent<Button>();
                AccButton.name = "Accept " + i;

                if (i == 0)
                    RejectButton = GameObject.Find("Reject ");
                else
                    RejectButton = GameObject.Find("Reject " + (i - 1));

                RejButton = RejectButton.GetComponent<Button>();
                RejButton.name = "Reject " + i;

                if (i == 0)
                    newItem.SetActive(false);
            }

            GameObject MSG = GameObject.Find("Message" + (i - 1));
            Text textMSG = MSG.GetComponent<Text>();
            textMSG.name = "Message";

            GameObject AcceptBut = GameObject.Find("Accept " + (i - 1));
            Button AButton = AcceptBut.GetComponent<Button>();
            AButton.name = "Accept ";

            GameObject RejectBut = GameObject.Find("Reject " + (i - 1)); ;
            Button RButton = RejectBut.GetComponent<Button>();
            RButton.name = "Reject ";

        }

    }


   

    public void OnDecisionButtonClicked()
	{   //on accept/reject request access private room button clicked,
		//send the request to server
		//to get clicked button name (accept/reject)
        string name = EventSystem.current.currentSelectedGameObject.name;
        int SpaceIndex = name.IndexOf(" ");//ge space index
        string DecisionButtonName = name.Substring(0, SpaceIndex);//get the decision 
        DecisionButtonName = DecisionButtonName.ToLower();//convert it to lower letters
        string DBIndex = name.Substring(SpaceIndex + 1);//get the request index in DB
        int index = int.Parse(DBIndex);//parse it to integer
        Debug.Log(" index " + index + " decision: " + DecisionButtonName + " room " + Room_ID);
        Room_ID = Transverser.RoomID;//get requested user room id
        decision = DecisionButtonName;
		//get username of the requster
        username = Transverser.userinfo.GetSFSObject(index).GetUtfString("username");
        Debug.Log("username: " + username + " decision: " + decision + " room " + Room_ID);
        ISFSObject objOut = new SFSObject();
        objOut.PutUtfString("Room_ID", Room_ID);
        objOut.PutUtfString("username", username);
        objOut.PutUtfString("decision", decision);
        Debug.Log("username: " + username + " decision: " + decision + " room " + Room_ID);
		NetworkManager.Instance.Decision(objOut,"AccessRoom");//send the access decision to server
    }



	public void AccessDecision(ISFSObject objIn)
	{
        string result = objIn.GetUtfString("Result");

        if (result == "Successful")
            Debug.Log("Successful");
        else
            Debug.Log("error");
    }//end extension

    

}


