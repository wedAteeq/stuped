using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UserInterfaces : MonoBehaviour {

	private static UserInterfaces instance;
	public static UserInterfaces Instance {
		get {
			return instance;
		}
	}
	void Awake() {
		instance = this;
	//	OnLoginButtonAtLoginPage ();
	}
	//to shwo tab do: auction.SetBool("loggedIn", true);
	//to show form:  membership.SetBool("panelOpen", !membership.GetBool("panelOpen"));

	//define interfaces
	public Animator startAuction;
	public Animator auction;
	public Animator reserve;
	public Animator membership;
	public Animator chooseTime;
	public Animator manageAccountForMember;
	public Animator friends;
	public Animator aucInfo;
	public Animator chatPanel;
	public Animator UserTab;
	public Animator logout;

	//admin
	public Animator ViewMemberAccount;
	public Animator MemberAccount;
	public Animator adminAccount;
	public Animator AddAdmin;
	public Animator CloseAuction;

	public GameObject HomePage;
	public GameObject LoginPage;
	public GameObject ForgetPass;
	public GameObject createAccount;
	public GameObject AuctionsItems;
	public GameObject DeleteAccount;
	public GameObject Notifications;
	public GameObject NOtForm;
	public GameObject FriendsForm;

	//admin
	public GameObject confirmDeleteAdmin;
	public GameObject ConfirmDelete;
	public GameObject SuccessDeleteMember;
	public GameObject AdminNOtForm;

	public Text AucInfo;
	private bool admin;
	//Home Page
	public void OnLoginButtonAtHomePage()
	{
		//show login Form
		HomePage.gameObject.SetActive(false);
		LoginPage.gameObject.SetActive(true);
	}

	//this called after login success
	public void OnLoginButtonAtLoginPage()
	{ admin = false;
		HomePage.gameObject.SetActive(false);
		//enable write & left side interfaces make login true :)
		EnableMainTabs();
		UserTab.SetBool ("loggedIn",true);
		logout.gameObject.SetActive (true);
		Notifications.SetActive (true);
	}
	public void OnAdminLoginButtonAtLoginPage()
	{admin = true;
		HomePage.gameObject.SetActive(false);
		//enable write & left side interfaces make login true :)
		EnableAdminMainTabs();
		UserTab.SetBool ("loggedIn",true);
		logout.gameObject.SetActive (true);
		Notifications.SetActive (true);
	}

	public void OnLogOut() {
		logout.SetBool("AuctionStart", !logout.GetBool("AuctionStart"));
		Notifications.SetActive (!logout.GetBool("AuctionStart"));
	}
	public void onconirmLogOut()
	{
		if (admin)
			EnableAdminMainTabs ();
		else
			EnableMainTabs ();

		UserTab.SetBool("loggedIn", !UserTab.GetBool("loggedIn"));
		logout.SetBool("loggedIn", !logout.GetBool("loggedIn"));
		HomePage.gameObject.SetActive (true);
	}
	public void OnUserTabClick() {
		UserTab.SetBool("panelOpen", !UserTab.GetBool("panelOpen"));
	}
	public void onCreateAccountAtHome()
	{
		createAccount.gameObject.SetActive(true);
		HomePage.gameObject.SetActive(false);
	}

	//login Form
	public void onCancelAtLogin()
	{
		HomePage.gameObject.SetActive(true);
		LoginPage.gameObject.SetActive(false);
	}
	public void onForgetPass()
	{
		ForgetPass.gameObject.SetActive(true);
		LoginPage.gameObject.SetActive(false);
	}
	public void onCreateAccount()
	{
		createAccount.gameObject.SetActive(true);
		LoginPage.gameObject.SetActive(false);
	}
	//Create Account Form
	public void onCancelCreateAccount()
	{
		createAccount.gameObject.SetActive(false);
		HomePage.gameObject.SetActive(true);
	}

	public void setStartAuction(string owner,string auctionName, int dur)
	{
		AucInfo.text = "Auction Name: "+ auctionName+ "/n Auction Owner: "+owner+"/nDuration: "+dur/60+" menutes"+
			"Auction Site: Auctions Room in the center of the city";
	}

	public void OnStartAuction() {
		startAuction.SetBool("AuctionStart", !startAuction.GetBool("AuctionStart"));
	}

	public void OnAccount()
	{
		EnableMainTabs ();

		manageAccountForMember.SetBool("panelOpen", !manageAccountForMember.GetBool("panelOpen"));
		manageAccountForMember.SetBool("loggedIn", !manageAccountForMember.GetBool("loggedIn"));
		auction.SetBool("loggedIn", !auction.GetBool("loggedIn"));
	}

		public void onDeleteAccountAtMember()
	{
		EnableMainTabs ();
		manageAccountForMember.SetBool("loggedIn", true);
		manageAccountForMember.SetBool("panelOpen", false);
		DeleteAccount.gameObject.SetActive (true);
	}

	public void onConfirmDeleteAccount()
	{
		EnableMainTabs ();
		HomePage.SetActive (true);
		DeleteAccount.gameObject.SetActive (false);

	}
	public void onCancelDeleteAccount()
	{
		DeleteAccount.gameObject.SetActive (false);
	}
	public void OnAuction() {
		EnableMainTabs ();
		auction.SetBool("panelOpen", !auction.GetBool("panelOpen"));
		auction.SetBool("loggedIn", !auction.GetBool("loggedIn"));
			}
	public void OnReserve() {
		auction.SetBool("loggedIn", false);
		auction.SetBool("panelOpen", false);
		reserve.SetBool("loggedIn", true);
		reserve.SetBool("panelOpen", true);
	}
	public void TimeAndAuction() {
		chooseTime.SetBool("loggedIn", true);
		chooseTime.SetBool("panelOpen", true);
		reserve.SetBool("panelOpen", false);
	}
	public void EndTimeAndAuction() {
		chooseTime.SetBool("loggedIn",false);
		reserve.SetBool("panelOpen", true);
	}

	public void EndReserveAuction()
	{
		reserve.SetBool("loggedIn", false);
		EnableMainTabs ();
	}
	public void OnMembership() {
		
		auction.SetBool("loggedIn", false);
		auction.SetBool("panelOpen", false);
		membership.SetBool("loggedIn", true);
		membership.SetBool("panelOpen", true);
	}
		
	public void OnChooseAuctionAtMemberShip()
	{
		aucInfo.SetBool("loggedIn", true);
		aucInfo.SetBool("panelOpen", true);
		membership.SetBool("panelOpen", false);
	}
	public void OnEndChooseAuctionAtMemberShip()
	{
		aucInfo.SetBool("loggedIn", false);
		membership.SetBool("panelOpen", true);
	}
	public void OnEndMemberShip()
	{
		membership.SetBool("loggedIn", false);
		EnableMainTabs ();
	}

	public void onChat()
	{
		if (!admin)
			EnableMainTabs ();
		else
			EnableAdminMainTabs ();
		chatPanel.SetBool("loggedIn", !chatPanel.GetBool("loggedIn"));
		chatPanel.SetBool("panelOpen", !chatPanel.GetBool("panelOpen"));
	}

	public void EnableMainTabs( )
	{
		
		auction.SetBool("loggedIn", !auction.GetBool("loggedIn"));
		manageAccountForMember.SetBool("loggedIn", !manageAccountForMember.GetBool("loggedIn"));
		friends.SetBool("loggedIn", !friends.GetBool("loggedIn"));
		chatPanel.SetBool("loggedIn", !chatPanel.GetBool("loggedIn"));
	}
	public void EnableAdminMainTabs()
	{
		ViewMemberAccount.SetBool("loggedIn", !ViewMemberAccount.GetBool("loggedIn"));
		AddAdmin.SetBool("loggedIn", !AddAdmin.GetBool("loggedIn"));
		adminAccount.SetBool("loggedIn", !adminAccount.GetBool("loggedIn"));
		chatPanel.SetBool("loggedIn", !chatPanel.GetBool("loggedIn"));
		CloseAuction.SetBool("loggedIn", !CloseAuction.GetBool("loggedIn"));

	}



	///////admin
	/// those are admin motions
	/// 
	public void OnViewMemberAccount() {
		EnableAdminMainTabs();
		ViewMemberAccount.SetBool("panelOpen", !ViewMemberAccount.GetBool("panelOpen"));
		ViewMemberAccount.SetBool("loggedIn", !ViewMemberAccount.GetBool("loggedIn"));
	}
	public void OnAdminViewedMemberAccount() {
		ViewMemberAccount.SetBool("loggedIn", false);
		ViewMemberAccount.SetBool("panelOpen", false);
		MemberAccount.SetBool("loggedIn", true);
		MemberAccount.SetBool("panelOpen", true);
	}

	public void OnEndViewMemAccount()
	{
		MemberAccount.SetBool("loggedIn", false);
		EnableAdminMainTabs ();
	}
	public void OnAdminConfirmDeleteMember()
	{
		MemberAccount.SetBool("loggedIn", false);
		EnableAdminMainTabs ();
		ConfirmDelete.gameObject.SetActive (true);
	}
	public void OnCancelComnfirmDelete()
	{
		ConfirmDelete.gameObject.SetActive (false);
	}

	public void OnAdminViewHisAccount()
	{
		EnableAdminMainTabs();
		adminAccount.SetBool("panelOpen", !adminAccount.GetBool("panelOpen"));
		adminAccount.SetBool("loggedIn", !adminAccount.GetBool("loggedIn"));
	}
	public void OnAdminAddAdmin()
	{
		EnableAdminMainTabs();
		AddAdmin.SetBool("panelOpen", !AddAdmin.GetBool("panelOpen"));
		AddAdmin.SetBool("loggedIn", !AddAdmin.GetBool("loggedIn"));
	}

	public void OnAdminDeleteHisAccount()
	{
		EnableAdminMainTabs ();
		adminAccount.SetBool("loggedIn", true);
		adminAccount.SetBool("panelOpen", false);
		ConfirmDelete.gameObject.SetActive (true);
	}
	public void AdminCancelDeleteHisAccount()
	{
		ConfirmDelete.gameObject.SetActive (false);

	}
	public void onAdminConfirmDeleteAccount()
	{
		EnableMainTabs ();
		HomePage.SetActive (true);
		ConfirmDelete.gameObject.SetActive (false);

	}

	public void ViewSuccessDeletemember()
	{
		SuccessDeleteMember.gameObject.SetActive (true);
	}
	public void HideSuccessDeletemember()
	{
		SuccessDeleteMember.gameObject.SetActive (false);
	}

	public void showNotifications()
	{
		if (admin)
			ViewAuction.Instance.OnViewNotificationsButtonclicked ();
		else
			ViewNotifications.Instance.OnViewNotificationsButtonclicked ();
	}

	public void ONCloseNotification()
	{
		NOtForm.SetActive (false);
	}

	public void ONAdminCloseNotification()
	{
		AdminNOtForm.SetActive (false);
	}
	public void ONCloseFriends()
	{
		FriendsForm.SetActive (false);
	}
}
