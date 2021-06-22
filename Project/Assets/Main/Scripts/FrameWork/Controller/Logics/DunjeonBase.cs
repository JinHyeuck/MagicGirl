using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBerry
{
    public abstract class DunjeonBase
    {
        //------------------------------------------------------------------------------------
        public abstract void InitDunjeonLogic();
        //------------------------------------------------------------------------------------
        public abstract void CreateMonster(string monsterid, Vector3 startpos);
        //------------------------------------------------------------------------------------
        public abstract void CheckClearDunjeon();
        //------------------------------------------------------------------------------------
    }
}