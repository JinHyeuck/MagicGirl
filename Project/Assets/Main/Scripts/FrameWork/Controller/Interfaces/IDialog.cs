using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBerry.UI
{
	public class IDialog : MonoBehaviour
	{
		protected RectTransform _rt;
		protected string _name;
		public GameObject dialogView;

        public bool isEnter { get { return _isEnter; } }

        protected bool _isEnter = false;

        protected IDialogAnimation _dialogAnimation;

        private bool m_endDialogAnimation = false;
        private bool m_endUIAnimations = false;

        [SerializeField]
        private List<IDialogAnimation> _uiInAnimations = null;

        [SerializeField]
        private List<IDialogAnimation> _uiOutAnimations = null;

        private int m_doingUIAnimationCount = 0;

        void Awake()
		{
			if (dialogView == null)
				throw new System.NullReferenceException(string.Format("{0} dialogView Null", this.name));
		}

		public void Load()
		{
			_name = GetType().Name;
			_rt = GetComponent<RectTransform>();
            _dialogAnimation = GetComponent<IDialogAnimation>();

            if (_dialogAnimation != null)
            {
                _dialogAnimation.OnInAnimationsStart.AddListener(OnEnterAniStart);
                _dialogAnimation.OnInAnimationsFinish.AddListener(OnEnterAniFinish);
                _dialogAnimation.OnOutAnimationsStart.AddListener(OnExitAniStart);
                _dialogAnimation.OnOutAnimationsFinish.AddListener(OnExitAniFinish);
            }

            if (_uiOutAnimations != null)
            {
                for (int i = 0; i < _uiOutAnimations.Count; ++i)
                {
                    _uiOutAnimations[i].OnOutAnimationsFinish.AddListener(OnEndUIAnimation);
                }
            }

            Message.AddListener<Event.ShowDialogMsg>(_name, Enter);
			Message.AddListener<Event.HideDialogMsg>(_name, Exit);

			int sibling = EnumExtensions.ParseToInt<UISibling>(_name);
			UIManager.Instance.SetSibling(_rt, sibling);

			dialogView.SetActive(false);
			OnLoad();
		}

        public void TestLoad()
        {
            _name = GetType().Name;
            _rt = GetComponent<RectTransform>();
            _dialogAnimation = GetComponent<IDialogAnimation>();

            if (_dialogAnimation != null)
            {
                _dialogAnimation.OnInAnimationsStart.AddListener(OnEnterAniStart);
                _dialogAnimation.OnInAnimationsFinish.AddListener(OnEnterAniFinish);
                _dialogAnimation.OnOutAnimationsStart.AddListener(OnExitAniStart);
                _dialogAnimation.OnOutAnimationsFinish.AddListener(OnExitAniFinish);
            }

            if (_uiOutAnimations != null)
            {
                for (int i = 0; i < _uiOutAnimations.Count; ++i)
                {
                    _uiOutAnimations[i].OnOutAnimationsFinish.AddListener(OnEndUIAnimation);
                }
            }

            Message.AddListener<Event.ShowDialogMsg>(_name, Enter);
            Message.AddListener<Event.HideDialogMsg>(_name, Exit);

            dialogView.SetActive(false);
            OnLoad();
        }

        protected virtual void OnLoad()
		{
		}

		public void Unload()
		{
			Message.RemoveListener<Event.ShowDialogMsg>(_name, Enter);
			Message.RemoveListener<Event.HideDialogMsg>(_name, Exit);

			OnExit();
			OnUnload();
		}

		protected virtual void OnUnload()
		{
		}

		private void Enter(Event.ShowDialogMsg msg)
		{
            //Log.Instance.log(string.Format("{0} - IDialog Enter", _name));

            if (dialogView != null)
            {
                if (dialogView.activeSelf)
                    return;
                dialogView.SetActive(true);
            }

            if (_dialogAnimation != null && msg.PlayAni == true)
                _dialogAnimation.PlayInAnimation();

            if (_uiInAnimations != null)
            {
                for (int i = 0; i < _uiInAnimations.Count; ++i)
                {
                    _uiInAnimations[i].PlayInAnimation();
                }
            }

            _isEnter = true;
			OnEnter();
		}

		private void Exit(Event.HideDialogMsg msg)
		{
            //Log.Instance.log(string.Format("{0} - IDialog Exit", _name));
            if (_dialogAnimation != null && msg.PlayAni == true && _uiInAnimations == null)
            {
                // DialogAniamtion만 사용할 때
                _dialogAnimation.PlayOutAnimation();
                return;
            }
            else if (_dialogAnimation != null && msg.PlayAni == true && _uiInAnimations != null)
            {
                // DialogAnimation과 UIAnimation도 같이 사용할 때

                _dialogAnimation.PlayOutAnimation();

                if (_uiInAnimations.Count <= 0)
                    return;

                m_endDialogAnimation = true;
                m_endUIAnimations = true;

                m_doingUIAnimationCount = _uiInAnimations.Count;

                for (int i = 0; i < _uiInAnimations.Count; ++i)
                {
                    _uiInAnimations[i].PlayOutAnimation();
                }

                return;
            }
            else if (_dialogAnimation == null && msg.PlayAni == true && _uiInAnimations != null)
            {
                // UIAnimation만 사용할 때

                if (_uiInAnimations.Count > 0)
                {
                    m_endUIAnimations = true;

                    m_doingUIAnimationCount = _uiInAnimations.Count;

                    for (int i = 0; i < _uiInAnimations.Count; ++i)
                    {
                        _uiInAnimations[i].PlayOutAnimation();
                    }

                    return;
                }
            }


            if (dialogView != null)
				dialogView.SetActive(false);

            _isEnter = false;
            OnExit();
		}

        protected virtual void OnDestroy()
        {
            Unload();
        }

		protected virtual void OnEnter()
		{
		}

		protected virtual void OnExit()
		{
		}

        /// 애니메이션 대비 함수
        protected virtual void OnEnterAniStart()
        {
        }

        protected virtual void OnEnterAniFinish()
        {
        }

        protected virtual void OnExitAniStart()
        {
        }

        /// OnEnterAniStart를 재정의 할 시 꼭 적절한 타이밍에 base.OnExitAniFinish();을 해줘야 한다.
        protected virtual void OnExitAniFinish()
        {
            m_endDialogAnimation = false;

            if(m_endUIAnimations == false)
                Exit(new Event.HideDialogMsg(false));
        }

        private void OnEndUIAnimation()
        {
            m_doingUIAnimationCount--;
            if (m_doingUIAnimationCount <= 0)
            {
                m_endUIAnimations = false;

                if (m_endDialogAnimation == false)
                    OnExitAniFinish();
            }
        }
        /// 애니메이션 대비 함수

        public static void RequestDialogEnter<T>(bool _playAni = false) where T : IDialog
		{
			Message.Send<UI.Event.ShowDialogMsg>(typeof(T).Name, new UI.Event.ShowDialogMsg(_playAni));
		}

		public static void RequestDialogExit<T>(bool _playAni = false) where T : IDialog
		{
            Message.Send<UI.Event.HideDialogMsg>(typeof(T).Name, new UI.Event.HideDialogMsg(_playAni));
		}

        public void EnterDialog(bool _playAni = false)
        {
            Enter(new Event.ShowDialogMsg(_playAni));
        }

        public void ExitDialog(bool _playAni = false)
        {
            Exit(new Event.HideDialogMsg(_playAni));
        }
    }
}
