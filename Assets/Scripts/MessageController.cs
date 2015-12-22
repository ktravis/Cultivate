using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum TextEffect {
    NONE,
    VIBRATE,
    WOBBLE
}

public class MessageController : MonoBehaviour {

    public Text currentMessage;
    public CanvasRenderer backdrop;
    Color col;
    Vector3 offset;
    Vector3 origin;
    TextEffect currentEffect = TextEffect.NONE;

    void Start () {
        col = currentMessage.color;
        currentMessage.text = "";
        origin = currentMessage.transform.position;
        offset = Vector3.zero;
	}
    void Update() {
        currentMessage.transform.position = origin + offset;
    }
    public void Hide() {
        SetVisible(false);
    }
    public void SetVisible(bool v) {
        col.a = v ? 1.0f : 0.0f;
        currentMessage.color = col;
        //backdrop.SetAlpha(v ? 1.0f : 0.0f);
    }
    public void Set(string msg, TextEffect effect = TextEffect.NONE) {
        if (msg == currentMessage.text) {
            return;
        }
        currentMessage.transform.position = origin;
        offset = Vector3.zero;
        if (col.a > 0) {
            StartCoroutine(FadeMessageSwap(msg, 0.4f, effect));
        } else {
            currentMessage.text = msg;
            StartCoroutine(FadeMessageIn(0.2f));
            currentEffect = effect;
            if (effect == TextEffect.VIBRATE) {
                StartCoroutine(Vibrate(5));
            }
        }
    }
    public IEnumerator Vibrate(float amt) {
        while (currentEffect == TextEffect.VIBRATE) {
            offset.x = Random.Range(0f, amt) - amt/2;
            offset.y = Random.Range(0f, amt) - amt / 2;
            offset.z = Random.Range(0f, amt) - amt / 2;
            yield return null;
        }
    }
    public IEnumerator FadeMessageSwap(string newMsg, float duration, TextEffect e = TextEffect.NONE) {
        yield return FadeMessageOut(duration / 2);
        currentMessage.text = newMsg;
        currentEffect = e;
        if (e == TextEffect.VIBRATE) {
            StartCoroutine(Vibrate(5));
        }
        yield return FadeMessageIn(duration / 2);
    }
    public IEnumerator FadeMessageOut(float duration) {
        yield return FadeText(currentMessage, 0, duration);
        currentEffect = TextEffect.NONE;
    }
    public IEnumerator FadeMessageIn(float duration) {
        yield return FadeText(currentMessage, 1, duration);
    }
    public IEnumerator FadeText(Text text, float a, float duration) {
        float t = 0;
        Color c = text.color;
        float start = c.a;
        while (t <= 1) {
            t += Time.deltaTime / duration;
            c.a = Mathf.Lerp(start, a, t);
            text.color = c;
            yield return t;
        }
        yield return null;
    }
    public IEnumerator FadeAlpha(CanvasRenderer r, float a, float duration) {
        float t = 0;
        float start = r.GetAlpha();
        while (t <= 1) {
            r.SetAlpha(Mathf.Lerp(start, a, t));
            t += Time.deltaTime / duration;
            yield return t;
        }
        yield return null;
    }
}
