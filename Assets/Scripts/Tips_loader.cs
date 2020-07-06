using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Tips_loader : MonoBehaviour {

    public string[] tips;//stings (tips) stored by player
    public Text tipText;//tip holder text
	// Use this for initialization
	void Start () {
        tipText.text = tips[Random.Range(0, tips.Length)];//choosing a random string(Tip) and assigning it to tip text holder
        gameObject.GetComponent<Animator>().Rebind();// rebinding animator to make it work properly

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OclickCloseBtn()//when clossing popup
    {
        tipText.text = tips[Random.Range(0, tips.Length)];//need to get another random string(tip) to show next time
        gameObject.SetActive(false);//turning tip's popUp off
    }

}
