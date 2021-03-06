using ProjectChan.Dummy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ProjectChan.Object
{
    using static ProjectChan.Define.Actor;
    using CamView = Define.Camera.CamView;
    using Input = Define.Input;

    /*
     *  플레이어 캐릭터의 입력 처리
     *  캐릭터 클래스에서 처리 안하는 이유
     *  캐릭터와 플레이어의 입력을 분리함으로써 캐릭터 클래스를 더 다양하게 사용할 수 있기 때문에
     *  EX: 멀티 환경에서의 캐릭터, NPC 등등..
     */

    /*
     *  Dictionary로 선언 한 이유
     *  평소에 하던 if문으로 키 입력 받기를 구현하면 하나하나 다 구현해야 하니깐 엄청 힘들고 귀찮음
     *  그래서 키 입력에 필요한 문자열 값과 키에 알맞는 행동을 실행하기 위해서 Dictionary로 생성함
     */

    /// <summary>
    /// 캐릭터를 컨트롤하는 클래스
    /// </summary>
    public class PlayerController : MonoBehaviour
    {
        // public
        public CameraController cameraController;       // 카메라를 관리하는 카메라 컨트롤러

        // private
        private Transform pointingTarget;               // 마우스 포인터에 걸린 타겟
        private bool canRot;                            // 캐릭터 회천 여부

        /// <summary>
        /// 컨트롤러를 지닐 캐릭터
        /// </summary>
        public Character PlayerCharacter { get; private set; }

        /// <summary>
        /// 현재 플레이어의 행동 여부
        /// </summary>
        public bool isPlayerAction { get; set; }

        /// <summary>
        /// 타겟이 감지 되었는지에 대한 여부
        /// </summary>
        public bool HasPointTarget { get; private set; }

        private Dictionary<string, InputHandler.AxisHandler> inputAxisDic;
        private Dictionary<string, InputHandler.ButtonHandler> inputButtonDic;

        /// <summary>
        /// 컨트롤러를 초기화 하는 메서드
        /// </summary>
        /// <param name="character"></param>
        public void Initialize(Character character)
        {
            character.playerController = transform.GetComponent<PlayerController>();
            character.transform.parent = transform;
            character.gameObject.layer = LayerMask.NameToLayer("Player");

            PlayerCharacter = character;

            // 현재 컨트롤러를 카메라가 추적하도록 한다
            cameraController.SetTarget(PlayerCharacter.transform);

            // 축 입력 관리
            inputAxisDic = new Dictionary<string, InputHandler.AxisHandler>();
            inputAxisDic.Add(Input.AxisX, new InputHandler.AxisHandler(GetAxisX));
            inputAxisDic.Add(Input.AxisZ, new InputHandler.AxisHandler(GetAxisZ));
            inputAxisDic.Add(Input.MouseX, new InputHandler.AxisHandler(GetMouseX));
            inputAxisDic.Add(Input.MouseY, new InputHandler.AxisHandler(GetMouseY));

            // 버튼 입력 관리
            inputButtonDic = new Dictionary<string, InputHandler.ButtonHandler>();
            inputButtonDic.Add(Input.MouseLeft, new InputHandler.ButtonHandler(OnPressMouseLeft, null));
            inputButtonDic.Add(Input.MouseRight, new InputHandler.ButtonHandler(OnPressMouseRight, OnNotMouseRight));
            inputButtonDic.Add(Input.FrontCam, new InputHandler.ButtonHandler(OnPressFrontCam, OnNotPressFrontCam));
        }

        private void FixedUpdate()
        {
            // 컨트롤러가 지닌 캐릭터가 없다면
            if (PlayerCharacter == null)
            {
                return;
            }

            // 캐릭터가 죽었다면
            if (PlayerCharacter.State == ActorState.Dead)
            {
                return;
            }

            // 캐릭터가 어떠한 행동을 하는 중이라면
            if (isPlayerAction)
            {
                return;
            }

            InputUpdate();
            CheckMousePointTarget();
        }

        /// <summary>
        /// 입력 함수들을 실행하는 메서드
        /// </summary>
        private void InputUpdate()
        {
            // 축 입력을 확인하는 작업
            foreach (var input in inputAxisDic)
            {
                // 축 입력에 대한 실수값을 반환 받음
                var value = UnityEngine.Input.GetAxis(input.Key);

                // 축 입력 시, 실행 시킬 델리게이트에게 실수값을 전달
                input.Value.GetAxisValue(value);
            }

            // 버튼 입력을 확인하는 작업
            foreach (var input in inputButtonDic)
            {
                if (UnityEngine.Input.GetButton(input.Key))
                {
                    // 버튼 입력 시, 실행 시킬 델리게이트 실행
                    input.Value.OnPress();
                }
                else
                {
                    input.Value.OnNotPress();
                }
            }
        }

        /// <summary>
        /// 마우스 포인터를 기준으로 레이를 사용하여 타겟을 찾아내는 메서드
        /// </summary>
        private void CheckMousePointTarget()
        {
            /// ScreenPointToRay : 스크린 좌표를 인수로 넘겨주면 카메라에서 시작하여 스크린 좌표에 해당하는 3차원의 좌표로 Ray를 생성
            // 현재 씬의 카메라에서 스크린 좌표계의 마우스 위치로 레이를 생성
            var ray = CameraController.Cam.ScreenPointToRay(UnityEngine.Input.mousePosition);

            // 생성한 레이를 통해 해당 레이 방향에 몬스터가 존재 하는지 체크
            var hits = Physics.RaycastAll(ray, 1000f, 1 << LayerMask.NameToLayer("Monster"));

            // 타겟이 존재한다면 True
            HasPointTarget = hits.Length != 0;

            // 캐릭터를 타겟 쪽으로 회전 시키기 위해, 0번째 타겟의 트랜스폼을 가져옴
            pointingTarget = HasPointTarget ? hits[0].transform : null;
        }

        #region 입력 구현

        private void GetAxisX(float value)
        {

        }

        private void GetAxisZ(float value)
        {
            var boActor = PlayerCharacter.boActor;
            var newDir = boActor.moveDir;
            var sprint = Define.StaticData.BaseSpeed + 
                UnityEngine.Input.GetAxis(Input.Sprint) * Define.StaticData.BaseSpeed;

            sprint += UnityEngine.Input.GetAxis(Input.Sprint) * Define.StaticData.BaseSpeed;
            newDir.z = (sprint * value);
            PlayerCharacter.boActor.moveDir = newDir;
        }

        private void GetMouseX(float value)
        {
            // Y축 회전
            var newDir = PlayerCharacter.boActor.rotDir;
            newDir.y = canRot ? value : 0;
            PlayerCharacter.boActor.rotDir = newDir;
        }

        private void GetMouseY(float value)
        {

        }

        private void OnPressFrontCam()
        {
            // 캠 회전
            cameraController.camView = CamView.Front;
        }

        private void OnNotPressFrontCam()
        {
            // 베이스 캠
            cameraController.camView = CamView.Standard;
        }

        private void OnPressMouseLeft()
        {
            // 공격 키
            // 마우스 레이로 타겟을 찾았다면
            if (pointingTarget != null)
            {
                // Y축 회전만 실행하고
                // 나머지 축은 기존 회전값을 보존한 채로 몬스터 쪽으로 회전

                // 캐릭터의 이전 회전값
                var originRot = PlayerCharacter.transform.eulerAngles;

                // 캐릭터가 타겟을 바라보도록 설정
                PlayerCharacter.transform.LookAt(pointingTarget);

                // 변경된 X,Z축 회전 값을 원래 회전 값으로 변경
                var newRot = PlayerCharacter.transform.eulerAngles;
                newRot.x = originRot.x;
                newRot.z = originRot.z;

                PlayerCharacter.transform.eulerAngles = newRot;
            }

            PlayerCharacter.SetState(ActorState.Attack);
        }

        private void OnPressMouseRight()
        {
            canRot = true;
        }

        private void OnNotMouseRight()
        {
            canRot = false;
        }

        #endregion

        /// <summary>
        /// => 종료 직전에 플레이어가 위치한 곳의 정보를 DB에 저장하는 메서드
        /// </summary>
        private void OnApplicationQuit()
        {
            // -> 플레이어의 현재 위치를 가져온다
            var playerPos = PlayerCharacter.transform.position;

            // -> 플레이어의 위치를 담아놓을 DtoStage 데이터를 가져온다
            var dtoStage = DummyServer.Instance.userData.dtoStage;
            dtoStage.lastStageIndex = GameManager.User.boStage.sdStage.index;
            dtoStage.lastPosX = playerPos.x;
            dtoStage.lastPosY = playerPos.y;
            dtoStage.lastPosZ = playerPos.z;

            DummyServer.Instance.Save();
        }

        private class InputHandler
        {
            /// <summary>
            /// 축 타입의 키 입력 시, 실행할 메서드를 대리할 델리게이트
            /// </summary>
            /// <param name="value"> 축 값</param>
            public delegate void InputAxisDel(float value);

            /// <summary>
            /// 버튼 타입의 키 입력 시, 실행할 메서드를 대리할 델리게이트
            /// </summary>
            public delegate void InputButtonDel();

            /// <summary>
            /// 축 입력 핸들러
            /// </summary>
            public class AxisHandler
            {
                private InputAxisDel axisDel;

                public AxisHandler(InputAxisDel axisDel)
                {
                    this.axisDel = axisDel;
                }

                public void GetAxisValue(float value)
                {
                    axisDel.Invoke(value);
                }
            }

            /// <summary>
            /// 버튼 입력 핸들러
            /// </summary>
            public class ButtonHandler
            {
                private InputButtonDel pressDel;
                private InputButtonDel notPressDel;

                public ButtonHandler(InputButtonDel pressDel, InputButtonDel notPressDel)
                {
                    this.pressDel = pressDel;
                    this.notPressDel = notPressDel;
                }

                public void OnPress()
                {
                    pressDel?.Invoke();
                }

                public void OnNotPress()
                {
                    notPressDel?.Invoke();
                }
            }
        }
    }
}