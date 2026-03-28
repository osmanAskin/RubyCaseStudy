using UnityEngine;

public class UIFail : UIPage
{
    public void OnTapPlayAgain()
    {
        GameEvents.Restart(true);
        Close();
    }

    public void OnTapHome()
    {
        GameEvents.LevelSetupRequested();
    }
}
