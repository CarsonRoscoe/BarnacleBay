using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserHandler {
    //static reference to the UserHandler
    private static UserHandler instance;

    //List of players in the game.
    public List<Player> players;

    //enum for getting which type of score
    public enum ScoreType { GAME, SESSION, TOTAL_GAMES };

    public enum GameType { ONE_GAME, THREE_GAME, FIVE_GAME, NONE_SET };

    public enum TeamType { LEFT, FFA, RIGHT };

    public GameType gameType = GameType.NONE_SET;

    //player count left in the round
    public int playersLeft {
        get {
            return _playersLeft;
        }
        private set {
            _playersLeft = value;
        }
    }
    private int _playersLeft;

    //returns the active instance of the UserHandler, creates and returns one if one didn't exist.
    public static UserHandler getInstance() {
        if (instance == null) {
            instance = new UserHandler();
        }
        return instance;
    }

    /*
    Private constructor, use getInstance to instantiate/get the 
    instance.
    */
    private UserHandler() {
        players = new List<Player>();
    }

    /*
    Adds a player to the list if the ID isn't already in the list.
    returns true if added as a new player.
    */
    public Player addPlayer(int deviceID, string uid, Color color) {
        if (!deviceIDExists(deviceID)) {
            Player p = new Player(deviceID, uid, color);
            players.Add(p);
            return p;
        }
        return null;
    }

    /*
    Deletes a player from the list if the ID is in the list.
    returns true if deleted
    */
    public bool deletePlayer(int deviceID) {
        int pos = findDeviceID(deviceID);
        if (pos != -1) {
            players[pos].outOfRound(true);
            players.RemoveAt(pos);
            return true;
        }
        return false;
    }

    /*
    Returns the index of which a player exists in the ID, and
    returns that index.
    Will return -1 if no deviceID is found.
    */
    public int findDeviceID(int ID) {
        int i = 0;
        foreach (Player p in players) {
            if (p.deviceID == ID)
                return i;
            i++;
        }
        return -1;
    }

    /*
    Returns true if the deviceID passed in is found in the list.
    */
    public bool deviceIDExists(int ID) {
        foreach (Player p in players) {
            if (p.deviceID == ID)
                return true;
        }
        return false;
    }

    /*
    Returns the player object based on the device ID
    Must be null checked
    */
    public Player getPlayerByID(int ID) {
        foreach (Player p in players) {
            if (p.deviceID == ID)
                return p;
        }
        return null;
    }

    /*
    Returns the player object based on the device ID
    Must be null checked
    */
    public bool userIDExists(string uid) {
        foreach (Player p in players) {
            if (p.uid.Equals(uid))
                return true;
        }
        return false;
    }

    /*
    Returns the player object based on the device ID
    Must be null checked
    */
    public Player getPlayerByUserID(string uid) {
        foreach (Player p in players) {
            if (p.uid.Equals(uid))
                return p;
        }
        return null;
    }

    /*
    Sets a player to be out of the round. Essentially programmatically
    tells the game that the player is officially out of the game.
    */
    public void playerEliminated(int ID, bool isElem) {
        int pos = findDeviceID(ID);
        if (pos != -1) {
            playersLeft--;
            players[pos].outOfRound(isElem);
        }
    }

    /*
    Resets all players ready for next game. Best to call the player reset functions
    from here.
    */
    public void resetPlayers() {
        foreach (Player p in players) {
            p.resetGame();
        }
        playersLeft = players.Count;
    }

    /*
    Resets all players ready for next TOURNAMENT. Best to call the player reset functions
    from here.
    */
    public void resetTournament() {
        foreach (Player p in players) {
            p.resetSession();
        }
        playersLeft = players.Count;
    }


    /*
    Checks to see if all players are ready to proceed to the 
    game. Requires all users to put their ready input in.
    */
    public bool allPlayersReady() {
        foreach (Player p in players) {
            if (!p.readyToPlay)
                return false;
        }
        return true;
    }

    /*
    Sets all players to not be ready to continue to next game yet.
    ALWAYS is called by all players ready if they all are. This
    way the player bools are reset each time when they should be.
    */
    public void setAllReady(bool ready) {
        foreach (Player p in players) {
            p.readyToPlay = ready;
        }
    }


    /*
    Gets the highest of the 4 types of scores and returns that player.
    */
    public Player getHighestScore(ScoreType type) {
        int highScore = 0;
        Player best = null;
        foreach (Player p in players) {
            switch (type) {
                case ScoreType.GAME:
                    if (p.gameScore == highScore) {
                        highScore = p.gameScore;
                        best = null;
                    } else if (p.gameScore > highScore) {
                        highScore = p.gameScore;
                        best = p;
                    }
                    break;
                case ScoreType.SESSION:
                    if (p.sessionScore == highScore) {
                        highScore = p.sessionScore;
                        best = null;
                    } else if (p.sessionScore > highScore) {
                        highScore = p.sessionScore;
                        best = p;
                    }
                    break;
                case ScoreType.TOTAL_GAMES:
                    if (p.gamesWon == highScore) {
                        highScore = p.gamesWon;
                        best = null;
                    } else if (p.gamesWon > highScore) {
                        highScore = p.gamesWon;
                        best = p;
                    }
                    break;
            }
        }
        return best;
    }

    /*
    Applies a point to the games won of the player who won.
    This is for game rounds rather than session scores.
    returns the player with the highest games won.
    */
    public Player applyRoundWin() {
        Player p = getHighestScore(ScoreType.GAME);
        if (p != null)
            p.gamesWon++;
        return getHighestScore(ScoreType.TOTAL_GAMES);
    }

    /*
    Applies points earned from this round to the session score
    and resets the game scores for everyone.
    Returns the player with the highest session score.
    */
    public Player applySessionScores() {
        foreach (Player q in players) {
            q.sessionScore += q.gameScore;
        }
        return getHighestScore(ScoreType.SESSION);
    }

    /*
    Class which represents a player in the game.
    */
    public class Player {
        public int deviceID;
        public string uid;
        public Color color;

        public int worldRank = int.MaxValue;
        public bool updateWorldScore = false;

        public GameObject playerObject;

        public int gameScore;
        public int sessionScore;
        public int gamesWon;

        //is the player ready for the game to start?
        public bool readyToPlay = false;

        //what team is the player on
        public TeamType teamType = TeamType.FFA;

        //is the player/dead eliminated from this round?
        private bool isOutOfRound;

        public Player(int ID, string uid, Color c) {
            deviceID = ID;
            this.uid = uid;
            color = c;
        }

        /*
        Resets the score from last game. Think of the games within
        a tournament.
        */
        public void resetGame() {
            gameScore = 0;
            playerObject = null;
            isOutOfRound = false;
        }

        /*
        Resets the players data after a tournament, resets
        all scores and player instance ID.
        */
        public void resetSession() {
            sessionScore = 0;
            gameScore = 0;
            gamesWon = 0;
            playerObject = null;
            isOutOfRound = false;
        }

        /*
        Is the player ready for next game.
        */
        public bool isReady() {
            return (gameScore == 0 && playerObject != null);
        }

        /*
        Gets the player ready for next game within a tournament
        */
        public void setupReady(GameObject p) {
            playerObject = p;
            gameScore = 0;
            isOutOfRound = false;
        }

        //meant to be package visiblity, required to be called by UserHandler
        public void outOfRound(bool outOf) {
            isOutOfRound = outOf;
            playerObject = null;
        }

        public void addToScore(int points)
        {
            sessionScore += points;
            gameScore += points;
        }
    }
}
