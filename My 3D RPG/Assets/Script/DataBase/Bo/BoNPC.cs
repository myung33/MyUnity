﻿using ProjectChan.Object;
using ProjectChan.SD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.DB
{
    /// <summary>
    /// => 클라이언트 내에서 사용할 NPC 데이터
    /// => 작업과정에서 데이터를 확인하기 위해서 Serializable
    /// </summary>
    [Serializable]
    public class BoNPC
    {
        public SDNPC sdNPC;             // -> NPC 기획데이터
        public PlayerController actor;  // -> 현재 NPC와 대화중인 플레이어
        public int[] quests;            // -> 여러 조건등을 거치고 최종적으로 현재 NPC가 지닌 퀘스트 인덱스 값들

        public BoNPC(SDNPC sdNPC)
        {
            this.sdNPC = sdNPC;
        }
    }
}