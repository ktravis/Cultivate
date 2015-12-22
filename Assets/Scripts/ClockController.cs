using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ClockController : MonoBehaviour {
    int time = 0000;
    string str_time = "";
    string str_time_no_colon = "";
    Text clockText;

    void Start() {
        clockText = GetComponent<Text>();
    }
    public void SetTime(int t) {
        this.time = t;
        this.str_time = string.Format("{0}:{1:00} {2}", t / 100, t % 100, t > 1159 ? "pm" : "am");
        this.str_time_no_colon = this.str_time.Replace(':', ' ');
    }
	
	void Update () {
        clockText.text = Time.frameCount % 120 < 60 ? this.str_time : this.str_time_no_colon;
    }
}
