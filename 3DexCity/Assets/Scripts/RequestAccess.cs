using UnityEngine;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Entities.Data;
using UnityEngine.UI;

public class RequestAccess : MonoBehaviour
{
    //----------------------------------------------------------
    // Private properties (Connection part)
    //----------------------------------------------------------
   
    private string username,RoomId;
    //----------------------------------------------------------
    // UI elements
    //----------------------------------------------------------
    public InputField UserName;
    public Button Room_ID;



	private static RequestAccess instance;
	public static RequestAccess Instance
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

    public void OnRequesAccessButtonClicked()
    {//on button request access private room clicked 
        username = UserName.text;//get username
		RoomId =Room_ID.name;//get room ID
		ISFSObject objOut = new SFSObject();
		objOut.PutUtfString("Room_ID", RoomId);
		objOut.PutUtfString("username", username);
		NetworkManager.Instance.RequestAccessRoom(objOut);//send request to server

    }

  
   

	public void RequestAccessPRoom(ISFSObject objIn)
    {
         string result = objIn.GetUtfString("RequestResult");

            if (result == "Successful")
                Debug.Log("Successful");
            else
                Debug.Log("error");
    }

}
