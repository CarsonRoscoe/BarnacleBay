using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

public class ScoreManager : MonoBehaviour {
    public static ScoreManager Instance;

    private GameData m_gameData = new GameData();
    public int Score { get { return m_gameData.Score; } }

    void Awake() {
        if ( Instance == null ) {
            DontDestroyOnLoad( gameObject );
            Instance = this;
        }
        else {
            Instance.LoadScore();
            Destroy( gameObject );
        }
    }

    public void LoadScore() {
        if ( File.Exists( Application.persistentDataPath + "/highscore.dat" ) ) {
            BinaryFormatter binaryFormatter = new BinaryFormatter();
            FileStream fileStream = File.Open( Application.persistentDataPath + "/highscore.dat", FileMode.Open, FileAccess.Read );
            m_gameData = (GameData)binaryFormatter.Deserialize( fileStream );
            fileStream.Close();
        }
    }

    public void SaveScore() {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream fs = File.Open( Application.persistentDataPath + "/highscore.dat", FileMode.OpenOrCreate );
        bf.Serialize( fs, m_gameData );
        fs.Close();
    }

    [Serializable]
    class GameData {
        public int Score;
    }
}
