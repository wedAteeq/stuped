using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Text;

public class RemyC : MonoBehaviour
{

    Animator anim;

    public Collider FramePic; //referrnse to frame of picture 
    public string filePath = ""; //the path of the picture 
    public Texture2D texture; //the picture itself


    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        move();
        sit();
        carry();
    }
    void move()
    {

        anim.SetFloat("Walk", Input.GetAxis("Vertical"));

        if (Input.GetKey(KeyCode.LeftShift))
            anim.SetFloat("jump", 1);
        else
            anim.SetFloat("jump", 0);
        //_______________________________________
        if (Input.GetKey(KeyCode.RightArrow))
            anim.SetBool("turnRight", true);
        else
            anim.SetBool("turnRight", false);
        //_______________________________________
        if (Input.GetKey(KeyCode.LeftArrow))
            anim.SetBool("turnLeft", true);
        else
            anim.SetBool("turnLeft", false);
        //_______________________________________

    }
    void sit()
    {
        if (Input.GetKey(KeyCode.S))
            anim.SetInteger("sit", 1);
        else
            anim.SetInteger("sit", 0);
    }
    void carry()
    {
        if (Input.GetKey(KeyCode.C))
            anim.SetInteger("carry", 1);
        else
            anim.SetInteger("carry", 0);
    }

    //Manage Room (Add/Delete/Like/Arrang contents) 



    void OnTriggerEnter(Collider other) // to know which fram will be affect 

    {
        if (other.gameObject.CompareTag("Frame"))  // we will know from tag 

        {

            FramePic = other;
        }
    }

    public void AddContents()

    {
#if UNITY_EDITOR
        filePath = EditorUtility.OpenFilePanel("Overwrite with png"
                                            , Application.streamingAssetsPath
                                            , "png");
#endif
        if (filePath.Length != 0)
        {
            WWW www = new WWW("file://" + filePath);
            texture = new Texture2D(64, 64);
            www.LoadImageIntoTexture(texture);
            FramePic.GetComponent<Renderer>().material.mainTexture = texture;

        }

    }


    public void DeleteContents()

    {
        FramePic.GetComponent<Renderer>().material.mainTexture = null;
    }


}
