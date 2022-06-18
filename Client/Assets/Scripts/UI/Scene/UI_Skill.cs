using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UI_Skill : UI_Base
{
    public List<UI_Skill_Item> Skills { get; } = new List<UI_Skill_Item>();


    public override void Init()
    {
        Skills.Clear();

        GameObject grid = transform.Find("SkillGrid").gameObject;
        foreach (Transform child in grid.transform)
            Destroy(child.gameObject);

        for(int i=0; i< 20; i++)
        {
            GameObject go = Managers.Resource.Instantiate("UI/Scene/UI_Skill_Item", grid.transform);
            UI_Skill_Item skill = go.GetOrAddComponent<UI_Skill_Item>();
            Skills.Add(skill);


        }

        RefreshUI();
    }

    public void RefreshUI()
    {

        if (Skills.Count == 0)
            return;


        // 처음에 초기화
        foreach(UI_Skill_Item t in Skills)
        {
            t.SetSkill(null);
        }
      

        List<Skills> skills = Managers.Skill.Skills.Values.ToList();

        // 슬롯에 따라 정렬
        skills.Sort((left, right) => { return left.Slot - right.Slot; });

        foreach(Skills skill in skills)
        {
            if (skill.Slot < 0 || skill.Slot >= 20)
                continue; // 예외처리
            Skills[skill.Slot].SetSkill(skill);

        }

    }
}
