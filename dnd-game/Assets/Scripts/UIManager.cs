using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    TextMeshProUGUI resultText;
    DiceContainer diceContainer;


    void Start()
    {

    }

    void Awake()
    {
        resultText = GameObject.Find("ResultText").GetComponent<TextMeshProUGUI>();
        diceContainer = GameObject.Find("DiceContainer").GetComponent<DiceContainer>();
    }

    void Update()
    {

    }

    void FixedUpdate()
    {
        var (total, breakdown) = diceContainer.Total();

        if (breakdown == "")
        {
            resultText.text = "";
            return;
        }

        resultText.text = $"{breakdown} = {total}";
    }
}
