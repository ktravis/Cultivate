using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour {

    public AudioClip statUp;
    public ChoiceController choices;
    public AudioSource sfx;
    public SpriteRenderer view;

    bool started;
    // Use this for initialization
    void Start () {
        started = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (!started) {
            if (Input.GetKeyDown("left") || Input.GetKeyDown("a")) {
                sfx.PlayOneShot(statUp);
                choices.leftArrow.Bounce();
                started = true;
                StartCoroutine("FadeToGame", 1.5f);
            } else if (Input.GetKeyDown("right") || Input.GetKeyDown("d")) {
                sfx.PlayOneShot(statUp);
                choices.rightArrow.Bounce();
                started = true;
                StartCoroutine("FadeToGame", 1.5f);
            }
        }
        
    }
    public IEnumerator FadeToGame(float duration) {
        var start = view.color;
        var end = start;
        end.a = 1;
        float t = 0.0f;
        while (t <= 1) {
            start.a = 1 - t;
            view.color = start;
            t += Time.deltaTime / duration;
            yield return null;
        }
        SceneManager.LoadScene("Main");
    }
}
