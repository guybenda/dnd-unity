using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    TextMeshProUGUI resultText;
    DiceContainer diceContainer;

    int counter = 0;


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
        counter = (counter + 1) % 10;
        if (counter != 0) return;

        var (total, breakdown) = diceContainer.Total();

        if (breakdown == "")
        {
            resultText.text = "";
            return;
        }

        resultText.text = $"{breakdown} = {total}";
    }
}
