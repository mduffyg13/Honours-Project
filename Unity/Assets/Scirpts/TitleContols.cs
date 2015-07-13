using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TitleContols : MonoBehaviour {

	//public GameObject canvas;

	public GameObject titleControls;
	Button btnStart;

	InputField inputField;
	private int toolbarInt = 1;
	private string[] toolbarStrings = {"LOW","AVERAGE","HIGH"};


	public string username;





	private void Awake (){
		titleControls = GameObject.Find ("Canvas");
		btnStart = titleControls.GetComponentInChildren<Button> ();
		btnStart.onClick.AddListener (() => btnOnclick ()); 
		inputField = titleControls.GetComponentInChildren<InputField> ();
	}
	private void Update(){
		if (Input.GetKey (KeyCode.Escape)) {
			Application.Quit();		
			Debug.Log("QUIT");
		}
		if (inputField.text == "") {
						//Debug.Log ("Button not selectable");	
						btnStart.interactable = false;
			btnStart.GetComponentInChildren<Text>().text = "Enter name...";
		
				} else {
			btnStart.interactable = true;	
			btnStart.GetComponentInChildren<Text>().text = "Click to Start!";
		}
	}
	void OnGUI() {

		toolbarInt = GUI.Toolbar (new Rect(Screen.width/2-105, Screen.height/2-30, 250, 50), toolbarInt, toolbarStrings);

	}

	public void btnOnclick ()
	{

		username = inputField.text;
		StartGame();
		//Debug.Log ("Start Clicked: " + inputField.text);
	}

	public void StartGame(){

		//Save entered user name
		PlayerPrefs.SetString("name",username);
		//Save experience selection
		PlayerPrefs.SetInt("difficulty", toolbarInt);
		//Create unique id
		PlayerPrefs.SetString("id", System.DateTime.Now.Millisecond.ToString () + System.DateTime.Now.Second.ToString());
		//Load game level
		Application.LoadLevel("Level");
	}
}
