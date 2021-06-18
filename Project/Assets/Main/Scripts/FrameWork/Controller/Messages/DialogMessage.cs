namespace GameBerry.UI.Event
{
    public class ShowDialogMsg : Message
    {
        public bool PlayAni = false;

        public ShowDialogMsg(bool _playAni)
        {
            PlayAni = _playAni;
        }
	}

	public class HideDialogMsg : Message
	{
        public bool PlayAni = false;

        public HideDialogMsg(bool _playAni)
        {
            PlayAni = _playAni;
        }
    }
}