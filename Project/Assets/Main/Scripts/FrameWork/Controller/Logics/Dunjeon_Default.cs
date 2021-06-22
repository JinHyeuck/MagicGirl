using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBerry
{
    public class Dunjeon_Default : DunjeonBase
    {
        private int m_spawnMaxCount = 8;
        private float m_monsterPosGab = 1.0f;
        
        private int m_maxMonsterWave = 3;
        private int m_currentWave = 0;

        //------------------------------------------------------------------------------------
        public override void InitDunjeonLogic()
        {
            m_currentWave = 0;
        }
        //------------------------------------------------------------------------------------
        public override void CreateMonster(string monsterid, Vector3 startpos)
        {
            m_currentWave++;

            for (int i = 0; i < m_spawnMaxCount; ++i)
            {
                Vector3 spawnPos = startpos;
                spawnPos.x += i * m_monsterPosGab;

                Managers.MonsterManager.Instance.SpawnMonster(monsterid, spawnPos);
            }
        }
        //------------------------------------------------------------------------------------
        public override void CheckClearDunjeon()
        {
            if(m_currentWave >= m_maxMonsterWave)
                Managers.DunjeonManager.Instance.DunjeonClear();
            else
                Managers.DunjeonManager.Instance.CreateMonster();
        }
        //------------------------------------------------------------------------------------

    }
}