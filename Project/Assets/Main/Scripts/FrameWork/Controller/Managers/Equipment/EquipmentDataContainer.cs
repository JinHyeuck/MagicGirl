using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameBerry
{
    public class PlayerEquipmentInfo
    {
        public int Id;
        public int Count = 0;
        public int Level = 0;
    }

    public static class EquipmentDataContainer
    {
        // 플레이어가 가지고있는 장비
        public static Dictionary<EquipmentType, Dictionary<int, PlayerEquipmentInfo>> m_equipmentInfo = new Dictionary<EquipmentType, Dictionary<int, PlayerEquipmentInfo>>();

        // 플레이어가 장착한 장비
        public static Dictionary<EquipmentType, int> m_equipId = new Dictionary<EquipmentType, int>();

    }

    public static class EquipmentDataOperator
    {
        //------------------------------------------------------------------------------------
        public static int GetOperationQualityValue(QualityType qualityType)
        {
            int orivalue = (int)qualityType;

            if (orivalue > 0)
            {
                return orivalue + ((orivalue - 1) * Define.EquipmentQualityOperationValue);
            }

            return orivalue;
        }
        //------------------------------------------------------------------------------------
        public static int GetNeedLevelUPEquipmentSton(EquipmentData equipmentdata, PlayerEquipmentInfo equipmentinfo)
        {
            int defaultcount = GetOperationQualityValue(equipmentdata.Quality) + ((int)equipmentdata.Grade * 20);

            if (equipmentinfo == null)
                return defaultcount;

            return defaultcount + (defaultcount * (int)((float)equipmentinfo.Level * 0.5f));
        }
        //------------------------------------------------------------------------------------
        public static double GetEquipmentOptionValue(EquipmentData equipmentdata, int equiplevel, EquipmentOption option)
        {
            if (equipmentdata == null)
                return 0.0;

            double basevalue = 0.0;

            equipmentdata.Option.TryGetValue(option, out basevalue);

            return basevalue + (equiplevel * basevalue);
        }
        //------------------------------------------------------------------------------------
    }
}
