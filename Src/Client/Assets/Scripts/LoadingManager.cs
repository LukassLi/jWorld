using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour {

	public GameObject UItips;
	public GameObject UILoading;
	public GameObject UILogin;

	public Slider progressBar;

	// Use this for initialization
	IEnumerator Start () {
        log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.xml"));
        UnityLogger.Init();
        //Common.Log.Init("Unity");
        Common.Log.Info("LoadingManager start");  // todo 这是unity的日志输出方式？


		UItips.SetActive(true);
		UILoading.SetActive(false);
		UILogin.SetActive(false);
		yield return new WaitForSeconds(2f);
		UILoading.SetActive(true);
		yield return new WaitForSeconds(1f);
		UItips.SetActive(false);

		//yield return Datamanager.Instance.LoadData;
		


		for(float i = 50; i < 100;)
        {
			i += Random.Range(0.1f, 1.5f);
			progressBar.value = i;
			yield return new WaitForEndOfFrame();
        }

		UILoading.SetActive(false);
		UILogin.SetActive(true);
		yield return null; // todo 不一定需要？

	}

	// Update is called once per frame
	void Update () {
		
	}
}
