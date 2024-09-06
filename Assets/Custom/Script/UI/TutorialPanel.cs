using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialPanel : MonoBehaviour
{
    public GameObject[] tutorialPanels;

    public Button previousButton;
    public Button nextButton;
    public TextMeshProUGUI indexShowText;

    private int index{
        get{
            return _index;
        }

        set{
            _index = value;
            _index = Mathf.Clamp(_index, 0, tutorialPanels.Length-1);
            if(_index == 0)
            {
                previousButton.interactable = false;
                nextButton.interactable = true;
            }else if(_index == tutorialPanels.Length-1)
            {
                previousButton.interactable = true;
                nextButton.interactable = false;
            }else
            {
                previousButton.interactable = true;
                nextButton.interactable = true;
            }

            foreach(GameObject g in tutorialPanels)
            {
                g.SetActive(false);
            }
            tutorialPanels[_index].SetActive(true);
            indexShowText.text = (_index + 1) + " / " + tutorialPanels.Length;
        }
    }

    private int _index = 0;

    private void OnEnable() {
        index = 0;
    }

    public void IndexUp()
    {
        index++;
    }

    public void IndexDown()
    {
        index--;
    }


}
