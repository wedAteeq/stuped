using UnityEngine;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Entities.Data;
using UnityEngine.UI;
using Sfs2X.Util;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEditor;

public class manageAdminAccount : MonoBehaviour
{

    //----------------------------------------------------------
    // Private properties (Connection part)
    //----------------------------------------------------------
    private string username;

    //----------------------------------------------------------
    // UI elements
    //----------------------------------------------------------
    public InputField UserName;
    public Text TextMessage;
  

    public InputField View_username;
    public InputField View_Password;
    public InputField ConPassword;
    public InputField Email;
    public InputField FirstName;
    public InputField LastName;
    public InputField Biography;


    private string pass;

    //----------------------------------------------------------
    // Unity calback methods
    //----------------------------------------------------------

    // Use this for initialization
	private static manageAdminAccount instance;
	public static manageAdminAccount Instance
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

    void Start()
    {
        TextMessage.text = "";
    }

 

    //----------------------------------------------------------
    // Public interface methods for UI
    //----------------------------------------------------------

	public void OnViewMyAccountButtonclicked()
	{   //on button view my account clicked by a member
		//it will request the account info from the server
		username = UserName.text;//variable to store username of the member
 		TextMessage.text = "";//displayed error message
		ISFSObject objOut = new SFSObject();
		objOut.PutUtfString("username", username);
		NetworkManager.Instance.GetAccountInfo(objOut,"");
	}

    private void enableInterface(bool enable)
    {
        TextMessage.text = "";

    }


	public void ViewAdminAccount(ISFSObject objIn)
	{   //method to display user account info after it retrieve the info from server
		ISFSArray useraccountinfo = objIn.GetSFSArray("account");//object that contains all info
		View_username.text = useraccountinfo.GetSFSObject(0).GetUtfString("username");//display username
		pass = useraccountinfo.GetSFSObject(0).GetUtfString("password");
		View_Password.text = pass;//display password as **** and encrypted
		ConPassword.text = pass;//display confirm password as **** and encrypted
		Email.text = useraccountinfo.GetSFSObject(0).GetUtfString("email");//display email
		FirstName.text = useraccountinfo.GetSFSObject(0).GetUtfString("firstName");//display frstname
		if (useraccountinfo.GetSFSObject(0).GetUtfString("biography") == null)//display bio
			Biography.text = "";
		else
			Biography.text = useraccountinfo.GetSFSObject(0).GetUtfString("biography");

		LastName.text = useraccountinfo.GetSFSObject(0).GetUtfString("lastName");//display lastname
		Email.interactable = false;//disable changing email
		View_username.interactable = false;//disable changing username
	}


      

	public void OnDeleteButtonClocked()
	{//start
		//on delete button clicked, the request
		//to delete the account send to the server
		ISFSObject objOut = new SFSObject();
		objOut.PutUtfString("username", username);
		NetworkManager.Instance.DeleteAccount(objOut,"");

	}//end delte account


	public void DeleteAccount(ISFSObject objIn)
	{   ///method to display delete request result
		string result = objIn.GetUtfString("DeleteResult");

		if (result == "Successful."+username)
		{
			Debug.Log("Successful");
			TextMessage.text = "Your account deleted successfully";
			//	EditorUtility.DisplayDialog("Waring Message", "         Your account deleted successfully", "ok");
 		//	Home.gameObject.SetActive(true);//to display home page 
		//	Delete.gameObject.SetActive(false);//to close curent delete form
			UserInterfaces.Instance.onAdminConfirmDeleteAccount();
		}
		else
		{
			Debug.Log("error");
			TextMessage.text = "Your account has not been deleted";
			//EditorUtility.DisplayDialog("Waring Message", "         Your account has not been deleted", "ok");

		}
	}



	public void OnUpdateButtonClocked()
    {  //on update button clicked, this values send to server to save them in DB
        int firstnameSpace, lastnameSpace;
        string password;
             Debug.Log("update account");
            if (requredFilled())//methos to check all required values filled
            {   firstnameSpace = FirstName.text.IndexOf(" ");
                lastnameSpace = LastName.text.IndexOf(" ");
                if (firstnameSpace == -1 && lastnameSpace == -1)//to make sure that names do not contains spaces
                {
				if (View_Password.text == ConPassword.text)////to make sure that password and its confirm match
                    { ISFSObject objOut = new SFSObject();
                        if (pass == View_Password.text)
                            password = pass;//if password not change, take it as it is
                        else
						     password = PasswordUtil.MD5Password(View_Password.text);//if password changed, encrypt it
                        objOut.PutUtfString("username", username);
                        objOut.PutUtfString("account", "Admin");
                        objOut.PutUtfString("password", password);
                        objOut.PutUtfString("firstName", FirstName.text);
                        objOut.PutUtfString("lastName", LastName.text);
                        objOut.PutUtfString("biography", Biography.text);
						NetworkManager.Instance.UpdateAccount(objOut);
                    }
                    else //error mesages "not maching passwords"
                        TextMessage.text = "The password and its confirm are not matching";
                }
			else//error mesages "spaces in names"
                    TextMessage.text = "firstname & lastname should not contains a space";
            }
		else  //error mesages "missing required values"
                TextMessage.text = "Missing to fill required value";
    }

	public void UpdateAccount(ISFSObject objIn)
	{///method to display edit request result  
		string result = objIn.GetUtfString("UpdateResult");
		if (result == "Successful." + username) {//if account updated successfully
			Debug.Log ("Successful");
			TextMessage.text = "Your account updated successfully";

		} else {//if account not updated successfully
			Debug.Log ("error");
			TextMessage.text = "Your account has not been updated";
		}
	}

    private bool requredFilled()
    {
        if (View_Password.text == "" || View_Password.text == " " || ConPassword.text == "" || ConPassword.text == " ")
            return false;
        else
            return true;
    } 
}