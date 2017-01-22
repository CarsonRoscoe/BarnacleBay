using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData {
    public enum PlayerColors { PURPLE, ORANGE, BLUE, RED, GREEN, YELLOW, WHITE, BLACK};

    public int playerID;
    public string name;
    public PlayerColors color;
    public int lives;
    const int MAX_LIVES = 3;

    public PlayerData(int p, string n) {
        playerID = p;
        name = n;
        color = (PlayerColors)p;
        lives = MAX_LIVES;
    }

    public void resetPlayer() {
        lives = MAX_LIVES;
    }
}
