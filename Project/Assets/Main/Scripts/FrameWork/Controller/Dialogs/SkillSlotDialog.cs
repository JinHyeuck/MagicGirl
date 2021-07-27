using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GameBerry.UI
{
    public class SkillSlotDialog : IDialog
    {
        [SerializeField]
        private Button m_autoSkillBtn;

        [SerializeField]
        private RectTransform m_autoSkillInteractionImage;

        [Header("------------------------------------")]
        [SerializeField]
        private Transform m_slotRoot;

        [SerializeField]
        private UISkillSlotElement m_uISkillSlotElement;

        [SerializeField]
        UnityEngine.U2D.SpriteAtlas m_skillSlotAtlas = null;

        private Dictionary<int, UISkillSlotElement> m_uICreatedSkillSlot_Dic = new Dictionary<int, UISkillSlotElement>();

        private bool m_playAutoSkillDirection = false;

        private float m_autoSkillDirectionStartTime = 0.0f;

        [SerializeField]
        private float m_autoSkillColorDirectionSpeed = 1.0f;

        [SerializeField]
        private float m_autoSkillRotationDirectionSpeed = 1.0f;

        //------------------------------------------------------------------------------------
        protected override void OnLoad()
        {
            if (m_autoSkillBtn != null)
                m_autoSkillBtn.onClick.AddListener(OnClick_AutoTrigger);

            Message.AddListener<GameBerry.Event.InitializeSkillSlotMsg>(InitializeSkillSlot);
            Message.AddListener<GameBerry.Event.SetAutoSkillModeMsg>(SetAutoSkillMode);

            Message.AddListener<GameBerry.Event.ChangeSlotStateMsg>(ChangeSlotState);
        }
        //------------------------------------------------------------------------------------
        protected override void OnUnload()
        {
            Message.RemoveListener<GameBerry.Event.InitializeSkillSlotMsg>(InitializeSkillSlot);
            Message.RemoveListener<GameBerry.Event.SetAutoSkillModeMsg>(SetAutoSkillMode);

            Message.RemoveListener<GameBerry.Event.ChangeSlotStateMsg>(ChangeSlotState);
        }
        //------------------------------------------------------------------------------------
        private void Update()
        {
            if (m_playAutoSkillDirection == true)
            {
                if (m_autoSkillBtn != null)
                {
                    Color dircolor = m_autoSkillBtn.image.color;
                    float colorvalur = MathDatas.Abs(MathDatas.Sin((Time.time - m_autoSkillDirectionStartTime) * m_autoSkillColorDirectionSpeed));
                    dircolor.r = colorvalur;
                    dircolor.g = colorvalur;
                    dircolor.b = colorvalur;

                    m_autoSkillBtn.image.color = dircolor;
                }

                if (m_autoSkillInteractionImage != null)
                {
                    Vector3 rota = m_autoSkillInteractionImage.eulerAngles;

                    rota.z += Time.deltaTime * m_autoSkillRotationDirectionSpeed;
                    m_autoSkillInteractionImage.eulerAngles = rota;
                }
            }
        }
        //------------------------------------------------------------------------------------
        private void InitializeSkillSlot(GameBerry.Event.InitializeSkillSlotMsg msg)
        {
            foreach (KeyValuePair<int, Managers.SkillSlotData> pair in msg.SkillSlot)
            {
                UISkillSlotElement element = CreateSlot();
                pair.Value.SlotElements.Add(element);
                element.SetSlotID(pair.Value.SlotID);
                element.SetState(pair.Value.CurrSlotState);
                element.SetSlotBG(m_skillSlotAtlas.GetSprite(pair.Value.CurrSlotState.ToString()));

                m_uICreatedSkillSlot_Dic.Add(pair.Value.SlotID, element);
            }
        }
        //------------------------------------------------------------------------------------
        private UISkillSlotElement CreateSlot()
        {
            GameObject clone = Instantiate(m_uISkillSlotElement.gameObject, m_slotRoot.transform);
            UISkillSlotElement slot = clone.GetComponent<UISkillSlotElement>();
            slot.Init(OnClick_SlotBtn);

            return slot;
        }
        //------------------------------------------------------------------------------------
        private void OnClick_SlotBtn(int slotid)
        {
            Managers.SkillSlotManager.Instance.OnClick_SkillSlot(slotid);
        }
        //------------------------------------------------------------------------------------
        private void OnClick_AutoTrigger()
        {
            Managers.SkillSlotManager.Instance.OnClick_AutoTrigger();
        }
        //------------------------------------------------------------------------------------
        private void SetAutoSkillMode(GameBerry.Event.SetAutoSkillModeMsg msg)
        {
            m_playAutoSkillDirection = msg.AutoSkillMode;
            if (m_playAutoSkillDirection == false)
            {
                if (m_autoSkillBtn != null)
                {
                    Color color = m_autoSkillBtn.image.color;
                    color.r = 0.0f;
                    color.g = 0.0f;
                    color.b = 0.0f;
                    m_autoSkillBtn.image.color = color;
                }
                

                if (m_autoSkillInteractionImage != null)
                {
                    m_autoSkillInteractionImage.eulerAngles = Vector3.zero;
                }
            }
            else
            {
                m_autoSkillDirectionStartTime = Time.time;
            }
        }
        //------------------------------------------------------------------------------------
        private void ChangeSlotState(GameBerry.Event.ChangeSlotStateMsg msg)
        {
            if (msg.SkillSlotData == null)
                return;

            for (int i = 0; i < msg.SkillSlotData.Count; ++i)
            {
                Managers.SkillSlotData slotdata = msg.SkillSlotData[i];

                if (slotdata == null)
                    continue;

                if (m_uICreatedSkillSlot_Dic.ContainsKey(slotdata.SlotID) == false)
                    continue;

                UISkillSlotElement element = null;

                if (m_uICreatedSkillSlot_Dic.TryGetValue(slotdata.SlotID, out element) == false)
                    continue;

                element.SetState(slotdata.CurrSlotState);
                element.SetSlotBG(m_skillSlotAtlas.GetSprite(slotdata.CurrSlotState.ToString()));
            }
        }
        //------------------------------------------------------------------------------------
    }
}