using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBerry.Scene
{
    public class InGameScene : IScene
    {
        protected override void OnLoadComplete()
        {// 여기서 각 매니저들을 초기화하고 다 끝났으면 여기서 처음으로 스타트 해준다.

            Managers.DunjeonManager.Instance.SetDunjeon();
            Managers.DunjeonManager.Instance.PlayDunjeon();
        }
    }
}