using UnityEngine;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Entities.Data;
using UnityEngine.UI;
using Sfs2X.Util;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class manageMemberAccount : MonoBehaviour
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
    public Toggle ActivateRoom;
    public Toggle AccountType;
    public Toggle FAvatar;
    public Toggle MAvatar;
    public int RoomsNum;


    private int CreateRoom = 0;
    private int DeleteRoom = 0;
    private int Room_ID = 1;
    private string pass;
    private string previousHasRoom;

    //----------------------------------------------------------
    // Unity calback methods
    //----------------------------------------------------------

    // Use this for initialization
    private static manageMemberAccount instance;
    public static manageMemberAccount Instance
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

    // Update is called once per frame


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

	public void ViewMemberAccount(ISFSObject objIn)
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


		if (useraccountinfo.GetSFSObject(0).GetUtfString("hasRoom").Equals("Y"))
			ActivateRoom.isOn = true;//turn on room radio button
		else
			ActivateRoom.isOn = false;//turn off room radio button

		previousHasRoom = useraccountinfo.GetSFSObject(0).GetUtfString("hasRoom");//to store value

		if (useraccountinfo.GetSFSObject(0).GetUtfString("accountType").Equals("private"))
			AccountType.isOn = true;//turn on account type radio button
		else
			AccountType.isOn = false;//turn off account type radio button

		if (useraccountinfo.GetSFSObject(0).GetUtfString("avatar").Equals("F"))
		{
			FAvatar.isOn = true;//turn on female avatar radio button
			MAvatar.isOn = false;//turn off male avatar radio button
		}

		else
		{
			MAvatar.isOn = true;//turn on male avatar radio button
			FAvatar.isOn = false;//turn off female avatar radio button
		}

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
			UserInterfaces.Instance.onConfirmDeleteAccount ();
		}
		else
		{
			Debug.Log("error");
			TextMessage.text = "Your account has not been deleted";
 
		}
	}

    
    

    public void OnUpdateButtonClocked()
	{  //on update button clicked, this values send to server to save them in DB
        int firstnameSpace, lastnameSpace;

        string Act_Room, Avt, Account_T, password;

            Debug.Log("update account");
		if (requredFilled())//methos to check all required values filled
            {
                firstnameSpace = FirstName.text.IndexOf(" ");
                lastnameSpace = LastName.text.IndexOf(" ");
			if (firstnameSpace == -1 && lastnameSpace == -1)//to make sure that names do not contains spaces
                {
				if (View_Password.text == ConPassword.text)////to make sure that password and its confirm match
                    {
                         ISFSObject objOut = new SFSObject();
                        if (ActivateRoom.isOn == true)//store activate room request
                            Act_Room = "Y";
                        else
                            Act_Room = "N";

				    	if (AccountType.isOn == true)//store accout type request
                            Account_T = "private";
                        else
                            Account_T = "public";

                        if (FAvatar.isOn == true) //store avatar gender
                            Avt = "F";
                        else
                            Avt = "M";

                        if (pass == View_Password.text)
						   password = pass;//if password not change, take it as it is
                        else
						   password = PasswordUtil.MD5Password(View_Password.text);//if password changed, encrypt it

					    Room[] rooms = Transverser.Rooms;//get rooms from class Transverser store all rooms in city in rooms array
                        int length = rooms.Length;
                        Debug.Log("length : " + length);
					    TextMessage.text = "";
                        if (length == RoomsNum && previousHasRoom == "N" && Act_Room == "Y")
                        {
                            Act_Room = "N";//since there no empty room 
						    TextMessage.text ="Sorry, your request to have room did not activated, since there is not any empty room.\n";
                            //EditorUtility.DisplayDialog("Waring Message", "Sorry, there is not any empty room.", "ok");
                        }

                        objOut.PutUtfString("username", username);//store username
                        objOut.PutUtfString("account", "member");//user type
                        objOut.PutUtfString("password", password);//store password
                        objOut.PutUtfString("firstName", FirstName.text);//store first name
                        objOut.PutUtfString("lastName", LastName.text);//store last name
                        objOut.PutUtfString("biography", Biography.text);//store bio
                        objOut.PutUtfString("hasRoom", Act_Room);//store has room request or not
					    objOut.PutUtfString("accountType", Account_T);//store account type (private , public)
                        objOut.PutUtfString("avatar", Avt);//store avatar gender
    					NetworkManager.Instance.UpdateAccount(objOut);//send them to server

                        if (Avt == "M")//acctivate choosen avatar
                        {
						//respwan avatar
						
                           // BoyAvatar.gameObject.SetActive(true);
                          //  GirlAvatar.gameObject.SetActive(false);
                        }
                        else
                        {
						//respwan avatar
                          //  BoyAvatar.gameObject.SetActive(false);
                          //  GirlAvatar.gameObject.SetActive(true);
                        }

                        if (length < RoomsNum && previousHasRoom == "N" && Act_Room == "Y")
                        {//if there is an empty room
                            Debug.Log("activate room");
                            int i;
                            if (length > 0)//to find room id
                                for (int j = 1; j <= length; j++)
                                {
                                    for (i = 1; i <= length; i++)
                                    {
                                        if (j == rooms[i - 1].getRoomId())
                                            break;
                                    }
                                    if (i == length)
                                    {
                                        Room_ID = j + 1;
                                        break;
                                    }
                                    else if (i > length)
                                    {
                                        Room_ID = j;
                                        break;
                                    }
                                }

                            Debug.Log("activate room");
                            Room room = new Room();
                            room.CreateRoom(Room_ID, username, Account_T);//to create room

                        }
                        else if (previousHasRoom == "Y" && Act_Room == "N")
                        {
                            Debug.Log("delete room");
                            Room room = new Room();
                            room.DeleteRoom(username);//to delete room
                        }
                    }
                    else
                        TextMessage.text = "The password and its confirm are not matching";
                }
                else
                    TextMessage.text = "firstname & lastname should not contains a space";
            }
            else
                TextMessage.text = "Missing to fill required value";
        
    }

    private bool requredFilled()
    {
        if (View_Password.text == "" || View_Password.text == " " || ConPassword.text == "" || ConPassword.text == " ")
            return false;
        else
            return true;
    }

	public void UpdateAccount(ISFSObject objIn)
	{///method to display edit request result  
		string result = objIn.GetUtfString("UpdateResult");
		if (result == "Successful." + username ) {//if account updated successfully
			Debug.Log ("Successful");
				TextMessage.text += "Your account updated successfully.";
		} else {//if account not updated successfully
			Debug.Log ("error");
			TextMessage.text = "Your account has not been updated.";
		}
	}

	public void createRoom(ISFSObject objIn)
	{
		string result = objIn.GetUtfString("CreateRoomResult");
		if (result == "Successful." + username ) {//if account updated successfully
			Debug.Log ("Successful");
			TextMessage.text +="\nYour room has been created and it's id is " + Room_ID;
		} else {//if account not updated successfully
			Debug.Log ("error");
			TextMessage.text +="\nYour room has not been created";
		}
	}

	public void deleteRoom(ISFSObject objIn)
	{
		string result = objIn.GetUtfString("DeleteRoom");
 		if (result == "Successful." + username ) {//if account updated successfully
			Debug.Log ("Successful");
			TextMessage.text +="\nYour room has been deleted " ;
		} else {//if account not updated successfully
			Debug.Log ("error");
			TextMessage.text +="\nYour room has not been deleted ";
		}
	}
}