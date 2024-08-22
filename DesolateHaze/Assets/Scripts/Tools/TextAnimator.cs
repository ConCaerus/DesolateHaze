using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public static class TextAnimator {

    public static void animateText(MonoBehaviour instance, TextMeshProUGUI t, string words, float timeBtwLetters) {
        instance.StartCoroutine(textAnim(t, words, timeBtwLetters));
    }

    static IEnumerator textAnim(TextMeshProUGUI t, string words, float timeBtwLetters) {
        //  sets up text for the animation
        t.text = "";
        Dictionary<int, string> specialSubs = new Dictionary<int, string>();
        for(int i = 0; i < words.Length; i++) {
            if(words[i] == '<') {
                int ind = i;
                do {
                    i++;
                } while(words[i] != '>');
                i++;
                specialSubs.Add(ind, words.Substring(ind, i - ind));
            }
            t.text += words[i];
        }
        string oriWords = words;
        words = t.text;
        t.color = Color.clear;
        yield return new WaitForEndOfFrame();
        t.color = Color.white;
        int prevInd = -1;
        for(int i = 0; i < words.Length; i++) {
            while(words[i] == ' ' && !specialSubs.ContainsKey(i)) {
                i++;
                prevInd++;
            }
            if(specialSubs.ContainsKey(i)) {
                words = words.Substring(0, i) + specialSubs[i] + words.Substring(i);
                i += specialSubs[i].Length;
            }



            if(i == 0 || prevInd == -1)
                t.text = words.Substring(0, i) + "<alpha=#00>" + words.Substring(i);
            else
                t.text = words.Substring(0, prevInd) + "<alpha=#35>" + words[prevInd] + "<alpha=#00>" + words.Substring(i);
            yield return new WaitForSeconds(timeBtwLetters);
            prevInd = i;
        }
        t.text = oriWords;
    }
}
