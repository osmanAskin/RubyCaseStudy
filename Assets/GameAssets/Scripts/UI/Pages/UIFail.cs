public class UIFail : UIPage
{
    public void OnTapPlayAgain()
    {
        GameEvents.Restart(true);
        Close();
    }
}
