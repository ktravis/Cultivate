using System.Collections.Generic;
using UnityEngine;

public class SceneDirective { }
public class GOTODirective : SceneDirective {
    public string label;
    public GOTODirective(string label) { this.label = label; }
}
public class RESTARTDirective : SceneDirective { }

public class Scene {
    Action[] actions;
    int i = 0;
    Dictionary<string, int> labels = new Dictionary<string, int>();
    public Scene(params Action[] actions) {
        this.actions = actions;
        for (int i = 0; i < actions.Length; i++) {
            if (actions[i] is A_LABEL) {
                this.labels[((A_LABEL)actions[i]).label] = i;
            }
        }
    }
    public void choose(Choice c) {
        var directive = c.result.run();
        if (directive is GOTODirective) {
            this.i = this.labels[((GOTODirective)directive).label];
        } else if (directive is RESTARTDirective) {
            this.i = 0;
        }
    }
    public void Next() {
        var directive = this.actions[i].run();
        if (directive == null) {
            i++;
        } else if (directive is GOTODirective) {
            this.i = this.labels[((GOTODirective)directive).label];
        } else if (directive is RESTARTDirective) {
            this.i = 0;
        }
    }
}

public class Choice {
    public string text = "NO TEXT";
    public Action result;
    public bool bounce;

    public Choice(string text, Action result = null, bool bounce = true) {
        this.text = text;
        this.result = result != null ? result : A.NOOP();
        this.bounce = bounce;
    }
}

public class Action {
    public Action child;

    public Action(Action child = null) {
        this.child = child;
    }
    virtual public SceneDirective run() {
        if (this.child != null) {
            return this.child.run();
        }
        return null;
    }
}
public class A {
    public static A_MULTI MULTI(params Action[] actions) { return new A_MULTI(actions); }
    public static A_GOTO GOTO(string label) { return new A_GOTO(label); }
    public static A_LABEL LABEL(string label) { return new A_LABEL(label); }
    public static A_RESTART RESTART() { return new A_RESTART(); }
    public static A_WAIT WAIT(float t = 0) { return new A_WAIT(t); }
    public static A_NOOP NOOP() { return new A_NOOP(); }
    public static A_SOUND SOUND(string name) { return new A_SOUND(name); }
}
public class A_MULTI : Action {
    Action[] actions;
    Dictionary<string, int> labels = new Dictionary<string, int>();
    public A_MULTI(params Action[] actions) {
        this.actions = actions;
        for (int i = 0; i < actions.Length; i++) {
            if (actions[i] is A_LABEL) {
                this.labels[((A_LABEL)actions[i]).label] = i;
            }
        }
    }
    override public SceneDirective run() {
        SceneDirective directive = null;
        int i = 0;
        while (i < this.actions.Length) {
            directive = this.actions[i].run();
            if (directive is GOTODirective) {
                string key = ((GOTODirective)directive).label;

                if (this.labels.ContainsKey(key)) {
                    i = this.labels[key];
                    continue;
                }
                return directive;
            } else if (directive is RESTARTDirective) {
                return directive;
            } else {
                i++;
            }
        }
        return null;
    }
}
public class A_GOTO : Action {
    public string label;
    public A_GOTO(string label) { this.label = label; }
    public override SceneDirective run() {
        return new GOTODirective(this.label);
    }
}
public class A_LABEL : Action {
    public string label;
    public A_LABEL(string label) { this.label = label; }
}
public class A_RESTART : Action {
    public override SceneDirective run() {
        return new RESTARTDirective();
    }
}
public class A_WAIT : Action {
    float t;
    public A_WAIT(float t = 0) { this.t = t; }
    public override SceneDirective run() {
        if (t == 0) { 
            Game.WaitForInput();
        } else {
            Game.WaitForSeconds(t);
        }        
        return null;
    }
}
public class A_NOOP : Action {
    public override SceneDirective run() {
        return null;
    }
}
public class A_SOUND : Action {
    string name;
    public A_SOUND(string name) { this.name = name; }
    public override SceneDirective run() {
        Game.PlaySound(this.name);
        return null;
    }
}

public class SET {
    public static SET_SCENE SCENE(string name) { return new SET_SCENE(name); }
    public static SET_CHOICES CHOICES(Choice left, Choice right, float t = 0) { return new SET_CHOICES(left, right, t); }
    public static SET_TIME TIME(int time) { return new SET_TIME(time); }
    public static SET_BG BG(string bg, Transition t = Transition.FADE_BLACK) { return new SET_BG(bg, t); }
    public static SET_MSG MSG(string msg, TextEffect t = TextEffect.NONE) { return new SET_MSG(msg, t); }
    public static SET_PROP PROP(string prop, int val) { return new SET_PROP(prop, val); }
    public static SET_FLAG FLAG(string prop) { return new SET_FLAG(prop); }
}
public class SET_SCENE : Action {
    string name;
    public SET_SCENE(string name) {
        this.name = name;
    }
    public override SceneDirective run() {
        Game.SetScene(this.name);
        return null;
    }
}
public class SET_CHOICES : Action {
    Choice left;
    Choice right;
    float t;
    public SET_CHOICES(Choice left, Choice right, float t = 0) {
        this.left = left;
        this.right = right;
        this.t = t;
    }
    public override SceneDirective run() {
        Game.SetChoices(this.left, this.right, this.t);
        return null;
    }
}
//public class SET_STATE : Action {
//    GameState state;
//    public SET_STATE(GameState state) { this.state = state; }
//    public override SceneDirective run() {
//        Game.SetState(state);
//        return null;
//    }
//}
public class SET_TIME : Action {
    int time;
    public SET_TIME(int time) {
        this.time = time;
    }
    public override SceneDirective run() {
        Game.SetTime(this.time);
        return null;
    }
}
public class SET_BG : Action {
    public string bgname;
    public Transition t;
    public SET_BG(string bgname, Transition t = Transition.FADE_BLACK) {
        this.bgname = bgname;
        this.t = t;
    }
    public override SceneDirective run() {
        Game.SetBG(this.bgname, this.t);
        return null;
    }
}
public class SET_MSG : Action {
    public string msg;
    public TextEffect e;
    public SET_MSG(string msg, TextEffect e) {
        this.msg = msg;
        this.e = e;
    }
    public override SceneDirective run() {
        Game.Message(this.msg, this.e);
        return null;
    }
}
public class SET_PROP : Action {
    string prop;
    int val;
    public SET_PROP(string prop, int val) {
        this.prop = prop;
        this.val = val;
    }
    public override SceneDirective run() {
        Game.SetProp(this.prop, this.val);
        return null;
    }
}
public class SET_FLAG : Action {
    string prop;
    public SET_FLAG(string prop) { this.prop = prop; }
    public override SceneDirective run() {
        Game.SetProp(this.prop, 1);
        return null;
    }
}
public class INC {
    public static INC_PROP PROP(string prop, int amt) { return new INC_PROP(prop, amt); }
    public static INC_STAT STAT(string prop, int amt, bool wait = true) { return new INC_STAT(prop, amt, wait); }
    public static INC_TIME TIME(int amt) { return new INC_TIME(amt); }
}
public class INC_PROP : Action {
    string prop;
    int amt;
    public INC_PROP(string prop, int amt) {
        this.prop = prop;
        this.amt = amt;
    }
    public override SceneDirective run() {
        Game.SetProp(this.prop, Game.Prop(this.prop) + this.amt);
        return null;
    }
}
public class INC_STAT : Action {
    string prop;
    int amt;
    bool wait;
    public INC_STAT(string prop, int amt, bool wait) {
        this.prop = prop;
        this.amt = amt;
        this.wait = wait;
    }
    public override SceneDirective run() {
        Game.IncStat(this.prop, this.amt, this.wait);
        return null;
    }
}
public class INC_TIME : Action {
    int amt;
    public INC_TIME(int amt) { this.amt = amt; }
    public override SceneDirective run() {
        int t = Game.GetTime();
        int hours = t / 100;
        int mins = t % 100;
        mins += this.amt;
        hours += mins / 60;
        mins = mins % 60;
        Game.SetTime(hours * 100 + mins);
        return null;
    }
}

public class COND {
    virtual public bool check() {
        return false;
    }
    public static COND_IF IF(COND cond, Action action, Action elseaction = null) { return new COND_IF(cond, action, elseaction); }
    public static COND_GT GT(string prop, int x) { return new COND_GT(prop, x); }
    public static COND_GTE GTE(string prop, int x) { return new COND_GTE(prop, x); }
    public static COND_LT LT(string prop, int x) { return new COND_LT(prop, x); }
    public static COND_LTE LTE(string prop, int x) { return new COND_LTE(prop, x); }
    public static COND_BEFORE BEFORE(int x) { return new COND_BEFORE(x); }
    public static COND_AFTER AFTER(int x) { return new COND_AFTER(x); }
    public static COND_FLAGGED FLAGGED(string prop) { return new COND_FLAGGED(prop); }
    public static COND_AND AND(params COND[] terms) { return new COND_AND(terms); }
    public static COND_OR OR(params COND[] terms) { return new COND_OR(terms); }
}
public class COND_IF : Action {
    public COND cond;
    public Action action;
    public Action elseaction;

    public COND_IF(COND cond, Action action, Action elseaction = null) {
        this.cond = cond;
        this.action = action;
        this.elseaction = elseaction;
    }
    override public SceneDirective run() {
        if (this.cond.check()) {
            return this.action.run();
        } else if (this.elseaction != null) {
            return this.elseaction.run();
        }
        return null;
    }
}
public class COND_GT : COND {
    string prop;
    int x;
    public COND_GT(string prop, int x) { this.prop = prop; this.x = x; }
    public override bool check() { return Game.Prop(this.prop) > this.x; }
}
public class COND_GTE : COND {
    string prop;
    int x;
    public COND_GTE(string prop, int x) { this.prop = prop; this.x = x; }
    public override bool check() { return Game.Prop(this.prop) >= this.x; }
}
public class COND_LT : COND {
    string prop;
    int x;
    public COND_LT(string prop, int x) { this.prop = prop; this.x = x; }
    public override bool check() { return Game.Prop(this.prop) < this.x; }
}
public class COND_LTE : COND {
    string prop;
    int x;
    public COND_LTE(string prop, int x) { this.prop = prop; this.x = x; }
    public override bool check() { return Game.Prop(this.prop) <= this.x; }
}
public class COND_BEFORE : COND {
    int t;
    public COND_BEFORE(int t) { this.t = t; }
    public override bool check() {
        return Game.GetTime() < t;
    }
}
public class COND_AFTER : COND {
    int t;
    public COND_AFTER(int t) { this.t = t; }
    public override bool check() {
        return Game.GetTime() >= t;
    }
}
public class COND_FLAGGED : COND {
    string prop;
    public COND_FLAGGED(string prop) { this.prop = prop; }
    public override bool check() { return Game.Prop(this.prop) != 0; }
}
public class COND_AND : COND {
    COND[] terms;
    public COND_AND(params COND[] terms) { this.terms = terms; }
    public override bool check() {
        foreach (COND c in this.terms) {
            if (!c.check()) {
                return false;
            }
        }
        return true;
    }
}
public class COND_OR : COND {
    COND[] terms;
    public COND_OR(params COND[] terms) { this.terms = terms; }
    public override bool check() {
        foreach (COND c in this.terms) {
            if (c.check()) {
                return true;
            }
        }
        return false;
    }
}