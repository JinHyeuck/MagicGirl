using System.Collections;
using UnityEngine;

namespace GameBerry.UI
{
    [System.Serializable]
    public class BaseAnimationStruct
    {
        public bool m_useAnimation;

        public float m_StartDelay;
        public float m_Duration;

        public bool m_Linear = true;
        public AnimationCurve m_AnimationCurve = new AnimationCurve(
            new Keyframe[2] { new Keyframe(0.0f, 0.0f, 0.5f, 0.5f), new Keyframe(1.0f, 1.0f, 0.5f, 0.5f) });
    }

    [System.Serializable]
    public class MoveAniStruct : BaseAnimationStruct
    {
        public enum MoveDirection
        {
            Left = 0,
            Right = 1,
            Top = 2,
            Bottom = 3,
            TopLeft = 4,
            TopCenter = 5,
            TopRight = 6,
            MiddleLeft = 7,
            MiddleCenter = 8,
            MiddleRight = 9,
            BottomLeft = 10,
            BottomCenter = 11,
            BottomRight = 12,
            CustomPosition = 13
        }

        public MoveDirection m_MoveFrom = MoveDirection.Left;

        public Vector3 m_CustomPosition = Vector3.zero;

        public Vector3 GetTargetPosition(RectTransform target, Vector3 startPosition)
        {
            Rect rootCanvasRect = target.rect;
            float xOffset = rootCanvasRect.width / 2 + target.rect.width * target.pivot.x;
            float yOffset = rootCanvasRect.height / 2 + target.rect.height * target.pivot.y;
            switch (m_MoveFrom)
            {
                case MoveDirection.Left: return new Vector3(-xOffset, startPosition.y, startPosition.z);
                case MoveDirection.Right: return new Vector3(xOffset, startPosition.y, startPosition.z);
                case MoveDirection.Top: return new Vector3(startPosition.x, yOffset, startPosition.z);
                case MoveDirection.Bottom: return new Vector3(startPosition.x, -yOffset, startPosition.z);
                case MoveDirection.TopLeft: return new Vector3(-xOffset, yOffset, startPosition.z);
                case MoveDirection.TopCenter: return new Vector3(0, yOffset, startPosition.z);
                case MoveDirection.TopRight: return new Vector3(xOffset, yOffset, startPosition.z);
                case MoveDirection.MiddleLeft: return new Vector3(-xOffset, 0, startPosition.z);
                case MoveDirection.MiddleCenter: return new Vector3(0, 0, startPosition.z);
                case MoveDirection.MiddleRight: return new Vector3(xOffset, 0, startPosition.z);
                case MoveDirection.BottomLeft: return new Vector3(-xOffset, -yOffset, startPosition.z);
                case MoveDirection.BottomCenter: return new Vector3(0, -yOffset, startPosition.z);
                case MoveDirection.BottomRight: return new Vector3(xOffset, -yOffset, startPosition.z);
                case MoveDirection.CustomPosition: return m_CustomPosition;
                default: return Vector3.zero;
            }
        }
    }

    [System.Serializable]
    public class RotateAniStruct : BaseAnimationStruct
    {
        public Vector3 m_Rotate = Vector3.zero;
    }

    [System.Serializable]
    public class ScaleAniStruct : BaseAnimationStruct
    {
        public Vector3 m_Scale = Vector3.one;
    }

    [System.Serializable]
    public class FadeAniStruct : BaseAnimationStruct
    {
        public float m_StartAlpha;
        public float m_EndAlpha;
    }

    [System.Serializable]
    public class IDialogAnimations
    {
        public MoveAniStruct m_MoveAni = new MoveAniStruct();
        public RotateAniStruct m_RotateAni = new RotateAniStruct();
        public ScaleAniStruct m_ScaleAni = new ScaleAniStruct();
        public FadeAniStruct m_FadeAni = new FadeAniStruct();

        public float m_totalDuration { get; private set; }

        public void SetTotalDuration()
        {
            m_totalDuration = GetTotalAnimationDuration();
        }

        private float GetTotalAnimationDuration()
        {
            float totaltime = 0.0f;

            if (totaltime < GetTotalDuration(m_MoveAni))
                totaltime = GetTotalDuration(m_MoveAni);

            if (totaltime < GetTotalDuration(m_RotateAni))
                totaltime = GetTotalDuration(m_RotateAni);

            if (totaltime < GetTotalDuration(m_ScaleAni))
                totaltime = GetTotalDuration(m_ScaleAni);

            if (totaltime < GetTotalDuration(m_FadeAni))
                totaltime = GetTotalDuration(m_FadeAni);

            return totaltime;
        }

        private float GetTotalDuration(BaseAnimationStruct ani)
        {
            if (ani == null)
                return 0.0f;

            if (ani.m_useAnimation == false)
                return 0.0f;

            return ani.m_StartDelay + ani.m_Duration;
        }
    }

    [RequireComponent(typeof(CanvasGroup))]
    public class IDialogAnimation : MonoBehaviour
    {
        [HideInInspector]
        public Transform m_AnimationTarget;

        [HideInInspector]
        public bool m_useInAnimation;
        [HideInInspector]
        public bool m_useOutAnimation;

        public bool IsDoingInAnimation { get { return m_doingInAnimation; } }
        private bool m_doingInAnimation;
        private float m_endTime_InAnimation;

        public bool IsDoingOutAnimation { get { return m_doingOutAnimation; } }
        private bool m_doingOutAnimation;
        private float m_endTime_OutAnimation;

        [HideInInspector]
        public IDialogAnimations m_InAnimation = new IDialogAnimations();
        [HideInInspector]
        public IDialogAnimations m_OutAnimation = new IDialogAnimations();

        private RectTransform m_rectTransform;

        private Vector3 m_startPos;
        private Vector3 m_startRotate;
        private Vector3 m_startScale;
        private CanvasGroup m_canvasGroup;

        private Coroutine m_moveCoroutine;
        private Coroutine m_rotateCoroutine;
        private Coroutine m_scaleCoroutine;
        private Coroutine m_fadeCoroutine;

        public UnityEngine.Events.UnityEvent OnInAnimationsStart;
        public UnityEngine.Events.UnityEvent OnInAnimationsFinish;
        public UnityEngine.Events.UnityEvent OnOutAnimationsStart;
        public UnityEngine.Events.UnityEvent OnOutAnimationsFinish;

        //------------------------------------------------------------------------------------
        private void Awake()
        {
            m_rectTransform = m_AnimationTarget == null ? GetComponent<RectTransform>() : m_AnimationTarget.GetComponent<RectTransform>();
            m_startPos = m_rectTransform.anchoredPosition3D;
            m_startRotate = m_rectTransform.eulerAngles;
            m_startScale = m_rectTransform.localScale;
            m_canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();
        }
        //------------------------------------------------------------------------------------
        private void Update()
        {
            if (m_doingInAnimation == true)
            {
                if (m_endTime_InAnimation <= Time.time)
                {
                    if (OnInAnimationsFinish != null)
                        OnInAnimationsFinish.Invoke();

                    m_doingInAnimation = false;
                }
            }

            if (m_doingOutAnimation == true)
            {
                if (m_endTime_OutAnimation <= Time.time)
                {
                    if (OnOutAnimationsFinish != null)
                        OnOutAnimationsFinish.Invoke();

                    m_doingOutAnimation = false;
                }
            }
        }
        //------------------------------------------------------------------------------------
        private void StopCoroutine_All()
        {
            if (m_moveCoroutine != null)
            {
                StopCoroutine(m_moveCoroutine);
                m_moveCoroutine = null;
            }

            if (m_rotateCoroutine != null)
            {
                StopCoroutine(m_rotateCoroutine);
                m_rotateCoroutine = null;
            }

            if (m_scaleCoroutine != null)
            {
                StopCoroutine(m_scaleCoroutine);
                m_scaleCoroutine = null;
            }

            if (m_fadeCoroutine != null)
            {
                StopCoroutine(m_fadeCoroutine);
                m_fadeCoroutine = null;
            }
        }
        //------------------------------------------------------------------------------------
        public void PlayInAnimation()
        {
            StopCoroutine_All();

            if (m_doingOutAnimation == true)
            {
                m_doingOutAnimation = false;

                if (OnOutAnimationsFinish != null)
                    OnOutAnimationsFinish.Invoke();
            }

            if (OnInAnimationsStart != null)
                OnInAnimationsStart.Invoke();

            if (m_useInAnimation == true)
                m_InAnimation.SetTotalDuration();

            if (m_InAnimation.m_totalDuration == 0.0f)
            {
                if (OnInAnimationsFinish != null)
                    OnInAnimationsFinish.Invoke();

                return;
            }
            else
            {
                m_endTime_InAnimation = Time.time + m_InAnimation.m_totalDuration;
                m_doingInAnimation = true;
            }

            if (gameObject.activeInHierarchy == false)
                return;

            if (m_InAnimation.m_MoveAni.m_useAnimation)
                m_moveCoroutine = StartCoroutine(PlayMove(m_InAnimation.m_MoveAni.m_StartDelay,
                    m_InAnimation.m_MoveAni.m_Duration,
                    m_InAnimation.m_MoveAni.GetTargetPosition(m_rectTransform, m_startPos),
                    m_startPos,
                    m_InAnimation.m_MoveAni.m_Linear == true ? null : m_InAnimation.m_MoveAni.m_AnimationCurve));

            if (m_InAnimation.m_RotateAni.m_useAnimation)
                m_rotateCoroutine = StartCoroutine(PlayRotate(m_InAnimation.m_RotateAni.m_StartDelay,
                    m_InAnimation.m_RotateAni.m_Duration,
                    m_InAnimation.m_RotateAni.m_Rotate,
                    m_startRotate,
                    m_InAnimation.m_RotateAni.m_Linear == true ? null : m_InAnimation.m_RotateAni.m_AnimationCurve));

            if (m_InAnimation.m_ScaleAni.m_useAnimation)
                m_scaleCoroutine = StartCoroutine(PlayScale(m_InAnimation.m_ScaleAni.m_StartDelay,
                    m_InAnimation.m_ScaleAni.m_Duration,
                    m_InAnimation.m_ScaleAni.m_Scale,
                    m_startScale,
                    m_InAnimation.m_ScaleAni.m_Linear == true ? null : m_InAnimation.m_ScaleAni.m_AnimationCurve));

            if (m_InAnimation.m_FadeAni.m_useAnimation)
                m_fadeCoroutine = StartCoroutine(PlayFade(m_InAnimation.m_FadeAni.m_StartDelay,
                    m_InAnimation.m_FadeAni.m_Duration,
                    m_InAnimation.m_FadeAni.m_StartAlpha,
                    m_InAnimation.m_FadeAni.m_EndAlpha,
                    m_InAnimation.m_FadeAni.m_Linear == true ? null : m_InAnimation.m_FadeAni.m_AnimationCurve));
        }
        //------------------------------------------------------------------------------------
        public void PlayOutAnimation()
        {
            StopCoroutine_All();

            if (m_doingInAnimation == true)
            {
                m_doingInAnimation = false;

                if (OnInAnimationsFinish != null)
                    OnInAnimationsFinish.Invoke();
            }

            if (OnOutAnimationsStart != null)
                OnOutAnimationsStart.Invoke();

            if (m_useOutAnimation == true)
                m_OutAnimation.SetTotalDuration();

            if (m_OutAnimation.m_totalDuration == 0.0f)
            {
                if (OnOutAnimationsFinish != null)
                    OnOutAnimationsFinish.Invoke();

                return;
            }
            else
            {
                m_endTime_OutAnimation = Time.time + m_OutAnimation.m_totalDuration;
                m_doingOutAnimation = true;
            }

            if (gameObject.activeInHierarchy == false)
                return;

            if (m_OutAnimation.m_MoveAni.m_useAnimation)
                m_moveCoroutine = StartCoroutine(PlayMove(m_OutAnimation.m_MoveAni.m_StartDelay,
                    m_OutAnimation.m_MoveAni.m_Duration,
                    m_startPos,
                    m_OutAnimation.m_MoveAni.GetTargetPosition(m_rectTransform, m_startPos),
                    m_OutAnimation.m_MoveAni.m_Linear == true ? null : m_OutAnimation.m_MoveAni.m_AnimationCurve));

            if (m_OutAnimation.m_RotateAni.m_useAnimation)
                m_rotateCoroutine = StartCoroutine(PlayRotate(m_OutAnimation.m_RotateAni.m_StartDelay,
                    m_OutAnimation.m_RotateAni.m_Duration,
                    m_startRotate,
                    m_OutAnimation.m_RotateAni.m_Rotate,
                    m_OutAnimation.m_RotateAni.m_Linear == true ? null : m_OutAnimation.m_RotateAni.m_AnimationCurve));

            if (m_OutAnimation.m_ScaleAni.m_useAnimation)
                m_scaleCoroutine = StartCoroutine(PlayScale(m_OutAnimation.m_ScaleAni.m_StartDelay,
                    m_OutAnimation.m_ScaleAni.m_Duration,
                    m_startScale,
                    m_OutAnimation.m_ScaleAni.m_Scale,
                    m_OutAnimation.m_ScaleAni.m_Linear == true ? null : m_OutAnimation.m_ScaleAni.m_AnimationCurve));

            if (m_OutAnimation.m_FadeAni.m_useAnimation)
                m_fadeCoroutine = StartCoroutine(PlayFade(m_OutAnimation.m_FadeAni.m_StartDelay,
                    m_OutAnimation.m_FadeAni.m_Duration,
                    m_OutAnimation.m_FadeAni.m_StartAlpha,
                    m_OutAnimation.m_FadeAni.m_EndAlpha,
                    m_OutAnimation.m_FadeAni.m_Linear == true ? null : m_OutAnimation.m_FadeAni.m_AnimationCurve));
        }
        //------------------------------------------------------------------------------------
        IEnumerator PlayMove(float delay, float duration, Vector3 startpos, Vector3 endpos, AnimationCurve animationcurve)
        {
            float starttime = Time.time;
            float endtime = starttime + delay;

            while (Time.time <= endtime)
                yield return null;

            starttime = Time.time;
            endtime = starttime + duration;

            Vector3 posGap = startpos - endpos;

            float ratio = 0.0f;
            while (Time.time <= endtime)
            {
                ratio = (Time.time - starttime) / duration;
                if (animationcurve != null)
                    ratio = animationcurve.Evaluate(ratio);

                m_rectTransform.anchoredPosition3D = startpos - (posGap * ratio);

                yield return null;
            }

            m_rectTransform.anchoredPosition3D = endpos;
            m_moveCoroutine = null;
        }
        //------------------------------------------------------------------------------------
        IEnumerator PlayRotate(float delay, float duration, Vector3 startrotate, Vector3 endrotate, AnimationCurve animationcurve)
        {
            float starttime = Time.time;
            float endtime = starttime + delay;

            while (Time.time <= endtime)
                yield return null;

            starttime = Time.time;
            endtime = starttime + duration;

            Vector3 rotateGap = startrotate - endrotate;

            float ratio = 0.0f;
            while (Time.time <= endtime)
            {
                ratio = (Time.time - starttime) / duration;
                if (animationcurve != null)
                    ratio = animationcurve.Evaluate(ratio);

                m_rectTransform.eulerAngles = startrotate - (rotateGap * ratio);

                yield return null;
            }

            m_rectTransform.eulerAngles = endrotate;
            m_rotateCoroutine = null;
        }
        //------------------------------------------------------------------------------------
        IEnumerator PlayScale(float delay, float duration, Vector3 startscale, Vector3 endscale, AnimationCurve animationcurve)
        {
            float starttime = Time.time;
            float endtime = starttime + delay;

            while (Time.time <= endtime)
                yield return null;

            starttime = Time.time;
            endtime = starttime + duration;

            Vector3 scaleGap = startscale - endscale;

            float ratio = 0.0f;
            while (Time.time <= endtime)
            {
                ratio = (Time.time - starttime) / duration;
                if (animationcurve != null)
                    ratio = animationcurve.Evaluate(ratio);

                m_rectTransform.localScale = startscale - (scaleGap * ratio);

                yield return null;
            }

            m_rectTransform.localScale = endscale;
            m_scaleCoroutine = null;
        }
        //------------------------------------------------------------------------------------
        IEnumerator PlayFade(float delay, float duration, float startfade, float endfade, AnimationCurve animationcurve)
        {
            m_canvasGroup.alpha = startfade;

            float starttime = Time.time;
            float endtime = starttime + delay;

            while (Time.time <= endtime)
                yield return null;

            starttime = Time.time;
            endtime = starttime + duration;

            float fadeGap = endfade - startfade;

            float ratio = 0.0f;
            while (Time.time <= endtime)
            {
                ratio = (Time.time - starttime) / duration;
                if (animationcurve != null)
                    ratio = animationcurve.Evaluate(ratio);

                m_canvasGroup.alpha= startfade + (fadeGap * ratio);

                yield return null;
            }

            m_canvasGroup.alpha = endfade;
            m_fadeCoroutine = null;
        }
        //------------------------------------------------------------------------------------
    }
}