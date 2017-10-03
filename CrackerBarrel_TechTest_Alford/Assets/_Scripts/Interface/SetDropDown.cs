using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Dropdown))]
public class SetDropDown : MonoBehaviour {
    public string key = "DragDrop";
    private TMP_Dropdown theDropdown;
    // Use this for initialization
    private void Awake()
    {
        theDropdown = GetComponent<TMP_Dropdown>();
    }

    void Start () {
        if (theDropdown != null)
        {
            theDropdown.onValueChanged.AddListener(delegate { OnValueChanged(theDropdown); });


            if (PlayerPrefs.HasKey(key))
            {
                theDropdown.value = PlayerPrefs.GetInt(key);
            }
            else
            {
                PlayerPrefs.SetInt(key, theDropdown.value);
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void OnValueChanged(TMP_Dropdown theControl)
    {
        if(theControl != null)
            PlayerPrefs.SetInt(key, theControl.value);
    }
}
