using UnityEngine;
using System.Collections;

/**
 * Assume that this class is attached to the Network Camera
 **/

public class NetworkManager : MonoBehaviour {
	/**
	 * All the states enumerated as integers
	 **/
	enum States {Start, CreateGame, ShowGames, Lobby};

	/** Server methods **/
	private const string typeName = "Assassins-City"; //the name of the game type

	//TODO see if we can have multiple characters
	private const int MAX_PLAYERS = 4;


	public AudioSource bgMusic;
	public GameObject playerPrefab;
	public GameObject AIPrefab;

	private void SpawnPlayer()
	{
		Network.Instantiate(playerPrefab, new Vector3(0f, 5f, 0f), Quaternion.identity, 0);
	}

	private void SpawnAI(Vector3 location)
	{
		GameObject rename = (GameObject) Network.Instantiate(AIPrefab, location, Quaternion.identity, 0);

		rename.name += networkView.GetInstanceID();
	}

	private void StartServer()
	{
		Network.InitializeServer(MAX_PLAYERS, 25000, !Network.HavePublicAddress());
		MasterServer.RegisterHost(typeName, gameName);
		Network.maxConnections = MAX_PLAYERS;
	}


	/**
	 * This is called multiple times per frame and renders 
	 * and displays whatever this method calls with GUI
	 * For buttons it will return true if the button was clicked 
	**/

	void OnGUI()
	{
		Rect backLocation = new Rect (Screen.width - BUTTON_WIDTH, 0, BUTTON_WIDTH, BUTTON_HEIGHT);
		
		if (splashState == States.Start){ //Start Screen
			StartScreen();
		} else if (splashState == States.CreateGame){ //Define the Game
			CreateGameScreen();
			
			if (GUI.Button (backLocation, "Back")) {
				splashState = States.Start;
			}
		} else if (splashState == States.ShowGames){ //Scroll View Screen
			RoomScreen();
			
			if (GUI.Button (backLocation, "Back")) {
				splashState = States.Start;
			}
			
		} else if (splashState == States.Lobby){ //Lobby Screen
			LobbyScreen(); 
			
			if (Network.isServer && GUI.Button (backLocation, "Back") ) {
				splashState = States.Start;
				Network.Disconnect();
			} else if (Network.isClient && GUI.Button (backLocation, "Back") ){
				splashState = States.ShowGames;
				Network.Disconnect();
			}
		}
	}

	private States splashState = States.Start;
	private const int BUTTON_WIDTH = 250;
	private const int BUTTON_HEIGHT = 100;

	void StartScreen(){
		if (GUI.Button(new Rect(Screen.width/2 - BUTTON_WIDTH/2, Screen.height/2 - BUTTON_HEIGHT , BUTTON_WIDTH, BUTTON_HEIGHT), "Create Game")){
			splashState = States.CreateGame;
		}
		
		if (GUI.Button (new Rect (Screen.width / 2 - BUTTON_WIDTH / 2, Screen.height / 2 + BUTTON_HEIGHT, BUTTON_WIDTH, BUTTON_HEIGHT), "Join Game")) {
			RefreshHostList ();
			splashState = States.ShowGames;
		}
	}

	//The name of the game room
	private string gameName = "Room Name";

	private int LABEL_WIDTH = 100;
	private int LABEL_HEIGHT = 20;
	private int TEXT_WIDTH = 150;

	void CreateGameScreen(){
		int BOX_WIDTH = Screen.width/2;
		int BOX_HEIGHT = Screen.height/2;
		int DATA_WIDTH = LABEL_WIDTH + TEXT_WIDTH;
		GUI.Box( new Rect(Screen.width/2 - BOX_WIDTH/2, Screen.height/2 - BOX_HEIGHT/2, BOX_WIDTH, BOX_HEIGHT), "" );
		GUI.Label (new Rect(Screen.width/2 - DATA_WIDTH/2, Screen.height/2 - BOX_HEIGHT/2, LABEL_WIDTH, LABEL_HEIGHT), "Game Name:");
		gameName = GUI.TextField (new Rect(Screen.width/2 - DATA_WIDTH/2 + LABEL_WIDTH, Screen.height/2 - BOX_HEIGHT/2, 
		                                   TEXT_WIDTH, LABEL_HEIGHT), gameName);

		if (GUI.Button(new Rect(Screen.width/2 - BUTTON_WIDTH/2, Screen.height/2 , BUTTON_WIDTH, BUTTON_HEIGHT), "Create Game")){
			StartServer();
			splashState = States.Lobby;
		}

	}

	private Vector2 scrollViewVector = Vector2.zero;
	private int SCROLL_WIDTH_PADDING = 100;
	private int SCROLL_HEIGHT_PADDING = 100;
	private int SCROLL_BUTON_PADDING = 50;

	void RoomScreen(){
		if (GUI.Button (new Rect (0, 0, BUTTON_WIDTH, BUTTON_HEIGHT), "Refresh")) {
			RefreshHostList ();
		}

		if (hostList != null)
		{
			int buttonHeightComp = BUTTON_HEIGHT + SCROLL_BUTON_PADDING;
			// Begin the ScrollView
			scrollViewVector = GUI.BeginScrollView (new Rect (SCROLL_WIDTH_PADDING, BUTTON_HEIGHT + SCROLL_HEIGHT_PADDING,
		                                                  Screen.width - 2*SCROLL_WIDTH_PADDING, 
		                                                  Screen.height - BUTTON_HEIGHT - 2*SCROLL_HEIGHT_PADDING), 
		                                        scrollViewVector, 
		                                        new Rect (0, 0, Screen.width - 2*SCROLL_WIDTH_PADDING, 
			          									hostList.Length*(buttonHeightComp)));


			int counter = 0;
			for (int i = 0; i < hostList.Length; i++)
			{
				if (hostList[i].connectedPlayers < hostList[i].playerLimit){
					if (GUI.Button(new Rect( Screen.width/2 - SCROLL_WIDTH_PADDING - BUTTON_WIDTH/2
					                        , (buttonHeightComp * counter), BUTTON_WIDTH, BUTTON_HEIGHT), hostList[i].gameName)){
						JoinServer(hostList[i]);
						splashState = States.Lobby;
						counter++;
					}
				}
			}

			// End the ScrollView
			GUI.EndScrollView();
		}
	}

	void LobbyScreen(){
		NetworkPlayer[] players = Network.connections;

		//players discludes you
		int BOX_WIDTH = Screen.width/2;
		int BOX_HEIGHT = Screen.height/2;
		int DATA_WIDTH = 50;
		GUI.Box( new Rect(Screen.width/2 - BOX_WIDTH/2, Screen.height/2 - BOX_HEIGHT/2, BOX_WIDTH, BOX_HEIGHT), "" );
		GUI.Label ( new Rect(Screen.width/2 - DATA_WIDTH/2, Screen.height/2 - BOX_HEIGHT/2, LABEL_WIDTH, LABEL_HEIGHT), 
		           "# of players:" + (players.Length + 1) );

		if (Network.isServer) {
			if (GUI.Button (new Rect (Screen.width / 2 - BUTTON_WIDTH / 2, Screen.height / 2, BUTTON_WIDTH, BUTTON_HEIGHT), "Start Game")) {
				SetupWorld();

				//Network.maxConnections = players.Length; //makes this the max for the game
				MasterServer.UnregisterHost(); //should just connect the players now
			}
		} else if (Network.isClient) {
			GUI.Box (new Rect (Screen.width / 2 - BUTTON_WIDTH / 2, Screen.height / 2, BUTTON_WIDTH, BUTTON_HEIGHT), 
			         "Waiting to Start");
		}
	}

	[RPC] void SetupWorld(){
		this.gameObject.SetActive(false); //do not use this camera
		SpawnPlayer (); //spwans my player
		
		if (bgMusic.isPlaying) {
			bgMusic.Stop (); //stop music for now
		}
		
		//initalize the score board on all machines
		GameScore score = (GameScore) GameObject.Find("Town").GetComponent("GameScore");
		score.StartGame ();
		
		if (Network.isServer) {
			//server spawns all the AI
			SpawnAI (new Vector3 (-1, 1, 15));
			SpawnAI (new Vector3 (-5, 1, 23));
			SpawnAI (new Vector3 (7, 1, 26));
			SpawnAI (new Vector3 (2, 1, 29));
			SpawnAI (new Vector3 (12, 1, 31));
			
			networkView.RPC ("SetupWorld", RPCMode.OthersBuffered);
		}
	}



	/** Clitent methods **/

	private HostData[] hostList;
	
	private void RefreshHostList()
	{
		MasterServer.RequestHostList(typeName);
	}


	private void JoinServer(HostData hostData)
	{
		Network.Connect(hostData);
	}

	void Update(){
		this.transform.RotateAround (Vector3.zero, Vector3.up, 0.2F); 
	}

	/**
	 * Message Callbacks
	 * */

	void OnMasterServerEvent(MasterServerEvent msEvent)
	{
		if (msEvent == MasterServerEvent.HostListReceived)
			hostList = MasterServer.PollHostList();
	}

	void OnConnectedToServer()
	{
		Debug.Log("Server Joined");
		//SpawnPlayer();
	}
	
	
	void OnServerInitialized()
	{
		Debug.Log("Server Initializied");
		//SpawnPlayer();
	}


}
