using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerData {
    public enum PlayerColors { GREEN, PURPLE, ORANGE, BLUE, RED, YELLOW, WHITE, BLACK};

    public int playerID;
    public string name;
    public PlayerColors color;
    public int lives;
    const int MAX_LIVES = 3;

    public PlayerData(int p, string n, PlayerColors c) {
        playerID = p;
        name = n;
        color = c;
        lives = MAX_LIVES;
    }

    public void resetPlayer() {
        lives = MAX_LIVES;
    }

    public static Color getColorData(PlayerColors color) {
        switch (color) {
            case PlayerData.PlayerColors.BLACK:
                return new Color(0, 0, 0, .5f);
            case PlayerData.PlayerColors.BLUE:
                return new Color(0, 0, 1, .5f);
            case PlayerData.PlayerColors.GREEN:
                return new Color(0, 1, 0, .5f);
            case PlayerData.PlayerColors.ORANGE:
                return new Color(1, .64f, 0, .5f);
            case PlayerData.PlayerColors.PURPLE:
                return new Color(.57f, .082f, .58f, .5f);
            case PlayerData.PlayerColors.RED:
                return new Color(1, 0, 0, .5f);
            case PlayerData.PlayerColors.WHITE:
                return new Color(1, 1, 1, .5f);
            case PlayerData.PlayerColors.YELLOW:
                return new Color(1, .92f, .016f, .5f);
            default:
                return new Color(.5f, .5f, .5f, .5f);
        }
    }

    public static string hexColor(PlayerColors c) {
        Color32 color = getColorData(c);
        string hex = color.r.ToString("X2") + color.g.ToString("X2") + color.b.ToString("X2");
        return hex;
    }
}
