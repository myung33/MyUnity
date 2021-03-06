using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectChan.SD
{
    /// <summary>
    /// => 스테이지 기획 데이터
    /// </summary>
    [Serializable]
    public class SDStage : StaticData
    {
        public string name;             // -> 현재 스테이지의 이름
        public string resourcePath;     // -> 현재 스테이지 프리팹이 저장된 경로
        public int stageMonCount;       // -> 현재 스테이지에 생성할 몬스터 개수
        public int[] genMonsters;       // -> 현재 스테이지에서 소환되는 몬스터 인덱스
        public int[] spawnArea;         // -> 현재 몬스터가 소환되는 지점 인덱스
        public int[] warpStageRef;      // -> 다음 스테이지의 인덱스 값
    }
}