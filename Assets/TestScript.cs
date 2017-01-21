using UnityEngine;
using System.Collections;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;

public class TestScript : MonoBehaviour {
    void Start() {
        AirConsole.instance.onMessage += OnMessage;
    }

    void OnMessage( int from, JToken data ) {
        print( from );
        AirConsole.instance.Message( from, "Full of pixels!" );
    }
}
