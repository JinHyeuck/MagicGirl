using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBerry.Contents
{
    public class DunjeonContent : IContent
    {
        protected override void OnLoadComplete()
        {
            Managers.DunjeonManager manager = Managers.DunjeonManager.Instance;
        }
    }
}