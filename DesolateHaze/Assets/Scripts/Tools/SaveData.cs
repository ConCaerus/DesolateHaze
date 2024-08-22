using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveData {
    static string saveIndexString = "Save Data Save Index";
    static string usedTagsTag = "UsedTagsTag";
    static string saveIndexTag() {
        return saveTag(PlayerPrefs.GetInt(saveIndexString));
    }
    static string saveTag(int i) {
        return "Save" + i.ToString() + " ";
    }

    //  NOTE: universal things are saved across all save slots (for things like time and such)
    static string universalTag() {
        return "Universal Save ";
    }



    public static void setString(string tag, string save) {
        storeTag(getCurrentSaveIndex(), tag);
        PlayerPrefs.SetString(saveIndexTag() + tag, save);
        PlayerPrefs.Save();
    }
    public static void setUniversalString(string tag, string save) {
        storeTag(-1, tag);
        PlayerPrefs.SetString(universalTag() + tag, save);
        PlayerPrefs.Save();
    }
    public static string getString(string tag, string catcher = null) {
        storeTag(getCurrentSaveIndex(), tag);
        return PlayerPrefs.GetString(saveIndexTag() + tag, catcher);
    }
    public static string getUniversalString(string tag, string catcher = null) {
        storeTag(-1, tag);
        return PlayerPrefs.GetString(universalTag() + tag, catcher);
    }
    public static string getStringInSave(int index, string tag) {
        storeTag(index, tag);
        return PlayerPrefs.GetString(saveTag(index) + tag, null);
    }


    public static void setInt(string tag, int save) {
        storeTag(getCurrentSaveIndex(), tag);
        PlayerPrefs.SetInt(saveIndexTag() + tag, save);
        PlayerPrefs.Save();
    }
    public static void setUniversalInt(string tag, int save) {
        storeTag(-1, tag);
        PlayerPrefs.SetInt(universalTag() + tag, save);
        PlayerPrefs.Save();
    }
    public static int getInt(string tag, int catcher = 0) {
        storeTag(getCurrentSaveIndex(), tag);
        return PlayerPrefs.GetInt(saveIndexTag() + tag, catcher);
    }
    public static int getUniversalInt(string tag, int catcher = 0) {
        storeTag(-1, tag);
        return PlayerPrefs.GetInt(universalTag() + tag, catcher);
    }
    public static int getIntInSave(int index, string tag) {
        storeTag(index, tag);
        return PlayerPrefs.GetInt(saveTag(index) + tag, 0);
    }


    public static void setFloat(string tag, float save) {
        storeTag(getCurrentSaveIndex(), tag);
        PlayerPrefs.SetFloat(saveIndexTag() + tag, save);
        PlayerPrefs.Save();
    }
    public static void setUniversalFloat(string tag, float save) {
        storeTag(-1, tag);
        PlayerPrefs.SetFloat(universalTag() + tag, save);
        PlayerPrefs.Save();
    }
    public static float getFloat(string tag, float catcher = 0.0f) {
        storeTag(getCurrentSaveIndex(), tag);
        return PlayerPrefs.GetFloat(saveIndexTag() + tag, catcher);
    }
    public static float getUniversalFloat(string tag, float catcher = 0.0f) {
        storeTag(-1, tag);
        return PlayerPrefs.GetFloat(universalTag() + tag, catcher);
    }
    public static float getFloatInSave(int index, string tag) {
        storeTag(index, tag);
        return PlayerPrefs.GetFloat(saveTag(index) + tag, 0.0f);
    }


    public static void deleteKey(string tag, bool uTag) {
        PlayerPrefs.DeleteKey(uTag ? universalTag() : saveIndexTag() + tag);
        PlayerPrefs.Save();
    }
    public static void deleteKeyInSave(int index, string tag) {
        PlayerPrefs.DeleteKey(saveTag(index) + tag);
        PlayerPrefs.Save();
    }

    public static void deleteCurrentSave() {
        deleteSave(PlayerPrefs.GetInt(saveIndexString));
    }
    public static void deleteSave(int i) {
        var prevIndex = getCurrentSaveIndex();
        setCurrentSaveIndex(i);

        deleteAllTagsInCurrentSave();

        setCurrentSaveIndex(prevIndex);
        PlayerPrefs.Save();
    }

    //  clears all player prefs
    public static void wipe() {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }

    static void deleteAllTagsInCurrentSave() {
        var th = getTagCollector();

        switch(getCurrentSaveIndex()) {
            case 0:
                //  deletes all keys
                foreach(var i in th.t1) 
                    deleteKey(i, false);

                //  saves the new, empty list
                th.t1 = new List<string>();
                break;
            case 1:
                //  deletes all keys
                foreach(var i in th.t2)
                    deleteKey(i, false);

                //  saves the new, empty list
                th.t2 = new List<string>();
                break;
            case 2:
                //  deletes all keys
                foreach(var i in th.t3)
                    deleteKey(i, false);

                //  saves the new, empty list
                th.t3 = new List<string>();
                break;
        }
        saveTagCollector(th);
    }


    static void storeTag(int saveIndex, string tag) {
        TagCollector holder = getTagCollector();

        //  adds the new tag into memory if it's new
        switch(saveIndex) {
            //  Universal save tags
            case -1:
                if(!holder.universals.Contains(tag))
                    holder.universals.Add(tag);
                break;
            case 0:
                if(!holder.t1.Contains(tag))
                    holder.t1.Add(tag);
                break;
            case 1:
                if(!holder.t2.Contains(tag))
                    holder.t2.Add(tag);
                break;
            case 2:
                if(!holder.t3.Contains(tag))
                    holder.t3.Add(tag);
                break;
        }

        //  saves the updated list
        saveTagCollector(holder);
    }
    static TagCollector getTagCollector() {
        TagCollector holder = new TagCollector();
        var data = PlayerPrefs.GetString(usedTagsTag, "");
        if(!string.IsNullOrEmpty(data))
            holder = JsonUtility.FromJson<TagCollector>(data);
        return holder;
    }
    static void saveTagCollector(TagCollector th) {
        var d = JsonUtility.ToJson(th);
        PlayerPrefs.SetString(usedTagsTag, d);
    }

    public static int getCurrentSaveIndex() {
        return PlayerPrefs.GetInt(saveIndexString);
    }
    public static void setCurrentSaveIndex(int i) {
        PlayerPrefs.SetInt(saveIndexString, i);
    }

    public static bool hasSaveDataForSlot(int i) {
        int prev = getCurrentSaveIndex();
        setCurrentSaveIndex(i);
        bool temp = TimeInfo.getSecondInSave() > 0f;
        setCurrentSaveIndex(prev);
        return temp;
    }
}

//  this stores all of the tags that have been used for each save
//  once a new tag is used that hasn't been used before, it gets stored in here
//  when clearing a save, I just loop over each tag in the desired array
[System.Serializable]
public class TagCollector {
    //  I tried using an array that held these lists, didn't work :(
    public List<string> universals = new List<string>();
    public List<string> t1 = new List<string>();
    public List<string> t2 = new List<string>();
    public List<string> t3 = new List<string>();
}
