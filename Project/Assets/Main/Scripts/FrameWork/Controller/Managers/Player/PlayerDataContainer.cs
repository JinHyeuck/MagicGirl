using System.Collections.Generic;
namespace GameBerry
{


    public static class PlayerDataContainer
    {


        public static int Level;
        public static double Exp;

        public static double Gold;
        public static int Dia;

        public static int EquipmentSton;
        public static int SkillSton;



        // ½ÇÁ¦ ¾Æ¿ôÇ² ½ºÅÝ
        // µ¥¹ÌÁö ¾Æ¿ôÇ² ¶§ ÇÊ¿äÇÑ Ä£±¸µé
        public static double Damage; // default + statupgrade + weaponstat
        public static double DamagePer; // default + weaponstat

        public static double DamageOperation; // damage * damageper

        public static double CriticalDamage;
        public static double CriticalDamagePer;

        public static double EndDamage;
        public static double SkillDamage;

        public static double DamageBuffValue;
        // µ¥¹ÌÁö ¾Æ¿ôÇ² ¶§ ÇÊ¿äÇÑ Ä£±¸µé

        public static double HPMax;
        public static double HPRecovery;

        public static double MPMax;
        public static double MPRecovery;

        public static double MoveSpeed;
        public static double AttackSpeed;

        public static double AddGoldPer;
        public static double AddExpPer;

        public static float CooltimeRatio;
        // ½ÇÁ¦ ¾Æ¿ôÇ² ½ºÅÝ


    }

    public static class PlayerDataOperator
    {
        
        public static double GetOperationDamage()
        { // Damage * DamagePer


            return 0.0;
        }
        //------------------------------------------------------------------------------------
    }
}