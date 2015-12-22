using UnityEngine;
using UnityEngine.UI;

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public enum GameState {
    CHOICE,
    INPUT,
    WAIT,
    NORMAL
}

public enum Transition {
    INSTANT,
    FADE_BLACK,
    FADE_WHITE,
    WIPE_RIGHT
}

public class Game : MonoBehaviour {
    static Game instance;

    public Dictionary<string, int> properties = new Dictionary<string, int>();
    string sceneName = "NONE";
    string bg = "";
    int time = 0930;
    Scene scene;
    GameState state = GameState.NORMAL;
    Choice currLeftChoice;
    Choice currRightChoice;

    public SpriteRenderer view;
    public ClockController clock;
    public MessageController message;
    public ChoiceController choices;
    public AudioSource sfx;
    public AudioClip step;
    public AudioClip hit;
    public AudioClip statUp;
    public AudioClip statDown;
    public AudioClip tvStatic;
    public AudioClip choose;
    public AudioClip noChoose;

    public Text popText;
    public Text gpaText;
    public Text empText;
    public Text rblText;
    public Text clvText;

    void Start() {
        Game.instance = this;
        Screen.fullScreen = false;

        this.properties["POP"] = 20;
        this.properties["GPA"] = 290;
        this.properties["EMP"] = 10;
        this.properties["RBL"] = 8;
        this.properties["CLV"] = 10;
        updateStats();

        this.scene = Scenes.Get("waking");
        
        choices.Hide();
    }

    void Update() {
        switch (state) {
            case GameState.INPUT:
                choices.SetArrowVisibility(true, true);
                choices.SetTextVisibility(false, false);
                if (Input.GetKeyDown("left") || Input.GetKeyDown("a")) {
                    //instance.sfx.PlayOneShot(instance.choose);
                    state = GameState.NORMAL;
                    choices.leftArrow.Bounce();
                }  else if (Input.GetKeyDown("right") || Input.GetKeyDown("d")) {
                    //instance.sfx.PlayOneShot(instance.choose);
                    state = GameState.NORMAL;
                    choices.rightArrow.Bounce();
                }
                break;
            case GameState.CHOICE:
                if (currLeftChoice != null && (Input.GetKeyDown("left") || Input.GetKeyDown("a"))) {
                    state = GameState.NORMAL;
                    StopCoroutine("ChoiceTimeout");
                    //instance.sfx.PlayOneShot(instance.choose);
                    if (currLeftChoice.bounce) {
                        choices.leftArrow.Bounce();
                    }
                    scene.choose(currLeftChoice);
                } else if (currRightChoice != null && (Input.GetKeyDown("right") || Input.GetKeyDown("d"))) {
                    state = GameState.NORMAL;
                    StopCoroutine("ChoiceTimeout");
                    //instance.sfx.PlayOneShot(instance.choose);
                    if (currRightChoice.bounce) {
                        choices.rightArrow.Bounce();
                    }
                    scene.choose(currRightChoice);
                }
                if (state != GameState.CHOICE) {
                    choices.Hide();
                }
                break;
            case GameState.WAIT:
                choices.Hide();
                break;
        }
        while (state == GameState.NORMAL) {
            scene.Next();
        }
    }
    public static int Prop(string key, int def = 0) {
        if (instance.properties.ContainsKey(key)) {
            return instance.properties[key];
        } else {
            return def;
        }
    }
    public static void SetProp(string key, int val) {
        instance.properties[key] = val;
        instance.updateStats();
    }
    public static void PlaySound(string name) {
        switch (name) {
            case "hit": instance.sfx.PlayOneShot(instance.hit); break;
            case "step": instance.sfx.PlayOneShot(instance.step); break;
            case "statUp": instance.sfx.PlayOneShot(instance.statUp); break;
            case "statDown": instance.sfx.PlayOneShot(instance.statDown); break;
            case "tvStatic": instance.sfx.PlayOneShot(instance.tvStatic); break;
        }
    }
    public static void IncStat(string stat, int amt, bool wait = true) {
        string name = "";
        string amtString = amt.ToString();
        Color c = amt > 0 ? Color.green : Color.red;
        if (!instance.properties.ContainsKey(stat)) {
            instance.properties[stat] = 0;
        }
        instance.sfx.PlayOneShot(amt > 0 ? instance.statUp : instance.statDown);
        instance.properties[stat] += amt;
        switch (stat) {
            case "POP":
                name = "popularity";
                instance.StartCoroutine(instance.FlashColor(instance.popText, c, 3));
                break;
            case "GPA":
                name = "GPA";
                instance.StartCoroutine(instance.FlashColor(instance.gpaText, c, 3));
                amtString = string.Format("{0:.00}", amt / 100.0f);
                break;
            case "EMP":
                name = "empathy";
                instance.StartCoroutine(instance.FlashColor(instance.empText, c, 3));
                break;
            case "RBL":
                name = "rebeliousness";
                instance.StartCoroutine(instance.FlashColor(instance.rblText, c, 3));
                break;
            case "CLV":
                name = "cleverness";
                instance.StartCoroutine(instance.FlashColor(instance.clvText, c, 3));
                break;
        }
        Message(string.Format("{0} {1} by {2}!", name, amt > 0 ? "increased" : "decreased", amt));
        if (wait) {
            WaitForInput();
        }
        instance.updateStats();
    }
    void updateStats() {
        popText.text = properties["POP"].ToString();

        gpaText.text = string.Format("{0:0.00}", properties["GPA"] / 100.0f);
        empText.text = properties["EMP"].ToString();
        rblText.text = properties["RBL"].ToString();
        clvText.text = properties["CLV"].ToString();
    }
    public static void SetChoices(Choice left, Choice right, float timeout = 0) {
        instance.state = GameState.CHOICE;
        instance.currLeftChoice = left;
        instance.currRightChoice = right;
        instance.choices.SetChoices(left, right);
        if (timeout > 0) {
            instance.StartCoroutine("ChoiceTimeout", timeout);
        }
    }
    public static void SetBG(string bgname, Transition t) {
        if (instance.view.sprite != null && bgname == instance.bg) {
            return;
        }
        instance.bg = bgname;
        if (bgname == "") {
            Color b = instance.view.color;
            b.a = 0;
            instance.view.color = b;
            return;
        }
        Color c = instance.view.color;
        c.a = 1;
        instance.view.color = c;
        var newBG = Instantiate(Resources.Load("Images/" + bgname, typeof(Sprite))) as Sprite; // these shouuld probably be loaded and stored up front
        switch (t) {
            case Transition.INSTANT:
                instance.view.sprite = newBG;
                break;
            case Transition.FADE_BLACK:
                instance.state = GameState.WAIT;
                instance.StartCoroutine(instance.FadeTransition(newBG, 1.0f));
                break;
            case Transition.FADE_WHITE:
                instance.view.sprite = newBG;
                break;
            case Transition.WIPE_RIGHT:
                instance.view.sprite = newBG;
                break;
        }
    }
    public static void Message(string msg, TextEffect e = TextEffect.NONE) {
        instance.message.Set(msg, e);
    }
    public static void WaitForInput() {
        instance.state = GameState.INPUT;
    }
    public static void WaitForSeconds(float t) {
        instance.state = GameState.WAIT;
        instance.StopAllCoroutines();
        instance.StartCoroutine(instance.DelayStateSwitch(GameState.NORMAL, t));
    }
    public static int GetTime() {
        return instance.time;
    }
    public static void SetTime(int time) {
        instance.time = time;
        instance.clock.SetTime(time);
    }
    public IEnumerator ChoiceTimeout(float timeout) {
        choices.StopAllCoroutines();
        yield return choices.StartCoroutine("SetTimeout", timeout);
        //choices.SetChoices(null, null);
        instance.sfx.PlayOneShot(instance.noChoose);
        choices.Hide();
        state = GameState.NORMAL;
        currLeftChoice = null;
        currRightChoice = null;
    }
    public IEnumerator FadeTransition(Sprite sprite, float duration) {
        Color start = view.color;
        Color end = start;
        end.a = 0;
        float t = 0.0f;
        while (t <= 1) {
            view.color = Color.Lerp(start, end, t);
            t += 2 * Time.deltaTime / duration;
            yield return null;
        }
        view.sprite = sprite;
        while (t >= 0) {
            view.color = Color.Lerp(start, end, t);
            t -= 2 * Time.deltaTime / duration;
            yield return null;
        }
        instance.state = GameState.NORMAL;
        yield return null;
    }
    public IEnumerator DelayStateSwitch(GameState state, float duration) {
        this.state = GameState.WAIT;
        yield return new WaitForSeconds(duration);
        this.state = state;
    }
    public static void SetScene(string scene) {
        if (scene == "MENU") {
            instance.state = GameState.WAIT;
            SceneManager.LoadScene("Menu");
            return;
        }
        instance.sceneName = scene;
        instance.scene = Scenes.Get(scene);
        Message(""); // maybe change
        instance.StartCoroutine(instance.DelayStateSwitch(GameState.NORMAL, 1f));
    }
    public IEnumerator FlashColor(Text obj, Color col, float duration) {
        float elapsed = 0;
        Color start = Color.white;
        while (elapsed < duration) {
            if (elapsed % 0.5 < 0.25) {
                obj.color = col;
            } else {
                obj.color = start;
            }
            elapsed += Time.deltaTime;
            yield return false;
        }
        obj.color = Color.white;
        yield return null;
    }
}