using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Tips_loader : MonoBehaviour {

    public string[] tips;
    public Text tipText;
	// Use this for initialization
	void Start () {
        tipText.text = tips[Random.Range(0, tips.Length)];
        gameObject.GetComponent<Animator>().Rebind();

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OclickCloseBtn()
    {
        tipText.text = tips[Random.Range(0, tips.Length)];
        gameObject.SetActive(false);
    }

}
