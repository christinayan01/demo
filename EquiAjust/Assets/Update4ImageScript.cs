using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Update4ImageScript : MonoBehaviour {

	public RawImage _east;
	public RawImage _south;
	public RawImage _wets;
	public RawImage _north;
	int _tick1 = 0;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		_tick1++;
		if (_tick1 > 60) {
			_tick1 = 0;

			//_east
		}
	}
}
