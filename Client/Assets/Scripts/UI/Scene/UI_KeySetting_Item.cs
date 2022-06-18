using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UI_KeySetting_Item : UI_Base
{

    [SerializeField]
    Image _icon = null;

    [SerializeField]
    Text _count = null;

    [SerializeField]
    public Text _name = null;

    public int TemplateId { get; private set; }
    public int Count { get; private set; }
    public string Name;
    public int KeyCode { get; set; }

    [SerializeField]
    Image _backgroundImage = null;

    public GameObject ItemInfoPopup;

    public override void Init()
    {



        _backgroundImage = transform.GetChild(0).GetComponent<Image>();

        _backgroundImage.gameObject.BindEvent((e) =>
        {
            // 왼쪽 클릭이 아니면 리턴
            if (e.button == PointerEventData.InputButton.Left)
            {
                
                // 다른 단축키도 안누르고 얘먼저 누른경우
                if (Managers.KeySetting.ChangeKey == -1)
                {
                    // 얘에 아무것도 없는거면 리턴
                    if (TemplateId == 0)
                        return;

                    // 뭐 있는거면 이제 단축키 시전을 한다.
                    Managers.KeySetting.ChangeKey = TemplateId;

                    // 커서 이미지
                    CursorManager obj = FindObjectOfType((typeof(CursorManager))) as CursorManager;
                    obj.transform.GetChild(0).gameObject.SetActive(true);
                    obj.transform.GetChild(0).GetComponent<Image>().sprite = _icon.sprite;
                }

                // 다른 단축키를 누르고 나서 얘 누른경우
                else
                {
                    // 다른 단축키가 있었던 액션을 갖고온다. = ChangeKey
                    
                    // 얘가 있던 KeyCode = KeyCode

                    // 서버 전송
                    C_KeySetting keysettingPacket = new C_KeySetting();
                    keysettingPacket.Key = KeyCode;
                    keysettingPacket.Action = Managers.KeySetting.ChangeKey;

                    Managers.Network.Send(keysettingPacket);

                    // 초기화
                    Managers.KeySetting.ChangeKey = -1;
                    Managers.KeySetting.ChangeItemDbId = -1;

                    // 이미지도 초기화
                    CursorManager obj = FindObjectOfType((typeof(CursorManager))) as CursorManager;
                    obj.transform.GetChild(0).gameObject.SetActive(false);

                }


            }

        }, Define.UIEvent.Click);




        _backgroundImage.gameObject.BindEvent((e) =>
        {

            if (ItemInfoPopup == null && TemplateId != 0)
            {
                GameObject go = Managers.Resource.Instantiate("UI/Popup/UI_ItemInfoPopup");
                ItemInfoPopup = go;

                // 아이템 정보는 Inventory 에서 갖고오라고 하고, 여기는 그냥 Slot만 넘겨준다.
                //ItemInfoPopup.GetComponent<UI_ItemInfoPopup>().SetItemInfo(Slot);

                ItemInfoPopup.GetComponent<UI_ItemInfoPopup>().SetItemInfo_Skill_Consume(TemplateId);

                ItemInfoPopup.transform.GetChild(0).transform.GetComponent<RectTransform>().pivot = new Vector2(1, 0);
                ItemInfoPopup.transform.GetChild(0).transform.position = this.transform.position;

                //Debug.Log(" 1) 설명창의 위치 :" + ItemInfoPopup.transform.GetChild(0).transform.position);

                //Debug.Log(" 2) 아이템창의 위치 :" + this.transform.position);
            }

            // 현재 커서 여기있다고 말해주기.
            Managers.KeySetting.CurrentCursor = 2;

        }, Define.UIEvent.Enter);


        _backgroundImage.gameObject.BindEvent((e) =>
        {

            //Debug.Log("마우스 해당 아이템으로부터 나갔다.");

            GameObject.Destroy(ItemInfoPopup);
            ItemInfoPopup = null;

            // 현재 커서 나갔다고 말해주기.
            Managers.KeySetting.CurrentCursor = -1;

        }, Define.UIEvent.Exit);



 

    }


    public void SetItem(Key key)
    {

        if (key == null)
        {

            TemplateId = 0;
            Count = 0;
       

            _icon.gameObject.SetActive(false);
            _count.gameObject.SetActive(false);
          

        }
        else
        {

            Item item = Managers.Inven.Get_template(key.Action);

            if (item == null)
            {
                // 스킬이다
                if(key.Action >= 1000000)
                {

                    Sprite skillIcon = Managers.Resource.Load<Sprite>("Textures/Skill/Icon/"+key.Action+"/1");
                    _icon.sprite = skillIcon;

                    _icon.gameObject.SetActive(true);
                    _coKeySettingCooltime = StartCoroutine("CoInputCooltime_KeySetting", 0.5f); // 0.2f

                    TemplateId = key.Action;
                }
                return;
            }

            // 아이템이다

            TemplateId = item.TemplateId;
            Count = item.Count;

            Data.ItemData itemData = null;
            Managers.Data.ItemDict.TryGetValue(TemplateId, out itemData);

            // 없으면 크래쉬

            Sprite icon = Managers.Resource.Load<Sprite>(itemData.iconPath);
            _icon.sprite = icon;

            _icon.gameObject.SetActive(true);

            // 200 이상이 포션임.
            if (TemplateId >= 200)
            {
                _count.gameObject.SetActive(true);
                _count.text = Count.ToString();


            }

            // 2초 동안은 딜레이
            _coKeySettingCooltime = StartCoroutine("CoInputCooltime_KeySetting", 0.5f); // 0.2f


      


        }

    }

    // 1초 동안은 딜레이
    public Coroutine _coKeySettingCooltime = null;

    // 1초 동안은 딜레이
    IEnumerator CoInputCooltime_KeySetting(float time)
    {
        yield return new WaitForSeconds(time);
        _coKeySettingCooltime = null;
    }




}
