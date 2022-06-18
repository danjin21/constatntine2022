using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class SkillManager
{

    public Dictionary<int, Skills> Skills { get; } = new Dictionary<int, Skills>();

    public void Add(Skills skill)
    {
        int t = 0;

        // 원래 스킬 중에 해당 스킬이 있으면 생략한다.
        foreach (Skills a in Skills.Values)
        {
            if (a.SkillId == skill.SkillId)
            {
                t = 1;
            }
        }

        if (t == 0)
            Skills.Add(skill.SkillDbId, skill);
        else
            return;
    }

    public Skills Get(int skillDbId)
    {
        Skills skill = null;
        Skills.TryGetValue(skillDbId, out skill);
        return skill;
    }

    public Skills Find(Func<Skills, bool> condition)
    {
        foreach (Skills skill in Skills.Values)
        {
            if (condition.Invoke(skill))
                return skill;
        }
        return null;
    }

    // 스킬을 넣는다.
    public void Delete(Skills skill)
    {

        foreach (Skills t in Skills.Values)
        {
            Console.WriteLine($"Pastitem : {t.Slot}");
        }

        Skills.Remove(skill.SkillDbId);

        foreach (Skills t in Skills.Values)
        {
            if (t.Slot > skill.Slot)
                t.Slot -= 1;
        }


        foreach (Skills t in Skills.Values)
        {
            Console.WriteLine($"Nowitem : {t.Slot}");
        }

    }


    // 비어있으면 슬롯 지정해주고, 꽉 차있으면 null을 리턴
    public int? GetEmptySlot()
    {
        for (int slot = 0; slot < 20; slot++)
        {
            Skills skill = Skills.Values.FirstOrDefault(i => i.Slot == slot);
            if (skill == null)
                return slot;
        }

        return null;
    }


    // 스킬의 이름을 보고 슬로에 있는지 확인해줌
    public int? GetSlotFromTemplateId(int skillId)
    {
        for (int slot = 0; slot < 20; slot++)
        {
            Skills skill = Skills.Values.FirstOrDefault(i => i.Slot == slot);
            if (skill.SkillId == skillId)
                return slot;
        }

        return null;
    }

    public void Clear()
    {
        Skills.Clear();
    }




}
