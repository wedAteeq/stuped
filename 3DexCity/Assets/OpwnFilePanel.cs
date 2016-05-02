using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Text;

public class OpwnFilePanel : MonoBehaviour
{
	public string filePath = "";
	public Texture2D texture;
	public GameObject m_goCube;

	// Use this for initialization
	void Start()
	{
		m_goCube = GameObject.Find("Cube");
	}
		

	void OnGUI()
	{
		if (GUI.Button(new Rect(210, 260, 100, 40), "Select Texture"))
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
				m_goCube.GetComponent<Renderer>().material.mainTexture = texture;

			}
		}
	}

	public void OpenFilePanel()
	{

	}
}
