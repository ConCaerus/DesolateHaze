using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public static class ConTween {
    public enum TweenType {
        None, Linear, Smooth, Smoother, Quad,
        InverseSmooth, InverseSmoother, InverseQuad,
        Reverse, ReverseSmooth, ReverseSmoother, ReverseQuad,
        ReverseInverseSmooth, ReverseInverseSmoother, ReverseInverseQuad
    }

    public delegate void task(float perc);

    public static List<CTweenInstance> activeTweens = new List<CTweenInstance>();
    static Dictionary<int, List<CTweenInstance>> assignedTweens = new Dictionary<int, List<CTweenInstance>>();

    public static MonoBehaviour instance;
    public static bool initted { get; private set; } = false;

    public static void init() {
        if(instance == null)
            instance = new GameObject("ConTweenManager").AddComponent<ConTweenHandler>();
        initted = true;
    }
    public static void reset() {
        activeTweens.Clear();

        instance = null;
        initted = false;
    }

    public static void deTween(CTweenInstance i) {
        if(activeTweens.Contains(i)) activeTweens.Remove(i);
    }

    public static float getPercValue(TweenType t, float perc) {
        switch(t) {
            //  regulars
            case TweenType.Linear: return perc;
            case TweenType.Smooth: return 3f * perc * perc - 2f * perc * perc * perc;
            case TweenType.Smoother: return 6f * Mathf.Pow(perc, 5f) - 15f * Mathf.Pow(perc, 4f) + 10f * Mathf.Pow(perc, 3f);
            case TweenType.Quad: return perc * perc;

            //  inverses
            case TweenType.InverseSmooth: return 1f;
            case TweenType.InverseSmoother: return 1f;
            case TweenType.InverseQuad: return -1f * ((perc - 1f) * (perc - 1f)) + 1f;

            //  reverses
            case TweenType.Reverse: return 1f - getPercValue(TweenType.Linear, perc);
            case TweenType.ReverseSmooth: return 1f - getPercValue(TweenType.Smooth, perc);
            case TweenType.ReverseSmoother: return 1f - getPercValue(TweenType.Smoother, perc);
            case TweenType.ReverseQuad: return 1f - getPercValue(TweenType.Quad, perc);

            //  reverse inverses
            case TweenType.ReverseInverseSmooth: return 1f - getPercValue(TweenType.InverseSmooth, perc);
            case TweenType.ReverseInverseSmoother: return 1f - getPercValue(TweenType.Smoother, perc);
            case TweenType.ReverseInverseQuad: return 1f - getPercValue(TweenType.InverseQuad, perc);
            default: return perc;
        }
    }

    #region BASICS
    //  NOTE: customTasks does not work with this
    //          - update and complete tasks get overridden to use actual perc instead of relevant float
    public static CTweenInstance tweenFloat(float from, float to, CTweenInfo info) {
        if(!initted || instance == null) init();
        var t = new CTweenInstance();

        var tempU = info.runOnUpdate;
        var tempC = info.runOnComplete;

        //  changes tasks to use the float parameters instead of the perc parameters
        task updator = (perc) => {
            if(tempU != null)
                tempU(from + (to - from) * perc);
        };
        task completer = (perc) => {
            if(tempC != null)
                tempC(from + (to - from) * perc);
        };

        info.runOnUpdate = updator;
        info.runOnComplete = completer;
        t.start(info);
        activeTweens.Add(t);
        return t;
    }
    public static CTweenInstance genericTween(CTweenInfo info, task otherU, task otherC, Transform assigned) {
        if(!initted || instance == null) init();
        var t = new CTweenInstance();

        //  creates custom tasks
        var tempU = info.runOnUpdate;
        var tempC = info.runOnComplete;
        task updator = (perc) => {
            if(tempU != null) tempU(perc);
            if(otherU != null) otherU(perc);
        };
        task completer = (perc) => {
            if(tempC != null) tempC(perc);
            if(otherC != null) otherC(perc);
        };

        info.runOnUpdate = updator;
        info.runOnComplete = completer;
        t.start(info);

        //  stores reference in lists
        activeTweens.Add(t);
        if(assigned != null) {
            if(assignedTweens.ContainsKey(assigned.GetInstanceID())) assignedTweens[assigned.GetInstanceID()].Add(t);   //  adds to the already created list
            else assignedTweens[assigned.GetInstanceID()] = new List<CTweenInstance>() { t };   //  otherwise, create a new list and add t to that
        }
        return t;
    }
    public static void kill(Transform t) {
        var id = t.GetInstanceID();
        if(!assignedTweens.ContainsKey(id)) return;
        foreach(var i in assignedTweens[id]) i.stop();
        assignedTweens.Remove(id);
    }
    #endregion

    #region EXTENSIONS
    public static T customTasks<T>(this T t, task onUpdate, task onComplete) where T : CTweenInstance {
        if(t == null || !t.running) {
            return t;
        }
        var tempU = t.info.runOnUpdate;
        var tempC = t.info.runOnComplete;

        task updator = (perc) => {
            if(tempU != null) tempU(perc);
            if(onUpdate != null) onUpdate(perc);
        };
        task completer = (perc) => {
            if(tempC != null) tempC(perc);
            if(onComplete != null) onComplete(perc);
        };

        t.info.runOnUpdate = updator;
        t.info.runOnComplete = completer;
        return t;
    }
    #endregion

    #region POSITION
    public static CTweenInstance tweenPos(Transform obj, Vector3 endPos, CTweenInfo info) {
        var startP = obj.position;
        return genericTween(info, (perc) => {
            if(obj != null) obj.position = startP + (endPos - startP) * perc;
        }, (perc) => {
            if(obj != null) obj.position = startP + (endPos - startP) * perc;
        }, obj);
    }
    public static CTweenInstance tweenLocalPos(Transform obj, Vector3 endPos, CTweenInfo info) {
        var startP = obj.localPosition;
        return genericTween(info, (perc) => {
            if(obj != null) obj.localPosition = startP + (endPos - startP) * perc;
        }, (perc) => {
            if(obj != null) obj.localPosition = startP + (endPos - startP) * perc;
        }, obj);
    }
    #endregion

    #region SCALE
    public static CTweenInstance tweenScale(Transform obj, Vector3 endScale, CTweenInfo info) {
        var startS = obj.localScale;
        return genericTween(info, (perc) => {
            if(obj != null) obj.localScale = startS + (endScale - startS) * perc;
        }, (perc) => {
            if(obj != null) obj.localScale = startS + (endScale - startS) * perc;
        }, obj);
    }
    public static CTweenInstance tweenScale(Transform obj, float endScale, CTweenInfo info) {
        return tweenScale(obj, Vector3.one * endScale, info);
    }
    #endregion

    #region ROTATION
    public static CTweenInstance tweenRotation(Transform obj, Vector3 endRot, CTweenInfo info) {
        var startR = obj.localEulerAngles;

        //  checks for best rot direction 
        if(Mathf.Abs(startR.z - endRot.z) > Mathf.Abs((startR.z - 360f) - endRot.z)) startR.z -= 360f;

        return genericTween(info, (perc) => {
            if(obj != null) obj.localEulerAngles = startR + (endRot - startR) * perc;
        }, (perc) => {
            if(obj != null) obj.localEulerAngles = startR + (endRot - startR) * perc;
        }, obj);
    }
    public static CTweenInstance tweenRotation(Transform obj, float endRot, CTweenInfo info) {
        return tweenRotation(obj, Vector3.forward * endRot, info);
    }
    #endregion

    #region COLOR
    public static CTweenInstance tweenColor(SpriteRenderer sr, Color endCol, CTweenInfo info) {
        var startC = sr.color;
        return genericTween(info, (perc) => {
            sr.color = startC+ (endCol - startC) * perc;
        }, (perc) => {
            sr.color = startC + (endCol - startC) * perc;
        }, sr.transform);
    }
    public static CTweenInstance tweenColor(Image img, Color endCol, CTweenInfo info) {
        var startC = img.color;
        return genericTween(info, (perc) => {
            img.color = startC + (endCol - startC) * perc;
        }, (perc) => {
            img.color = startC + (endCol - startC) * perc;
        }, img.transform);
    }
    public static CTweenInstance tweenColor(TextMeshProUGUI t, Color endCol, CTweenInfo info) {
        var startC = t.color;
        return genericTween(info, (perc) => {
            t.color = startC + (endCol - startC) * perc;
        }, (perc) => {
            t.color = startC + (endCol - startC) * perc;
        }, t.transform);
    }
    #endregion
}

//  Instance of tween
[System.Serializable]
public class CTweenInstance {
    public float percentage;
    public bool running = false;
    public CTweenInfo info;
    Coroutine runner;

    public void start(CTweenInfo i) {
        info = i;
        stop();
        running = true;
        runner = ConTween.instance.StartCoroutine(runnerWaiter());
    }
    public void stop() {
        if(runner != null) {
            ConTween.instance.StopCoroutine(runner);
            runner = null;
        }
        percentage = ConTween.getPercValue(info.tweenType, 0);
        running = false;
        ConTween.deTween(this);
    }

    IEnumerator runnerWaiter() {
        var elapsedTime = 0f;
        var startTime = (info.timeDependent ? Time.time : Time.realtimeSinceStartup);
        if(info.runOnUpdate != null) info.runOnUpdate(percentage);
        while(elapsedTime < info.duration) {
            yield return new WaitForEndOfFrame();
            elapsedTime += (info.timeDependent ? Time.time : Time.realtimeSinceStartup) - startTime;
            startTime = (info.timeDependent ? Time.time : Time.realtimeSinceStartup);
            percentage = ConTween.getPercValue(info.tweenType, elapsedTime / info.duration);

            if(info.runOnUpdate != null) info.runOnUpdate(percentage);
        }
        percentage = ConTween.getPercValue(info.tweenType, 1);

        if(info.runOnComplete != null) info.runOnComplete(percentage);

        runner = null;
        stop();
    }
}

//  Storage for information about a tween
[System.Serializable]
public class CTweenInfo {
    public float duration;
    public ConTween.task runOnUpdate, runOnComplete;
    public bool timeDependent;
    public ConTween.TweenType tweenType;

    public CTweenInfo(float duration, bool timeDependent, ConTween.TweenType tweenType, ConTween.task runOnUpdate, ConTween.task runOnComplete) {
        this.duration = duration;
        this.timeDependent = timeDependent;
        this.tweenType = tweenType;
        this.runOnUpdate = runOnUpdate;
        this.runOnComplete = runOnComplete;
    }
    public CTweenInfo(float duration, bool timeDependent, ConTween.TweenType tweenType) {
        this.duration = duration;
        this.timeDependent = timeDependent;
        this.tweenType = tweenType;
    }
    public CTweenInfo(float duration, bool timeDependent) {
        this.duration = duration;
        this.timeDependent = timeDependent;
        this.tweenType = ConTween.TweenType.Smoother;
    }
    public CTweenInfo(float duration) {
        this.duration = duration;
        this.timeDependent = true;
        this.tweenType = ConTween.TweenType.Smoother;
    }
}

//  Monobehaviour instance that is used to handle all tween coroutines
public class ConTweenHandler : MonoBehaviour {
    private void OnDisable() {
        ConTween.reset();
    }
}
