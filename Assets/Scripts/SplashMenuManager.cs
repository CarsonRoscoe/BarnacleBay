using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashMenuManager : MonoBehaviour {
    public void GoToPlaySetup() {
        SceneManager.LoadScene( "SelectTeams" );
    }
}
