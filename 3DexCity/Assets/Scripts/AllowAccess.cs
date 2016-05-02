using UnityEngine;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Sfs2X;
using Sfs2X.Util;
using Sfs2X.Core;
using Sfs2X.Requests;
using Sfs2X.Entities.Data;

public class AllowAccess : MonoBehaviour
{
    //----------------------------------------------------------
    // Private properties (Connection part)
    //----------------------------------------------------------
    private string ServerIP = "127.0.0.1";// Default host
    private int defaultTcpPort = 9933;// Default TCP port
    private int defaultWsPort = 8888;           // Default WebSocket port
    private string ZoneName = "3DexCityZone";
    private int ServerPort = 0;

    private SmartFox sfs;
    private string Room_ID;

    public InputField UserName;//get its value from login

  
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


    public void CanAccess()
    {
        //if ( Transverser.enter == 0)
        //{enter
        //Transverser.enter = 1;
        //if any avatar enters any  room, get its id and store it in variable name Room_ID
        //if it's Room_ID == Transverser.MyRoomID 
        //then removet the door and the bell and allow manging its content
        //else if the room door is active
        //{ 
        //then check if the user is friend to this room from the db

#if UNITY_WEBGL
		{
		sfs = new SmartFox(UseWebSocket.WS);
		ServerPort = defaultWsPort;
		}
#else
        {
            sfs = new SmartFox();
            ServerPort = defaultTcpPort;
        }
#endif

        sfs.ThreadSafeMode = true;
        sfs.AddEventListener(SFSEvent.CONNECTION, OnConnection);
        sfs.AddEventListener(SFSEvent.CONNECTION_LOST, OnConnectionLost);
        sfs.AddEventListener(SFSEvent.LOGIN, OnLogin);
        sfs.AddEventListener(SFSEvent.LOGIN_ERROR, OnLoginError);
        sfs.AddEventListener(SFSEvent.EXTENSION_RESPONSE, OnExtensionResponse);

        sfs.Connect(ServerIP, ServerPort);
        //}
        //enter}
        //else 
        //{ 
        //Transverser.enter =0;
        //if the room is private, then return the door and bell
        // for (int i=0;i<Transverser.Rooms.Length;i++)
        //  if (Transverser.Rooms[i].getRoomId()+""==Room_ID && Transverser.Rooms[i].getaccountType()=="private" )
        // return the door and bell
        ///
        //}else

    }//end

    private void OnConnection(BaseEvent evt)
    {
        if ((bool)evt.Params["success"])
        {
            // Login
            Debug.Log("Successfully Connected!");

            sfs.Send(new LoginRequest("", "", ZoneName));
        }
        else
        {
            Debug.Log("Connection Failed!");
            // Remove SFS2X listeners and re-enable interface
            reset();

            // Show error message
            Debug.Log("Connection failed; is the server running at all?");
        }
    }

    private void OnLogin(BaseEvent evt)
    {
        Debug.Log("Logged In: " + evt.Params["user"]);

        ISFSObject objOut = new SFSObject();
        objOut.PutUtfString("Room_ID", Room_ID);
        objOut.PutUtfString("username", UserName.text);
        sfs.Send(new ExtensionRequest("ViewFriends", objOut));
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
    }//end

    private void OnLoginError(BaseEvent evt)
    {    // Show error message
        string message = (string)evt.Params["errorMessage"];
        Debug.Log("Login failed: " + message);

        // Disconnect
        sfs.Disconnect();

        // Remove SFS2X listeners and re-enable interface
        reset();
    }

    private void OnExtensionResponse(BaseEvent evt)
    {
        Debug.Log("extension");
        ISFSObject objIn = (SFSObject)evt.Params["params"];
        ISFSArray Friend = objIn.GetSFSArray("Friends");

        //if (Friend.Size()!=0)
        //then remove the door and bell


    }//end extension


    private void reset()
    {
        // Remove SFS2X listeners
        sfs.RemoveAllEventListeners();

        sfs = null;
    }


}
