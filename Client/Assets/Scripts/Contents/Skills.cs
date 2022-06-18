using Data;
using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    public class Skills
    {
        public SkillInfo Info { get; } = new SkillInfo();

        int skillDbId;
        int slot;
        int mp;

        public int SkillId
        {
            get { return Info.SkillId; }
            set { Info.SkillId = value; }

        }


        public int SkillDbId
        {
            get { return skillDbId; }
            set { skillDbId = value; }
        }

        public int SkillLevel
        {
            get { return Info.SkillLevel; }
            set { Info.SkillLevel = value; }
        }



        public int Slot
        {
            get { return slot; }
            set { slot = value; }
        }

        public int Mp
        {
            get { return mp; }
            set { mp = value; }
        }

        public static Skills MakeSkill(SkillInfo skillInfo)
        {
            // 스킬을 생성하고,
            Skills skill = new Skills()
            {
                SkillDbId = skillInfo.SkillDbId,
                SkillId = skillInfo.SkillId,
                Slot = skillInfo.Slot,
                SkillLevel = skillInfo.SkillLevel,
            };

            // 마나소비 정보는 그냥 Data에서 갖고온다.

            Skill skillData = null;
            Managers.Data.SkillDict.TryGetValue(skill.SkillId, out skillData);

            skill.mp = skillData.mp;



            return skill;
        }
}
