using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Entities.Data;
using UnityEngine.UI;
using Sfs2X.Util;
using UnityEngine;
using UnityEditor;

public class AddAdmin : MonoBehaviour
{
    //----------------------------------------------------------
    // Private properties (Connection part)
    //----------------------------------------------------------
    private string username;
    private string password;
    private string Conpassword;
    private string email;
    private string biography;
    private string firstname;
    private string lastname;

    //----------------------------------------------------------
    // UI elements
    //----------------------------------------------------------
    public InputField UserName;
    public InputField Password;
    public InputField ConPassword;
    public Text TextMessage;
    public InputField Email;
    public InputField FirstName;
    public InputField LastName;
    public InputField Biography;

    


	private static AddAdmin instance;
	public static AddAdmin Instance
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

    // Use this for initialization
    void Start()
    {
        TextMessage.text = "";

        UserName.text = "";
        Password.text = "";
        ConPassword.text = "";
        Email.text = "";
        Biography.text = "";
        FirstName.text = "";
        LastName.text = "";
        enableInterface(true);
    }

    // Update is called once per frame
   
    //----------------------------------------------------------
    // Public interface methods for UI
    //----------------------------------------------------------


    public void OnCreateAccountButtonClicked()
    {   //on create account button clicked
        int usernameSpace, firstnameSpace, lastnameSpace;
        username = UserName.text;//store username
        usernameSpace = username.IndexOf(" ");//get space index 
        password = Password.text;//store password
        Conpassword = ConPassword.text;//store confirm password 
        email = Email.text;//store email
        biography = Biography.text;//store bio
        firstname = FirstName.text;//store first name 
		firstnameSpace = firstname.IndexOf(" ");//get space index
        lastname = LastName.text;//store last name
		lastnameSpace = lastname.IndexOf(" ");//get space index
        if (requredFilled())//method to check all required values filled
        {   //to prevent spaces in names
            if (usernameSpace == -1 && firstnameSpace == -1 && lastnameSpace == -1)
            {  //check password and its confirm are matching
                if (password == Conpassword)
                {///validate eamil
                    if (email.IndexOf("@") != -1)
                    {   // method to Enable interface
                        enableInterface(false);
						ISFSObject objOut = new SFSObject();
						int AdminIndex = username.IndexOf("n");//get "n" index
						string admin = username.Substring(0, AdminIndex + 1);
						//to check that all admin username start with "Admin"
						if (admin.Equals("admin"))
							username = "A" + username.Substring(1);
						else if (!admin.Equals("Admin"))
							username = "Admin" + username;
						password = PasswordUtil.MD5Password(password);//to incrypt the password
						objOut.PutUtfString("username", username);//set username
						objOut.PutUtfString("password", password);//set password
						objOut.PutUtfString("email", email);//set email
						objOut.PutUtfString("firstName", firstname);//set firstname
						objOut.PutUtfString("lastName", lastname);//set lastname
						objOut.PutUtfString("biography", biography);//set bio
						objOut.PutUtfString("isAdmin", "Y");//set the account is admin
						NetworkManager.Instance.AddNewAdmin(objOut);//send request to server
                    }
                    else//error message "invali email"
                        TextMessage.text = "Invalid email account";
                }
				else//error message "password and its confirm are not matching"
                    TextMessage.text = "The password and its confirm are not matching";
            }
            else//error message "spaces in names"
                TextMessage.text = "Username,firstname & lastname should not contains a space";
        }
		else //error message"missing values"
            TextMessage.text = "Missing to fill required value";
    }//end create account




	public void AddNewAdminResult(ISFSObject objIn)
    {   //createAccount.gameObject.SetActive(false);
		//SuccesResult.gameObject.SetActive(true);
		enableInterface(true);
		string result = objIn.GetUtfString("Result");
		if (result == "Successful" ) {//if account updated successfully
			Debug.Log ("Successful");
			TextMessage.text ="The admin account has been created successfully";
		} else {//if account not updated successfully
			Debug.Log ("error");
			TextMessage.text ="Sorry, the admin account has not been created";
		}
    }

    private void enableInterface(bool enable)
    {
        UserName.interactable = enable;
        Password.interactable = enable;
        ConPassword.interactable = enable;
        Email.interactable = enable;
        FirstName.interactable = enable;
        LastName.interactable = enable;


        TextMessage.text = "";
    }

    private bool requredFilled()
    {
        if (username == "" || username == " " || password == "" || password == " " || Conpassword == "" || Conpassword == " " || email == "" || email == " ")
            return false;
        else
            return true;

    }

}
