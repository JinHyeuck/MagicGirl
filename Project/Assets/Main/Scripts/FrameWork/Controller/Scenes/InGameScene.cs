using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBerry.Scene
{
    public class InGameScene : IScene
    {
        protected override void OnLoadComplete()
        {// ���⼭ �� �Ŵ������� �ʱ�ȭ�ϰ� �� �������� ���⼭ ó������ ��ŸƮ ���ش�.

            Managers.DunjeonManager.Instance.SetDunjeon();
            Managers.DunjeonManager.Instance.PlayDunjeon();
        }
    }
}