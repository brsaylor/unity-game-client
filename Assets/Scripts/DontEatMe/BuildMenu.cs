﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.EventSystems;

public class BuildMenu : MonoBehaviour
{

    // Background material
    public Material backgroundMaterial;

    // Access to DemButton script for button creation
    public DemButton demButton;

	//Access to DemRectUI script for RectUI creation
	public DemRectUI demRectUI;
	private GameObject quitUI;	//instance of UI for after pressed quit button
	private float qBX; //quit button width
	private float qBY; //quit button height

    // Toggle counter
    int toggleCount = 0;

    // Currently building...
    public  BuildInfo currentlyBuilding;

    public  DemAnimalFactory currentAnimalFactory;

    // Currently about to delete?
    public  bool currentlyDeleting = false;

    // Player's current resource amount
    public  int currentResources = 250;

    // Player's current score amount
    public  int score = 0;

    // Plant prefabs
    public DemAnimalFactory[] plants;

    // Prey prefabs
    public DemAnimalFactory[] prey;

    // Menu buttons
    public GameObject[] menuButtons;

    private  GameObject mainObject;

    private DemTurnSystem turnSystem;

    private DemMain main;

    public GameObject panelObject;

	//mainUI
	public GameObject mainUIObject;
	public GameObject canvasObject;

    // this method increases score every 2s
    void increaseResources()
    {
        currentResources += 50;
    }


    //Loading Resources
    void Awake()
    {
      
      backgroundMaterial = Resources.Load<Material>("DontEatMe/Materials/DontEatMeBg");  

      mainObject = GameObject.Find ("MainObject");

      main = mainObject.GetComponent<DemMain> ();

      turnSystem = mainObject.GetComponent<DemTurnSystem> ();

	  //mainUI add here
	  canvasObject = GameObject.Find ("Canvas");
	  mainUIObject = GameObject.Find ("Canvas/mainUI");
	  mainUIObject.transform.SetParent (canvasObject.transform);
		mainUIObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
    mainUIObject.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
    mainUIObject.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
    mainUIObject.GetComponent<RectTransform>().offsetMax = new Vector2(0, 0);
    mainUIObject.GetComponent<RectTransform>().offsetMin = new Vector2(0, 0);
		
      //panelObject = GameObject.Find("Canvas/Panel");
	  panelObject = GameObject.Find("Canvas/mainUI/Panel");
    panelObject.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
    panelObject.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0);
    panelObject.GetComponent<RectTransform>().offsetMax = new Vector2(-100, 50);
    panelObject.GetComponent<RectTransform>().offsetMin = new Vector2(100, 0);
    //panelObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);

		quitUI = null;
    }

 

    // Use this for initialization
    void Start()
    {
        turnSystem.IsTurnLocked();

        currentAnimalFactory = null;
        currentlyBuilding = null;
        // set resources to grow over time
        InvokeRepeating("increaseResources", 2, 3.0F);

        /**
         * Elaine:
         * All this can be done in the editor beforehand since the background never changes
         * but we can decide on this later
         * I also commented out the background code in DemMain.cs since the background looks fine w/o it
         */

        // Creates the background manually
        GameObject backgroundImage = GameObject.CreatePrimitive(PrimitiveType.Quad);
        backgroundImage.name = "DemBackGround";

        Destroy(backgroundImage.GetComponent<MeshCollider>());

        backgroundImage.GetComponent<Renderer>().material = backgroundMaterial;

        backgroundImage.transform.localPosition =
            new Vector3(backgroundImage.transform.localPosition.x, backgroundImage.transform.localPosition.y, 1);
        backgroundImage.transform.localScale = new Vector3(14, 7, backgroundImage.transform.localScale.z);


        // Constructing the plants and prey the player can use
        plants = new DemAnimalFactory[6];
        plants[0] = new DemAnimalFactory("Acacia");
        plants[1] = new DemAnimalFactory("Baobab");
        plants[2] = new DemAnimalFactory("Big Tree");
        plants[3] = new DemAnimalFactory("Fruits And Nectar");
        plants[4] = new DemAnimalFactory("Grass And Herbs");
        plants[5] = new DemAnimalFactory("Trees And Shrubs");


        prey = new DemAnimalFactory[6];
        prey[0] = new DemAnimalFactory("Bohor Reedbuck");
        prey[1] = new DemAnimalFactory("Bat-Eared Fox");
        prey[2] = new DemAnimalFactory("Kori Buskard");
        prey[3] = new DemAnimalFactory("Black Backed Jackal");
        prey[4] = new DemAnimalFactory("Dwarf Mongoose");
        prey[5] = new DemAnimalFactory("Dwarf Epauletted Bat");


        // NEW BUTTON CREATION STARTS HERE
        // To use old buttons comment out the following lines (until line 287) and uncomment OnGUI()
        
        
        // Building the buttons
        gameObject.AddComponent<DemButton>();
        demButton = gameObject.GetComponent<DemButton>();

		    // building the RectUI
		    gameObject.AddComponent<DemRectUI>();
		    demRectUI = gameObject.GetComponent<DemRectUI> ();

        
        // Toggle button to switch between plant and prey menu
        demButton.setSize(Screen.width * 0.1f, Screen.height/14);
        GameObject toggleButton = demButton.CreateButton(0, 0, "Toggle");
        demButton.SetButtonText(toggleButton, "Plants");


        // Creates a buttons for plant/prey menu
        demButton.setSize(Screen.width * 0.1f, Screen.height/7);
        menuButtons = new GameObject[6];
        for (int i = 0; i < 6; i++)
        {

            GameObject button = demButton.CreateButton(0, 0 - ((Screen.height / 14) + 10 + i * (demButton.getYSize() - 2)), i.ToString());

            // Set the button images
            demButton.SetButtonImage(plants[i], button);
            demButton.SetButtonImage(prey[i], button);
            
            // Set the images of the untoggled menu to inactive
            button.transform.Find(prey[i].GetName()).gameObject.SetActive(false);

            // Add an onClick listener to detect button clicks
            button.GetComponent<Button>().onClick.AddListener(() => { selectSpecies(button); });
            button.AddComponent<DemButton>();

            menuButtons[i] = button;
        }

        // Add an onClick listener to dectect button clicks
        toggleButton.GetComponent<Button>().onClick.AddListener(() => { selectMenu(toggleButton, menuButtons); });

		//quit button 
		float qBX = Screen.width / 10.0f;
		float qBY = Screen.height / 10.0f;
		demButton.setSize (qBX, qBY);
		GameObject quitButton = demButton.CreateButton (Screen.width - qBX, 0, "Quit");
		demButton.SetButtonText (quitButton, "Quit");
		quitButton.GetComponent<Button> ().onClick.AddListener (() => {selectQuit();});
        
    }


    // Toggle between plant and prey menu when the toggle button is clicked
    void selectMenu(GameObject tButton, GameObject[] mButtons)
    {
        Debug.Log("Clicked " + tButton.name);

        toggleCount += 1;

        if (toggleCount % 2 == 0)
        {
            tButton.GetComponentInChildren<Text>().text = "Plants";
            for (int i = 0; i < 6; i++)
            {
                menuButtons[i].transform.Find(prey[i].GetName()).gameObject.SetActive(false);
                menuButtons[i].transform.Find(plants[i].GetName()).gameObject.SetActive(true);
            }
        }

        else
        {
            tButton.GetComponentInChildren<Text>().text = "Prey";
            for (int i = 0; i < 6; i++)
            {
                menuButtons[i].transform.Find(plants[i].GetName()).gameObject.SetActive(false);
                menuButtons[i].transform.Find(prey[i].GetName()).gameObject.SetActive(true);
            }
        }
    }


    // Specie selection based on the button clicked and setting down species on the gameboard
    void selectSpecies(GameObject button)
    {
        Debug.Log("Clicked " + button.name);
        turnSystem.IsTurnLocked();

        DemAnimalFactory[] species;
        short speciesType = 0;

        if (toggleCount % 2 == 0)
        {
            species = plants;
            speciesType = 0;
        }   
            
        
        else
        {
            species = prey;
            speciesType = 1;
        }  
            
        

        DemAudioManager.audioClick.Play();

        // If a selection is currently in progress...
        if (main.currentSelection)
        {
            // Ignore button click if for the same species
            if (currentAnimalFactory == species[int.Parse(button.name)])
                return;
            // Otherwise, destroy the current selection before continuing
            else
                Destroy(main.currentSelection);

            main.boardController.ClearAvailableTiles();
                
        }
        // Set / reset currentlyBuilding

        //currentlyBuilding = info;
        currentAnimalFactory = species[int.Parse(button.name)];


        // Create the current prey object
        //GameObject currentPlant = plant.Create(); //DemAnimalFactory.Create(currentlyBuilding.name , 0 ,0) as GameObject;

        // Define the current prey's initial location relative to the world (i.e. NOT on a screen pixel basis)
        Vector3 init_pos = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y));
        init_pos.z = -1.5f;

        // Instantiate the current prey
        main.currentSelection = species[int.Parse(button.name)].Create();
        //main.currentSelection.GetComponent<BuildInfo>().isPlant = true;
        main.currentSelection.GetComponent<BuildInfo>().speciesType = speciesType;


        // Set DemMain's preyOrigin as the center of the button
        main.setBuildOrigin(Camera.main.ScreenToWorldPoint(Input.mousePosition));

        main.boardController.SetAvailableTiles();
    }

	//click on quit button
	void selectQuit(){
		DemAudioManager.audioClick.Play();
		Debug.Log (Screen.width);

		if (main.currentSelection) {
			Destroy(main.currentSelection);
			main.boardController.ClearAvailableTiles();
		}

		if (quitUI == null) {
			quitUI = demRectUI.createRectUI ("quitUI", 0, 0, Screen.width / 2.0f, Screen.height / 2.0f);
			demRectUI.setUIText (quitUI, "Are you sure you want to quit?");

			//Quit Button on Quit UI
			GameObject yesButton = demButton.CreateButton (0, 0, "Yes");
			yesButton.transform.SetParent (quitUI.transform);
			yesButton.GetComponent<RectTransform> ().anchoredPosition = 
				new Vector2 (quitUI.GetComponent<RectTransform> ().sizeDelta.x/5.0f,
							-quitUI.GetComponent<RectTransform> ().sizeDelta.y/5.0f*3.0f);
			demButton.SetButtonText (yesButton, "Quit");
			yesButton.GetComponent<Button> ().onClick.AddListener (()=>{DemAudioManager.audioClick.Play(); Game.SwitchScene("World");});

			//back button on Quit UI
			GameObject noButton = demButton.CreateButton (0, 0, "No");
			noButton.transform.SetParent (quitUI.transform);
			noButton.GetComponent<RectTransform> ().anchoredPosition = 
				new Vector2 (quitUI.GetComponent<RectTransform> ().sizeDelta.x/5.0f*3.0f,
					-quitUI.GetComponent<RectTransform> ().sizeDelta.y/5.0f*3.0f);
			demButton.SetButtonText (noButton, "Back");
			//noButton.GetComponent<Button> ().onClick.AddListener (()=>{quitUI.SetActive(false);});
			noButton.GetComponent<Button> ().onClick.AddListener (()=>{DemAudioManager.audioClick.Play(); quitUI.SetActive(false); mainUIObject.SetActive(true);});

			mainUIObject.SetActive (false);

			return;
		}

		if (!quitUI.activeInHierarchy) {
			quitUI.SetActive (true);
			mainUIObject.SetActive (false);
		}
	}



    public void endGame()
    {
        /*
            Debug.Log("Game ended with X coins: " + coins);

            //LOBBY TEAM, PUT YOUR RETURN CODE HERE, PASS BACK
            //coins variable
            NetworkManager.Send(
                EndGameProtocol.Prepare(1, coins),
                ProcessEndGame
            );
      */
    }


    // Updates player's credits
    public void ProcessEndGame(NetworkResponse response)
    {
        ResponsePlayGame args = response as ResponsePlayGame;

        if (args.status == 1)
        {

            GameState.player.credits = args.creditDiff;
            Debug.Log(args.creditDiff);
        }
    }


    // Update is called once per frame
    void Update()
    {

    }

  public DemAnimalFactory GetCurrentAnimalFactory(){
    return currentAnimalFactory;
  }

  public void SetCurrentAnimalFactory(DemAnimalFactory newAnimalFactory){
    currentAnimalFactory = newAnimalFactory;
  }

  public void ToggleButtonLocks(){
    if (turnSystem.IsTurnLocked ()) {
      for (int i = 0; i < 6; i++) {
        menuButtons [i].GetComponent<Button> ().interactable = true;
        foreach(Image image in menuButtons [i].GetComponentsInChildren<Image>()){
          image.color = new Color(1.0F, 1.0F, 1.0F, 1.0F);
        }
      }
    } else {

      for (int i = 0; i < 6; i++) {
        menuButtons [i].GetComponent<Button> ().interactable = false;
        foreach(Image image in menuButtons [i].GetComponentsInChildren<Image>()){
          image.color = new Color(1.0F, 1.0F, 1.0F, 0.8F);
        }
      }
    }

  }

}
