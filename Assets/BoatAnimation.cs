using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TeamSelection { One, FreeForAll, Two }

public class BoatAnimation : MonoBehaviour {
    private TeamSelection _selectedTeam = TeamSelection.FreeForAll;
    public TeamSelection SelectedTeam { get { return _selectedTeam; } }

    public void SetSelectedTeam( TeamSelection selectedTeam ) {
        if (_selectedTeam != selectedTeam) {
            StartCoroutine( Move( _selectedTeam, selectedTeam ) );
            _selectedTeam = selectedTeam;
        }
    }

    IEnumerator Move( TeamSelection old, TeamSelection @new ) {
        var secondsToMove = Mathf.Abs( (int)@new - (int)old );
        var moveRight = (int)@new - (int)old > 0;
        var moveInDir = (moveRight ? Vector3.right : Vector3.left);
        var secondsToDate = 0f;
        while(secondsToDate < secondsToMove) {
            print( secondsToDate );
            secondsToDate += Time.deltaTime;
            transform.position += moveInDir;
            yield return null;
        }
        yield return null;
    }
}
