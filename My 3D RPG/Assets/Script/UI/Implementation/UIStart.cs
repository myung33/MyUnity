using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectChan.UI
{
    public class UIStart : MonoBehaviour
    {
        public Text loadStateDesc;
        public Image loadFillGauge;

        public void SetLoadStateDescription(string loadState)
        {
            loadStateDesc.text = $"Load{loadState.ToString()}...";
        }

        public IEnumerator LoadGaugeUpdate(float loadPer)
        {
            // fillAmount���� �Ķ���ͷ� ���� loadPer�� ���Ͽ� ������������ �ݺ�
            while (!Mathf.Approximately(loadFillGauge.fillAmount, loadPer))
            {
                loadFillGauge.fillAmount = Mathf.Lerp(loadFillGauge.fillAmount, loadPer, Time.deltaTime * 2f);
                yield return null;
            }
        }
    }
}