using RubyCase.Event;

namespace RubyCase.UI.Pages
{
    public class UIFail : UIPage
    {
        public void OnTapPlayAgain()
        {
            GameEvents.Restart(true);
            Close();
        }
    }
}
