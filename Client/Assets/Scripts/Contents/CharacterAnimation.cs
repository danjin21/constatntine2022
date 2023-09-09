using Google.Protobuf.Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimation : MonoBehaviour
{


    public int Hair;
    public int Face;
    public int Skin;
    public int HairColor;

    public int Helmet = -1;
    public int RightHand = -1;
    public int LeftHand = -1;
    public int Shirts = -1;
    public int Pants = -1;
    public int Shoes = -1;


    public int weaponKind = -1; // 0 : 한손검, 1: 활, 2: 두손검, 3: 창, 4: 아대, 5 : 지팡이
    public int attackForm = -1; // 0 : 베기, 1: 찌르기

    // ============================================== 외형 라인 =================================================//

    public List<Sprite> HeadList = new List<Sprite>();
    public List<Sprite> FaceList = new List<Sprite>();
    public List<Sprite> HairList = new List<Sprite>();
    public List<Sprite> HelmetList = new List<Sprite>();

    // ============================================== 쌩몸 라인 =================================================//

    public List<Sprite> BodyList = new List<Sprite>(); // 몸 걸을때 쌩몸
    public List<Sprite> BodyList_OneHand = new List<Sprite>(); // 무기 들고 걸을 때 쌩몸 (선 없이)
    public List<Sprite> BodyList_TwoHand = new List<Sprite>(); // 두손검 들고 걸을 때 쌩몸  (선 없이)

    public List<Sprite> Body_Attack_List = new List<Sprite>(); // 한손검 공격할 때의 쌩몸
    public List<Sprite> Body_Attack_List_TwoHand_1 = new List<Sprite>(); // (베기)두손검 공격할 때의 쌩몸
    public List<Sprite> Body_Attack_List_TwoHand_2 = new List<Sprite>(); // (찌르기)두손검 공격할 때의 쌩몸

    public List<Sprite> Body_Spell_List = new List<Sprite>(); // 스펠 할 때의 쌩몸
    public List<Sprite> Body_Drop_List = new List<Sprite>(); // 줍기 할 때의 쌩몸

    public List<Sprite> Body_Bow_Attack_List = new List<Sprite>(); // 활 들고 공격할 때의 쌩몸


    // ============================================== 갑옷 라인 =================================================//

    public List<Sprite> ArmorList = new List<Sprite>();
    public List<Sprite> ArmorList_OneHand = new List<Sprite>();
    public List<Sprite> ArmorList_TwoHand = new List<Sprite>();

    public List<Sprite> Armor_Attack_List = new List<Sprite>(); // 한손검 공격할 때의 갑옷
    public List<Sprite> Armor_Attack_List_TwoHand_1 = new List<Sprite>(); // (베기)두손검 공격할 때의 갑옷
    public List<Sprite> Armor_Attack_List_TwoHand_2 = new List<Sprite>(); // (찌르기)두손검 공격할 때의 갑옷

    public List<Sprite> Armor_Spell_List = new List<Sprite>();
    public List<Sprite> Armor_Drop_List = new List<Sprite>();
    public List<Sprite> Armor_Bow_Attack_List = new List<Sprite>();

    // ============================================== 무기 라인 =================================================//

    public List<Sprite> RightHandList = new List<Sprite>(); // 무기 들고 걸을 때의 무기
    public List<Sprite> RightHand_Attack_List = new List<Sprite>(); // 무기 들고 공격할 때의 무기
    public List<Sprite> RightHand_Sting_List = new List<Sprite>(); // 무기 들고 공격할 때의 무기
    // public List<Sprite> RightHand_Bow_Attack_List = new List<Sprite>(); // 활 들고 공격할 때의 무기

    //public List<Sprite> RightHandList_TwoHand = new List<Sprite>(); // 두손검 들고 걸을 때의 무기
    //public List<Sprite> RightHand_Attack_List_TwoHand = new List<Sprite>(); // 두손검 들고 공격할 때의 무기

    // ============================================== 외형 변경 =================================================//

    public void CharacterApearance_Refresh()
    {

        Hair = transform.GetComponent<BaseController>().Stat.Hair;
        Face = transform.GetComponent<BaseController>().Stat.Face;
        Skin = transform.GetComponent<BaseController>().Stat.Skin;
        HairColor = transform.GetComponent<BaseController>().Stat.HairColor;


        Helmet = transform.GetComponent<BaseController>().Stat.Helmet;
        RightHand =  Math.Max(transform.GetComponent<BaseController>().Stat.RightHand - 1,-1); // -1 보다 작으면 -1 이다.
        Debug.Log("오른손무기" + RightHand);


        Item weaponItem = null;
        weaponItem = Managers.Inven.Find(i => i.Equipped && i.ItemType == ItemType.Weapon);

        if(weaponItem == null)
        {
            // 아무것도 안낌
            weaponKind = -1;
        }
        else
        {
            switch (((Weapon)weaponItem).WeaponType)
            {
                case WeaponType.Sword:
                    weaponKind = 0;
                    break;

                case WeaponType.Bow:
                    weaponKind = 1;
                    break;

                case WeaponType.Twohanded:
                    weaponKind = 2;
                    break;

                case WeaponType.Spear:
                    weaponKind = 3;
                    break;
            }
        }





        LeftHand = transform.GetComponent<BaseController>().Stat.LeftHand;

        // 셔츠는 1000부터 시작하므로 0,1,2,3,4 가려면 아래처럼 해야함.
        if (transform.GetComponent<BaseController>().Stat.Shirts != -1)
            Shirts = transform.GetComponent<BaseController>().Stat.Shirts - 999;
        else
            Shirts = 0;

        // 헬멧은 1000부터 시작하므로 0,1,2,3,4 가려면 아래처럼 해야함.
        if (transform.GetComponent<BaseController>().Stat.Helmet != -1)
        {
            Helmet = transform.GetComponent<BaseController>().Stat.Helmet - 2000;
            transform.GetChild(7).transform.gameObject.SetActive(true);
        }
        else
            transform.GetChild(7).transform.gameObject.SetActive(false);





        Pants = transform.GetComponent<BaseController>().Stat.Pants;
        Shoes = transform.GetComponent<BaseController>().Stat.Shoes;

        HairColorChange();

    }

    // ============================================== 머리 색상 변경 =================================================//

    public void HairColorChange()
    {
        Color color;

        switch (HairColor)
        {

            case 0:
                ColorUtility.TryParseHtmlString("#C5C5C5", out color);
                transform.GetChild(4).GetComponent<SpriteRenderer>().color = color;
                break;
            case 1:
                ColorUtility.TryParseHtmlString("#FDEC9C", out color);
                transform.GetChild(4).GetComponent<SpriteRenderer>().color = color;
                break;

            default:
                break;

        }

    }

    public void ChangeMotion()
    {
        int result = UnityEngine.Random.Range(0, 2);

        attackForm = result;

        // 창이면 무조건 찌르기
        if(weaponKind == 3)
        {
            attackForm = 1;
        }
    }


    // Start is called before the first frame update
    void Start()
    {

        attackForm = 1;

        // DB에서 값 가져오기

        CharacterApearance_Refresh();

        // 걷기 모션 스프라이트 넣기 (쌩몸)

        BodyList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Down/Body/1") as Sprite); // 0
        BodyList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Down/Body/2") as Sprite); // 1
        BodyList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Down/Body/4") as Sprite); // 2
        BodyList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Left/Body/1") as Sprite); // 3
        BodyList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Left/Body/2") as Sprite); // 4
        BodyList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Left/Body/4") as Sprite); // 5
        BodyList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Right/Body/1") as Sprite); // 6
        BodyList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Right/Body/2") as Sprite); // 7
        BodyList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Right/Body/4") as Sprite); // 8
        BodyList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Up/Body/1") as Sprite); // 9
        BodyList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Up/Body/2") as Sprite); // 10
        BodyList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Up/Body/4") as Sprite); // 11


        // 걷기 모션_ 무기 들었을 때 스프라이트 넣기 (쌩몸)

        BodyList_OneHand.Add(Resources.Load<Sprite>("Textures/Character/OneHand/Base/Down/Body/1") as Sprite); //0 
        BodyList_OneHand.Add(Resources.Load<Sprite>("Textures/Character/OneHand/Base/Down/Body/2") as Sprite); // 1
        BodyList_OneHand.Add(Resources.Load<Sprite>("Textures/Character/OneHand/Base/Down/Body/4") as Sprite); // 2
        BodyList_OneHand.Add(Resources.Load<Sprite>("Textures/Character/OneHand/Base/Left/Body/1") as Sprite); // 3 
        BodyList_OneHand.Add(Resources.Load<Sprite>("Textures/Character/OneHand/Base/Left/Body/2") as Sprite); // 4
        BodyList_OneHand.Add(Resources.Load<Sprite>("Textures/Character/OneHand/Base/Left/Body/4") as Sprite); // 5
        BodyList_OneHand.Add(Resources.Load<Sprite>("Textures/Character/OneHand/Base/Right/Body/1") as Sprite); // 6
        BodyList_OneHand.Add(Resources.Load<Sprite>("Textures/Character/OneHand/Base/Right/Body/2") as Sprite); // 7
        BodyList_OneHand.Add(Resources.Load<Sprite>("Textures/Character/OneHand/Base/Right/Body/4") as Sprite); // 8
        BodyList_OneHand.Add(Resources.Load<Sprite>("Textures/Character/OneHand/Base/Up/Body/1") as Sprite); // 9
        BodyList_OneHand.Add(Resources.Load<Sprite>("Textures/Character/OneHand/Base/Up/Body/2") as Sprite); // 10
        BodyList_OneHand.Add(Resources.Load<Sprite>("Textures/Character/OneHand/Base/Up/Body/4") as Sprite); // 11


        // 걷기 모션_ 두손검 들었을 때 스프라이트 넣기 (쌩몸)

       BodyList_TwoHand.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Base/Down/Body/1") as Sprite); //0 
       BodyList_TwoHand.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Base/Down/Body/2") as Sprite); // 1
       BodyList_TwoHand.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Base/Down/Body/4") as Sprite); // 2
       BodyList_TwoHand.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Base/Left/Body/1") as Sprite); // 3 
       BodyList_TwoHand.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Base/Left/Body/2") as Sprite); // 4
       BodyList_TwoHand.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Base/Left/Body/4") as Sprite); // 5
       BodyList_TwoHand.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Base/Right/Body/1") as Sprite); // 6
       BodyList_TwoHand.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Base/Right/Body/2") as Sprite); // 7
       BodyList_TwoHand.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Base/Right/Body/4") as Sprite); // 8
       BodyList_TwoHand.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Base/Up/Body/1") as Sprite); // 9
       BodyList_TwoHand.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Base/Up/Body/2") as Sprite); // 10
       BodyList_TwoHand.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Base/Up/Body/4") as Sprite); // 11


        // 얼굴

        FaceList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Down/Face/1") as Sprite); //0
        FaceList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Left/Face/1") as Sprite); //1
        FaceList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Right/Face/1") as Sprite); //2
        FaceList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Up/Face/1") as Sprite); //3

        FaceList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Down/Face/2") as Sprite); //4
        FaceList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Left/Face/2") as Sprite); //5
        FaceList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Right/Face/2") as Sprite); //6
        FaceList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Up/Face/2") as Sprite); //7

        FaceList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Down/Face/3") as Sprite); //8
        FaceList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Left/Face/3") as Sprite); //9
        FaceList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Right/Face/3") as Sprite); //10
        FaceList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Up/Face/3") as Sprite); //11

        FaceList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Down/Face/4") as Sprite); //12
        FaceList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Left/Face/4") as Sprite); //13
        FaceList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Right/Face/4") as Sprite); //14
        FaceList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Up/Face/4") as Sprite); //15

        FaceList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Down/Face/5") as Sprite); //16
        FaceList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Left/Face/5") as Sprite); //17
        FaceList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Right/Face/5") as Sprite); //18
        FaceList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Up/Face/5") as Sprite); //19

        FaceList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Down/Face/6") as Sprite); //20
        FaceList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Left/Face/6") as Sprite); //21
        FaceList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Right/Face/6") as Sprite); //22
        FaceList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Up/Face/6") as Sprite); //23

        // 헤어

        HairList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Down/Hair/1") as Sprite); // 0
        HairList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Left/Hair/1") as Sprite); // 1
        HairList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Right/Hair/1") as Sprite); // 2
        HairList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Up/Hair/1") as Sprite); // 3

        HairList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Down/Hair/2") as Sprite); // 4
        HairList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Left/Hair/2") as Sprite); // 5
        HairList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Right/Hair/2") as Sprite); // 6
        HairList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Up/Hair/2") as Sprite); // 7

        HairList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Down/Hair/3") as Sprite); // 8
        HairList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Left/Hair/3") as Sprite); // 9
        HairList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Right/Hair/3") as Sprite); // 10
        HairList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Up/Hair/3") as Sprite); // 11


        HairList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Down/Hair/4") as Sprite); // 12
        HairList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Left/Hair/4") as Sprite); // 13
        HairList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Right/Hair/4") as Sprite); // 14
        HairList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Up/Hair/4") as Sprite); // 15

        // 헤드

        HeadList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Down/Head/1") as Sprite); // 0 
        HeadList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Left/Head/1") as Sprite); // 1
        HeadList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Right/Head/1") as Sprite); // 2
        HeadList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Up/Head/1") as Sprite); // 3

        // 투구
        HelmetList.Add(Resources.Load<Sprite>("Textures/Item/2000/Down/1") as Sprite); // 0
        HelmetList.Add(Resources.Load<Sprite>("Textures/Item/2000/Left/1") as Sprite); // 1
        HelmetList.Add(Resources.Load<Sprite>("Textures/Item/2000/Right/1") as Sprite); // 2
        HelmetList.Add(Resources.Load<Sprite>("Textures/Item/2000/Up/1") as Sprite); // 3


        // 공격 모션 스프라이트 넣기 (쌩몸)

        Body_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Attack/Down/Body/1") as Sprite); // 0
        Body_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Attack/Down/Body/2") as Sprite); // 1
        Body_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Attack/Down/Body/3") as Sprite); // 2

        Body_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Attack/Left/Body/1") as Sprite); // 3
        Body_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Attack/Left/Body/2") as Sprite); // 4
        Body_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Attack/Left/Body/3") as Sprite); // 5

        Body_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Attack/Right/Body/1") as Sprite); // 6
        Body_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Attack/Right/Body/2") as Sprite); // 7
        Body_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Attack/Right/Body/3") as Sprite); // 8

        Body_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Attack/Up/Body/1") as Sprite); // 9
        Body_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Attack/Up/Body/2") as Sprite); // 10
        Body_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Attack/Up/Body/3") as Sprite); // 11


        // 두손검 공격 모션 스프라이트 넣기 (쌩몸)

        Body_Attack_List_TwoHand_1.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_1/Down/Body/1") as Sprite); // 0
        Body_Attack_List_TwoHand_1.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_1/Down/Body/2") as Sprite); // 1
        Body_Attack_List_TwoHand_1.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_1/Down/Body/3") as Sprite); // 2

        Body_Attack_List_TwoHand_1.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_1/Left/Body/1") as Sprite); // 3
        Body_Attack_List_TwoHand_1.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_1/Left/Body/2") as Sprite); // 4
        Body_Attack_List_TwoHand_1.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_1/Left/Body/3") as Sprite); // 5

        Body_Attack_List_TwoHand_1.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_1/Right/Body/1") as Sprite); // 6
        Body_Attack_List_TwoHand_1.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_1/Right/Body/2") as Sprite); // 7
        Body_Attack_List_TwoHand_1.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_1/Right/Body/3") as Sprite); // 8

        Body_Attack_List_TwoHand_1.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_1/Up/Body/1") as Sprite); // 9
        Body_Attack_List_TwoHand_1.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_1/Up/Body/2") as Sprite); // 10
        Body_Attack_List_TwoHand_1.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_1/Up/Body/3") as Sprite); // 11


        // 두손검 찌르기 공격 모션 스프라이트 넣기 (쌩몸)

        Body_Attack_List_TwoHand_2.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_2/Down/Body/1") as Sprite); // 0
        Body_Attack_List_TwoHand_2.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_2/Down/Body/2") as Sprite); // 1
        Body_Attack_List_TwoHand_2.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_2/Down/Body/3") as Sprite); // 2

        Body_Attack_List_TwoHand_2.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_2/Left/Body/1") as Sprite); // 3
        Body_Attack_List_TwoHand_2.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_2/Left/Body/2") as Sprite); // 4
        Body_Attack_List_TwoHand_2.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_2/Left/Body/3") as Sprite); // 5

        Body_Attack_List_TwoHand_2.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_2/Right/Body/1") as Sprite); // 6
        Body_Attack_List_TwoHand_2.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_2/Right/Body/2") as Sprite); // 7
        Body_Attack_List_TwoHand_2.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_2/Right/Body/3") as Sprite); // 8

        Body_Attack_List_TwoHand_2.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_2/Up/Body/1") as Sprite); // 9
        Body_Attack_List_TwoHand_2.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_2/Up/Body/2") as Sprite); // 10
        Body_Attack_List_TwoHand_2.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_2/Up/Body/3") as Sprite); // 11


        // 줍기 모션 스프라이트 넣기 (쌩몸)

        Body_Drop_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Drop/Down/Body/1") as Sprite); // 0
        Body_Drop_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Drop/Down/Body/2") as Sprite); // 1

        Body_Drop_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Drop/Left/Body/1") as Sprite); // 2
        Body_Drop_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Drop/Left/Body/2") as Sprite); // 3

        Body_Drop_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Drop/Right/Body/1") as Sprite); // 4
        Body_Drop_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Drop/Right/Body/2") as Sprite); // 5

        Body_Drop_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Drop/Up/Body/1") as Sprite); // 6
        Body_Drop_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Drop/Up/Body/2") as Sprite); // 7


        // 주문 모션 스프라이트 넣기 (쌩몸)

        Body_Spell_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Spell/Down/Body/1") as Sprite); // 0 
        Body_Spell_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Spell/Down/Body/2") as Sprite); // 1

        Body_Spell_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Spell/Left/Body/1") as Sprite); // 2
        Body_Spell_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Spell/Left/Body/2") as Sprite); // 3

        Body_Spell_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Spell/Right/Body/1") as Sprite); // 4
        Body_Spell_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Spell/Right/Body/2") as Sprite); // 5

        Body_Spell_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Spell/Up/Body/1") as Sprite); // 6
        Body_Spell_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Spell/Up/Body/2") as Sprite); // 7


        // 활 모션 스프라이트 넣기 (쌩몸)

        Body_Bow_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Bow_Attack/Down/Body/1") as Sprite); // 0
        Body_Bow_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Bow_Attack/Down/Body/2") as Sprite); // 1
        Body_Bow_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Bow_Attack/Down/Body/3") as Sprite); // 2

        Body_Bow_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Bow_Attack/Left/Body/1") as Sprite); // 3
        Body_Bow_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Bow_Attack/Left/Body/2") as Sprite); // 4
        Body_Bow_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Bow_Attack/Left/Body/3") as Sprite); // 5

        Body_Bow_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Bow_Attack/Right/Body/1") as Sprite); // 6
        Body_Bow_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Bow_Attack/Right/Body/2") as Sprite); // 7
        Body_Bow_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Bow_Attack/Right/Body/3") as Sprite); // 8

        Body_Bow_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Bow_Attack/Up/Body/1") as Sprite); // 9
        Body_Bow_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Bow_Attack/Up/Body/2") as Sprite); // 10
        Body_Bow_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Bow_Attack/Up/Body/3") as Sprite); // 11





        // 옷 걷기 모션 스프라이트 넣기

        ArmorList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Down/Armor/1") as Sprite); // 0
        ArmorList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Down/Armor/2") as Sprite); // 1
        ArmorList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Down/Armor/4") as Sprite); // 2
        ArmorList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Left/Armor/1") as Sprite); // 3
        ArmorList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Left/Armor/2") as Sprite); // 4
        ArmorList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Left/Armor/4") as Sprite); // 5
        ArmorList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Right/Armor/1") as Sprite); // 6
        ArmorList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Right/Armor/2") as Sprite); // 7
        ArmorList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Right/Armor/4") as Sprite); // 8
        ArmorList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Up/Armor/1") as Sprite); // 9
        ArmorList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Up/Armor/2") as Sprite); // 10
        ArmorList.Add(Resources.Load<Sprite>("Textures/Character/Base 2/Up/Armor/4") as Sprite); // 11

        ArmorList.Add(Resources.Load<Sprite>("Textures/Item/100/Base/Down/Armor/1") as Sprite); // 0 +12
        ArmorList.Add(Resources.Load<Sprite>("Textures/Item/100/Base/Down/Armor/2") as Sprite); // 1 +12
        ArmorList.Add(Resources.Load<Sprite>("Textures/Item/100/Base/Down/Armor/4") as Sprite); // 2 +12
        ArmorList.Add(Resources.Load<Sprite>("Textures/Item/100/Base/Left/Armor/1") as Sprite); // 3 +12
        ArmorList.Add(Resources.Load<Sprite>("Textures/Item/100/Base/Left/Armor/2") as Sprite); // 4 +12
        ArmorList.Add(Resources.Load<Sprite>("Textures/Item/100/Base/Left/Armor/4") as Sprite); // 5 +12
        ArmorList.Add(Resources.Load<Sprite>("Textures/Item/100/Base/Right/Armor/1") as Sprite); // 6 +12
        ArmorList.Add(Resources.Load<Sprite>("Textures/Item/100/Base/Right/Armor/2") as Sprite); // 7 +12
        ArmorList.Add(Resources.Load<Sprite>("Textures/Item/100/Base/Right/Armor/4") as Sprite); // 8 +12
        ArmorList.Add(Resources.Load<Sprite>("Textures/Item/100/Base/Up/Armor/1") as Sprite); // 9 +12
        ArmorList.Add(Resources.Load<Sprite>("Textures/Item/100/Base/Up/Armor/2") as Sprite); // 10 +12
        ArmorList.Add(Resources.Load<Sprite>("Textures/Item/100/Base/Up/Armor/4") as Sprite); // 11 +12


        // 옷 걷기 모션_ 검 들었을 때 스프라이트 넣기 

        ArmorList_OneHand.Add(Resources.Load<Sprite>("Textures/Character/OneHand/Base/Down/Armor/1") as Sprite); // 0
        ArmorList_OneHand.Add(Resources.Load<Sprite>("Textures/Character/OneHand/Base/Down/Armor/2") as Sprite); // 1
        ArmorList_OneHand.Add(Resources.Load<Sprite>("Textures/Character/OneHand/Base/Down/Armor/4") as Sprite); // 2
        ArmorList_OneHand.Add(Resources.Load<Sprite>("Textures/Character/OneHand/Base/Left/Armor/1") as Sprite); // 3
        ArmorList_OneHand.Add(Resources.Load<Sprite>("Textures/Character/OneHand/Base/Left/Armor/2") as Sprite); // 4
        ArmorList_OneHand.Add(Resources.Load<Sprite>("Textures/Character/OneHand/Base/Left/Armor/4") as Sprite); // 5
        ArmorList_OneHand.Add(Resources.Load<Sprite>("Textures/Character/OneHand/Base/Right/Armor/1") as Sprite); // 6
        ArmorList_OneHand.Add(Resources.Load<Sprite>("Textures/Character/OneHand/Base/Right/Armor/2") as Sprite); // 7
        ArmorList_OneHand.Add(Resources.Load<Sprite>("Textures/Character/OneHand/Base/Right/Armor/4") as Sprite); // 8
        ArmorList_OneHand.Add(Resources.Load<Sprite>("Textures/Character/OneHand/Base/Up/Armor/1") as Sprite); // 9
        ArmorList_OneHand.Add(Resources.Load<Sprite>("Textures/Character/OneHand/Base/Up/Armor/2") as Sprite); // 10
        ArmorList_OneHand.Add(Resources.Load<Sprite>("Textures/Character/OneHand/Base/Up/Armor/4") as Sprite); // 11

        ArmorList_OneHand.Add(Resources.Load<Sprite>("Textures/Item/100/OneHand_Walk/Down/Armor/1") as Sprite); // 0 +12
        ArmorList_OneHand.Add(Resources.Load<Sprite>("Textures/Item/100/OneHand_Walk/Down/Armor/2") as Sprite); // 1 +12
        ArmorList_OneHand.Add(Resources.Load<Sprite>("Textures/Item/100/OneHand_Walk/Down/Armor/4") as Sprite); // 2 +12
        ArmorList_OneHand.Add(Resources.Load<Sprite>("Textures/Item/100/OneHand_Walk/Left/Armor/1") as Sprite); // 3 +12
        ArmorList_OneHand.Add(Resources.Load<Sprite>("Textures/Item/100/OneHand_Walk/Left/Armor/2") as Sprite); // 4 +12
        ArmorList_OneHand.Add(Resources.Load<Sprite>("Textures/Item/100/OneHand_Walk/Left/Armor/4") as Sprite); // 5 +12
        ArmorList_OneHand.Add(Resources.Load<Sprite>("Textures/Item/100/OneHand_Walk/Right/Armor/1") as Sprite); // 6 +12
        ArmorList_OneHand.Add(Resources.Load<Sprite>("Textures/Item/100/OneHand_Walk/Right/Armor/2") as Sprite); // 7 +12
        ArmorList_OneHand.Add(Resources.Load<Sprite>("Textures/Item/100/OneHand_Walk/Right/Armor/4") as Sprite); // 8 +12
        ArmorList_OneHand.Add(Resources.Load<Sprite>("Textures/Item/100/OneHand_Walk/Up/Armor/1") as Sprite); // 9 +12
        ArmorList_OneHand.Add(Resources.Load<Sprite>("Textures/Item/100/OneHand_Walk/Up/Armor/2") as Sprite); // 10 +12
        ArmorList_OneHand.Add(Resources.Load<Sprite>("Textures/Item/100/OneHand_Walk/Up/Armor/4") as Sprite); // 11 +12


        // 옷 걷기 모션_ 두손 검 들었을 때 스프라이트 넣기 

        ArmorList_TwoHand.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Base/Down/Armor/1") as Sprite); // 0
        ArmorList_TwoHand.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Base/Down/Armor/2") as Sprite); // 1
        ArmorList_TwoHand.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Base/Down/Armor/4") as Sprite); // 2
        ArmorList_TwoHand.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Base/Left/Armor/1") as Sprite); // 3
        ArmorList_TwoHand.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Base/Left/Armor/2") as Sprite); // 4
        ArmorList_TwoHand.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Base/Left/Armor/4") as Sprite); // 5
        ArmorList_TwoHand.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Base/Right/Armor/1") as Sprite); // 6
        ArmorList_TwoHand.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Base/Right/Armor/2") as Sprite); // 7
        ArmorList_TwoHand.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Base/Right/Armor/4") as Sprite); // 8
        ArmorList_TwoHand.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Base/Up/Armor/1") as Sprite); // 9
        ArmorList_TwoHand.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Base/Up/Armor/2") as Sprite); // 10
        ArmorList_TwoHand.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Base/Up/Armor/4") as Sprite); // 11

        ArmorList_TwoHand.Add(Resources.Load<Sprite>("Textures/Item/100/TwoHand_Walk/Down/Armor/1") as Sprite); // 0 +12
        ArmorList_TwoHand.Add(Resources.Load<Sprite>("Textures/Item/100/TwoHand_Walk/Down/Armor/2") as Sprite); // 1 +12
        ArmorList_TwoHand.Add(Resources.Load<Sprite>("Textures/Item/100/TwoHand_Walk/Down/Armor/4") as Sprite); // 2 +12
        ArmorList_TwoHand.Add(Resources.Load<Sprite>("Textures/Item/100/TwoHand_Walk/Left/Armor/1") as Sprite); // 3 +12
        ArmorList_TwoHand.Add(Resources.Load<Sprite>("Textures/Item/100/TwoHand_Walk/Left/Armor/2") as Sprite); // 4 +12
        ArmorList_TwoHand.Add(Resources.Load<Sprite>("Textures/Item/100/TwoHand_Walk/Left/Armor/4") as Sprite); // 5 +12
        ArmorList_TwoHand.Add(Resources.Load<Sprite>("Textures/Item/100/TwoHand_Walk/Right/Armor/1") as Sprite); // 6 +12
        ArmorList_TwoHand.Add(Resources.Load<Sprite>("Textures/Item/100/TwoHand_Walk/Right/Armor/2") as Sprite); // 7 +12
        ArmorList_TwoHand.Add(Resources.Load<Sprite>("Textures/Item/100/TwoHand_Walk/Right/Armor/4") as Sprite); // 8 +12
        ArmorList_TwoHand.Add(Resources.Load<Sprite>("Textures/Item/100/TwoHand_Walk/Up/Armor/1") as Sprite); // 9 +12
        ArmorList_TwoHand.Add(Resources.Load<Sprite>("Textures/Item/100/TwoHand_Walk/Up/Armor/2") as Sprite); // 10 +12
        ArmorList_TwoHand.Add(Resources.Load<Sprite>("Textures/Item/100/TwoHand_Walk/Up/Armor/4") as Sprite); // 11 +12


        // 옷 한손검 공격 모션 스프라이트 넣기

        Armor_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Attack/Down/Armor/1") as Sprite); // 0
        Armor_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Attack/Down/Armor/2") as Sprite); // 1
        Armor_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Attack/Down/Armor/3") as Sprite); // 2

        Armor_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Attack/Left/Armor/1") as Sprite); // 3
        Armor_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Attack/Left/Armor/2") as Sprite); // 4
        Armor_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Attack/Left/Armor/3") as Sprite); // 5

        Armor_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Attack/Right/Armor/1") as Sprite); // 6
        Armor_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Attack/Right/Armor/2") as Sprite); // 7
        Armor_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Attack/Right/Armor/3") as Sprite); // 8

        Armor_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Attack/Up/Armor/1") as Sprite); // 9
        Armor_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Attack/Up/Armor/2") as Sprite); // 10
        Armor_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Attack/Up/Armor/3") as Sprite);  // 11


        Armor_Attack_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Attack/Down/Armor/1") as Sprite); // 0 +12
        Armor_Attack_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Attack/Down/Armor/2") as Sprite); // 1 +12
        Armor_Attack_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Attack/Down/Armor/3") as Sprite); // 2 +12

        Armor_Attack_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Attack/Left/Armor/1") as Sprite); // 3 +12
        Armor_Attack_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Attack/Left/Armor/2") as Sprite); // 4 +12
        Armor_Attack_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Attack/Left/Armor/3") as Sprite); // 5 +12

        Armor_Attack_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Attack/Right/Armor/1") as Sprite); // 6 +12
        Armor_Attack_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Attack/Right/Armor/2") as Sprite); // 7 +12
        Armor_Attack_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Attack/Right/Armor/3") as Sprite); // 8 +12

        Armor_Attack_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Attack/Up/Armor/1") as Sprite); // 9 +12
        Armor_Attack_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Attack/Up/Armor/2") as Sprite); // 10 +12
        Armor_Attack_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Attack/Up/Armor/3") as Sprite); // 11 +12


        // 옷 두손검 (베기) 공격 모션 스프라이트 넣기

        Armor_Attack_List_TwoHand_1.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_1/Down/Armor/1") as Sprite); // 0
        Armor_Attack_List_TwoHand_1.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_1/Down/Armor/2") as Sprite); // 1
        Armor_Attack_List_TwoHand_1.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_1/Down/Armor/3") as Sprite); // 2
        Armor_Attack_List_TwoHand_1.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_1/Left/Armor/1") as Sprite); // 3
        Armor_Attack_List_TwoHand_1.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_1/Left/Armor/2") as Sprite); // 4
        Armor_Attack_List_TwoHand_1.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_1/Left/Armor/3") as Sprite); // 5
        Armor_Attack_List_TwoHand_1.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_1/Right/Armor/1") as Sprite); // 6
        Armor_Attack_List_TwoHand_1.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_1/Right/Armor/2") as Sprite); // 7
        Armor_Attack_List_TwoHand_1.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_1/Right/Armor/3") as Sprite); // 8
        Armor_Attack_List_TwoHand_1.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_1/Up/Armor/1") as Sprite); // 9
        Armor_Attack_List_TwoHand_1.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_1/Up/Armor/2") as Sprite); // 10
        Armor_Attack_List_TwoHand_1.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_1/Up/Armor/3") as Sprite);  // 11

        Armor_Attack_List_TwoHand_1.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Attack_TwoHand_1/Down/Armor/1") as Sprite); // 0 +12
        Armor_Attack_List_TwoHand_1.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Attack_TwoHand_1/Down/Armor/2") as Sprite); // 1 +12
        Armor_Attack_List_TwoHand_1.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Attack_TwoHand_1/Down/Armor/3") as Sprite); // 2 +12
        Armor_Attack_List_TwoHand_1.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Attack_TwoHand_1/Left/Armor/1") as Sprite); // 3 +12
        Armor_Attack_List_TwoHand_1.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Attack_TwoHand_1/Left/Armor/2") as Sprite); // 4 +12
        Armor_Attack_List_TwoHand_1.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Attack_TwoHand_1/Left/Armor/3") as Sprite); // 5 +12
        Armor_Attack_List_TwoHand_1.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Attack_TwoHand_1/Right/Armor/1") as Sprite); // 6 +12
        Armor_Attack_List_TwoHand_1.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Attack_TwoHand_1/Right/Armor/2") as Sprite); // 7 +12
        Armor_Attack_List_TwoHand_1.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Attack_TwoHand_1/Right/Armor/3") as Sprite); // 8 +12
        Armor_Attack_List_TwoHand_1.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Attack_TwoHand_1/Up/Armor/1") as Sprite); // 9 +12
        Armor_Attack_List_TwoHand_1.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Attack_TwoHand_1/Up/Armor/2") as Sprite); // 10 +12
        Armor_Attack_List_TwoHand_1.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Attack_TwoHand_1/Up/Armor/3") as Sprite); // 11 +12


        // 옷 두손검 (찌르기) 공격 모션 스프라이트 넣기

        Armor_Attack_List_TwoHand_2.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_2/Down/Armor/1") as Sprite); // 0
        Armor_Attack_List_TwoHand_2.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_2/Down/Armor/2") as Sprite); // 1
        Armor_Attack_List_TwoHand_2.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_2/Down/Armor/3") as Sprite); // 2
        Armor_Attack_List_TwoHand_2.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_2/Left/Armor/1") as Sprite); // 3
        Armor_Attack_List_TwoHand_2.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_2/Left/Armor/2") as Sprite); // 4
        Armor_Attack_List_TwoHand_2.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_2/Left/Armor/3") as Sprite); // 5
        Armor_Attack_List_TwoHand_2.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_2/Right/Armor/1") as Sprite); // 6
        Armor_Attack_List_TwoHand_2.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_2/Right/Armor/2") as Sprite); // 7
        Armor_Attack_List_TwoHand_2.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_2/Right/Armor/3") as Sprite); // 8
        Armor_Attack_List_TwoHand_2.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_2/Up/Armor/1") as Sprite); // 9
        Armor_Attack_List_TwoHand_2.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_2/Up/Armor/2") as Sprite); // 10
        Armor_Attack_List_TwoHand_2.Add(Resources.Load<Sprite>("Textures/Character/TwoHand/Attack_2/Up/Armor/3") as Sprite);  // 11

        Armor_Attack_List_TwoHand_2.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Attack_TwoHand_2/Down/Armor/1") as Sprite); // 0 +12
        Armor_Attack_List_TwoHand_2.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Attack_TwoHand_2/Down/Armor/2") as Sprite); // 1 +12
        Armor_Attack_List_TwoHand_2.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Attack_TwoHand_2/Down/Armor/3") as Sprite); // 2 +12
        Armor_Attack_List_TwoHand_2.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Attack_TwoHand_2/Left/Armor/1") as Sprite); // 3 +12
        Armor_Attack_List_TwoHand_2.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Attack_TwoHand_2/Left/Armor/2") as Sprite); // 4 +12
        Armor_Attack_List_TwoHand_2.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Attack_TwoHand_2/Left/Armor/3") as Sprite); // 5 +12
        Armor_Attack_List_TwoHand_2.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Attack_TwoHand_2/Right/Armor/1") as Sprite); // 6 +12
        Armor_Attack_List_TwoHand_2.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Attack_TwoHand_2/Right/Armor/2") as Sprite); // 7 +12
        Armor_Attack_List_TwoHand_2.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Attack_TwoHand_2/Right/Armor/3") as Sprite); // 8 +12
        Armor_Attack_List_TwoHand_2.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Attack_TwoHand_2/Up/Armor/1") as Sprite); // 9 +12
        Armor_Attack_List_TwoHand_2.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Attack_TwoHand_2/Up/Armor/2") as Sprite); // 10 +12
        Armor_Attack_List_TwoHand_2.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Attack_TwoHand_2/Up/Armor/3") as Sprite); // 11 +12

        // 옷 줍기 모션 스프라이트 넣기


        Armor_Drop_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Drop/Down/Armor/1") as Sprite); // 0
        Armor_Drop_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Drop/Down/Armor/2") as Sprite); // 1
        Armor_Drop_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Drop/Left/Armor/1") as Sprite); // 2
        Armor_Drop_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Drop/Left/Armor/2") as Sprite); // 3
        Armor_Drop_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Drop/Right/Armor/1") as Sprite); // 4
        Armor_Drop_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Drop/Right/Armor/2") as Sprite); // 5
        Armor_Drop_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Drop/Up/Armor/1") as Sprite); // 6
        Armor_Drop_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Drop/Up/Armor/2") as Sprite); // 7

        Armor_Drop_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Drop/Down/Armor/1") as Sprite); // 0 + 8
        Armor_Drop_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Drop/Down/Armor/2") as Sprite); // 1 + 8
        Armor_Drop_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Drop/Left/Armor/1") as Sprite); // 2 + 8
        Armor_Drop_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Drop/Left/Armor/2") as Sprite); // 3 + 8
        Armor_Drop_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Drop/Right/Armor/1") as Sprite); // 4 + 8
        Armor_Drop_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Drop/Right/Armor/2") as Sprite); // 5 + 8
        Armor_Drop_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Drop/Up/Armor/1") as Sprite); // 6 + 8
        Armor_Drop_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Drop/Up/Armor/2") as Sprite); // 7 + 8


        // 옷 주문 모션 스프라이트 넣기

        Armor_Spell_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Spell/Down/Armor/1") as Sprite); // 0 
        Armor_Spell_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Spell/Down/Armor/2") as Sprite); // 1
        Armor_Spell_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Spell/Left/Armor/1") as Sprite); // 2
        Armor_Spell_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Spell/Left/Armor/2") as Sprite); // 3
        Armor_Spell_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Spell/Right/Armor/1") as Sprite); // 4
        Armor_Spell_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Spell/Right/Armor/2") as Sprite); // 5
        Armor_Spell_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Spell/Up/Armor/1") as Sprite); // 6
        Armor_Spell_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Spell/Up/Armor/2") as Sprite); // 7

        Armor_Spell_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Spell/Down/Armor/1") as Sprite); // 0 + 8
        Armor_Spell_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Spell/Down/Armor/2") as Sprite); // 1 + 8
        Armor_Spell_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Spell/Left/Armor/1") as Sprite); // 2 + 8
        Armor_Spell_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Spell/Left/Armor/2") as Sprite); // 3 + 8
        Armor_Spell_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Spell/Right/Armor/1") as Sprite); // 4 + 8
        Armor_Spell_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Spell/Right/Armor/2") as Sprite); // 5 + 8
        Armor_Spell_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Spell/Up/Armor/1") as Sprite); // 6 + 8
        Armor_Spell_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Spell/Up/Armor/2") as Sprite); // 7 + 8



        // 옷 활 공격 모션 스프라이트 넣기

        Armor_Bow_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Bow_Attack/Down/Armor/1") as Sprite); // 0
        Armor_Bow_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Bow_Attack/Down/Armor/2") as Sprite); // 1
        Armor_Bow_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Bow_Attack/Down/Armor/3") as Sprite); // 2

        Armor_Bow_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Bow_Attack/Left/Armor/1") as Sprite); // 3
        Armor_Bow_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Bow_Attack/Left/Armor/2") as Sprite); // 4
        Armor_Bow_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Bow_Attack/Left/Armor/3") as Sprite); // 5

        Armor_Bow_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Bow_Attack/Right/Armor/1") as Sprite); // 6
        Armor_Bow_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Bow_Attack/Right/Armor/2") as Sprite); // 7
        Armor_Bow_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Bow_Attack/Right/Armor/3") as Sprite); // 8

        Armor_Bow_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Bow_Attack/Up/Armor/1") as Sprite); // 9
        Armor_Bow_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Bow_Attack/Up/Armor/2") as Sprite); // 10
        Armor_Bow_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/Base_Bow_Attack/Up/Armor/3") as Sprite);  // 11


        Armor_Bow_Attack_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Bow_Attack/Down/Armor/1") as Sprite); // 0 +12
        Armor_Bow_Attack_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Bow_Attack/Down/Armor/2") as Sprite); // 1 +12
        Armor_Bow_Attack_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Bow_Attack/Down/Armor/3") as Sprite); // 2 +12

        Armor_Bow_Attack_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Bow_Attack/Left/Armor/1") as Sprite); // 3 +12
        Armor_Bow_Attack_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Bow_Attack/Left/Armor/2") as Sprite); // 4 +12
        Armor_Bow_Attack_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Bow_Attack/Left/Armor/3") as Sprite); // 5 +12

        Armor_Bow_Attack_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Bow_Attack/Right/Armor/1") as Sprite); // 6 +12
        Armor_Bow_Attack_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Bow_Attack/Right/Armor/2") as Sprite); // 7 +12
        Armor_Bow_Attack_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Bow_Attack/Right/Armor/3") as Sprite); // 8 +12

        Armor_Bow_Attack_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Bow_Attack/Up/Armor/1") as Sprite); // 9 +12
        Armor_Bow_Attack_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Bow_Attack/Up/Armor/2") as Sprite); // 10 +12
        Armor_Bow_Attack_List.Add(Resources.Load<Sprite>("Textures/Item/100/Base_Bow_Attack/Up/Armor/3") as Sprite); // 11 +12




        // 무기 걷기 모션 스프라이트 넣기

        // 1) 검
        RightHandList.Add(Resources.Load<Sprite>("Textures/Character/OneHand/1/Walk/Down/1") as Sprite); // 0
        RightHandList.Add(Resources.Load<Sprite>("Textures/Character/OneHand/1/Walk/Left/1") as Sprite); // 1
        RightHandList.Add(Resources.Load<Sprite>("Textures/Character/OneHand/1/Walk/Right/2") as Sprite); // 2
        RightHandList.Add(Resources.Load<Sprite>("Textures/Character/OneHand/1/Walk/Up/2") as Sprite); // 3

        // 2) 활
        RightHandList.Add(Resources.Load<Sprite>("Textures/Character/OneHand/2/Walk/Down/1") as Sprite); // 4
        RightHandList.Add(Resources.Load<Sprite>("Textures/Character/OneHand/2/Walk/Left/1") as Sprite); // 5
        RightHandList.Add(Resources.Load<Sprite>("Textures/Character/OneHand/2/Walk/Right/2") as Sprite); // 6
        RightHandList.Add(Resources.Load<Sprite>("Textures/Character/OneHand/2/Walk/Up/2") as Sprite); // 7

        // 3) 두손검 // 수정 필요함
        RightHandList.Add(Resources.Load<Sprite>("Textures/Character/OneHand/3/Walk/Down/1") as Sprite); // 0
        RightHandList.Add(Resources.Load<Sprite>("Textures/Character/OneHand/3/Walk/Left/1") as Sprite); // 1
        RightHandList.Add(Resources.Load<Sprite>("Textures/Character/OneHand/3/Walk/Right/1") as Sprite); // 2
        RightHandList.Add(Resources.Load<Sprite>("Textures/Character/OneHand/3/Walk/Up/1") as Sprite); // 3

        // 4) 창 // 수정 필요함
        RightHandList.Add(Resources.Load<Sprite>("Textures/Character/OneHand/4/Walk/Down/1") as Sprite); // 0
        RightHandList.Add(Resources.Load<Sprite>("Textures/Character/OneHand/4/Walk/Down/1") as Sprite); // 1
        RightHandList.Add(Resources.Load<Sprite>("Textures/Character/OneHand/4/Walk/Down/1") as Sprite); // 2
        RightHandList.Add(Resources.Load<Sprite>("Textures/Character/OneHand/4/Walk/Down/1") as Sprite); // 3

        // 무기 공격 모션 스프라이트 넣기

        // 1) 검
        RightHand_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/1/Attack/Down/2") as Sprite); // 0
        RightHand_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/1/Attack/Down/3") as Sprite); // 1
        RightHand_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/1/Attack/Left/2") as Sprite); // 2
        RightHand_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/1/Attack/Left/3") as Sprite); // 3
        RightHand_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/1/Attack/Right/2") as Sprite); // 4
        RightHand_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/1/Attack/Right/3") as Sprite); // 5
        RightHand_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/1/Attack/Up/2") as Sprite); // 6
        RightHand_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/1/Attack/Up/3") as Sprite); // 7

        RightHand_Sting_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/1/Attack/Down/2") as Sprite); // 0
        RightHand_Sting_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/1/Attack/Down/3") as Sprite); // 1
        RightHand_Sting_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/1/Attack/Left/2") as Sprite); // 2
        RightHand_Sting_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/1/Attack/Left/3") as Sprite); // 3
        RightHand_Sting_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/1/Attack/Right/2") as Sprite); // 4
        RightHand_Sting_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/1/Attack/Right/3") as Sprite); // 5
        RightHand_Sting_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/1/Attack/Up/2") as Sprite); // 6
        RightHand_Sting_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/1/Attack/Up/3") as Sprite); // 7

        // 2) 활
        RightHand_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/2/Attack/Down/2") as Sprite); // 0
        RightHand_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/2/Attack/Down/3") as Sprite); // 1
        RightHand_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/2/Attack/Left/2") as Sprite); // 2
        RightHand_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/2/Attack/Left/3") as Sprite); // 3
        RightHand_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/2/Attack/Right/2") as Sprite); // 4
        RightHand_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/2/Attack/Right/3") as Sprite); // 5
        RightHand_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/2/Attack/Up/2") as Sprite); // 6
        RightHand_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/2/Attack/Up/3") as Sprite); // 7

        RightHand_Sting_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/2/Attack/Down/2") as Sprite); // 0
        RightHand_Sting_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/2/Attack/Down/3") as Sprite); // 1
        RightHand_Sting_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/2/Attack/Left/2") as Sprite); // 2
        RightHand_Sting_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/2/Attack/Left/3") as Sprite); // 3
        RightHand_Sting_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/2/Attack/Right/2") as Sprite); // 4
        RightHand_Sting_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/2/Attack/Right/3") as Sprite); // 5
        RightHand_Sting_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/2/Attack/Up/2") as Sprite); // 6
        RightHand_Sting_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/2/Attack/Up/3") as Sprite); // 7

        // 3) 두손검
        RightHand_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/3/Attack/Down/2") as Sprite); // 0
        RightHand_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/3/Attack/Down/3") as Sprite); // 1
        RightHand_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/3/Attack/Left/2") as Sprite); // 2
        RightHand_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/3/Attack/Left/3") as Sprite); // 3
        RightHand_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/3/Attack/Right/2") as Sprite); // 4
        RightHand_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/3/Attack/Right/3") as Sprite); // 5
        RightHand_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/3/Attack/Up/2") as Sprite); // 6
        RightHand_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/3/Attack/Up/3") as Sprite); // 7

        RightHand_Sting_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/3/Attack_2/Down/2") as Sprite); // 0
        RightHand_Sting_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/3/Attack_2/Down/3") as Sprite); // 1
        RightHand_Sting_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/3/Attack_2/Left/2") as Sprite); // 2
        RightHand_Sting_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/3/Attack_2/Left/2") as Sprite); // 3
        RightHand_Sting_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/3/Attack_2/Right/2") as Sprite); // 4
        RightHand_Sting_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/3/Attack_2/Right/2") as Sprite); // 5
        RightHand_Sting_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/3/Attack_2/Up/2") as Sprite); // 6
        RightHand_Sting_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/3/Attack_2/Up/2") as Sprite); // 7

        // 4) 창
        RightHand_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/4/Attack/Down/2") as Sprite); // 0
        RightHand_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/4/Attack/Down/2") as Sprite); // 1
        RightHand_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/4/Attack/Left/2") as Sprite); // 2
        RightHand_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/4/Attack/Left/2") as Sprite); // 3
        RightHand_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/4/Attack/Right/2") as Sprite); // 4
        RightHand_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/4/Attack/Right/2") as Sprite); // 5
        RightHand_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/4/Attack/Up/2") as Sprite); // 6
        RightHand_Attack_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/4/Attack/Up/2") as Sprite); // 7

        RightHand_Sting_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/4/Attack_2/Down/2") as Sprite); // 0
        RightHand_Sting_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/4/Attack_2/Down/2") as Sprite); // 1
        RightHand_Sting_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/4/Attack_2/Left/2") as Sprite); // 2
        RightHand_Sting_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/4/Attack_2/Left/2") as Sprite); // 3
        RightHand_Sting_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/4/Attack_2/Right/2") as Sprite); // 4
        RightHand_Sting_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/4/Attack_2/Right/2") as Sprite); // 5
        RightHand_Sting_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/4/Attack_2/Up/2") as Sprite); // 6
        RightHand_Sting_List.Add(Resources.Load<Sprite>("Textures/Character/OneHand/4/Attack_2/Up/2") as Sprite); // 7

    }


    public void IsOneHand()
    {
        
        if(RightHand == -1 && transform.GetChild(6).transform.gameObject.activeSelf)
            transform.GetChild(6).transform.gameObject.SetActive(false);
        else if(RightHand != -1 && !transform.GetChild(6).transform.gameObject.activeSelf)
            transform.GetChild(6).transform.gameObject.SetActive(true);
    }

    public void DeleteOneHand()
    {
        transform.GetChild(6).transform.gameObject.SetActive(false);
    }


    // ===============================================================================================//

    public void Walk_Down_1()
    {
        IsOneHand();

        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = HeadList[0];
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = FaceList[Face * 4];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = HairList[Hair * 4];
        

        if (RightHand == -1)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList[1];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList[1 +(Shirts* 12)];
        }
        else
        {
            if(weaponKind == 0 || weaponKind == 1 || weaponKind == 3 ) // 검 혹은 활 혹은 창
            {
                transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList_OneHand[1];
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList_OneHand[1 + (Shirts * 12)];
                transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHandList[0 + RightHand * 4];
                transform.GetChild(6).localPosition = new Vector3(-10, -2, -5); // RightHand
            }
            else if (weaponKind == 2 ) // 두손검
            {
                transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList_TwoHand[1];
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList_TwoHand[1 + (Shirts * 12)];
                transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHandList[0 + RightHand * 4];
                transform.GetChild(6).localPosition = new Vector3(-9, 5, -5); // RightHand
            }

        }



        transform.GetChild(1).localPosition = new Vector3(0, 0, 0);
        transform.GetChild(2).localPosition = new Vector3(0, 15,-1);
        transform.GetChild(3).localPosition = new Vector3(0, 15,-2);
        transform.GetChild(4).localPosition = new Vector3(0, 15,-4);
        transform.GetChild(5).localPosition = new Vector3(0, 0,-3);


        if (Helmet != -1){
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet];
            transform.GetChild(7).localPosition = new Vector3(0, 21, -4.5f); }

    }

    public void Walk_Down_2_4()
    {
        IsOneHand();

        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = HeadList[0];
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = FaceList[Face * 4];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = HairList[Hair * 4];

        if (RightHand == -1)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList[0];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList[0 + (Shirts * 12)];
        }
        else
        {

            if (weaponKind == 0 || weaponKind == 1 || weaponKind == 3)  // 검 혹은 활 혹은 창
            {
                transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList_OneHand[0];
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList_OneHand[0 + (Shirts * 12)];
                transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHandList[0 + RightHand * 4];
                transform.GetChild(6).localPosition = new Vector3(-10, -1, -5);
            }
            else if (weaponKind == 2) // 두손검
            {
                transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList_TwoHand[0];
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList_TwoHand[0 + (Shirts * 12)];
                transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHandList[0 + RightHand * 4];
                transform.GetChild(6).localPosition = new Vector3(-9, 6, -5);
            }


        }

        if (Helmet != -1)
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet];


        transform.GetChild(1).localPosition = new Vector3(0, 0, 0);
        transform.GetChild(2).localPosition = new Vector3(0, 16, -1);
        transform.GetChild(3).localPosition = new Vector3(0, 16, -2);
        transform.GetChild(4).localPosition = new Vector3(0, 16, -4);
        transform.GetChild(5).localPosition = new Vector3(0, 0, -3);


        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet];
            transform.GetChild(7).localPosition = new Vector3(0, 22, -4.5f);
        }

    }

    public void Walk_Down_3()
    {
        IsOneHand();




        if (RightHand == -1)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList[2];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList[2 + (Shirts * 12)];
        }
        else
        {
            if (weaponKind == 0 || weaponKind == 1 || weaponKind == 3) // 검 혹은 활 혹은 창
            {
                transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList_OneHand[2];
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList_OneHand[2 + (Shirts * 12)];
                transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHandList[0 + RightHand * 4];
                transform.GetChild(6).localPosition = new Vector3(-10, -2, -5);
            }
            else if (weaponKind == 2) // 두손검
            {
                transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList_TwoHand[2];
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList_TwoHand[2 + (Shirts * 12)];
                transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHandList[0 + RightHand * 4];
                transform.GetChild(6).localPosition = new Vector3(-9, 5, -5);
            }


        }

        if (Helmet != -1)
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet];

        transform.GetChild(1).localPosition = new Vector3(0, 0, 0);
        transform.GetChild(2).localPosition = new Vector3(0, 15, -1);
        transform.GetChild(3).localPosition = new Vector3(0, 15, -2);
        transform.GetChild(4).localPosition = new Vector3(0, 15, -4);
        transform.GetChild(5).localPosition = new Vector3(0, 0, -3);



        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet];
            transform.GetChild(7).localPosition = new Vector3(0, 21, -4.5f);
        }

    }


    // =============================//

    public void Walk_Left_1()
    {
        IsOneHand();

        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = HeadList[1];
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = FaceList[Face * 4 + 1];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = HairList[Hair * 4 + 1];


        transform.GetChild(1).localPosition = new Vector3(0, 0, 0);
        transform.GetChild(2).localPosition = new Vector3(0, 15, -1);
        transform.GetChild(3).localPosition = new Vector3(0, 15, -2);
        transform.GetChild(4).localPosition = new Vector3(0, 15, -4);
        transform.GetChild(5).localPosition = new Vector3(0, 0, -3);


        if (RightHand == -1)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList[4];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList[4 + (Shirts * 12)];
        }
        else
        {

            if (weaponKind == 0 || weaponKind == 1 || weaponKind == 3) // 검 혹은 활 혹은 창
            {
                transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList_OneHand[4];
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList_OneHand[4 + (Shirts * 12)];
                transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHandList[1 + RightHand * 4];

                transform.GetChild(6).localPosition = new Vector3(-10, -2, -5);

                // 활이라면 레이어 아래로 내려가게 해준다.
                if (weaponKind == 1)
                {
                    transform.GetChild(6).localPosition = new Vector3(-10, -2, 10);
                }
            }
            else if (weaponKind == 2) // 두손검
            {
                transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList_TwoHand[4];
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList_TwoHand[4 + (Shirts * 12)];
                transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHandList[1 + RightHand * 4];
                transform.GetChild(6).localPosition = new Vector3(-10, 5, -5);

            }



        }



        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet+1];
            transform.GetChild(7).localPosition = new Vector3(0, 21, -4.5f);
        }

    }

    public void Walk_Left_2_4()
    {
        IsOneHand();

        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = HeadList[1];
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = FaceList[Face * 4 + 1];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = HairList[Hair * 4 + 1];


        transform.GetChild(1).localPosition = new Vector3(0, 0, 0);
        transform.GetChild(2).localPosition = new Vector3(0, 16, -1);
        transform.GetChild(3).localPosition = new Vector3(0, 16, -2);
        transform.GetChild(4).localPosition = new Vector3(0, 16, -4);
        transform.GetChild(5).localPosition = new Vector3(0, 0, -3);



        if (RightHand == -1)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList[3];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList[3 + (Shirts * 12)];
        }
        else
        {

            if (weaponKind == 0 || weaponKind == 1 || weaponKind == 3) // 검 혹은 활 혹은 창
            {
                transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList_OneHand[3];
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList_OneHand[3 + (Shirts * 12)];
                transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHandList[1 + RightHand * 4];
                transform.GetChild(6).localPosition = new Vector3(-9, -1, -5);

                // 활이라면 레이어 아래로 내려가게 해준다.
                if (weaponKind == 1)
                {
                    transform.GetChild(6).localPosition = new Vector3(-9, -1, 10);
                }
            }
            else if (weaponKind == 2) // 두손검
            {
                transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList_TwoHand[3];
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList_TwoHand[3 + (Shirts * 12)];
                transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHandList[1 + RightHand * 4];
                transform.GetChild(6).localPosition = new Vector3(-10, 6, -5);
            }




        }



        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet + 1];
            transform.GetChild(7).localPosition = new Vector3(0, 22, -4.5f);
        }

    }

    public void Walk_Left_3()
    {
        IsOneHand();


        transform.GetChild(1).localPosition = new Vector3(0, 0, 0);
        transform.GetChild(2).localPosition = new Vector3(0, 15, -1);
        transform.GetChild(3).localPosition = new Vector3(0, 15, -2);
        transform.GetChild(4).localPosition = new Vector3(0, 15, -4);
        transform.GetChild(5).localPosition = new Vector3(0, 0, -3);


        if (RightHand == -1)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList[5];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList[5 + (Shirts * 12)];
        }
        else
        {

            if (weaponKind == 0 || weaponKind == 1 || weaponKind == 3 ) // 검 혹은 활 혹은 창
            {
                transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList_OneHand[5];
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList_OneHand[5 + (Shirts * 12)];
                transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHandList[1 + RightHand * 4];
                transform.GetChild(6).localPosition = new Vector3(-8, -2, -5);

                // 활이라면 레이어 아래로 내려가게 해준다.
                if (weaponKind == 1)
                {
                    transform.GetChild(6).localPosition = new Vector3(-8, -2, 10);
                }
            }
            else if (weaponKind == 2) // 두손검
            {
                transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList_TwoHand[5];
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList_TwoHand[5 + (Shirts * 12)];
                transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHandList[1 + RightHand * 4];
                transform.GetChild(6).localPosition = new Vector3(-10, 5, -5);
            }


        }





        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet + 1];
            transform.GetChild(7).localPosition = new Vector3(0, 21, -4.5f);
        }

    }

    // =============================//


    public void Walk_Right_1()
    {
        IsOneHand();

        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = HeadList[2];
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = FaceList[Face * 4 + 2];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = HairList[Hair * 4 + 2];


        if (RightHand == -1)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList[7];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList[7 + (Shirts * 12)];
        }
        else
        {

            if (weaponKind == 0 || weaponKind == 1 || weaponKind == 3) // 검 혹은 활 혹은 창
            {
                transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList_OneHand[7];
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList_OneHand[7 + (Shirts * 12)];
                transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHandList[2 + RightHand * 4];
                transform.GetChild(6).localPosition = new Vector3(6 + 1, -2, -5);
            }
            else if (weaponKind == 2) // 두손검
            {
                transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList_TwoHand[7];
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList_TwoHand[7 + (Shirts * 12)];
                transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHandList[2 + RightHand * 4];
                transform.GetChild(6).localPosition = new Vector3(10, 5, -5);
            }



        }

        transform.GetChild(1).localPosition = new Vector3(0, 0, 0);
        transform.GetChild(2).localPosition = new Vector3(0, 15, -1);
        transform.GetChild(3).localPosition = new Vector3(0, 15, -2);
        transform.GetChild(4).localPosition = new Vector3(0, 15, -4);
        transform.GetChild(5).localPosition = new Vector3(0, 0, -3);



        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet + 2];
            transform.GetChild(7).localPosition = new Vector3(0, 21, -4.5f);
        }


    }

    public void Walk_Right_2_4()
    {
        IsOneHand();

        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = HeadList[2];
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = FaceList[Face * 4 + 2];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = HairList[Hair * 4 + 2];


        if (RightHand == -1)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList[6];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList[6 + (Shirts * 12)];
        }
        else
        {

            if (weaponKind == 0 || weaponKind == 1 || weaponKind == 3) // 검 혹은 활 혹은 창
            {
                transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList_OneHand[6];
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList_OneHand[6 + (Shirts * 12)];
                transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHandList[2 + RightHand * 4];
                transform.GetChild(6).localPosition = new Vector3(6, -1, -5);
            }
            else if (weaponKind == 2) // 두손검
            {
                transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList_TwoHand[6];
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList_TwoHand[6 + (Shirts * 12)];
                transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHandList[2 + RightHand * 4];
                transform.GetChild(6).localPosition = new Vector3(10, 6, -5);
            }

            

        }

        transform.GetChild(1).localPosition = new Vector3(0, 0, 0);
        transform.GetChild(2).localPosition = new Vector3(0, 16, -1);
        transform.GetChild(3).localPosition = new Vector3(0, 16, -2);
        transform.GetChild(4).localPosition = new Vector3(0, 16, -4);
        transform.GetChild(5).localPosition = new Vector3(0, 0, -3);



        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet + 2];
            transform.GetChild(7).localPosition = new Vector3(0, 22, -4.5f);
        }

    }

    public void Walk_Right_3()
    {
        IsOneHand();

        if (RightHand == -1)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList[8];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList[8 + (Shirts * 12)];
        }
        else
        {
            if (weaponKind == 0 || weaponKind == 1 || weaponKind == 3) // 검 혹은 활 혹은 창
            {
                transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList_OneHand[8];
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList_OneHand[8 + (Shirts * 12)];
                transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHandList[2 + RightHand * 4];
                transform.GetChild(6).localPosition = new Vector3(6 - 1, -2, -5);
            }
            else if (weaponKind == 2) // 두손검
            {
                transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList_TwoHand[8];
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList_TwoHand[8 + (Shirts * 12)];
                transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHandList[2 + RightHand * 4];
                transform.GetChild(6).localPosition = new Vector3(10, 5, -5);
            }

        }


        transform.GetChild(1).localPosition = new Vector3(0, 0, 0);
        transform.GetChild(2).localPosition = new Vector3(0, 15, -1);
        transform.GetChild(3).localPosition = new Vector3(0, 15, -2);
        transform.GetChild(4).localPosition = new Vector3(0, 15, -4);
        transform.GetChild(5).localPosition = new Vector3(0, 0, -3);



        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet + 2];
            transform.GetChild(7).localPosition = new Vector3(0, 21, -4.5f);
        }

    }

    // =============================//

    public void Walk_Up_1()
    {
        IsOneHand();

        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = HeadList[3];
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = FaceList[Face * 4 + 3];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = HairList[Hair * 4 + 3];


        if (RightHand == -1)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList[10];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList[10 + (Shirts * 12)];
        }
        else
        {

            if (weaponKind == 0 || weaponKind == 1 || weaponKind == 3) // 검 혹은 활 혹은 창
            {
                transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList_OneHand[10];
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList_OneHand[10 + (Shirts * 12)];
                transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHandList[3 + RightHand * 4];
                transform.GetChild(6).localPosition = new Vector3(10, -2, 1);
            }
            else if (weaponKind == 2) // 두손검
            {
                transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList_TwoHand[10];
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList_TwoHand[10 + (Shirts * 12)];
                transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHandList[3 + RightHand * 4];
                transform.GetChild(6).localPosition = new Vector3(9, 6, 1);
            }

            
        }


        transform.GetChild(1).localPosition = new Vector3(0, 0, 0);
        transform.GetChild(2).localPosition = new Vector3(0, 15, -1);
        transform.GetChild(3).localPosition = new Vector3(0, 15, -2);
        transform.GetChild(4).localPosition = new Vector3(0, 15, -4);
        transform.GetChild(5).localPosition = new Vector3(0, 0, -3);



        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet + 3];
            transform.GetChild(7).localPosition = new Vector3(0, 21, -4.5f);
        }

    }

    public void Walk_Up_2_4()
    {
        IsOneHand();

        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = HeadList[3];
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = FaceList[Face * 4 + 3];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = HairList[Hair * 4 + 3];


        if (RightHand == -1)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList[9];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList[9 + (Shirts * 12)];
        }
        else
        {

            if (weaponKind == 0 || weaponKind == 1 || weaponKind == 3) // 검 혹은 활 혹은 창
            {
                transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList_OneHand[9];
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList_OneHand[9 + (Shirts * 12)];
                transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHandList[3 + RightHand * 4];
                transform.GetChild(6).localPosition = new Vector3(10, -1, 1);
            }
            else if (weaponKind == 2) // 두손검
            {
                transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList_TwoHand[9];
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList_TwoHand[9 + (Shirts * 12)];
                transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHandList[3 + RightHand * 4];
                transform.GetChild(6).localPosition = new Vector3(9, 7, 1);
            }

            
        }


        transform.GetChild(1).localPosition = new Vector3(0, 0, 0);
        transform.GetChild(2).localPosition = new Vector3(0, 16, -1);
        transform.GetChild(3).localPosition = new Vector3(0, 16, -2);
        transform.GetChild(4).localPosition = new Vector3(0, 16, -4);
        transform.GetChild(5).localPosition = new Vector3(0, 0, -3);



        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet + 3];
            transform.GetChild(7).localPosition = new Vector3(0, 22, -4.5f);
        }

    }


    public void Walk_Up_3()
    {
        IsOneHand();

        if (RightHand == -1)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList[11];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList[11 + (Shirts * 12)];
        }
        else
        {

            if (weaponKind == 0 || weaponKind == 1 || weaponKind == 3) // 검 혹은 활 혹은 창
            {
                transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList_OneHand[11];
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList_OneHand[11 + (Shirts * 12)];
                transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHandList[3 + RightHand * 4];
                transform.GetChild(6).localPosition = new Vector3(10, -2, 1);
            }
            else if (weaponKind == 2) // 두손검
            {
                transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList_TwoHand[11];
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList_TwoHand[11 + (Shirts * 12)];
                transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHandList[3 + RightHand * 4];
                transform.GetChild(6).localPosition = new Vector3(9, 6, 1);
            }

            
        }


        transform.GetChild(1).localPosition = new Vector3(0, 0, 0);
        transform.GetChild(2).localPosition = new Vector3(0, 15, -1);
        transform.GetChild(3).localPosition = new Vector3(0, 15, -2);
        transform.GetChild(4).localPosition = new Vector3(0, 15, -4);
        transform.GetChild(5).localPosition = new Vector3(0, 0, -3);



        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet + 3];
            transform.GetChild(7).localPosition = new Vector3(0, 21, -4.5f);
        }
    }


    // ===============================================================================================//


    public void Attack_Down_1()
    {
        if(attackForm == 1)
        {
            Sting_Down_1();
            return;
        }

        IsOneHand();




        // 착용한 무기에 따라 모션다르게
        if (weaponKind == 0)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Attack_List[1];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Attack_List[1 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 16, -1); // 헤드
            transform.GetChild(3).localPosition = new Vector3(0, 16, -2); // 얼굴
            transform.GetChild(4).localPosition = new Vector3(0, 16, -4); // 머리

            // 한손검일 경우
            transform.GetChild(6).localPosition = new Vector3(-12, 12, 1);


        }
        else if(weaponKind == 1)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Bow_Attack_List[1];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Bow_Attack_List[1 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 15, -1); // 활 헤드
            transform.GetChild(3).localPosition = new Vector3(0, 15, -2); // 활 얼굴
            transform.GetChild(4).localPosition = new Vector3(0, 15, -4); // 활 머리

            // 활인 경우
            transform.GetChild(6).localPosition = new Vector3(-2, -5, -5);

        }
        else if(weaponKind == 2)
        {

            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Attack_List_TwoHand_1[1];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Attack_List_TwoHand_1[1 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 16, -1); // 헤드
            transform.GetChild(3).localPosition = new Vector3(0, 16, -2); // 얼굴
            transform.GetChild(4).localPosition = new Vector3(0, 16, -4); // 머리

            // 두손검일 경우
            transform.GetChild(6).localPosition = new Vector3(-11, 15, 1);
        }

        // 무기이미지
        transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHand_Attack_List[0 + RightHand * 8];

        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = HeadList[0];
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = FaceList[Face * 4];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = HairList[Hair * 4];

        // Body Armor
        transform.GetChild(1).localPosition = new Vector3(0, 0, 0);
        transform.GetChild(5).localPosition = new Vector3(0, 0, -3);


        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet];

            if(weaponKind == 0) // 한손검
            {
                transform.GetChild(7).localPosition = new Vector3(0, 22, -4.5f);
            }
            else if(weaponKind == 1) // 활
            {
                transform.GetChild(7).localPosition = new Vector3(0, 21, -4.5f);
            }
            else if(weaponKind == 2) // 두손검
            {

            }
        }

    }

    public void Attack_Down_2()
    {
        if (attackForm == 1)
        {
            Sting_Down_2();
            return;
        }

        IsOneHand();

        // 착용한 무기에 따라 모션다르게 (화살이 아닌경우)
        if (weaponKind ==0)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Attack_List[2];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Attack_List[2 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 15, -1);
            transform.GetChild(3).localPosition = new Vector3(0, 15, -2);
            transform.GetChild(4).localPosition = new Vector3(0, 15, -4);

            // 한손검일 경우
            transform.GetChild(6).localPosition = new Vector3(-2, -7, -5);
        }
        else if(weaponKind == 1)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Bow_Attack_List[2];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Bow_Attack_List[2 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 15, -1);
            transform.GetChild(3).localPosition = new Vector3(0, 15, -2);
            transform.GetChild(4).localPosition = new Vector3(0, 15, -4);

            // 활인 경우
            transform.GetChild(6).localPosition = new Vector3(-2, -5, -5);
        }
        else if (weaponKind == 2)
        {

            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Attack_List_TwoHand_1[2];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Attack_List_TwoHand_1[2 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 15, -1);
            transform.GetChild(3).localPosition = new Vector3(0, 15, -2);
            transform.GetChild(4).localPosition = new Vector3(0, 15, -4);

            // 두손검일 경우
            transform.GetChild(6).localPosition = new Vector3(7, -11, -5);
        }

        // 무기이미지
        transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHand_Attack_List[1 + RightHand * 8];

        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = HeadList[0];
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = FaceList[Face * 4];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = HairList[Hair * 4];




        transform.GetChild(1).localPosition = new Vector3(0, 0, 0);
        transform.GetChild(5).localPosition = new Vector3(0, 0, -3);


        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet];

            if (weaponKind == 0) // 한손검
            {
                transform.GetChild(7).localPosition = new Vector3(0, 22 - 1, -4.5f);
            }
            else if(weaponKind ==1) // 활
            {
                transform.GetChild(7).localPosition = new Vector3(0, 21 , -4.5f);
            }
        }
    }

    public void Attack_Down_3()
    {
        IsOneHand();

        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = HeadList[0];
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = FaceList[Face * 4];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = HairList[Hair * 4];




        // 마지막 3타 공격 모션 주먹쥔것과 검쥔것
        if (RightHand == -1)
        {

            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Attack_List[0];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Attack_List[0 + (Shirts * 12)];

        }
        else
        {

            if(weaponKind ==0 || weaponKind == 1 || weaponKind == 3)
            {
                transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList_OneHand[0];
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList_OneHand[0 + (Shirts * 12)];
                transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHandList[0 + RightHand * 4];

                transform.GetChild(6).localPosition = new Vector3(-10, -1, -5);
            }
            else if(weaponKind == 2)
            {
                transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList_TwoHand[0];
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList_TwoHand[0 + (Shirts * 12)];
                transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHandList[0 + RightHand * 4];

                transform.GetChild(6).localPosition = new Vector3(-9, 6, -5);
            }


        }


        transform.GetChild(1).localPosition = new Vector3(0, 0, 0);
        transform.GetChild(2).localPosition = new Vector3(0, 16, -1);
        transform.GetChild(3).localPosition = new Vector3(0, 16, -2);
        transform.GetChild(4).localPosition = new Vector3(0, 16, -4);
        transform.GetChild(5).localPosition = new Vector3(0, 0, -3);


        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet];

            if (weaponKind ==0) // 한손검
            {
                transform.GetChild(7).localPosition = new Vector3(0, 22, -4.5f);
            }
            else if(weaponKind ==1) // 활
            {
                transform.GetChild(7).localPosition = new Vector3(0, 22, -4.5f);
            }
            else if(weaponKind == 2) // 두손검
            {

            }
           
        }
    }

    // =============================//


    public void Attack_Left_1()
    {

        if (attackForm == 1)
        {
            Sting_Left_1();
            return;
        }

        IsOneHand();

        // 착용한 무기에 따라 모션다르게
        if (weaponKind ==0)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Attack_List[4];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Attack_List[4 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 15, -1); // 잠깐 위로
            transform.GetChild(3).localPosition = new Vector3(0, 15, -2);
            transform.GetChild(4).localPosition = new Vector3(0, 15, -4);

            // 한손검
            transform.GetChild(6).localPosition = new Vector3(2, 2, 1);

        }
        else if(weaponKind ==1)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Bow_Attack_List[4];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Bow_Attack_List[4 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 15, -1); // 잠깐 위로
            transform.GetChild(3).localPosition = new Vector3(0, 15, -2);
            transform.GetChild(4).localPosition = new Vector3(0, 15, -4);

            // 활인 경우
            transform.GetChild(6).localPosition = new Vector3(-16, 7, 1);
        }
        else if (weaponKind == 2)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Attack_List_TwoHand_1[4];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Attack_List_TwoHand_1[4 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 15, -1); // 잠깐 위로
            transform.GetChild(3).localPosition = new Vector3(0, 15, -2);
            transform.GetChild(4).localPosition = new Vector3(0, 15, -4);

            // 두손검일 경우
            transform.GetChild(6).localPosition = new Vector3(-6, 11, 1);
        }

        // 무기이미지
        transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHand_Attack_List[2 + RightHand * 8];


        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = HeadList[1];
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = FaceList[Face * 4 + 1];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = HairList[Hair * 4 + 1];




        transform.GetChild(1).localPosition = new Vector3(0, 0, 0);
        transform.GetChild(5).localPosition = new Vector3(0, 0, -3);


        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet + 1];

            if (weaponKind == 0) // 한손검
            {
                transform.GetChild(7).localPosition = new Vector3(0, 22 - 1, -4.5f);
            }
            else if(weaponKind ==1) // 활
            {
                transform.GetChild(7).localPosition = new Vector3(0, 21, -4.5f);
            }
            else if(weaponKind ==2)
            {

            }
        }


    }

    public void Attack_Left_2()
    {

        if (attackForm == 1)
        {
            Sting_Left_2();
            return;
        }

        IsOneHand();

        // 착용한 무기에 따라 모션다르게
        if (weaponKind == 0 )
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Attack_List[5];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Attack_List[5 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(-2, 13, -1);
            transform.GetChild(3).localPosition = new Vector3(-2, 13, -2);
            transform.GetChild(4).localPosition = new Vector3(-2, 13, -4);

            // 한손검
            transform.GetChild(6).localPosition = new Vector3(-2, -8, -5);

        }
        else if (weaponKind == 1)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Bow_Attack_List[5];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Bow_Attack_List[5 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 15, -1); // 잠깐 위로
            transform.GetChild(3).localPosition = new Vector3(0, 15, -2);
            transform.GetChild(4).localPosition = new Vector3(0, 15, -4);

            // 활인 경우
            transform.GetChild(6).localPosition = new Vector3(-16, 7, 1);
        }
        else if (weaponKind == 2)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Attack_List_TwoHand_1[5];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Attack_List_TwoHand_1[5 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(-2, 13, -1);
            transform.GetChild(3).localPosition = new Vector3(-2, 13, -2);
            transform.GetChild(4).localPosition = new Vector3(-2, 13, -4);

            // 두손검일 경우
            transform.GetChild(6).localPosition = new Vector3(-4, -12, -5);

        }

        // 무기이미지
        transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHand_Attack_List[3 + RightHand * 8];


        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = HeadList[1];
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = FaceList[Face * 4 + 1];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = HairList[Hair * 4 + 1];


        transform.GetChild(1).localPosition = new Vector3(0, 0, 0);
        transform.GetChild(5).localPosition = new Vector3(0, 0, -3);


        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet + 1];

            if (weaponKind == 0 ) // 한손검
            {
                transform.GetChild(7).localPosition = new Vector3(0 - 2, 22 - 3, -4.5f);
            }
            else if(weaponKind ==1)// 활
            {
                transform.GetChild(7).localPosition = new Vector3(0, 21, -4.5f);
            }
            else if(weaponKind == 2)
            {

            }
        }
    }

    public void Attack_Left_3()
    {
        IsOneHand();

        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = HeadList[1];
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = FaceList[Face * 4 + 1];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = HairList[Hair * 4 + 1];


        transform.GetChild(1).localPosition = new Vector3(0, 0, 0);
        transform.GetChild(2).localPosition = new Vector3(0, 16, -1);
        transform.GetChild(3).localPosition = new Vector3(0, 16, -2);
        transform.GetChild(4).localPosition = new Vector3(0, 16, -4);
        transform.GetChild(5).localPosition = new Vector3(0, 0, -3);

        // 마지막 3타 공격 모션 주먹쥔것과 검쥔것
        if (RightHand == -1)
        {

            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Attack_List[3];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Attack_List[3 + (Shirts * 12)];
        }
        else
        {
            if (weaponKind == 0 || weaponKind == 1 || weaponKind == 3)
            {
                transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList_OneHand[3];
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList_OneHand[3 + (Shirts * 12)];
                transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHandList[1 + RightHand * 4];

                // 활이 아니면
                if (weaponKind != 1)
                    transform.GetChild(6).localPosition = new Vector3(-9, -1, -5);
                else // 활이면
                    transform.GetChild(6).localPosition = new Vector3(-9, -1, 10);
            }
            else if(weaponKind == 2)
            {
                transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList_TwoHand[3];
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList_TwoHand[3 + (Shirts * 12)];
                transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHandList[1 + RightHand * 4];

                transform.GetChild(6).localPosition = new Vector3(-10, 6, -5);
            }


        }



        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet + 1];

            if (weaponKind == 0) // 한손검
            {
                transform.GetChild(7).localPosition = new Vector3(0 - 0, 22 - 0, -4.5f);
            }
            else if(weaponKind ==1) // 활
            {
                transform.GetChild(7).localPosition = new Vector3(0, 22, -4.5f);
            }
            else if(weaponKind == 2)
            {

            }
        }
    }

    // =============================//


    public void Attack_Right_1()
    {

        if (attackForm == 1)
        {
            Sting_Right_1();
            return;
        }

        IsOneHand();

        // 공격 오른쪽 바디만 flip 안되게.
        transform.GetChild(1).GetComponent<SpriteRenderer>().flipX = false;
        transform.GetChild(5).GetComponent<SpriteRenderer>().flipX = false;

        // 착용한 무기에 따라 모션다르게
        if (weaponKind == 0 )
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Attack_List[7];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Attack_List[7 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 15, -1);
            transform.GetChild(3).localPosition = new Vector3(0, 15, -2);
            transform.GetChild(4).localPosition = new Vector3(0, 15, -4);

            // 한손검            
            transform.GetChild(6).localPosition = new Vector3(-6, 0, -5);
        }
        else if(weaponKind == 1)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Bow_Attack_List[7];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Bow_Attack_List[7 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 15, -1);
            transform.GetChild(3).localPosition = new Vector3(0, 15, -2);
            transform.GetChild(4).localPosition = new Vector3(0, 15, -4);

            // 활인 경우
            transform.GetChild(6).localPosition = new Vector3(15, 7, 1);
        }
        else if (weaponKind == 2)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Attack_List_TwoHand_1[7];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Attack_List_TwoHand_1[7 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 15, -1);
            transform.GetChild(3).localPosition = new Vector3(0, 15, -2);
            transform.GetChild(4).localPosition = new Vector3(0, 15, -4);

            // 두손검
            transform.GetChild(6).localPosition = new Vector3(8, 10, -5);
        }

        // 무기이미지
        transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHand_Attack_List[4 + RightHand * 8];


        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = HeadList[2];
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = FaceList[Face * 4 + 2];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = HairList[Hair * 4 + 2];

        transform.GetChild(1).localPosition = new Vector3(0, 0, 0);
        transform.GetChild(5).localPosition = new Vector3(0, 0, -3);


        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet + 2];

            if (weaponKind ==0) // 한손검
            {
                transform.GetChild(7).localPosition = new Vector3(0 - 0, 22 - 1, -4.5f);
            }
            else if(weaponKind ==1) // 활
            {
                transform.GetChild(7).localPosition = new Vector3(0, 21, -4.5f);
            }
            else if (weaponKind == 2)
            {

            }
        }
    }

    public void Attack_Right_2()
    {

        if (attackForm == 1)
        {
            Sting_Right_2();
            return;
        }

        IsOneHand();

        // 공격 오른쪽 바디만 flip 안되게.
        transform.GetChild(1).GetComponent<SpriteRenderer>().flipX = false;
        transform.GetChild(5).GetComponent<SpriteRenderer>().flipX = false;

        // 착용한 무기에 따라 모션다르게
        if (weaponKind ==0)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Attack_List[8];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Attack_List[8 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(2, 13, -1);
            transform.GetChild(3).localPosition = new Vector3(2, 13, -2);
            transform.GetChild(4).localPosition = new Vector3(2, 13, -4);

            // 한손검            
            transform.GetChild(6).localPosition = new Vector3(4, -5, 1);

        }
        else if(weaponKind ==1)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Bow_Attack_List[8];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Bow_Attack_List[8 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 15, -1); // Head
            transform.GetChild(3).localPosition = new Vector3(0, 15, -2); // Face
            transform.GetChild(4).localPosition = new Vector3(0, 15, -4); // Hair

            // 활인 경우
            transform.GetChild(6).localPosition = new Vector3(15, 7, 1);
        }
        else if (weaponKind == 2)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Attack_List_TwoHand_1[8];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Attack_List_TwoHand_1[8 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(2, 13, -1);
            transform.GetChild(3).localPosition = new Vector3(2, 13, -2);
            transform.GetChild(4).localPosition = new Vector3(2, 13, -4);

            // 두손검
            transform.GetChild(6).localPosition = new Vector3(7, -15, 1);
        }

        // 무기 이미지
        transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHand_Attack_List[5 + RightHand * 8];

        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = HeadList[2];
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = FaceList[Face * 4 + 2];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = HairList[Hair * 4 + 2];

        transform.GetChild(1).localPosition = new Vector3(0, 0, 0);

        transform.GetChild(5).localPosition = new Vector3(0, 0, -3);



        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet + 2];

            if (weaponKind ==0) // 한손검
            {
                transform.GetChild(7).localPosition = new Vector3(0 + 2, 22 - 3, -4.5f);
            }
            else if(weaponKind ==1) // 활
            {
                transform.GetChild(7).localPosition = new Vector3(0, 21, -4.5f);
            }
            else if (weaponKind == 2)
            {

            }
        }
    }

    public void Attack_Right_3()
    {
        IsOneHand();

        // 공격 오른쪽 바디만 flip 안되게.
        transform.GetChild(1).GetComponent<SpriteRenderer>().flipX = false;
        transform.GetChild(5).GetComponent<SpriteRenderer>().flipX = false;


        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = HeadList[2];
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = FaceList[Face * 4 + 2];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = HairList[Hair * 4 + 2];



        // 마지막 3타 공격 모션 주먹쥔것과 검쥔것
        if (RightHand == -1)
        {

            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Attack_List[6];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Attack_List[6 + (Shirts * 12)];

        }
        else
        {
            if (weaponKind == 0 || weaponKind == 1 || weaponKind == 3)
            {
                transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList_OneHand[6];
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList_OneHand[6 + (Shirts * 12)];
                transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHandList[2 + RightHand * 4];

                transform.GetChild(6).localPosition = new Vector3(6, -1, -5);
            }
            else if (weaponKind == 2)
            {
                transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList_TwoHand[6];
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList_TwoHand[6 + (Shirts * 12)];
                transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHandList[2 + RightHand * 4];

                transform.GetChild(6).localPosition = new Vector3(10, 6, -5);
            }

        }



        transform.GetChild(1).localPosition = new Vector3(0, 0, 0); // Body
        transform.GetChild(2).localPosition = new Vector3(0, 16, -1); // Head
        transform.GetChild(3).localPosition = new Vector3(0, 16, -2); // Face
        transform.GetChild(4).localPosition = new Vector3(0, 16, -4); // Hair
        transform.GetChild(5).localPosition = new Vector3(0, 0, -3); // Armor



        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet + 2];

            if (weaponKind ==0) // 한손검
            {
                transform.GetChild(7).localPosition = new Vector3(0 - 0, 22 - 0, -4.5f);
            }
            else if(weaponKind == 1) // 활
            {
                transform.GetChild(7).localPosition = new Vector3(0, 22, -4.5f);
            }
            else if (weaponKind == 2)
            {

            }
        }
    }


    // =============================//

    public void Attack_Up_1()
    {
        if (attackForm == 1)
        {
            Sting_Up_1();
            return;
        }

        IsOneHand();

        // 착용한 무기에 따라 모션다르게
        if (weaponKind ==0)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Attack_List[10];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Attack_List[10 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 16, -1);
            transform.GetChild(3).localPosition = new Vector3(0, 16, -2);
            transform.GetChild(4).localPosition = new Vector3(0, 16, -4);

            // 한손검            
            transform.GetChild(6).localPosition = new Vector3(12, 11, -5);

        }
        else if(weaponKind == 1)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Bow_Attack_List[10];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Bow_Attack_List[10 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 16, -1);
            transform.GetChild(3).localPosition = new Vector3(0, 16, -2);
            transform.GetChild(4).localPosition = new Vector3(0, 16, -4);

            // 활인 경우
            transform.GetChild(6).localPosition = new Vector3(1, 17, 1);
        }
        else if (weaponKind == 2)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Attack_List_TwoHand_1[10];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Attack_List_TwoHand_1[10 + (Shirts * 12)];


            transform.GetChild(2).localPosition = new Vector3(0, 16, -1);
            transform.GetChild(3).localPosition = new Vector3(0, 16, -2);
            transform.GetChild(4).localPosition = new Vector3(0, 16, -4);

            // 두손검
            transform.GetChild(6).localPosition = new Vector3(12, 21, -5);
        }

        // 무기 이미지
        transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHand_Attack_List[6 + RightHand * 8];


        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = HeadList[3];
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = FaceList[Face * 4 + 3];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = HairList[Hair * 4 + 3];

        transform.GetChild(1).localPosition = new Vector3(0, 0, 0);
        transform.GetChild(5).localPosition = new Vector3(0, 0, -3);


        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet + 3];

            if (weaponKind == 0) // 한손검
            {
                transform.GetChild(7).localPosition = new Vector3(0 - 0, 22 - 0, -4.5f);
            }
            else if(weaponKind == 1) // 활
            {

            }
            else if (weaponKind == 2)
            {

            }
        }
    }

    public void Attack_Up_2()
    {
        if (attackForm == 1)
        {
            Sting_Up_2();
            return;
        }

        IsOneHand();

        // 착용한 무기에 따라 모션다르게
        if (weaponKind ==0)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Attack_List[11];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Attack_List[11 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 15, -1);
            transform.GetChild(3).localPosition = new Vector3(0, 15, -2);
            transform.GetChild(4).localPosition = new Vector3(0, 15, -4);

            // 한손검            
            transform.GetChild(6).localPosition = new Vector3(4, -3, 1);
        }
        else if(weaponKind == 1)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Bow_Attack_List[11];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Bow_Attack_List[11 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 16, -1);
            transform.GetChild(3).localPosition = new Vector3(0, 16, -2);
            transform.GetChild(4).localPosition = new Vector3(0, 16, -4);

            // 활인 경우
            transform.GetChild(6).localPosition = new Vector3(1, 17, 1);
        }
        else if (weaponKind == 2)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Attack_List_TwoHand_1[11];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Attack_List_TwoHand_1[11 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 15, -1);
            transform.GetChild(3).localPosition = new Vector3(0, 15, -2);
            transform.GetChild(4).localPosition = new Vector3(0, 15, -4);

            // 두손검일 경우
            transform.GetChild(6).localPosition = new Vector3(-11, -9, 1);
        }

        // 무기 이미지
        transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHand_Attack_List[7 + RightHand * 8];

        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = HeadList[3];
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = FaceList[Face * 4 + 3];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = HairList[Hair * 4 + 3];

        transform.GetChild(1).localPosition = new Vector3(0, 0, 0);
        transform.GetChild(5).localPosition = new Vector3(0, 0, -3);


        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet + 3];


            if (weaponKind == 0) // 한손검
            {
                transform.GetChild(7).localPosition = new Vector3(0 - 0, 22 - 1, -4.5f);
            }
            else if (weaponKind == 1) // 활
            {

            }
            else if (weaponKind == 2)
            {

            }
        }
    }

    public void Attack_Up_3()
    {
        IsOneHand();


        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = HeadList[3];
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = FaceList[Face * 4 + 3];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = HairList[Hair * 4 + 3];

 

        transform.GetChild(1).localPosition = new Vector3(0, 0, 0);
        transform.GetChild(2).localPosition = new Vector3(0, 16, -1);
        transform.GetChild(3).localPosition = new Vector3(0, 16, -2);
        transform.GetChild(4).localPosition = new Vector3(0, 16, -4);
        transform.GetChild(5).localPosition = new Vector3(0, 0, -3);


        // 마지막 3타 공격 모션 주먹쥔것과 검쥔것
        if (RightHand == -1)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Attack_List[9];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Attack_List[9 + (Shirts * 12)];
        }
        else
        {

            if (weaponKind == 0 || weaponKind == 1 || weaponKind == 3)
            {
                transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList_OneHand[9];
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList_OneHand[9 + (Shirts * 12)];
                transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHandList[3 + RightHand * 4];

                transform.GetChild(6).localPosition = new Vector3(10, -1, 1);
            }
            else if (weaponKind == 2)
            {
                transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = BodyList_TwoHand[9];
                transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = ArmorList_TwoHand[9 + (Shirts * 12)];
                transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHandList[3 + RightHand * 4];

                transform.GetChild(6).localPosition = new Vector3(9, 7, 1);
            }


        }



        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet + 3];

            if (weaponKind == 0) // 한손검
            {
                transform.GetChild(7).localPosition = new Vector3(0 - 0, 22 - 0, -4.5f);
            }
            else if(weaponKind ==1) // 활
            {

            }
            else if (weaponKind == 2)
            {

            }
        }
    }

  

    // ===============================================================================================//

    
    public void Drop_Down()
    {
        DeleteOneHand();

        transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Drop_List[1];
        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = HeadList[0];
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = FaceList[Face * 4];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = HairList[Hair * 4];
        transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Drop_List[ 1 + ( Shirts*8 ) ];

        transform.GetChild(1).localPosition = new Vector3(0, 0, 0); // Body
        transform.GetChild(2).localPosition = new Vector3(0, 7, -1); // Head
        transform.GetChild(3).localPosition = new Vector3(0, 7, -2); // Face
        transform.GetChild(4).localPosition = new Vector3(0, 7, -4); // Hair
        transform.GetChild(5).localPosition = new Vector3(0, 0, -0.5f); // Armor

        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet];
            transform.GetChild(7).localPosition = new Vector3(0 - 0, 22 - 9, -4.5f);


        }

    }

    public void Drop_Left()
    {
        DeleteOneHand();

        transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Drop_List[3];
        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = HeadList[1];
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = FaceList[Face * 4 + 1];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = HairList[Hair * 4 + 1];
        transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Drop_List[3 + (Shirts * 8)];

        transform.GetChild(1).localPosition = new Vector3(0, 0, 0); // Body
        transform.GetChild(2).localPosition = new Vector3(0, 7, -1); // Head // 잠깐 위로
        transform.GetChild(3).localPosition = new Vector3(0, 7, -2); // Face
        transform.GetChild(4).localPosition = new Vector3(0, 7, -4); // Hair
        transform.GetChild(5).localPosition = new Vector3(0, 0, -0.5f); // Armor

        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet + 1];
            transform.GetChild(7).localPosition = new Vector3(0 - 0, 22 - 9, -4.5f);
        }
    }



    public void Drop_Right()
    {
        DeleteOneHand();

        transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Drop_List[5];
        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = HeadList[2];
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = FaceList[Face * 4 + 2];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = HairList[Hair * 4 + 2];
        transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Drop_List[5 + (Shirts * 8)];

        transform.GetChild(1).localPosition = new Vector3(0, 0, 0); // Body
        transform.GetChild(2).localPosition = new Vector3(0, 7, -1); // Head // 잠깐 위로
        transform.GetChild(3).localPosition = new Vector3(0, 7, -2); // Face
        transform.GetChild(4).localPosition = new Vector3(0, 7, -4); // Hair
        transform.GetChild(5).localPosition = new Vector3(0, 0, -0.5f); // Armor

        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet + 2];
            transform.GetChild(7).localPosition = new Vector3(0 - 0, 22 - 9, -4.5f);
        }
    }


    public void Drop_Up()
    {
        DeleteOneHand();

        transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Drop_List[7];
        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = HeadList[3];
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = FaceList[Face * 4 + 3];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = HairList[Hair * 4 + 3];
        transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Drop_List[7 + (Shirts * 8)];

        transform.GetChild(1).localPosition = new Vector3(0, 0, 0); // Body
        transform.GetChild(2).localPosition = new Vector3(0, 6, -1); // Head 
        transform.GetChild(3).localPosition = new Vector3(0, 6, -2); // Face
        transform.GetChild(4).localPosition = new Vector3(0, 6, -4); // Hair
        transform.GetChild(5).localPosition = new Vector3(0, 0, -3); // Armor

        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet + 3];
            transform.GetChild(7).localPosition = new Vector3(0 - 0, 22 - 10, -4.5f);
        }
    }
    // ===============================================================================================//



    public void Spell_Down()
    {
        DeleteOneHand();

        transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Spell_List[1];
        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = HeadList[0];
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = FaceList[Face * 4];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = HairList[Hair * 4];
        transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Spell_List[1 + (Shirts * 8)];

        transform.GetChild(1).localPosition = new Vector3(0, 0, 0); // Body
        transform.GetChild(2).localPosition = new Vector3(0, 16, -1); // Head 
        transform.GetChild(3).localPosition = new Vector3(0, 16, -2); // Face
        transform.GetChild(4).localPosition = new Vector3(0, 16, -4); // Hair
        transform.GetChild(5).localPosition = new Vector3(0, 0, -3); // Armor

        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet];
            transform.GetChild(7).localPosition = new Vector3(0 - 0, 22 - 0, -4.5f);
        }
    }

    public void Spell_Left()
    {
        DeleteOneHand();

        transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Spell_List[3];
        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = HeadList[1];
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = FaceList[Face * 4 + 1];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = HairList[Hair * 4 + 1];
        transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Spell_List[3 + (Shirts * 8)];

        transform.GetChild(1).localPosition = new Vector3(0, 0, 0); // Body
        transform.GetChild(2).localPosition = new Vector3(0, 16, -1); // Head  // 잠깐 위로
        transform.GetChild(3).localPosition = new Vector3(0, 16, -2); // Face
        transform.GetChild(4).localPosition = new Vector3(0, 16, -4); // Hair
        transform.GetChild(5).localPosition = new Vector3(0, 0, -3); // Armor

        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet + 1];
            transform.GetChild(7).localPosition = new Vector3(0 - 0, 22 - 0, -4.5f);
        }
    }

    public void Spell_Right()
    {
        DeleteOneHand();

        transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Spell_List[5];
        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = HeadList[2];
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = FaceList[Face * 4 + 2];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = HairList[Hair * 4 + 2];
        transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Spell_List[5 + (Shirts * 8)];

        transform.GetChild(1).localPosition = new Vector3(0, 0, 0); // Body
        transform.GetChild(2).localPosition = new Vector3(0, 16, -1); // Head  // 잠깐 위로
        transform.GetChild(3).localPosition = new Vector3(0, 16, -2); // Face
        transform.GetChild(4).localPosition = new Vector3(0, 16, -4); // Hair
        transform.GetChild(5).localPosition = new Vector3(0, 0, -3); // Armor

        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet + 2];
            transform.GetChild(7).localPosition = new Vector3(0 - 0, 22 - 0, -4.5f);
        }
    }

    public void Spell_Up()
    {
        DeleteOneHand();

        transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Spell_List[7];
        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = HeadList[3];
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = FaceList[Face * 4 + 3];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = HairList[Hair * 4 + 3];
        transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Spell_List[7 + (Shirts * 8)];

        transform.GetChild(1).localPosition = new Vector3(0, 0, 0); // Body
        transform.GetChild(2).localPosition = new Vector3(0, 16, -1); // Head
        transform.GetChild(3).localPosition = new Vector3(0, 16, -2); // Face
        transform.GetChild(4).localPosition = new Vector3(0, 16, -4); // Hair
        transform.GetChild(5).localPosition = new Vector3(0, 0, -3); // Armor

        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet + 3];
            transform.GetChild(7).localPosition = new Vector3(0 - 0, 22 - 0, -4.5f);
        }
    }
    // ===============================================================================================//




    // ===================================찌르기 중간 1-2 ========================================//


    public void Sting_Down_1()
    {
        IsOneHand();




        // 착용한 무기에 따라 모션다르게
        if (weaponKind == 0)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Attack_List[1];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Attack_List[1 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 16, -1); // 헤드
            transform.GetChild(3).localPosition = new Vector3(0, 16, -2); // 얼굴
            transform.GetChild(4).localPosition = new Vector3(0, 16, -4); // 머리

            // 한손검일 경우
            transform.GetChild(6).localPosition = new Vector3(-12, 12, 1);


        }
        else if (weaponKind == 1)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Bow_Attack_List[1];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Bow_Attack_List[1 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 15, -1); // 활 헤드
            transform.GetChild(3).localPosition = new Vector3(0, 15, -2); // 활 얼굴
            transform.GetChild(4).localPosition = new Vector3(0, 15, -4); // 활 머리

            // 활인 경우
            transform.GetChild(6).localPosition = new Vector3(-2, -5, -5);

        }
        else if (weaponKind == 2 || weaponKind ==3 )
        {

            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Attack_List_TwoHand_2[1];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Attack_List_TwoHand_2[1 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 15, -1); // 헤드
            transform.GetChild(3).localPosition = new Vector3(0, 15, -2); // 얼굴
            transform.GetChild(4).localPosition = new Vector3(0, 15, -4); // 머리

            // 두손검일 경우
            transform.GetChild(6).localPosition = new Vector3(-8, 2, 1);
        }

        // 무기이미지
        transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHand_Sting_List[0 + RightHand * 8];

        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = HeadList[0];
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = FaceList[Face * 4];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = HairList[Hair * 4];

        // Body Armor
        transform.GetChild(1).localPosition = new Vector3(0, 0, 0);
        transform.GetChild(5).localPosition = new Vector3(0, 0, -3);


        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet];

            if (weaponKind == 0) // 한손검
            {
                transform.GetChild(7).localPosition = new Vector3(0, 22, -4.5f);
            }
            else if (weaponKind == 1) // 활
            {
                transform.GetChild(7).localPosition = new Vector3(0, 21, -4.5f);
            }
            else if (weaponKind == 2) // 두손검
            {

            }
        }

    }

    public void Sting_Down_2()
    {
        IsOneHand();

        // 착용한 무기에 따라 모션다르게 (화살이 아닌경우)
        if (weaponKind == 0)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Attack_List[2];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Attack_List[2 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 15, -1);
            transform.GetChild(3).localPosition = new Vector3(0, 15, -2);
            transform.GetChild(4).localPosition = new Vector3(0, 15, -4);

            // 한손검일 경우
            transform.GetChild(6).localPosition = new Vector3(-2, -7, -5);
        }
        else if (weaponKind == 1)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Bow_Attack_List[2];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Bow_Attack_List[2 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 15, -1);
            transform.GetChild(3).localPosition = new Vector3(0, 15, -2);
            transform.GetChild(4).localPosition = new Vector3(0, 15, -4);

            // 활인 경우
            transform.GetChild(6).localPosition = new Vector3(-2, -5, -5);
        }
        else if (weaponKind == 2 || weaponKind == 3)
        {

            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Attack_List_TwoHand_2[2];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Attack_List_TwoHand_2[2 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 13, -1);
            transform.GetChild(3).localPosition = new Vector3(0, 13, -2);
            transform.GetChild(4).localPosition = new Vector3(0, 13, -4);

            // 두손검일 경우
            transform.GetChild(6).localPosition = new Vector3(-6, -10, -5);
        }

        // 무기이미지
        transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHand_Sting_List[1 + RightHand * 8];

        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = HeadList[0];
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = FaceList[Face * 4];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = HairList[Hair * 4];




        transform.GetChild(1).localPosition = new Vector3(0, 0, 0);
        transform.GetChild(5).localPosition = new Vector3(0, 0, -3);


        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet];

            if (weaponKind == 0) // 한손검
            {
                transform.GetChild(7).localPosition = new Vector3(0, 22 - 1, -4.5f);
            }
            else if (weaponKind == 1) // 활
            {
                transform.GetChild(7).localPosition = new Vector3(0, 21, -4.5f);
            }
        }
    }

    // =============================//

    public void Sting_Left_1()
    {
        IsOneHand();

        // 착용한 무기에 따라 모션다르게
        if (weaponKind == 0)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Attack_List[4];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Attack_List[4 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 15, -1); // 잠깐 위로
            transform.GetChild(3).localPosition = new Vector3(0, 15, -2);
            transform.GetChild(4).localPosition = new Vector3(0, 15, -4);

            // 한손검
            transform.GetChild(6).localPosition = new Vector3(2, 2, 1);

        }
        else if (weaponKind == 1)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Bow_Attack_List[4];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Bow_Attack_List[4 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 15, -1); // 잠깐 위로
            transform.GetChild(3).localPosition = new Vector3(0, 15, -2);
            transform.GetChild(4).localPosition = new Vector3(0, 15, -4);

            // 활인 경우
            transform.GetChild(6).localPosition = new Vector3(-16, 7, 1);
        }
        else if (weaponKind == 2 || weaponKind == 3)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Attack_List_TwoHand_2[4];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Attack_List_TwoHand_2[4 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 15, -1); // 잠깐 위로
            transform.GetChild(3).localPosition = new Vector3(0, 15, -2);
            transform.GetChild(4).localPosition = new Vector3(0, 15, -4);

            // 두손검일 경우
            transform.GetChild(6).localPosition = new Vector3(6, 1, 1);
        }

        // 무기이미지
        transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHand_Sting_List[2 + RightHand * 8];


        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = HeadList[1];
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = FaceList[Face * 4 + 1];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = HairList[Hair * 4 + 1];




        transform.GetChild(1).localPosition = new Vector3(0, 0, 0);
        transform.GetChild(5).localPosition = new Vector3(0, 0, -3);


        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet + 1];

            if (weaponKind == 0) // 한손검
            {
                transform.GetChild(7).localPosition = new Vector3(0, 22 - 1, -4.5f);
            }
            else if (weaponKind == 1) // 활
            {
                transform.GetChild(7).localPosition = new Vector3(0, 21, -4.5f);
            }
            else if (weaponKind == 2)
            {

            }
        }


    }

    public void Sting_Left_2()
    {
        IsOneHand();

        // 착용한 무기에 따라 모션다르게
        if (weaponKind == 0)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Attack_List[5];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Attack_List[5 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(-2, 13, -1);
            transform.GetChild(3).localPosition = new Vector3(-2, 13, -2);
            transform.GetChild(4).localPosition = new Vector3(-2, 13, -4);

            // 한손검
            transform.GetChild(6).localPosition = new Vector3(-2, -8, -5);

        }
        else if (weaponKind == 1)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Bow_Attack_List[5];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Bow_Attack_List[5 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 15, -1); // 잠깐 위로
            transform.GetChild(3).localPosition = new Vector3(0, 15, -2);
            transform.GetChild(4).localPosition = new Vector3(0, 15, -4);

            // 활인 경우
            transform.GetChild(6).localPosition = new Vector3(-16, 7, 1);
        }
        else if (weaponKind == 2 || weaponKind == 3)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Attack_List_TwoHand_2[5];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Attack_List_TwoHand_2[5 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(-2, 13, -1);
            transform.GetChild(3).localPosition = new Vector3(-2, 13, -2);
            transform.GetChild(4).localPosition = new Vector3(-2, 13, -4);

            // 두손검일 경우
            transform.GetChild(6).localPosition = new Vector3(-16, 4, -5);

        }

        // 무기이미지
        transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHand_Sting_List[3 + RightHand * 8];


        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = HeadList[1];
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = FaceList[Face * 4 + 1];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = HairList[Hair * 4 + 1];


        transform.GetChild(1).localPosition = new Vector3(0, 0, 0);
        transform.GetChild(5).localPosition = new Vector3(0, 0, -3);


        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet + 1];

            if (weaponKind == 0) // 한손검
            {
                transform.GetChild(7).localPosition = new Vector3(0 - 2, 22 - 3, -4.5f);
            }
            else if (weaponKind == 1)// 활
            {
                transform.GetChild(7).localPosition = new Vector3(0, 21, -4.5f);
            }
            else if (weaponKind == 2)
            {

            }
        }
    }


    // =============================//


    public void Sting_Right_1()
    {
        IsOneHand();

        // 공격 오른쪽 바디만 flip 안되게.
        transform.GetChild(1).GetComponent<SpriteRenderer>().flipX = false;
        transform.GetChild(5).GetComponent<SpriteRenderer>().flipX = false;

        // 착용한 무기에 따라 모션다르게
        if (weaponKind == 0)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Attack_List[7];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Attack_List[7 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 15, -1);
            transform.GetChild(3).localPosition = new Vector3(0, 15, -2);
            transform.GetChild(4).localPosition = new Vector3(0, 15, -4);

            // 한손검            
            transform.GetChild(6).localPosition = new Vector3(-6, 0, -5);
        }
        else if (weaponKind == 1)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Bow_Attack_List[7];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Bow_Attack_List[7 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 15, -1);
            transform.GetChild(3).localPosition = new Vector3(0, 15, -2);
            transform.GetChild(4).localPosition = new Vector3(0, 15, -4);

            // 활인 경우
            transform.GetChild(6).localPosition = new Vector3(15, 7, 1);
        }
        else if (weaponKind == 2 || weaponKind == 3)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Attack_List_TwoHand_2[7];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Attack_List_TwoHand_2[7 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 15, -1);
            transform.GetChild(3).localPosition = new Vector3(0, 15, -2);
            transform.GetChild(4).localPosition = new Vector3(0, 15, -4);

            // 두손검
            transform.GetChild(6).localPosition = new Vector3(-6, 1, -5);
        }

        // 무기이미지
        transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHand_Sting_List[4 + RightHand * 8];


        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = HeadList[2];
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = FaceList[Face * 4 + 2];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = HairList[Hair * 4 + 2];

        transform.GetChild(1).localPosition = new Vector3(0, 0, 0);
        transform.GetChild(5).localPosition = new Vector3(0, 0, -3);


        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet + 2];

            if (weaponKind == 0) // 한손검
            {
                transform.GetChild(7).localPosition = new Vector3(0 - 0, 22 - 1, -4.5f);
            }
            else if (weaponKind == 1) // 활
            {
                transform.GetChild(7).localPosition = new Vector3(0, 21, -4.5f);
            }
            else if (weaponKind == 2)
            {

            }
        }
    }

    public void Sting_Right_2()
    {
        IsOneHand();

        // 공격 오른쪽 바디만 flip 안되게.
        transform.GetChild(1).GetComponent<SpriteRenderer>().flipX = false;
        transform.GetChild(5).GetComponent<SpriteRenderer>().flipX = false;

        // 착용한 무기에 따라 모션다르게
        if (weaponKind == 0)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Attack_List[8];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Attack_List[8 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(2, 13, -1);
            transform.GetChild(3).localPosition = new Vector3(2, 13, -2);
            transform.GetChild(4).localPosition = new Vector3(2, 13, -4);

            // 한손검            
            transform.GetChild(6).localPosition = new Vector3(4, -5, 1);

        }
        else if (weaponKind == 1)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Bow_Attack_List[8];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Bow_Attack_List[8 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 15, -1); // Head
            transform.GetChild(3).localPosition = new Vector3(0, 15, -2); // Face
            transform.GetChild(4).localPosition = new Vector3(0, 15, -4); // Hair

            // 활인 경우
            transform.GetChild(6).localPosition = new Vector3(15, 7, 1);
        }
        else if (weaponKind == 2 || weaponKind == 3)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Attack_List_TwoHand_2[8];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Attack_List_TwoHand_2[8 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(2, 13, -1);
            transform.GetChild(3).localPosition = new Vector3(2, 13, -2);
            transform.GetChild(4).localPosition = new Vector3(2, 13, -4);

            // 두손검
            transform.GetChild(6).localPosition = new Vector3(16, 4, 1);
        }

        // 무기 이미지
        transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHand_Sting_List[5 + RightHand * 8];

        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = HeadList[2];
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = FaceList[Face * 4 + 2];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = HairList[Hair * 4 + 2];

        transform.GetChild(1).localPosition = new Vector3(0, 0, 0);

        transform.GetChild(5).localPosition = new Vector3(0, 0, -3);



        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet + 2];

            if (weaponKind == 0) // 한손검
            {
                transform.GetChild(7).localPosition = new Vector3(0 + 2, 22 - 3, -4.5f);
            }
            else if (weaponKind == 1) // 활
            {
                transform.GetChild(7).localPosition = new Vector3(0, 21, -4.5f);
            }
            else if (weaponKind == 2)
            {

            }
        }
    }

    // =============================//

    public void Sting_Up_1()
    {
        IsOneHand();

        // 착용한 무기에 따라 모션다르게
        if (weaponKind == 0)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Attack_List[10];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Attack_List[10 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 16, -1);
            transform.GetChild(3).localPosition = new Vector3(0, 16, -2);
            transform.GetChild(4).localPosition = new Vector3(0, 16, -4);

            // 한손검            
            transform.GetChild(6).localPosition = new Vector3(12, 11, -5);

        }
        else if (weaponKind == 1)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Bow_Attack_List[10];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Bow_Attack_List[10 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 16, -1);
            transform.GetChild(3).localPosition = new Vector3(0, 16, -2);
            transform.GetChild(4).localPosition = new Vector3(0, 16, -4);

            // 활인 경우
            transform.GetChild(6).localPosition = new Vector3(1, 17, 1);
        }
        else if (weaponKind == 2 || weaponKind == 3)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Attack_List_TwoHand_2[10];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Attack_List_TwoHand_2[10 + (Shirts * 12)];


            transform.GetChild(2).localPosition = new Vector3(0, 14, -1);
            transform.GetChild(3).localPosition = new Vector3(0, 14, -2);
            transform.GetChild(4).localPosition = new Vector3(0, 14, -4);

            // 두손검
            transform.GetChild(6).localPosition = new Vector3(7, -4, -5);
        }

        // 무기 이미지
        transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHand_Sting_List[6 + RightHand * 8];


        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = HeadList[3];
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = FaceList[Face * 4 + 3];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = HairList[Hair * 4 + 3];

        transform.GetChild(1).localPosition = new Vector3(0, 0, 0);
        transform.GetChild(5).localPosition = new Vector3(0, 0, -3);


        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet + 3];

            if (weaponKind == 0) // 한손검
            {
                transform.GetChild(7).localPosition = new Vector3(0 - 0, 22 - 0, -4.5f);
            }
            else if (weaponKind == 1) // 활
            {

            }
            else if (weaponKind == 2)
            {

            }
        }
    }

    public void Sting_Up_2()
    {
        IsOneHand();

        // 착용한 무기에 따라 모션다르게
        if (weaponKind == 0)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Attack_List[11];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Attack_List[11 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 15, -1);
            transform.GetChild(3).localPosition = new Vector3(0, 15, -2);
            transform.GetChild(4).localPosition = new Vector3(0, 15, -4);

            // 한손검            
            transform.GetChild(6).localPosition = new Vector3(4, -3, 1);
        }
        else if (weaponKind == 1)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Bow_Attack_List[11];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Bow_Attack_List[11 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 16, -1);
            transform.GetChild(3).localPosition = new Vector3(0, 16, -2);
            transform.GetChild(4).localPosition = new Vector3(0, 16, -4);

            // 활인 경우
            transform.GetChild(6).localPosition = new Vector3(1, 17, 1);
        }
        else if (weaponKind == 2 || weaponKind == 3)
        {
            transform.GetChild(1).GetComponent<SpriteRenderer>().sprite = Body_Attack_List_TwoHand_2[11];
            transform.GetChild(5).GetComponent<SpriteRenderer>().sprite = Armor_Attack_List_TwoHand_2[11 + (Shirts * 12)];

            transform.GetChild(2).localPosition = new Vector3(0, 16, -1);
            transform.GetChild(3).localPosition = new Vector3(0, 16, -2);
            transform.GetChild(4).localPosition = new Vector3(0, 16, -4);

            // 두손검일 경우
            transform.GetChild(6).localPosition = new Vector3(7, 7, 1);
        }

        // 무기 이미지
        transform.GetChild(6).GetComponent<SpriteRenderer>().sprite = RightHand_Sting_List[7 + RightHand * 8];

        transform.GetChild(2).GetComponent<SpriteRenderer>().sprite = HeadList[3];
        transform.GetChild(3).GetComponent<SpriteRenderer>().sprite = FaceList[Face * 4 + 3];
        transform.GetChild(4).GetComponent<SpriteRenderer>().sprite = HairList[Hair * 4 + 3];

        transform.GetChild(1).localPosition = new Vector3(0, 0, 0);
        transform.GetChild(5).localPosition = new Vector3(0, 0, -3);


        if (Helmet != -1)
        {
            transform.GetChild(7).GetComponent<SpriteRenderer>().sprite = HelmetList[Helmet + 3];


            if (weaponKind == 0) // 한손검
            {
                transform.GetChild(7).localPosition = new Vector3(0 - 0, 22 - 1, -4.5f);
            }
            else if (weaponKind == 1) // 활
            {

            }
            else if (weaponKind == 2)
            {

            }
        }
    }

}
