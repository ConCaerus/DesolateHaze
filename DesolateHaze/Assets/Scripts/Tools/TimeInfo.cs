using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TimeInfo {
    static string timeTag = "TimeInfoSeconds";
    static float lastUpdatedTime;

    public static void setStartTime() {
        lastUpdatedTime = Time.realtimeSinceStartup;
    }
    public static void saveTime() { //  gets called in transitioncanvas everytime it hides
        float passedTime = Time.realtimeSinceStartup - lastUpdatedTime;
        var totTime = SaveData.getFloat(timeTag) + passedTime;
        SaveData.setFloat(timeTag, totTime);
        lastUpdatedTime = Time.realtimeSinceStartup;
    }

    public static void addTime(float f) {
        SaveData.setFloat(timeTag, SaveData.getFloat(timeTag) + f);
    }

    public static void deleteTimeInfo() {
        SaveData.deleteKey(timeTag, false);
    }

    public static string timeToString() {
        var seconds = SaveData.getFloat(timeTag);
        var hours = seconds / 3600f;
        int h = (int)hours; //  save the hours without the decimal
        hours -= h; //  get the decimal off the the hours
        var minutes = hours * 60f;
        int m = (int)minutes;   //  save the minutes without the decimal

        int s = (int)((minutes - m) * 60f);

        return h.ToString("0") + ":" + m.ToString("00") + ":" + s.ToString("00");
    }

    public static void setSavedTime(float seconds) {
        SaveData.setFloat(timeTag, seconds);
    }

    public static float getSecondInSave() {
        return SaveData.getFloat(timeTag, 0f);
    }
}
