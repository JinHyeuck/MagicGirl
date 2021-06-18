using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppInitializeUI : MonoBehaviour
{
    [Header("----------Notice----------")]
    [SerializeField]
    private Transform m_noticeGroup;

    [SerializeField]
    private Text m_noticeText;

    [Header("----------LoginGroup----------")]
    [SerializeField]
    private Transform m_loginGroup;

    [Header("----------LoginButtonGroup----------")]
    [SerializeField]
    private Transform m_loginBtnGroup;

    [SerializeField]
    private Button m_facebookLogin_Btn;

    [SerializeField]
    private Button m_googleLogin_Btn;

    [SerializeField]
    private Button m_guestLogin_Btn;

    [SerializeField]
    private Button m_customLogin_Btn;

    [Header("----------CustomLoginGroup----------")]
    [SerializeField]
    private Transform m_customLoginGroup;

    [SerializeField]
    private InputField m_customLoginID;

    [SerializeField]
    private InputField m_customLoginPW;

    [SerializeField]
    private Button m_customLoginAuth_Btn;

    [SerializeField]
    private Toggle m_customSignUpToggle;

    [Header("----------NickNameGroup----------")]
    [SerializeField]
    private Transform m_createNickNameGroup;

    [SerializeField]
    private InputField m_nickNameInputField;

    [SerializeField]
    private Button m_nickNameAuth_Btn;

    [Header("----------PatchGroup----------")]
    [SerializeField]
    private Transform m_patchGroup;

    [SerializeField]
    private Image m_patchProgressBar;

    [SerializeField]
    private Text m_patchProgressText;


    //------------------------------------------------------------------------------------
    System.Action<GameBerry.TheBackEnd.LoginType, BackEnd.FederationType> m_loginTypeCallBack = null;
    System.Action<string, string, bool> m_customLoginCallBack = null;
    System.Action<string> m_nickNameCallBack = null;

    //------------------------------------------------------------------------------------
    public void Init()
    {
        if (m_noticeGroup != null)
            m_noticeGroup.gameObject.SetActive(false);

        if (m_loginGroup != null)
            m_loginGroup.gameObject.SetActive(false);

        if (m_loginBtnGroup != null)
            m_loginBtnGroup.gameObject.SetActive(false);

        if (m_customLoginGroup != null)
            m_customLoginGroup.gameObject.SetActive(false);

        if (m_createNickNameGroup != null)
            m_createNickNameGroup.gameObject.SetActive(false);

        if (m_patchGroup != null)
            m_patchGroup.gameObject.SetActive(false);


        if (m_facebookLogin_Btn != null)
            m_facebookLogin_Btn.onClick.AddListener(OnClick_FaceBookBtn);

        if (m_googleLogin_Btn != null)
            m_googleLogin_Btn.onClick.AddListener(OnClick_GoogleBtn);

        if (m_guestLogin_Btn != null)
            m_guestLogin_Btn.onClick.AddListener(OnClick_GuestBtn);

        if (m_customLogin_Btn != null)
            m_customLogin_Btn.onClick.AddListener(OnClick_CustomBtn);

        if (m_customLoginAuth_Btn != null)
            m_customLoginAuth_Btn.onClick.AddListener(OnClick_CustomLoginAuthBtn);

        if (m_nickNameAuth_Btn != null)
            m_nickNameAuth_Btn.onClick.AddListener(OnClick_NickNameAuthBtn);

        if (m_customSignUpToggle != null)
            m_customSignUpToggle.isOn = false;

        Message.AddListener<GameBerry.Event.SetNoticeMsg>(SetNotice);
    }
    //------------------------------------------------------------------------------------
    public void Release()
    {
        Message.RemoveListener<GameBerry.Event.SetNoticeMsg>(SetNotice);
    }
    //------------------------------------------------------------------------------------
    public void VisibleNoticeGroup(bool visible)
    {
        if (m_noticeGroup != null)
            m_noticeGroup.gameObject.SetActive(visible);
    }
    //------------------------------------------------------------------------------------
    private void SetNotice(GameBerry.Event.SetNoticeMsg msg)
    {
        SetNoticeText(msg.NoticeStr);
    }
    //------------------------------------------------------------------------------------
    public void SetNoticeText(string noticetext)
    {
        if (m_noticeText != null)
            m_noticeText.text = noticetext;
    }
    //------------------------------------------------------------------------------------
    public void VisibleLoginProcess(bool visible)
    {
        if (m_loginGroup != null)
            m_loginGroup.gameObject.SetActive(visible);
    }
    //------------------------------------------------------------------------------------
    public void VisibleLoginButtonGroup(bool visible)
    {
        if (m_loginBtnGroup != null)
            m_loginBtnGroup.gameObject.SetActive(visible);
    }
    //------------------------------------------------------------------------------------
    public void SetLoginCallBack(System.Action<GameBerry.TheBackEnd.LoginType, BackEnd.FederationType> callback)
    {
        m_loginTypeCallBack = callback;
    }
    //------------------------------------------------------------------------------------
    private void OnClick_FaceBookBtn()
    {
        if (m_loginTypeCallBack != null)
            m_loginTypeCallBack(GameBerry.TheBackEnd.LoginType.Social, BackEnd.FederationType.Facebook);
    }
    //------------------------------------------------------------------------------------
    private void OnClick_GoogleBtn()
    {
        if (m_loginTypeCallBack != null)
            m_loginTypeCallBack(GameBerry.TheBackEnd.LoginType.Social, BackEnd.FederationType.Google);
    }
    //------------------------------------------------------------------------------------
    private void OnClick_GuestBtn()
    {
        if (m_loginTypeCallBack != null)
            m_loginTypeCallBack(GameBerry.TheBackEnd.LoginType.Guest, BackEnd.FederationType.Google);
    }
    //------------------------------------------------------------------------------------
    private void OnClick_CustomBtn()
    {
        if (m_loginTypeCallBack != null)
            m_loginTypeCallBack(GameBerry.TheBackEnd.LoginType.CustomLogin, BackEnd.FederationType.Google);

        VisibleLoginButtonGroup(false);

        VisibleCustomLogin(true);
    }
    //------------------------------------------------------------------------------------
    public void SetCustomLoginCallBack(System.Action<string, string, bool> callback)
    {
        m_customLoginCallBack = callback;
    }
    //------------------------------------------------------------------------------------
    public void VisibleCustomLogin(bool visible)
    {
        if (m_customLoginGroup != null)
            m_customLoginGroup.gameObject.SetActive(visible);
    }
    //------------------------------------------------------------------------------------
    private void OnClick_CustomLoginAuthBtn()
    {
        if (m_customLoginCallBack != null)
            m_customLoginCallBack(m_customLoginID.text, m_customLoginPW.text, m_customSignUpToggle.isOn);
    }
    //------------------------------------------------------------------------------------
    public void VisibleCreateNickName(bool visible)
    {
        if (m_createNickNameGroup != null)
            m_createNickNameGroup.gameObject.SetActive(visible);
    }
    //------------------------------------------------------------------------------------
    public void SetNickNameCallBack(System.Action<string> callback)
    {
        m_nickNameCallBack = callback;
    }
    //------------------------------------------------------------------------------------
    private void OnClick_NickNameAuthBtn()
    {
        if (m_nickNameCallBack != null)
            m_nickNameCallBack(m_nickNameInputField.text);
    }
    //------------------------------------------------------------------------------------
    public void VisiblePatchzProcess()
    {

    }
    //------------------------------------------------------------------------------------
}
