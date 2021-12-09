﻿using ProjectChan.Object;
using ProjectChan.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectChan.UI
{
    public class UIBattle : UIWindow
    {
        public PlayerController playerController;       // -> PlayerController
        private GameObject itemHolder;                  // -> 아이템들이 담겨있는 오브젝트
        public BubbleGauge hpBubbleGauge;               // -> Hp 게이지 컴포넌트
        public BubbleGauge energyBubbleGauge;           // -> Energy 게이지 컴포넌트

        public Texture2D normalCursor;                  // -> 기본 마우스 커서 이미지
        public Texture2D targetPointCursor;             // -> 몬스터 타겟팅 커서 이미지

        public Canvas worldCanvas;                      // -> 월드 컨버스 객체

        private List<MonHpBar> allMonHpBar = new List<MonHpBar>();  

        public override void Start()
        {
            base.Start();
            itemHolder = GameObject.Find("ItemHolder");
        }

        private void Update()
        {
            PlayerCursorUpdate();
            BillboardUpdate();
            BubbleGaugeUpdate();
            MonHpBarUpdate();
        }

        /// <summary>
        /// => 플레이어의 커서를 상황에 따라 바꿔주는 메서드
        /// </summary>
        private void PlayerCursorUpdate()
        {
            Cursor.SetCursor
                (playerController.HasPointTarget ? targetPointCursor : normalCursor, Vector2.zero, CursorMode.Auto);
        }

        /// <summary>
        /// => 아이템 홀더에 존재하는 자식객체들이 카메라를 바라보게 하는 메서드
        /// </summary>
        public void BillboardUpdate()
        {
            if (itemHolder == null)
            {
                return;
            }

            var camTrans = CameraController.Cam.transform;

            for (int i = 0; i < itemHolder.transform.childCount; i++)
            {
                var child = itemHolder.transform.GetChild(i);

                /// => LookAt : 지정된 오브젝트들이 파라미터로 들어간 Target을 바라보게 해줌
                child.LookAt(camTrans, Vector3.up);

                /// => 표류하지 않거나 의도하지 않은 회전을 일으킬 수 있기때문에 직접적으로 바꾸지 않음 
                var newRot = child.eulerAngles;
                newRot.x = 0;
                newRot.y = 0;

                child.eulerAngles = newRot;
            }
        }

        /// <summary>
        /// => 플레이어의 Hp, Mana 버블 게이지를 관리하는 메서드
        /// </summary>
        private void BubbleGaugeUpdate()
        {
            var actor = playerController.PlayerCharacter?.boActor;

            // -> 액터가 없다면
            if (actor == null)
            { 
                return;
            }

            var currentHp = actor.currentHp / actor.maxHp;
            var currentEnergy = actor.currentEnergy / actor.maxEnergy;

            hpBubbleGauge.SetGauge(currentHp);
            energyBubbleGauge.SetGauge(currentEnergy);
        }

        /// <summary>
        /// => 몬스터에게 달아줄 체력바를 생성하는 메서드
        ///    배틀매니저에 몬스터가 등록될때 생성해준다
        /// </summary>
        /// <param name="target"> 체력바를 달아줄 몬스터 </param>
        public void AddMonHpBar(Actor target)
        {
            var monHpBar = ObjectPoolManager.Instance.GetPool<MonHpBar>(Define.PoolType.MonHpBar).GetPoolableObject();
            monHpBar.transform.SetParent(worldCanvas.transform);
            monHpBar.Initialize(target);
            monHpBar.gameObject.SetActive(true);

            allMonHpBar.Add(monHpBar);
        }

        /// <summary>
        /// => 몬스터 체력바를 관리하는 메서드
        /// </summary>
        public void MonHpBarUpdate()
        {
            for (int i = 0; i < allMonHpBar.Count; i++)
            {
                allMonHpBar[i].MonHpBarUpdate();
            }
        }

        /// <summary>
        /// => 스테이지 전환 시, 현재 스테이지에 있는 모든 체력바 객체를 풀에 반환하는 메서드
        /// </summary>
        public void Clear()
        {
            // -> 몬스터 체력바 풀을 가져온다
            var monHpBarPool = ObjectPoolManager.Instance.GetPool<MonHpBar>(Define.PoolType.MonHpBar);

            // -> allMonHpBar에 저장된 풀들을 다 반환
            for (int i = 0; i < allMonHpBar.Count; i++)
            {
                monHpBarPool.ReturnPoolableObject(allMonHpBar[i]);
            }

            allMonHpBar.Clear();
        }
    }
}