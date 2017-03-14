using NDream.AirConsole;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDHandler : MonoBehaviour {
    public static PlayerHUDHandler instance;

    public GameObject[] slots;
    public Transform ScoringPrefab;

    void Awake() {
        instance = this;
    }

    void Start() {
        loadList();
    }

    public void loadList() {
        int slot = 0;
        foreach (var player in UserHandler.getInstance().players.OrderByDescending(x => x.gameScore)) {
            slots[slot].SetActive(true);
            slots[slot].GetComponentInChildren<Text>().text = AirConsole.instance.GetNickname(player.deviceID);
            slots[slot].GetComponent<Image>().color = player.color;
            slot++;
            if (slot >= 8)
                break;
        }
    }

    public void CreateScore(int score, GameObject boat) {
        foreach(var slot in slots) {
            slot.SetActive(false);
        }
        var scoring = Instantiate(ScoringPrefab, boat.transform.position.WithY(3), Quaternion.identity);
        var scoringMesh = scoring.GetComponent<TextMesh>();
        scoringMesh.text = "+" + score;
        scoringMesh.color = UserHandler.getInstance().getPlayerByID(boat.GetComponent<shipController>().PlayerID).color;
        scoring.localScale = new Vector3(10, 10, 10);
        loadList();
    }
}
