using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashMenuManager : MonoBehaviour {

    void Start() {
        //Make do intro then loop
        AudioManager.instance.playGameMusic( AudioManager.MusicID.MENULOOP );
    }

    public void GoToPlaySetup() {
        SceneManager.LoadScene( "SelectTeams" );
    }
}
