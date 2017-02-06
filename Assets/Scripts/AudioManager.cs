using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {
    public static AudioManager instance;

    public bool playSFX = true;
    private bool _playMusic = true;
    public bool playMusic {
        get {
            return _playMusic;
        }
        set {
            _playMusic = value;
            if ( !value ) {
                stopAllMusic();
            }
            else {
                playGameMusic( getSceneMusic() );
            }
        }
    }

    public enum SFXID { CANNONFIRE, CANNONIMPACT, ROCKCOLLISION, SINKING, WAVES, SEAGULLS, ONBUTTONCLICK }
    public AudioSource CannonFire;
    public AudioSource CannonImpact;
    public AudioSource RockCollision;
    public AudioSource Sinking;
    public AudioSource Waves;
    public AudioSource Seagulls;
    public AudioSource OnButtonClick;

    public enum MusicID { MENUINTRO, MENULOOP, GAME, NONE }
    public AudioSource MenuMusicIntro;
    public AudioSource MenuMusicLoop;
    public AudioSource GameMusic;

    void Awake() {
        if ( instance == null ) {
            instance = this;
            DontDestroyOnLoad( gameObject );
        }
        else
            Destroy( this );
    }

    public void UIClick() {
        playSound( SFXID.ONBUTTONCLICK );
    }

    public void playSound( SFXID sound ) {
        if ( !playSFX )
            return;
        switch ( sound ) {
            case SFXID.CANNONFIRE:
                if ( !CannonFire.isPlaying )
                    CannonFire.Play();
                break;
            case SFXID.CANNONIMPACT:
                if ( !CannonImpact.isPlaying )
                    CannonImpact.Play();
                break;
            case SFXID.ROCKCOLLISION:
                if ( !RockCollision.isPlaying )
                    RockCollision.Play();
                break;
            case SFXID.SINKING:
                if ( !Sinking.isPlaying )
                    Sinking.Play();
                break;
            case SFXID.WAVES:
                if ( !Waves.isPlaying )
                    Waves.Play();
                break;
            case SFXID.SEAGULLS:
                if ( !Seagulls.isPlaying )
                    Seagulls.loop = true;
                    Seagulls.Play();
                break;
            case SFXID.ONBUTTONCLICK:
                if ( !OnButtonClick.isPlaying )
                    OnButtonClick.Play();
                break;
            default:
                break;
        }
    }

    public void playGameMusic( MusicID sound ) {
        if ( !playMusic ) {
            stopAllMusic();
            return;
        }
        switch ( sound ) {
            case MusicID.MENUINTRO:
                if ( !MenuMusicIntro.isPlaying )
                    stopAllMusic();
                MenuMusicIntro.Play();
                break;
            case MusicID.MENULOOP:
                if ( !MenuMusicIntro.isPlaying )
                    stopAllMusic();
                MenuMusicLoop.loop = true;
                MenuMusicLoop.Play();
                break;
            case MusicID.GAME:
                if ( !GameMusic.isPlaying )
                    stopAllMusic();
                GameMusic.loop = true;
                GameMusic.Play();
                break;
            case MusicID.NONE:
                break;
        }
    }

    public MusicID getSceneMusic() {
        switch ( SceneMusicManager.instance.getSceneID() ) {
            case SceneMusicManager.SceneID.GAME:
                return MusicID.GAME;
            case SceneMusicManager.SceneID.MENU:
                return MusicID.MENUINTRO;
            case SceneMusicManager.SceneID.UNKNOWN:
            default:
                return MusicID.NONE;
        }
    }

    public void stopAllMusic() {
        if ( GameMusic.isPlaying )
            GameMusic.Pause();
        if ( MenuMusicLoop.isPlaying )
            MenuMusicLoop.Pause();
        if ( MenuMusicIntro.isPlaying )
            MenuMusicIntro.Pause();
    }
}