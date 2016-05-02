using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class ManageItems : MonoBehaviour {
	//this class will comunicate with the server side
	//if there is an item in auction, then it will define its position and rotations
	//and then spwan it as an 3d object with its fram and add bid button
	//the object saved in a prefab calles itemPrefab
	private Dictionary<int, GameObject> bidItems = new Dictionary<int, GameObject>();
	private Dictionary<int, Vector3> positons = new Dictionary<int, Vector3>();
	private Dictionary<int, Vector3> rotations = new Dictionary<int, Vector3>();
	public GameObject itemPrebaf;//this means item of auction
	public GameObject framesContainer;
	private static ManageItems instance;
	public static ManageItems Instance {
		get {
			return instance;
		}
	}
	void Awake() {
		instance = this;
	}
	//1-create items dectionary this called whenever an auction started
	public void InitItemsPositions()
	{
		positons.Add (0, new Vector3 (44.5f,35f,-35.4f));
		positons.Add (1, new Vector3 (44.5f,38f,-63.5f));
		positons.Add (2, new Vector3 (297f,7.78f,197.45f));
		positons.Add (3, new Vector3 (297f,7.78f,197.45f));
		positons.Add (4, new Vector3 (297f,7.78f,197.45f));
		positons.Add (5, new Vector3 (297f,7.78f,197.45f));
		///////////////////////
		rotations.Add (0, new Vector3 (0f,90f,0f));
		rotations.Add (1, new Vector3 (0f,90f,0f));
		rotations.Add (2, new Vector3 (297f,7.78f,197.45f));
		rotations.Add (3, new Vector3 (297f,7.78f,197.45f));
		rotations.Add (4, new Vector3 (297f,7.78f,197.45f));
		rotations.Add (5, new Vector3 (297f,7.78f,197.45f));
	}

	//2-spwan specific item
	//we need to the component attached with this
	public void spwanItem(int id, double minPrice)
	{
		if (bidItems.ContainsKey(id) && bidItems[id] != null) {
			Destroy(bidItems[id]);
			bidItems.Remove(id);
		}
		// Lets spawn our remote player model
		GameObject item = GameObject.Instantiate(itemPrebaf) as GameObject;
		item.transform.SetParent (framesContainer.transform);
		item.transform.localPosition= positons[id];
		Debug.Log ("positons[id];" +positons[id].x);
		Debug.Log ("positon;" +item.transform.position.x);
		item.transform.localEulerAngles = rotations[id];
		Debug.Log ("inside spwan item");
		bidItems.Add(id, item);
		item.GetComponent<bidding> ().Init (id,minPrice);
	}

	public void updatePrice (int id, double price)
	{
		bidItems [id].GetComponent<bidding> ().updatePrice (price);	
		BiddingForm.Instance.updatePrice (id,price);
	}
	public double getMinimumBid(int id)
	{
		bidding c = bidItems [id].GetComponent<bidding> ();
		return c.GetMinimumBid ();
	}
	public double getCurrenPrice(int id)
	{
		bidding c = bidItems [id].GetComponent<bidding> ();
		return c.getCurrentPrice ();
	}
}