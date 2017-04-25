using System.Collections;
using System.Collections.Generic;
using NDream.AirConsole;
using UnityEngine;
using UnityEngine.UI;

public class ToolTipManager : MonoBehaviour {
    public static ToolTipManager instance;

    public GameObject ToolTipBackground;
    public GameObject ToolTipTitle;
    public GameObject ToolTipMessage;

    public float AnimationDuration = 1f;
    public float LastDuration = 2f;

    private Text m_toolTipTitle;
    private Text m_toolTipMessage;
    private Image m_toolTipBackground;
    private Image m_toolTipForeground;
    private bool m_inUse;

	// Use this for initialization
	void Start () {
		m_toolTipForeground = GetComponent<Image>();
        m_toolTipBackground = ToolTipBackground.GetComponent<Image>();
        m_toolTipTitle = ToolTipTitle.GetComponent<Text>();
        m_toolTipMessage = ToolTipMessage.GetComponent<Text>();
        adjustAlpha(0f);
	}
	
	public bool DisplayMessage(string title, string message) {
        if (m_inUse)
            return false;
        m_inUse = true;
        m_toolTipMessage.text = message;
        m_toolTipTitle.text = title;
        StartCoroutine(animateToolTip());
        return true;
    }
    
    IEnumerator animateToolTip() {
        var currentTime = 0f;
        var alphaAmount = 0f;
        //Fade in
        while(currentTime < AnimationDuration) {
            currentTime += Time.deltaTime;
            alphaAmount = currentTime / AnimationDuration;
            adjustAlpha(alphaAmount);
            yield return null;
        }
        //Hold
        yield return new WaitForSeconds(LastDuration);
        currentTime = 0f;
        //Fade out
        while(currentTime < AnimationDuration) {
            currentTime += Time.deltaTime;
            alphaAmount = 1f - currentTime / AnimationDuration;
            adjustAlpha(alphaAmount);
            yield return null;
        }
        m_inUse = false;
    }

    void adjustAlpha(float alpha) {
        var textColor = m_toolTipMessage.color;
        var foregroundColor = m_toolTipForeground.color;
        var backgroundColor = m_toolTipBackground.color;
        textColor.a = alpha;
        foregroundColor.a = alpha * 0.6f;
        backgroundColor.a = alpha * 0.2f;
        m_toolTipTitle.color = textColor;
        m_toolTipMessage.color = textColor;
        m_toolTipBackground.color = backgroundColor;
        m_toolTipForeground.color = foregroundColor;
    }

    struct Message {
        public string Title;
        public string Text;
        public Message(string title, string text) {
            Title = title;
            Text = text;
        }
    }
}
