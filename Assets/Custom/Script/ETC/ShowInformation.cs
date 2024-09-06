using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShowInformation : MonoBehaviour
{
    public InformationUI.InformationSituation informationSituation;
    public void showInformationPanel()
    {
        InformationUI.instance.ShowInformation(informationSituation);
    }
}
