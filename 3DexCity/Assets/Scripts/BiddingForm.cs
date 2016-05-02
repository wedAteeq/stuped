using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class BiddingForm : MonoBehaviour {


	private static BiddingForm instance;
	public Animator BidForm;
	public Text CurrentPrice;//this contains the displayed currentPrice
	//output
	public Text ErrorMessage;
	//input
	public InputField BiddingAmount;
	//output
	public Text MinimumBidMessage;
	public Text NewPrice;
	private int currentItem;
	public static BiddingForm Instance {
		get {
			return instance;
		}
	}
	void Awake() {
		instance = this;
	
	}

	public void ShowForm(int id)
	{
		double minBid = ManageItems.Instance.getMinimumBid (id);
		double currentPrice = ManageItems.Instance.getCurrenPrice (id);
		this.MinimumBidMessage.text = "Next Minimum Bid is: " + minBid + "$";
		this.CurrentPrice.text = "Current Price: "+currentPrice + "$";
		this.NewPrice.text = "New Price: 0.0$";
		ErrorMessage.text = "";
		BidForm.gameObject.SetActive(true);
		BidForm.SetBool("loggedIn", true);
		BidForm.SetBool("panelOpen", true);
		currentItem = id;
	}
	public void HideForm()
	{
		BidForm.SetBool("loggedIn", false);
		BidForm.gameObject.SetActive(false);
	}
	public void updatePrice ( int id, double price)
	{
		if (BidForm.gameObject.activeInHierarchy && currentItem==id)
			this.CurrentPrice.text = "Current Price: "+price + "$";	
	}
	public void calculateNewPrice()
	{
		string bid = BiddingAmount.text; 
		double x = double.Parse (bid) + double.Parse((this.CurrentPrice.text).Substring(15, this.CurrentPrice.text.Length - 16));
		NewPrice.text = "New Price: " + x + "$";
	}
    public void OnOkButtonClicked()
    {
		double current = double.Parse((this.CurrentPrice.text).Substring(15, this.CurrentPrice.text.Length - 16));
		string bid = BiddingAmount.text; 
		double x = double.Parse (bid);
		double newPrice = x+ ManageItems.Instance.getCurrenPrice (currentItem);
		double minBid= ManageItems.Instance.getMinimumBid (currentItem);
		if (minBid<=newPrice) {
				//check if bid accepted
				ErrorMessage.text = "Your Bid Will added";
				//call method to send from network
			NetworkManager.Instance.SendBid(newPrice,currentItem);
				//close this form
			BidForm.SetBool("loggedIn", false);
			BidForm.gameObject.SetActive(false);
				return;
			}		
           ErrorMessage.text = "Your Bid Is Lower Than Minimum Bid";
    }
}
