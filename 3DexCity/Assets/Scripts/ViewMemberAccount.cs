using UnityEngine;
using UnityEngine.UI;
using Sfs2X;
using Sfs2X.Util;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Entities.Data;
 
public class ViewMemberAccount : MonoBehaviour
{
 
     private string username;

    //----------------------------------------------------------
    // UI elements
    //----------------------------------------------------------
    public InputField UserName;
	public Text TextMessage,TextMessage2;
   
    public Text View_username;
    public Text Email;
    public Text FirstName;
    public Text LastName;
    public Text accountType, act_code, active;
    public Text ActivateRoom, Biography;
    public Text Avatar;



    //----------------------------------------------------------
    // Unity calback methods
    //----------------------------------------------------------

    // Use this for initialization
    void Start()
    {
        TextMessage.text = "";

    }

    // Update is called once per frame


    //----------------------------------------------------------
    // Public interface methods for UI
    //----------------------------------------------------------
	private static ViewMemberAccount instance;
	public static ViewMemberAccount Instance
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

    public void OnViewMemberAccountButtonclicked()
    {   ///on button view member account clicked, display the user info  
        username = UserName.text;//take username from the admin
        TextMessage.text = "";
        View_username.text = "UserName:\n";//display username
        Email.text = "Email:\n";//dispaly email
        FirstName.text = "First Name:\n";//display first name
        LastName.text = "Last Name:\n";//display last name
        Biography.text = "Biography:\n";//display bio
        ActivateRoom.text = "Has Room:\n";//display has room value
        Avatar.text = "Avatar Gender:\n";//display avatar gender
        accountType.text = "Account Type:\n";//display account tyoe
        act_code.text = "Activation Code:\n";//display activation code
        active.text = "Activate Account:\n";//display satus of actiation

        int AdminIndex = username.IndexOf("n");
        string admin = username.Substring(0, AdminIndex + 1);
		if (!admin.Equals ("Admin")) 
		{//if the username is not admin username
			Debug.Log ("view account");
			ISFSObject objOut = new SFSObject();
			objOut.PutUtfString("username", username);
			NetworkManager.Instance.GetAccountInfo(objOut,"AdminRequest");//send request to server
		}
		else //if username is admin username, the admin cannot view his/her accout info
            TextMessage.text = "Sorry, You can not view any admin account only members";
    }//end

	public void ViewAccountInfo(ISFSObject objIn)
	{ ///to display the info after retrieve them from server
		ISFSArray useraccountinfo = objIn.GetSFSArray("account");
		if (useraccountinfo.Size() == 0)//if the username invalid
 			TextMessage.text = "Invalid username";
		else
		{  // viewPage.gameObject.SetActive(true);//display view info form
			//view.gameObject.SetActive(false); //close request to view account form 
			UserInterfaces.Instance.OnAdminViewedMemberAccount();
			View_username.text = View_username.text + useraccountinfo.GetSFSObject(0).GetUtfString("username");//display username
			Email.text = Email.text + useraccountinfo.GetSFSObject(0).GetUtfString("email");//display email
			FirstName.text = FirstName.text + useraccountinfo.GetSFSObject(0).GetUtfString("firstName");//display first name
			if (useraccountinfo.GetSFSObject(0).GetUtfString("biography") == null)//display bio
				Biography.text = Biography.text + " null";
			else
				Biography.text = Biography.text + useraccountinfo.GetSFSObject(0).GetUtfString("biography");
			LastName.text = LastName.text + useraccountinfo.GetSFSObject(0).GetUtfString("lastName");//display last name
			if (useraccountinfo.GetSFSObject(0).GetUtfString("hasRoom").Equals("Y"))//display has room value
				ActivateRoom.text = ActivateRoom.text + " Yes";
			else
				ActivateRoom.text = ActivateRoom.text + " No";
				if (useraccountinfo.GetSFSObject(0).GetUtfString("avatar").Equals("F"))//dsplay afatar gender
				Avatar.text = Avatar.text + " Female";
			else
				Avatar.text = Avatar.text + " Male";
			accountType.text = accountType.text + useraccountinfo.GetSFSObject(0).GetUtfString("accountType");//display account type
			act_code.text = act_code.text + useraccountinfo.GetSFSObject(0).GetUtfString("act_code");//display activation code
			if (useraccountinfo.GetSFSObject(0).GetUtfString("active").Equals("Y"))//display activation status
				active.text = active.text + " Yes";
			else
				active.text = active.text + " No";
		}
	}
   

    public void OnDeleteButtonClocked()
	{ //start
	//on delete button clicked, the request
	//to delete the account send to the server
	ISFSObject objOut = new SFSObject();
	objOut.PutUtfString("username", username);
	NetworkManager.Instance.DeleteAccount(objOut,"AdminRequest");

  } //end delte account

	public void DeleteAccount(ISFSObject objIn)
	{   ///method to display delete request result
		string result = objIn.GetUtfString("DeleteResult");
		if (result == "Successful."+username)
		{//if request was successfully
			Debug.Log("Successful");
			//display request result 
 			TextMessage2.text ="The account has been deleted successfully: " + username;
			//ConfirmDelete.gameObject.SetActive(true);//display admin view
			//viewPage.gameObject.SetActive(false);//close view request page 
			//Delete.gameObject.SetActive(false);//close confirm delete page
			UserInterfaces.Instance.ViewSuccessDeletemember();
		}
		else
		{
			Debug.Log("error");
			TextMessage.text = "the account has not been deleted";//display request result 
 		}
	}

    


    private void enableInterface(bool enable)
    {
        TextMessage.text = "";

    }


}