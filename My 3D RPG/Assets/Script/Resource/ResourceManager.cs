using ProjectChan.Define;
using ProjectChan.Object;
using ProjectChan.UI;
using ProjectChan.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;

namespace ProjectChan.Resource
{
    /// <summary>
    /// 런타임에 필요한 리소스를 불러오는 기능을 가진 클래스
    /// </summary>
    public class ResourceManager : Singleton<ResourceManager>
    {
        public void Initialize()
        {
            LoadAllAtlas();
            LoadAllPrefabs();
        }

        /// <summary>
        /// 리스스 폴더 안 경로의 프리팹을 가져와서 반환하는 메서드
        /// </summary>
        /// <param name="path"> 불러올 프리팹 에셋의 경로 </param>
        /// <returns></returns>
        public GameObject LoadObject(string path)
        {
            // 에셋 폴더 안에 리소스 폴더가 존재한다면, 해당 경로 부터 Path를 읽는다
            // 해당 경로에 파일이 GameObject 형태로 부를 수 있다면 불러온다
            return Resources.Load<GameObject>(path);
        }

        /// <summary>
        /// 리소스 폴더 내의 모든 아틀라스를 불러와 스프라이트 로더에 등록하는 메서드
        /// </summary>
        private void LoadAllAtlas()
        {
            var portraitAtlase = Resources.LoadAll<SpriteAtlas>("Atlase/PortraitAtlase");
            SpriteLoader.SetAtlas(portraitAtlase);

            var uiAtlase = Resources.LoadAll<SpriteAtlas>("Atlase/UIAtlase");
            SpriteLoader.SetAtlas(uiAtlase);

            var itemAtlase = Resources.LoadAll<SpriteAtlas>("Atlase/ItemAtlase");
            SpriteLoader.SetAtlas(itemAtlase);
        }

        /// <summary>
        /// 인게임에서 사용할 프리팹을 풀에 등록 요청을 보내는 메서드
        /// </summary>
        public void LoadAllPrefabs()
        {
            LoadPoolableObject<MonHpBar>(PoolType.MonHpBar, "Prefabs/UI/MonHpBar", 3);
            LoadPoolableObject<DialogueButton>(PoolType.DialogueButton, "Prefabs/UI/DialogueButton", 10);
            LoadPoolableObject<QuestSlot>(PoolType.QuestSlot, "Prefabs/UI/QuestSlot", 10);
            LoadPoolableObject<Object.Item>(PoolType.Item, "Prefabs/Item/Potion", 10);
        }

        /// <summary>
        /// 노벨 게임에서 사용할 배경 아틀라스를 불러오는 메서드
        /// </summary>
        public void LoadBackGround()
        {
            var backAtlase = Resources.LoadAll<SpriteAtlas>("Atlase/BackAtlase");
            SpriteLoader.SetAtlas(backAtlase);
        }

        /// <summary>
        /// 오브젝트 풀로 사용할 프리팹을 생성하고 풀에 등록 하는 메서드
        /// </summary>
        /// <typeparam name="T">        로드 하고자 하는 프리팹이 갖는 타입 </typeparam>
        /// <param name="poolType">     풀에 등록된 오브젝트를 가져올 때 사용할 키 값 </param>
        /// <param name="path">         프리팹 경로 </param>
        /// <param name="poolCount">    생성시키고자 하는 풀 객체의 개수 </param>
        /// <param name="loadComplete"> 프리팹을 로드하고 오브젝트 풀에 등록 후 실행시킬 이벤트 </param>
        public void LoadPoolableObject<T>(PoolType poolType, string path, int poolCount = 1, Action loadComplete = null)
            where T : MonoBehaviour, IPoolableObject
        {
            // 프리팹을 가져온다
            var obj = LoadObject(path);

            // 프리팹에 존재하는 T타입의 컴포넌트를 가져온다
            var tComponent = obj.GetComponent<T>();

            // 풀에 등록 요청을 한다
            ObjectPoolManager.Instance.RegistPool<T>(poolType, tComponent, poolCount);

            // 위의 작업이 모두 끝난 후, 실행 시킬 작업이 있다면 실행
            loadComplete?.Invoke();
        }
    }
}