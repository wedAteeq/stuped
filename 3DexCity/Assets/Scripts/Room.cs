using UnityEngine;
using UnityEditor;
using System.Collections;
using Sfs2X;
using Sfs2X.Requests;
using Sfs2X.Entities.Data;
using UnityEngine.UI;



public class Room : MonoBehaviour
{


    public Animator anim; //to attach the avatar
    Collider FramePic; //referrnse to frame of picture
    string filePath = ""; //the path of the picture 
    Texture2D texture; //the picture itself
    int flag; // i will use it for arrange
    public Material Clear;     //matrial that in delete 
    public GameObject add;     //for add button 
    public GameObject delete;  //for delete button
    public GameObject arrange; //for arrange button 
    private SmartFox sfs;
    private string userName, accountType;
    private int RoomId;
    string RoomName; //the name of the room 
    string RoomIDInTheCity; //the ID of the room
    string name; // i need this in arrange 
    Collider FramePicforArrange; //referrnse to frame of picture
    Material temp; //i need this in arrange to swap between texture
    public Text test;
    string checkedMessage; //i need it for displaying message when the avatar in the room
	private static Room instance;
	public static Room Instance
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
        anim = GetComponent<Animator>(); //define the avatar
        add.SetActive(false);
        delete.SetActive(false);
        arrange.SetActive(false);
        name = null;
        checkedMessage ="no";
    }



    public int getRoomId() { return RoomId; }
    //Manage Room (Add/Delete/Like/Arrang contents) 
    public Room()
    {
        RoomId = 0;
        userName = "";
        accountType = "";
    }
    public Room(int RoomID, string user, string accType)
    {
        RoomId = RoomID;
        userName = user;
        accountType = accType;
    }
    // Use this for initialization

    void OnTriggerEnter(Collider other) //if the user touch anu collider

    {
        if (other.gameObject.CompareTag("Frame"))  //  to know which fram will be affect  

        {

            if (name == null)
            {
                FramePic = other; //assign the touced object to FramePic
                name = other.gameObject.name;
            }

            else
            {
                FramePicforArrange = other;
                name = null;
            } 


        }

        if (other.gameObject.CompareTag("ActiveGround"))
        {
            
             if (RoomIDInTheCity == Transverser.MyRoomID) //if the room is his room show this the buttons
            {
            add.SetActive(true);
            delete.SetActive(true);
            arrange.SetActive(true);
             
                if (checkedMessage == "no")
                {
                    if (EditorUtility.DisplayDialog("Instructions", "If you want to take advantage of the characteristics of your room (add, delete, arrange) Please clung to the fram that you want to make changes to it", "OK", "don't show this again"))
                        checkedMessage = "no";
                    else
                    {
                        checkedMessage = "yes";
                    }
                }
            }
        }


        if (other.gameObject.CompareTag("DisactiveGround"))
        {
            RoomName=other.gameObject.name; //to tack the name of object
            int index= RoomName.IndexOf("#"); //to know the postion of the room  
            RoomIDInTheCity = RoomName.Substring(index+1); //to know the id 
            test.text = RoomIDInTheCity;
            add.SetActive(false);
            delete.SetActive(false);
            arrange.SetActive(false);
        }


    }

    public void AddContents()

    {
#if UNITY_EDITOR
        filePath = EditorUtility.OpenFilePanel("Overwrite with png"
                                            , Application.streamingAssetsPath
                                            , "png");  //to open panel so the user will choose picture from his pc and save file path
#endif
        if (filePath.Length != 0)  //if the user choose picture that means filePath not empty
        {
            WWW www = new WWW("file://" + filePath);
            texture = new Texture2D(64, 64);
            www.LoadImageIntoTexture(texture);  //load image as Texture
            FramePic.GetComponent<Renderer>().material.mainTexture = texture; //assign Texture to FramePic (touced object)

        }

        name = null;
        FramePic = null;

    }


    public void DeleteContents()

    {

        
            if (EditorUtility.DisplayDialog("Warning Message", "Are you sure you want to delete this content?", "OK", "Cancel"))
            {

            FramePic.GetComponent<Renderer>().material.mainTexture = null; //delete the texture 
            FramePic.GetComponent<Renderer>().material = Clear; //assign this standerd matrial 
            
        }

        name = null;
        FramePic = null;


    }

    public void ArrangeContents()
    {



        if (EditorUtility.DisplayDialog("Warning Message", "Are you sure you want to rearrange this fram?", "OK", "Cancel"))
        {

            EditorUtility.DisplayDialog("Warning Message", "please select the second fram that you want to replace with", "OK");
           // FramePic.transform.parent = anim.transform; //make the frame child of the avatar 
            //FramePic.transform.position = anim.transform.forward + anim.transform.up + anim.transform.up + anim.transform.position; //to make the object in frount of the avatar
            //FramePic.transform.rotation = new Quaternion (0f,180f,0f,0f); 


        }

        else
        {
            name = null;
            FramePic = null;
            //EditorUtility.DisplayDialog("Warning Message", "Please press Z if you find the appropriate postion for this frame ", "OK");
        }

    }


    void Update()
    {
        // As Unity is not thread safe, we process the queued up callbacks on every frame
        if (sfs != null)
            sfs.ProcessEvents();

        //I will use when the user carry the frame
        //if (Input.GetKey(KeyCode.Z))
           // FramePic.transform.parent = null;
           if (name != null)

           {
            temp.SetTexture("",FramePic.GetComponent<Renderer>().material.mainTexture) ;
            FramePic.GetComponent<Renderer>().material.SetTexture("", FramePicforArrange.GetComponent<Renderer>().material.mainTexture);
            FramePicforArrange.GetComponent<Renderer>().material.SetTexture("", temp.mainTexture);
            name = null;
           }

    }


    public void CreateRoom( int Room_ID, string user, string account)
    {
        Debug.Log("in method ");
        userName = user;
        accountType = account;
        ISFSObject objOut = new SFSObject();
        objOut.PutUtfString("Room_ID", Room_ID + "");
        objOut.PutUtfString("username", userName);
        objOut.PutUtfString("accountType", accountType);
		NetworkManager.Instance.CreateRoom(objOut);


    }
	public void createRoom( SmartFox sfs2x,int Room_ID, string user, string account)
	{
		sfs = sfs2x;

		Debug.Log("in method ");
		userName = user;
		accountType = account;
		ISFSObject objOut = new SFSObject();
		objOut.PutUtfString("Room_ID", Room_ID + "");
		objOut.PutUtfString("username", userName);
		objOut.PutUtfString("accountType", accountType);
		sfs.Send(new ExtensionRequest("createRoom", objOut));
	}

    public void DeleteRoom( string user)
    {
        Debug.Log("in method ");
        userName = user;
        ISFSObject objOut = new SFSObject();
        objOut.PutUtfString("username", userName);
		NetworkManager.Instance.DeleteRoom(objOut);
    }

    public void getAllRooms(SmartFox sfs2x)
    {
        sfs = sfs2x;
        Debug.Log("in method ");
        ISFSObject objOut = new SFSObject();
        sfs.Send(new ExtensionRequest("GetRooms", objOut));
    }
}

