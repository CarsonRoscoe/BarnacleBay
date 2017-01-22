using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHUDHandler : MonoBehaviour {
    public static PlayerHUDHandler instance;

    public GameObject[] slots;

    void Awake() {
        if (instance == null)
            instance = this;
        else
            Destroy(this);
    }

    void Start() {
        loadList();
    }

    public void loadList() {
        Debug.Log("LETS DO ONE OF THEM LOADS: " + AirConsoleManager.instance.players.Count);
        int slot = 0;
        foreach (var p in AirConsoleManager.instance.players) {
            slots[slot].SetActive(true);
            slots[slot].GetComponentInChildren<Text>().text = p.name;
            slots[slot].GetComponent<Image>().color = getColorFromPlayerData(p.color);
            slot++;
            if (slot >= 8)
                break;
        }
        for (var i = slot; i < 8; i++) {
            slots[i].SetActive(false);
        }
    }

    Color getColorFromPlayerData(PlayerData.PlayerColors c) {
        switch(c) {
            case PlayerData.PlayerColors.BLACK:
                return new Color(0,0,0,.5f);
            case PlayerData.PlayerColors.BLUE:
                return new Color(0,0,1,.5f);
            case PlayerData.PlayerColors.GREEN:
                return new Color(0,1,0,.5f);
            case PlayerData.PlayerColors.ORANGE:
                return new Color(255,165,0, .5f);
            case PlayerData.PlayerColors.PURPLE:
                return new Color(146,21,150, .5f);
            case PlayerData.PlayerColors.RED:
                return new Color(1,0,0,.5f);
            case PlayerData.PlayerColors.WHITE:
                return new Color(1,1,1,.5f);
            case PlayerData.PlayerColors.YELLOW:
                return new Color(1,.92f,.016f, .5f);
            default:
                return new Color(.5f,.5f,.5f,.5f);
        }
    }
}
