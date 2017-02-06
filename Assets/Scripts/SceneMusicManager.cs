using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class SceneMusicManager : MonoBehaviour {
    public static SceneMusicManager instance;

    public enum SceneID { MENU, GAME, UNKNOWN }

    void Awake() {
        if ( instance == null )
            instance = this;
        else
            Destroy( this );
    }

    public void endGame( bool didWin ) {
        UnityEngine.SceneManagement.SceneManager.LoadScene( "MenuScene" );
        AudioManager.instance.playGameMusic( AudioManager.MusicID.MENUINTRO );
    }

    public void restartGame() {
        UnityEngine.SceneManagement.SceneManager.LoadScene( "Game" );
        AudioManager.instance.playGameMusic( AudioManager.MusicID.GAME );
    }


    public SceneID getSceneID() {
        string scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        if ( scene.Equals( "EnvironmentFinal" ) )
            return SceneID.GAME;
        if ( scene.Equals( "MenuScene" ) || scene.Equals("SelectTeams") )
            return SceneID.MENU;
        return SceneID.UNKNOWN;
    }
}