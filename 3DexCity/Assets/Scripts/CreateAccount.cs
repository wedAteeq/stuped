using UnityEngine;
using Sfs2X;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Entities.Data;
using UnityEngine.UI;
using Sfs2X.Util;
 
public class CreateAccount : MonoBehaviour
{
    //----------------------------------------------------------
    // Private properties (Connection part)
    //----------------------------------------------------------
    private string ServerIP = "127.0.0.1";// Default host
    private int defaultTcpPort = 9933;// Default TCP port
    private int defaultWsPort = 8888;			// Default WebSocket port
    private string ZoneName = "3DexCityZone";
    private int ServerPort = 0;

    private SmartFox sfs;
    private string username;
    private string password;
    private string Conpassword;
    private string email;
    private string biography;
    private string firstname;
    private string lastname;
    private string Act_Room;
    private string Account_T;
    private string Avt;
    private int Error = 0;
    private int CreateRoom = 0;
    private int Room_ID = 1;
    private int updateAccount = 0;

    //----------------------------------------------------------
    // UI elements
    //----------------------------------------------------------
    public InputField UserName;
    public InputField Password;
    public InputField ConPassword;
	public Text TextMessage,TextMessage2;
    public InputField Email;
    public InputField FirstName;
    public InputField LastName;
    public InputField Biography;
    public Toggle ActivateRoom;
    public Toggle AccountType;
    public Toggle FAvatar;
    public Toggle MAvatar;
    public Transform Result;
    public Transform createAccount;
    public int RoomsNum;
    string CMD_Signup = "$SignUp.Submit";


    // Use this for initialization
    void Start()
    {
        TextMessage.text = "";
        enableInterface(true);
        UserName.text = "";
        Password.text = "";
        ConPassword.text = "";
        Email.text = "";
        FirstName.text = "";
        LastName.text = "";
    }

    // Update is called once per frame
    void Update()
    {
        // As Unity is not thread safe, we process the queued up callbacks on every frame
        if (sfs != null)
            sfs.ProcessEvents();
    }

    //----------------------------------------------------------
    // Public interface methods for UI
    //----------------------------------------------------------


    public void OnCreateAccountButtonClicked()
	{   //on create button clicked, all values are stored in variables then login to server 
        int usernameSpace, firstnameSpace, lastnameSpace;
        username = UserName.text;//to store username
        usernameSpace = username.IndexOf(" ");
        password = Password.text;//to store password
        Conpassword = ConPassword.text;//to store confirm password
        email = Email.text;///tpstore email
        biography = Biography.text;//to store bio
        firstname = FirstName.text;//to storefirst name
        firstnameSpace = firstname.IndexOf(" ");
        lastname = LastName.text;//to store last name
        lastnameSpace = lastname.IndexOf(" ");
        bool ARoom = ActivateRoom.isOn;//to store activate room
        if (ARoom == true)
            Act_Room = "Y";
        else
            Act_Room = "N";

		bool Account = AccountType.isOn;//to store account type (public, private)
        if (Account == true)
            Account_T = "private";
        else
            Account_T = "public";

        bool avatar = MAvatar.isOn;//to store avatar gender
        if (avatar == true)
            Avt = "M";
        else
            Avt = "F";

        int AdminIndex = username.IndexOf("n");
        string admin = username.Substring(0, AdminIndex + 1);

        if (requredFilled())//method to check that all required values filled
        {
            if (usernameSpace == -1 && firstnameSpace == -1 && lastnameSpace == -1)//to prevent spaces in names 
            {
                if (!admin.Equals("Admin"))//to prevent the name start with "Admin" 
                {
					if (password == Conpassword)//to check password and it's confirm are matching
                    {
                        if (email.IndexOf("@") != -1)//to validate email
                        {   // Enable interface
                            enableInterface(false);//method to disable typying in input field while proccesing
							///connect with server
							#if UNITY_WEBGL//type of building "WEBGL"
                        {
                         sfs = new SmartFox(UseWebSocket.WS);
                         ServerPort = defaultWsPort;
                        }
							#else //type of building "Web player"
                            {
                                sfs = new SmartFox();
                                ServerPort = defaultTcpPort;
                            }
                            #endif
                            sfs.ThreadSafeMode = true;//create threds
							//events listeners
                            sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
                            sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
                            sfs.AddEventListener(SFSEvent.LOGIN, OnLogin);
                            sfs.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
                            sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);
                            sfs.Connect(ServerIP, ServerPort);

							enableInterface(true);//method to enable typying in input field while proccesing
                            UserName.text = "";
                            Password.text = "";
                            ConPassword.text = "";
                            Email.text = "";
                            FirstName.text = "";
                            LastName.text = "";

                        }
                        else //error messages "invalid email"
                            TextMessage.text = "Invalid email account";
                    }
					else //error messages "password not equal to it's confirm"
                        TextMessage.text = "The password and its confirm are not matching";
                }
				else //error messages "username not start with admin"
                    TextMessage.text = "The username should not start with \"Admin\" string";
            }
			else//error messages "names must not contains spaces"
                TextMessage.text = "Username,firstname & lastname should not contains a space";
        }
		else//error messages "missing values"
            TextMessage.text = "Missing to fill required value";


    }//end create account



    private void reset()
    {
        // Remove SFS2X listeners
        sfs.RemoveAllEventListeners();

        sfs = null;

        // Enable interface
        enableInterface(true);
        UserName.text = "";
        Password.text = "";
        ConPassword.text = "";
        Email.text = "";
        FirstName.text = "";
        LastName.text = "";
    }

    private void OnConnectionLost(BaseEvent evt)
    {
        // Remove SFS2X listeners and re-enable interface
        reset();

        string reason = (string)evt.Params["reason"];

        if (reason != ClientDisconnectionReason.MANUAL)
        {
            // Show error message
            Debug.Log("Connection was lost; reason is: " + reason);
        }
    }

    private void OnExtensionResponse(BaseEvent evt)
    {
        Debug.Log("2");
        string cmd = (string)evt.Params["cmd"];
        ISFSObject objIn = (SFSObject)evt.Params["params"];

        string message;
        if (cmd == CMD_Signup)
        {
            Debug.Log("3");

            if (objIn.ContainsKey("success"))
            {
                message = "Signup Successful";
                Debug.Log(message);
                TextMessage.text = message;
            }
            else
            {
                Error = 1;
                message = objIn.GetUtfString("errorMessage");
                message = "Signup Error: " + message;
                 Debug.Log(message);
                TextMessage.text = message;
                reset();
            }
        }
        else if (Error == 0 && Act_Room == "Y" && CreateRoom == 0)
        {
            ISFSArray Rooms = objIn.GetSFSArray("Rooms");
            int length = Rooms.Size();
            Debug.Log("length : " + length);

            if (length < RoomsNum)
            {
                int i;
                if (length > 0)
                    for (int j = 1; j <= length; j++)
                    {
                        for (i = 1; i <= length; i++)
                        {
                            if (j == int.Parse(Rooms.GetSFSObject(i - 1).GetUtfString("Room_ID")))
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

                CreateRoom = 1;
                Debug.Log("activate room");
                Room room = new Room();
                room.createRoom(sfs, Room_ID, username, Account_T);

            }
            else
            {
				TextMessage2.text = "The account has been created successfully, But your room has not been created; since there is not any empty room.";
                 updateAccount = 1;
                ISFSObject objOut = new SFSObject();
                Act_Room = "N";
                password = PasswordUtil.MD5Password(password);//to incrypt the password
                objOut.PutUtfString("username", username);
                objOut.PutUtfString("account", "member");
                objOut.PutUtfString("password", password);
                objOut.PutUtfString("email", email);
                objOut.PutUtfString("firstName", firstname);
                objOut.PutUtfString("lastName", lastname);
                objOut.PutUtfString("biography", biography);
                objOut.PutUtfString("hasRoom", Act_Room);
                objOut.PutUtfString("accountType", Account_T);
                objOut.PutUtfString("avatar", Avt);

                sfs.Send(new ExtensionRequest("UpdateAccount", objOut));
            }
        }
        else if (Error == 0 && Act_Room == "Y" && CreateRoom == 1)
        {
            CreateRoom = 0;
            string result = objIn.GetUtfString("CreateRoomResult");

            if (result == "Successful")
            {
                Debug.Log("Successful");
				TextMessage2.text = "The account has been created successfully. Your room Id is " + Room_ID;
              }
            else
                Debug.Log("error");
          }
        else if (Error == 0 && updateAccount == 1)
        {
            string result = objIn.GetUtfString("UpdateResult");
            if (result == "Successful")
                Debug.Log("Successful");
            else
                Debug.Log("error");
            updateAccount = 0;
        }

        if (Error == 0)
        {
            Result.gameObject.SetActive(true);
            createAccount.gameObject.SetActive(false);
        }
    }

    private void OnLoginError(BaseEvent evt)
    {
        // Disconnect
        sfs.Disconnect();

        // Remove SFS2X listeners and re-enable interface
        reset();

        // Show error message
        Debug.Log("Login failed: " + (string)evt.Params["errorMessage"]);
    }

    private void OnConnection(BaseEvent evt)
    {
        if ((bool)evt.Params["success"])
        {
            Debug.Log("Successfully Connected!");
            sfs.Send(new LoginRequest("", "", ZoneName));
        }
        else
        {
            Debug.Log("Connection Failed!");
            // Remove SFS2X listeners and re-enable interface
            reset();

            // Show error message
            TextMessage.text = "Connection failed !";
        }
    }

    private void OnLogin(BaseEvent evt)
    {   //this method will send values to server in order to create account
        Debug.Log("Logged In: " + evt.Params["user"]);
        Error = 0;

        ISFSObject objOut = new SFSObject();
        Transverser.MemberUsername = username;
        Transverser.MemberEmail = email;

        objOut.PutUtfString("username", username);//store username
        objOut.PutUtfString("password", password);//store password
        objOut.PutUtfString("email", email);//store email
        objOut.PutUtfString("firstName", firstname);//store first name
        objOut.PutUtfString("lastName", lastname);//store last name
        objOut.PutUtfString("biography", biography);//store bio
        objOut.PutUtfString("hasRoom", Act_Room);//store has room values
        objOut.PutUtfString("accountType", Account_T);//store account type
        objOut.PutUtfString("avatar", Avt);//store avatar gender
        sfs.Send(new ExtensionRequest(CMD_Signup, objOut));


        Debug.Log("1");

        Room room = new Room();
        room.getAllRooms(sfs);
    }


    private void enableInterface(bool enable)
    {
        UserName.interactable = enable;
        Password.interactable = enable;
        ConPassword.interactable = enable;
        Email.interactable = enable;
        FirstName.interactable = enable;
        LastName.interactable = enable;
        ActivateRoom.interactable = enable;
        AccountType.interactable = enable;

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
