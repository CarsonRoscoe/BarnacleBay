using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MoveToBoat : MonoBehaviour {
    public Transform NewParent;
    public GameObject MoveToPoint;
    public GameObject Logo;
    private RectTransform logoRectTransform;
    public GameObject[] DisabledUI;
    public GameObject Paper;
    private RectTransform paperRectTransform;
    public int ReduceLogoByFactor = 2;
    private bool moving = false;
    private bool canMove = true;
    private float amount = 0;
    private Vector3 logoStartPosition;
    private Vector2 logoStartSize;
    

	void Start() {
		Paper.GetComponent<Image>().material.SetFloat( "_Threshold", 0.0f );
        logoRectTransform = Logo.GetComponent<RectTransform>();
        logoStartSize = logoRectTransform.sizeDelta;
        logoStartPosition = logoRectTransform.position;
        paperRectTransform = Paper.GetComponent<RectTransform>();
        paperRectTransform.localScale = Vector3.one.WithY(0f);
        GameObject.Find("RowRowRow").GetComponent<Animator>().speed = 0.5f;
    }

    void Update() {
        if ( moving ) {
            if (amount < 1) {
                amount += Time.deltaTime;
                var oldSize = logoRectTransform.sizeDelta;
                logoRectTransform.sizeDelta = logoStartSize * (1 - (amount));
                var sizeDifference = oldSize - logoRectTransform.sizeDelta;
                var newLogoX = logoRectTransform.position.x + sizeDifference.x / ReduceLogoByFactor * .75f;
                var newLogoY = logoRectTransform.position.y + sizeDifference.y / ReduceLogoByFactor * .75f;
                logoRectTransform.position = new Vector3( newLogoX, newLogoY, 0 );
                transform.position = Vector3.Lerp( transform.position, MoveToPoint.transform.position, amount );
                transform.rotation = Quaternion.Lerp( transform.rotation, MoveToPoint.transform.rotation, amount );
                paperRectTransform.localScale = Vector3.one.WithY( amount );
            } else {
                paperRectTransform.rotation = Quaternion.Euler( 0, 0, 0 );
                transform.position = MoveToPoint.transform.position;
                transform.rotation = MoveToPoint.transform.rotation;
                transform.parent = NewParent;
                amount = 0;
                moving = false;
                GameObject.Find("RowRowRow").GetComponent<Animator>().speed = 0f;
            }
        }
    }

    public void MoveToNewPosition() {
		if (canMove) {
			AudioManager.instance.playSound (AudioManager.SFXID.ONBUTTONCLICK);
            foreach(var UI in DisabledUI) {
                UI.SetActive( false );
            }
            canMove = false;
            moving = true;
            var transform = MoveToPoint.transform;
            while(transform.parent != null ) {
                transform = transform.parent;
            }
        }
    }

	public void StartGame() {
        var canStartResult = BoatManager.instance.CanStart();
        if (canStartResult == BoatManager.CanStartResult.CanStart && BoatManager.instance.IsActiveUpdatingBoats()) {
		    AudioManager.instance.playSound (AudioManager.SFXID.ONBUTTONCLICK);
            StartCoroutine( BurnPaperThenStart(2f) );
        } else {
            ShowErrorMessage(canStartResult);
        }
    }

    IEnumerator BurnPaperThenStart(float seconds) {
        var currentTime = 0f;
        var material = Paper.GetComponent<Image>().material;
        BoatManager.instance.SetActiveUpdatingBoats(false);
		BoatManager.instance.SetActiveBoatSprites (false);
        while(currentTime < seconds ) {
            material.SetFloat( "_Threshold", currentTime / seconds );
            yield return null;
            currentTime += Time.deltaTime;
        }
        material.SetFloat( "_Threshold", 1.1f );
		yield return new WaitForSeconds (.25f);
		foreach (var image in Paper.GetComponentsInChildren<Image>()) {
			image.gameObject.SetActive (false);
		}
        GameDataManager.instance.ReadyPlayers(BoatManager.instance.PlayersSelection);
        ToolTipManager.instance = null;
		SceneManager.LoadScene( "GameScene" );
        //Go to next scene
    }

    private void ShowErrorMessage(BoatManager.CanStartResult startResult) {
        var titleResult = string.Empty;
        var messageResult = string.Empty;
        switch( startResult ) {
            case BoatManager.CanStartResult.NotEnoughPlayers:
                titleResult = "Not Enough Players";
                messageResult = "This game is a 2 to 8 player game.";
                break;
            case BoatManager.CanStartResult.TooManyPlayers:
                titleResult = "Too Many Players";
                messageResult = "This game is a 2 to 8 player game.";
                break;
            case BoatManager.CanStartResult.NoPlayersTeamOne:
                titleResult = "No Players On Team One";
                messageResult = "When playing team vs team, each team needs at least one player.";
                break;
            case BoatManager.CanStartResult.NoPlayersTeamTwo:
                titleResult = "No Players On Team Two";
                messageResult = "When playing team vs team, each team needs at least one player.";
                break;
            case BoatManager.CanStartResult.FFAAndTeam:
                titleResult = "Free-For-All Or Teams";
                messageResult = "Move players to either all be in Free-For-All mode, or all be on a team.";
                break;
            case BoatManager.CanStartResult.CanStart:
                titleResult = "Game Can Start";
                messageResult = "If you are seeing this, this is a problem. The game should be able to start...";
                break;
            default:
                titleResult = "Unexpected case...";
                messageResult = "This should never happen. Our apologies.";
                break;
        }
        ShowErrorMessage(titleResult, messageResult);
    }

    private void ShowErrorMessage(string title, string message) {
        ToolTipManager.instance.DisplayMessage(title, message);
    }
}
