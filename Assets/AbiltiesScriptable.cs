using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ability", menuName = "ScriptableObjects/Ability", order = 1)]
public class AbiltiesScriptable : ScriptableObject
{
    string ability_name;
    DogAbilities ability;
}

