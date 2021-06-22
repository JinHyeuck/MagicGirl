using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBerry
{
    public class Dunjeon_Story : DunjeonBase
    {
        //------------------------------------------------------------------------------------
        public override void InitDunjeonLogic()
        {
        }
        //------------------------------------------------------------------------------------
        public override void CreateMonster(string monsterid, Vector3 startpos)
        {
            Vector3 spawnPos = startpos;

            Managers.MonsterManager.Instance.SpawnMonster(monsterid, spawnPos);
        }
        //------------------------------------------------------------------------------------
        public override void CheckClearDunjeon()
        {
            Managers.DunjeonManager.Instance.DunjeonClear();
        }
        //------------------------------------------------------------------------------------
    }
}