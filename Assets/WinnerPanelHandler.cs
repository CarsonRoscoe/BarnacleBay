using NDream.AirConsole;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinnerPanelHandler : MonoBehaviour {
    public GameObject WinnerBackground;
    public GameObject WinnerText;
    public GameObject WinnerTitle;
    Image m_winnerBackground;
    Text m_winnerText;
    Image m_canvas;

    void Start()
    {
        m_winnerBackground = WinnerBackground.GetComponent<Image>();
        m_winnerText = WinnerText.GetComponent<Text>();
        m_canvas = GetComponent<Image>();
        SetIsActive(false);
    }

    public void PlayerWon(UserHandler.Player player)
    {
        var color = player.color;
        var name = AirConsole.instance.GetNickname(player.deviceID);

        SetIsActive(true);

        m_winnerText.text = name;
        m_winnerBackground.color = color;
    }

    public void SetIsActive(bool isActive)
    {
        WinnerTitle.SetActive(isActive);
        m_winnerBackground.enabled = isActive;
        m_winnerText.enabled = isActive;
        m_canvas.enabled = isActive;
    }
}
