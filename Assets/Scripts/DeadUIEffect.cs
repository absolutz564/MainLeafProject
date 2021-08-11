using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DeadUIEffect : MonoBehaviour
{
    public TextMeshProUGUI CurrentDeadText;
    public bool IsDead = false;
    public string DeadText = "YOU ARE DEAD!";
    public void DeadEffect()
    {
        StartCoroutine(WaitToText());
    }

    IEnumerator WaitToText()
    {
        Debug.Log(DeadText.Length);
        for (int i = 0; i < DeadText.Length; i++)
        {
            CurrentDeadText.text += DeadText[i];
            yield return new WaitForSeconds(0.2f);
        }
    }
}
