using UnityEngine;

public class DemoCanvas : MonoBehaviour {

    public void replay() {
        SaveData.wipe();
        TransitionCanvas.I.loadGame(Saver.areaType.Outside);
    }
    public void mainMenu() {
        TransitionCanvas.I.loadMainMenu();
    }
}
