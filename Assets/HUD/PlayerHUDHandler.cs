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
        int slot = 0;
        foreach (var p in AirConsoleManager.instance.players) {
            slots[slot].SetActive(true);
            slots[slot].GetComponentInChildren<Text>().text = p.name;
            slots[slot].GetComponent<Image>().color = PlayerData.getColorData(p.color);
            slot++;
            if (slot >= 8)
                break;
        }
        for (var i = slot; i < 8; i++) {
            slots[i].SetActive(false);
        }
    }
}
