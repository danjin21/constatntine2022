using Google.Protobuf.Protocol;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Stat_Item : UI_Base
{
    [SerializeField]
    Image _icon = null;



    public int ItemDbId { get; private set; }

    public float m_DoubleClickSecond = 0.1f;
    private bool m_IsOneClick = false;
    private double m_Timer = 0;

    //  한박자 느리게 해야 아이템 먹어지는게 초기화된다.
    public void Update()
    {
        //  한번만 클릭했을 때 KeySetting
        //  0.05 초 이상되면 원상태로
        if (m_IsOneClick && ((Time.time - m_Timer) > m_DoubleClickSecond))
        {
            m_IsOneClick = false;
        }

    }


    public override void Init()
    {
        m_DoubleClickSecond = 0.5f;

        _icon.gameObject.BindEvent((e) =>
        {
            //Debug.Log("마우스 해당 아이템에 들어왔따.");


        }, Define.UIEvent.Enter);

        _icon.gameObject.BindEvent((e) =>
        {
            //Debug.Log("마우스 해당 아이템으로부터 나갔다.");


        }, Define.UIEvent.Exit);



        _icon.gameObject.BindEvent((e) =>
        {
            // 왼쪽 클릭이 아니면 리턴
            if (e.button == PointerEventData.InputButton.Left)
            {

                if (!m_IsOneClick)
                {
                    // 더블클릭 전 한번
                    m_Timer = Time.time;
                    m_IsOneClick = true;
                }
                else if (m_IsOneClick && ((Time.time - m_Timer) < m_DoubleClickSecond))
                {
                    // 더블클릭 되는 순간
                    m_IsOneClick = false;

                    // 아이템 해제 패킷 보내기

                    C_EquipItem equipPacket = new C_EquipItem();
                    equipPacket.ItemDbId = ItemDbId;
                    equipPacket.Equipped = false;

                    Managers.Network.Send(equipPacket);

                }


            }

        }, Define.UIEvent.Click);
    }





    public void SetItem(Item item)
    {

        if (item == null)
        {
            ItemDbId = -1;
        }
        else
        {
            ItemDbId = item.ItemDbId;
        }

    }














}
