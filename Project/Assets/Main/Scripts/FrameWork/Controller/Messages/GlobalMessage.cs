namespace GameBerry.Event
{
    public class ShowPopup_OkMsg : Message
    {
        public string titletext;
        public System.Action okAction;

        public ShowPopup_OkMsg(string titletext, System.Action okAction)
        {
            this.titletext = titletext;
            this.okAction = okAction;
        }
    }

    public class ShowPopup_OkCalcelMsg : Message
    {
        public string titletext;
        public System.Action okAction;
        public System.Action cancelAction = null;

        public ShowPopup_OkCalcelMsg(string titletext, System.Action okAction, System.Action cancelAction)
        {
            this.titletext = titletext;
            this.okAction = okAction;
            this.cancelAction = cancelAction;
        }
    }

    public class ShowPopup_InputMsg : Message
    {
        public string titletext;
        public string defaultstr;
        public string placeholder;
        public System.Action<string> okAction;
        public System.Action cancelAction = null;

        public ShowPopup_InputMsg(string titletext, string defaultstr, string placeholder, System.Action<string> okAction, System.Action cancelAction)
        {
            this.titletext = titletext;
            this.defaultstr = defaultstr;
            this.placeholder = placeholder;
            this.okAction = okAction;
            this.cancelAction = cancelAction;
        }
    }

    public class SetBGMVolumeMsg : Message
    {
        public float volume;

        public SetBGMVolumeMsg(float volume)
        {
            this.volume = volume;
        }
    }

    public class SetFXVolumeMsg : Message
    {
        public float volume;

        public SetFXVolumeMsg(float volume)
        {
            this.volume = volume;
        }
    }

    public class DoFadeMsg : Message
    {
        public float duration;
        public bool visible; // false 투명 -> 검은색으로
    }
}