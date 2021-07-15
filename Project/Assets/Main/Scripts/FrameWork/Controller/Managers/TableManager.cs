//#define LOAD_FROM_ASSETBUNDLE

using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace GameBerry.Managers
{
    public class TableManager : MonoSingleton<TableManager>
    {
        bool _alreadyLoading = false;
        bool _loadComplete = false;
        string basePath = "Tables/";

        readonly Dictionary<System.Type, object> _tables = new Dictionary<System.Type, object>();


        public T GetTableClass<T>() where T : class
        {
            object table;
            if (_tables.TryGetValue(typeof(T), out table))
                return (T)table;

            Debug.LogErrorFormat("{0} is null", typeof(T).Name);
            return null;
        }

        public IEnumerator Load()
        {
            _tables.Clear();

            if (_loadComplete)
                yield break;

            if (_alreadyLoading)
            {
                while (!_loadComplete)
                    yield return null;

                yield break;
            }

            

            SoundTableAsset soundtable = null;
            string bundleName = "SoundTable";

            AssetBundleLoader.Instance.Load<SoundTableAsset>(string.Format("{0}{1}", basePath, bundleName), bundleName, o =>
            {
                soundtable = o as SoundTableAsset;
            });

            if (soundtable == null)
                Debug.LogError("SoundTable Load Error");
            else
                _tables.Add(Type.GetType("GameBerry.SoundTableAsset"), soundtable);


            MonsterLocalChart monsterLocalTable = new MonsterLocalChart();
            monsterLocalTable.InitData();
            _tables.Add(Type.GetType("GameBerry.MonsterLocalChart"), monsterLocalTable);


            DunjeonLocalChart dunjeonLocalTable = new DunjeonLocalChart();
            dunjeonLocalTable.InitData();
            _tables.Add(Type.GetType("GameBerry.DunjeonLocalChart"), dunjeonLocalTable);


            LevelLocalChart levelLocalChart = new LevelLocalChart();
            levelLocalChart.InitData();
            _tables.Add(Type.GetType("GameBerry.LevelLocalChart"), levelLocalChart);


            StatUpGradeLocalChart statUpGradeLocalChart = new StatUpGradeLocalChart();
            statUpGradeLocalChart.InitData();
            _tables.Add(Type.GetType("GameBerry.StatUpGradeLocalChart"), statUpGradeLocalChart);


            EquipmentLocalChart equipmentLocalChart = new EquipmentLocalChart();
            equipmentLocalChart.InitData();
            _tables.Add(Type.GetType("GameBerry.EquipmentLocalChart"), equipmentLocalChart);


            SkillLocalChart skillLocalTable = new SkillLocalChart();
            skillLocalTable.InitData();
            _tables.Add(Type.GetType("GameBerry.SkillLocalChart"), skillLocalTable);


            GachaLocalChart gachaLocalChart = new GachaLocalChart();
            gachaLocalChart.InitData();
            _tables.Add(Type.GetType("GameBerry.GachaLocalChart"), gachaLocalChart);


            _alreadyLoading = false;
            _loadComplete = true;
        }

        public void Clear()
        {
            _tables.Clear();
            Debug.Log("Clear Tables. - " + GetInstanceID());
        }

        protected override void Release()
        {
            Clear();
        }
    }
}
