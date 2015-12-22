using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class Arrow : MonoBehaviour {
    float acc_y = 0;
    float vel_y = 0;
    float dy = 0;
    public float tension = .1f;
    public float damping = 0.9f;
    public float bounceOffset = -8f;
    Vector3 origin;
    Vector3 offset;
	// Use this for initialization
	void Start () {
        origin = gameObject.transform.position;
        offset = Vector3.zero;
	}
	
	// Update is called once per frame
	void Update () {
        acc_y = -dy * tension;
        vel_y += acc_y;
        dy += vel_y;
        vel_y *= damping;
        offset.y = dy;
        if (Mathf.Abs(dy) > 0.01f) {
            gameObject.transform.position = origin + offset;
        }
	}

    public void Bounce() {
        dy = bounceOffset;
    }

    public void SetVisible(bool v) {
        // change this to a coroutine if there's time
        var i = GetComponent<RawImage>();
        Color c = i.color;
        c.a = v ? 1.0f : 0.3f;
        i.color = c;
    }
}
