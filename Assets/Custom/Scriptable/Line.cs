using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Line", menuName = "Puzzle Game/Line", order = 0)]
public class Line : ScriptableObject {
    public int stringCount;
    [Space]
    [Space]
    [Space]
    [TextArea]
    public string[] line1;
    public string[] line1Animation ;

    [Space]
    [Space]
    [Space]
    [TextArea]
    public string[] line2;
    public string[] line2Animation ;

    [Space]
    [Space]
    [Space]
    [TextArea]
    public string[] line3;
    public string[] line3Animation ;

    [Space]
    [Space]
    [Space]
    [TextArea]
    public string[] line4;
    public string[] line4Animation ;

    [Space]
    [Space]
    [Space]
    [TextArea]
    public string[] line5;
    public string[] line5Animation ;
}
