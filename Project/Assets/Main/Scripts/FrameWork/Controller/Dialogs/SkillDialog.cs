using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameBerry.UI
{
    [System.Serializable]
    public class SkillPageTabBtn
    {
        public Button ChangeTabBtn;
        public EquipmentType ButtonType;

        private System.Action<EquipmentType> CallBack;

        public void SetCallBack(System.Action<EquipmentType> callback)
        {
            CallBack = callback;
        }

        public void OnClick()
        {
            if (CallBack != null)
                CallBack(ButtonType);
        }
    }

    public class SkillDialog : IDialog
    {
        [Header("---------SkillContent---------")]
        [SerializeField]
        private RectTransform m_skillRoot;

        [Header("---------------------------")]
        [SerializeField]
        private UISkillElement m_skillElement;

        private List<UISkillElement> m_skillElement_List = new List<UISkillElement>();
        private Dictionary<int, UISkillElement> m_skillElement_Dic = new Dictionary<int, UISkillElement>();

        private SkillLocalChart m_skillLocalChart = null;

        //------------------------------------------------------------------------------------
        protected override void OnLoad()
        {
            m_skillLocalChart = Managers.TableManager.Instance.GetTableClass<SkillLocalChart>();

        }
        //------------------------------------------------------------------------------------
    }
}