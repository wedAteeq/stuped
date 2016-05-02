using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class bidding : MonoBehaviour {

	public TextMesh CurrentPrice;
	private double currentPrice;
	private int itemID;
	private static bidding instance;
	public static bidding Instance {
		get {
			return instance;
		}
	}
	void Awake() {
		instance = this;	
	}


	public void Init(int itemID, double minPrice)
	{
		this.itemID = itemID;
		this.currentPrice = minPrice;
		CurrentPrice.text="Current Price: "+currentPrice+"$";
	}

	public void updatePrice(double newPrice)
	{
		this.currentPrice = newPrice;
		CurrentPrice.text="Current Price: "+currentPrice+"$";
	}

	public int getItemID()
	{
		return itemID;
	}

		
	public double getCurrentPrice()
	{
		return currentPrice;
	}

	//every current price has minimum bid and this function will retrive it
	public double GetMinimumBid()
    {
        if (currentPrice >= 0 && currentPrice <= 4.55)
            return currentPrice + 0.25;
        else if (currentPrice >= 5 && currentPrice <= 24.99)
            return currentPrice + 0.5;
        else if (currentPrice >= 25 && currentPrice <= 99.99)
            return currentPrice + 1;
        else if (currentPrice >= 100 && currentPrice <= 249.99)
            return currentPrice + 2.5;
        else if (currentPrice >= 250 && currentPrice <= 499.99)
            return currentPrice + 5;
        else if (currentPrice >= 500 && currentPrice <= 999.99)
            return currentPrice + 10;
        else if (currentPrice >= 1000 && currentPrice <= 2499.99)
            return currentPrice + 25;
        else if (currentPrice >= 5000)
            return currentPrice + 100;
        else
            return 0;

    }
}
