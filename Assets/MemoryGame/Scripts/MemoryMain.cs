using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MemoryMain : MonoBehaviour {
	#region Card UI
	public int numOfCards = 18;
	public int numOfCardsPerRow = 6;
	public Button cardPrefab;
	public GameObject rowPrefab;
	public GameObject cardPanel;
	public List<Button> cards;
	public List<GameObject> rows;
	public Sprite cardBack;
	public Sprite[] cardFaces;
	private int[] matchIndexes;
	#endregion

	#region Card Selection
	private bool isCardSelected = false;
	private bool isAMatch;
	private bool resetSelection = false;
	private int firstCardIndex;
	private int secondCardIndex;
	private int playerMatches;
	private Button firstButtonClicked = null;
	private Button secondButtonClicked = null;
	private Color clearColor = new Color(0,0,0,0);
	#endregion

	#region Score and Timing
	public static int score;
	public static float time;
	private bool isGameDone = false;
	public Text scoreText;
	public Text timeText;
	public Text matchText;
	#endregion

	#region Audio
	public AudioClip[] matchAudioClips;
	public AudioSource matchAudio;
	#endregion

	private History gamePlay;

	// Use this for initialization
	void Awake () {
		GameControl.control.GetComponent<AudioSource> ().Stop ();
		gamePlay = new History (); //start new game session
		//set defaults for beginning of game
		cards = new List<Button> (numOfCards);
		rows = new List<GameObject> ();
		matchIndexes = new int[numOfCards];
		playerMatches = 0;
		isAMatch = false;
		score = 1000;
		scoreText.text = "Score: 1000";
		time = 0f;
		timeText.text = "Time: 0:00.00";
		matchText.text = "Number of Matches: 0/" + (numOfCards/2).ToString();
		//instantiate cards and show cards with back face
		for (int i = 0; i < numOfCards; i++) {
			Button card = Instantiate (cardPrefab, Vector3.zero, Quaternion.identity) as Button; //instantiate new card
			if (cards.Count % numOfCardsPerRow == 0) { //if the row has the max number of cards it can handle, create a new row
				GameObject newRow = Instantiate (rowPrefab, Vector3.zero, Quaternion.identity) as GameObject; //instantiate new row
				rows.Add (newRow); //add new row
			}
			int index = i; //card index
			card.onClick.AddListener(() => buttonClicked(index)); //add listener with index attached
			card.onClick.AddListener(() => ClickAudio.click.Click()); //add listener with click sound
			card.image.sprite = cardBack; //start card image sprite with card back
			cards.Add (card); //add card to cards
		}
		foreach (GameObject row in rows) {
			row.transform.SetParent (cardPanel.transform); //set parent of row to card panel
			//set position within the hierarchy and the panel itself
			row.transform.SetAsFirstSibling ();
			row.transform.localPosition = Vector3.zero;
			row.transform.localScale = new Vector3 (1, 1, 1);
		}
		int rowCount = 0;
		Transform rowParent = cardPanel.transform.GetChild (rowCount); //get transform of first row
		foreach (Button card in cards) {
			card.transform.SetParent (rowParent); //set card parent to current row
			//set position of card within row
			card.transform.localScale = new Vector3 (1, 1, 1);
			card.transform.localPosition = new Vector3 (0, 0, 0);
			card.transform.SetAsLastSibling ();
			if (rowParent.childCount % numOfCardsPerRow == 0 && card != cards[numOfCards-1]){ //if its at the end of a row, and it isn't the last card, go to the next row
				rowParent = cardPanel.transform.GetChild(++rowCount);
			}
		}
		//populate match index array and add listeners
		for (int i = 0; i < numOfCards / 2; i++) {
			matchIndexes [i] = i;
			matchIndexes [i + numOfCards / 2] = i;
		}
		//shuffle indexes
		Shuffle(matchIndexes);
		Canvas.ForceUpdateCanvases (); //update canvas
	}
		
	
	// Update is called once per frame
	void Update () {
		if (!isGameDone) { //if the game isn't done, keep updating the UI texts
			time += Time.deltaTime;
			float currentMinutes = Mathf.Floor (time / 60);
			float currentSeconds = time % 60;
			timeText.text = "Time: " + currentMinutes.ToString ("00") + ":" + currentSeconds.ToString ("00.00");
			scoreText.text = "Score: " + score.ToString ();
			matchText.text = "Number of Matches: " + playerMatches + "/9";
		}
	}
		
	void Shuffle(int[] indexes){
		int indexRange = indexes.Length;
		System.Random random = new System.Random ();
		while (indexRange != 0) {
			indexRange -= 1;
			//random index within range
			int randIndex = random.Next(indexRange);
			//swap
			int temp = indexes [randIndex];
			indexes [randIndex] = indexes [indexRange];
			indexes [indexRange] = temp;
		}
	}

	public void buttonClicked(int index){
		if (!resetSelection) { //if reset selection is false
			if (!isCardSelected) { //if no card is selected
				firstCardIndex = index; //get the index of this card as the first card
				firstButtonClicked = cards [firstCardIndex]; //get the card with this index
				firstButtonClicked.image.sprite = cardFaces [matchIndexes [firstCardIndex]]; //change the sprite to the appropriate image
				firstButtonClicked.interactable = false;
				isCardSelected = true;
			} else if (isCardSelected) { //if one card is selected
				secondCardIndex = index; //set second index
				if (firstCardIndex == secondCardIndex) { //do nothing if the cards are the same
					return;
				}
				secondButtonClicked = cards [secondCardIndex]; //get the card with this index
				secondButtonClicked.image.sprite = cardFaces [matchIndexes [secondCardIndex]]; //change the sprite to the appropriate image
				if (matchIndexes[firstCardIndex] == matchIndexes[secondCardIndex]) { //cards match
					CardMatch ();
				} else {
					NoMatch ();
				}
			}
		}
	}

	void CardMatch(){
		isAMatch = true; //its a match
		resetSelection = true; //reset selection
		StartCoroutine("ResetCardSelection");
	}

	void NoMatch(){
		isAMatch = false; //no match
		resetSelection = true; //reset selection
		StartCoroutine ("ResetCardSelection");
	}
		
	//Reset card delay
	IEnumerator ResetCardSelection(){
		foreach (Button card in cards) {
			card.enabled = false; //disable cards while reset occurs
		}
		if (isAMatch) {
			matchAudio.clip = matchAudioClips [1]; //play match sound
			matchAudio.Play ();
			yield return new WaitForSeconds(2f);
			playerMatches++; //increase matches
			//make cards non-interactable and invisible to keep card panel structure in place
			cards [firstCardIndex].interactable = false;
			cards [firstCardIndex].GetComponent<Image> ().color = clearColor;
			cards [secondCardIndex].interactable = false;
			cards [secondCardIndex].GetComponent<Image> ().color = clearColor;
			//reset buttons
			firstButtonClicked = null;
			secondButtonClicked = null;
		} else {
			matchAudio.clip = matchAudioClips [0]; //play no match sound
			matchAudio.Play ();
			yield return new WaitForSeconds(2f);
			HideCardFace ();
			score -= 40;
			//make cards interactable again
			cards [firstCardIndex].interactable = true;
			cards [secondCardIndex].interactable = true;
		}
		isAMatch = false; //reset to false
		isCardSelected = false; //reset to false
		if (playerMatches == numOfCards / 2 || score <= 0) { //game is done
			isGameDone = true;
			gamePlay.SetGamePlayScore (score); //set score of session
			GameControl.control.currentUser.memoryGameHistory.Add (gamePlay); //add session to memory game history
			GameControl.control.SaveData (); //save
			SceneManager.LoadScene ("MemoryGameEnd");
		}
		resetSelection = false; //set reset selection to false
		//reset indexes
		firstCardIndex = -1;
		secondCardIndex = -1;
		foreach (Button card in cards) { //enable buttons
			card.enabled = true;
		}
		StopCoroutine ("ResetCardSelection");
	}

	void HideCardFace(){ //set sprites to back of card
		cards[firstCardIndex].image.sprite = cardBack;
		cards[secondCardIndex].image.sprite = cardBack;
		firstButtonClicked = null;
		secondButtonClicked = null;
	}

}
