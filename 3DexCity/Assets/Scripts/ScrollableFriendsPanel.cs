using UnityEngine;
using UnityEngine.UI;
using Sfs2X.Entities.Data;
using UnityEngine.EventSystems;
using Sfs2X;
using Sfs2X.Util;
using Sfs2X.Core;
using Sfs2X.Requests;

public class ScrollableFriendsPanel : MonoBehaviour
{

 
    private ISFSArray Friends;
    private int itemCount, columnCount = 1;
    private string decision, Room_ID, username;

    public GameObject itemPrefab;
    public GameObject itemPrefabParent;
	private static ScrollableFriendsPanel instance;
	public static ScrollableFriendsPanel Instance
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
        Friends = Transverser.Friends;
        Room_ID = Transverser.RoomID;

        if (Friends != null && Friends.Size() > 0)
        {

            itemCount = Friends.Size();

            itemPrefab = Transverser.itemPrefab2;
            itemPrefabParent = Transverser.itemPrefab2Parent;

            itemPrefab.gameObject.SetActive(true);

            RectTransform rowRectTransform = itemPrefab.GetComponent<RectTransform>();
            RectTransform containerRectTransform = itemPrefabParent.GetComponent<RectTransform>();

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
                newItem.name = itemPrefabParent.name + " item at (" + i + "," + j + ")";
                newItem.transform.parent = itemPrefabParent.transform;

                //move and size the new item
                RectTransform rectTransform = newItem.GetComponent<RectTransform>();

                float x = -containerRectTransform.rect.width / 2 + width * (i % columnCount);
                float y = containerRectTransform.rect.height / 2 - height * j;
                rectTransform.offsetMin = new Vector2(x, y);

                x = rectTransform.offsetMin.x + width;
                y = rectTransform.offsetMin.y + height;
                rectTransform.offsetMax = new Vector2(x, y);

                GameObject Message, DeleButton;
                Text textMessage;

                if (i == 0)
                    Message = GameObject.Find("message");
                else
                    Message = GameObject.Find("message" + (i - 1));


                textMessage = Message.GetComponent<Text>();
                textMessage.name = "message" + i;
                if (i < itemCount)
                {
                    textMessage.text = Friends.GetSFSObject(i).GetUtfString("name") + " is my friend";
                    Debug.Log(Friends.GetSFSObject(i).GetUtfString("name"));
                }

                Button DeleteButton;
                if (i == 0)
                    DeleButton = GameObject.Find("Delete ");
                else
                    DeleButton = GameObject.Find("Delete " + (i - 1));

                DeleteButton = DeleButton.GetComponent<Button>();
                DeleteButton.name = "Delete " + i;

                if (i == 0)
                    newItem.SetActive(false);
            }

            GameObject MSG = GameObject.Find("message" + (i - 1));
            Text textMSG = MSG.GetComponent<Text>();
            textMSG.name = "message";

            GameObject DeleteBut = GameObject.Find("Delete " + (i - 1));
            Button DButton = DeleteBut.GetComponent<Button>();
            DButton.name = "Delete ";
        }
    }

    


    public void OnDeleteButtonClicked()
	{ //on delete a friend button clicked,
		//send the request to server
		//to get clicked button name (delete)
        string name = EventSystem.current.currentSelectedGameObject.name;
		Friends = Transverser.Friends;		//get friends
		Room_ID = Transverser.RoomID;		//get requested room id
        int SpaceIndex = name.IndexOf(" ");//get space index
		string DeleteButtonName = name.Substring(0, SpaceIndex);//get the decision 
		string DBIndex = name.Substring(SpaceIndex + 1);//get the request index in DB
		int index = int.Parse(DBIndex);//parse it to integer
        decision = DeleteButtonName;
		//get username of the requster
        username = Friends.GetSFSObject(index).GetUtfString("username");
        Debug.Log("username: " + username + " decision: " + decision + " room " + Room_ID);
		ISFSObject objOut = new SFSObject();
        objOut.PutUtfString("Room_ID", Room_ID);
        objOut.PutUtfString("username", username);
        objOut.PutUtfString("decision", decision);
        Debug.Log("username: " + username + " decision: " + decision + " room " + Room_ID);
		//send the delete friend decision to server
		NetworkManager.Instance.Decision(objOut,"DeleteFriend");
    }

	public void DeleteDecision(ISFSObject objIn)
	{
		string result = objIn.GetUtfString("Result");

        if (result == "Successful")
            Debug.Log("Successful");
        else
            Debug.Log("error");

    }//end extension     
}