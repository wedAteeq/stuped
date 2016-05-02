using UnityEngine;
using System.Collections;

public class AddBid : MonoBehaviour {
	
	private void OnMouseDown()
	{
		//call method that will show the form
		int id=transform.parent.parent.gameObject.GetComponent<bidding>().getItemID();
		BiddingForm.Instance.ShowForm(id);
	}
}
