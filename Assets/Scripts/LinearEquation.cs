using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LinearEquation : MonoBehaviour
{
    public TMP_InputField variableCountInput, equationCountInput;
    public GameObject matrix, equation, variable, constant, plus, equal;
    private int variableCount, equationCount;
    List<List<GameObject>> equations;
    List<GameObject> eqStuffs;
    private void Start()
    {
        variable.GetComponentInChildren<TMP_InputField>().text = "1";
        constant.GetComponentInChildren<TMP_InputField>().text = "1";
        string variablePlaceholderValue = variableCountInput.placeholder.GetComponent<TextMeshProUGUI>().text;
        string equationPlaceholderValue = equationCountInput.placeholder.GetComponent<TextMeshProUGUI>().text;
        variableCount = int.Parse(variablePlaceholderValue);
        equationCount = int.Parse(equationPlaceholderValue);
        CreateAllEq();
    }

    public void GenerateEquationMatrix()
    {
        string variablePlaceholderValue = variableCountInput.placeholder.GetComponent<TextMeshProUGUI>().text;
        string equationPlaceholderValue = equationCountInput.placeholder.GetComponent<TextMeshProUGUI>().text;
        string variableInputValue = variableCountInput.text;
        string equationInputValue = equationCountInput.text;
        if (variableInputValue == "")
            variableCount = int.Parse(variablePlaceholderValue);
        else
            variableCount = int.Parse(variableInputValue);
        if (equationInputValue == "")
            equationCount = int.Parse(equationPlaceholderValue);
        else
            equationCount = int.Parse(equationInputValue);
        if (variableCount > equationCount)
            variableCount = equationCount;
        Debug.Log($"varaible count: {variableCount}   equation count: {equationCount}");

        RemoveAllEq();
        CreateAllEq();
    }

    public void RemoveAllEq()
    {
        for (int i = 0; i < matrix.transform.childCount; i++)
            Destroy(matrix.transform.GetChild(i).gameObject);
    }

    public void CreateAllEq()
    {
        equations = new List<List<GameObject>>();
        int eqStuffInEqCount = 2 * variableCount + 1;
        for (int i = 0; i < equationCount; i++)
        {
            GameObject currentEq = Instantiate(equation, matrix.transform);
            eqStuffs = new List<GameObject>();
            for (int j = 0; j < eqStuffInEqCount; j++)
            {
                GameObject eqStuff;
                if (j == eqStuffInEqCount - 1)
                    eqStuff = Instantiate(constant, currentEq.transform);
                else if (j == eqStuffInEqCount - 2)
                    eqStuff = Instantiate(equal, currentEq.transform);
                else if (j % 2 == 1)
                    eqStuff = Instantiate(plus, currentEq.transform);
                else
                {
                    int variableCount = j / 2 + 1;
                    eqStuff = Instantiate(variable, currentEq.transform);
                    eqStuff.transform.Find("name").GetComponent<TextMeshProUGUI>().text = $"X<sub>{variableCount}";
                }
                eqStuffs.Add(eqStuff);
            }
            equations.Add(eqStuffs);
        }
    }

    public void SolveEquations()
    {

    }
}
