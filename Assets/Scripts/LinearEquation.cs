using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class LinearEquation : MonoBehaviour
{
    public TMP_InputField variableCountInput, equationCountInput;
    public TextMeshProUGUI resultText;
    public GameObject matrix, equation, variable, constant, plus, equal;
    private int variableCount, equationCount;
    public int defaultCoefficientValue = 0, defaultConstantValue = 0;
    public int defaultEquationCount = 3, defaultVariableCount = 3;
    List<List<GameObject>> equations;
    List<GameObject> eqStuffs;
    TMP_InputField[] allInputs;
    private bool shiftDown = false;

    private void Start()
    {
        variableCount = defaultVariableCount;
        equationCount = defaultEquationCount;
        allInputs = FindObjectsOfType<TMP_InputField>();
        System.Array.Reverse(allInputs);
        CreateAllEq();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
            shiftDown = true;
        else if (Input.GetKeyUp(KeyCode.LeftShift))
            shiftDown = false;

        if (Input.GetKeyDown(KeyCode.Tab) && !shiftDown)
            GotoNextInput();
        else if (Input.GetKeyDown(KeyCode.Tab) && shiftDown)
            GotoPreviousInput();
    }

    private void GotoNextInput()
    {
        int targetInputIndex = 0;
        int len = allInputs.Length;
        for (int i = 0; i < len - 1; i++)
        {
            if (allInputs[i].isFocused)
            {
                targetInputIndex = i + 1;
                break;
            }
        }
        allInputs[targetInputIndex].ActivateInputField();
    }

    private void GotoPreviousInput()
    {
        int targetInputIndex = 0;
        int len = allInputs.Length;
        for (int i = 1; i < len; i++)
        {
            if (allInputs[i].isFocused)
            {
                targetInputIndex = i - 1;
                break;
            }
        }

        allInputs[targetInputIndex].ActivateInputField();
    }

    public void GenerateEquationUI()
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
        if (variableCount < 1)
            variableCount = 3;
        if (variableCount > equationCount)
            variableCount = equationCount;

        RemoveAllEq();
        CreateAllEq();
        allInputs = FindObjectsOfType<TMP_InputField>();
        System.Array.Reverse(allInputs);
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
                {
                    eqStuff = Instantiate(constant, currentEq.transform);
                    eqStuff.transform.GetComponentInChildren<TMP_InputField>().text = defaultConstantValue.ToString();
                }
                else if (j == eqStuffInEqCount - 2)
                    eqStuff = Instantiate(equal, currentEq.transform);
                else if (j % 2 == 1)
                    eqStuff = Instantiate(plus, currentEq.transform);
                else
                {
                    int variableCount = j / 2 + 1;
                    eqStuff = Instantiate(variable, currentEq.transform);
                    eqStuff.transform.Find("name").GetComponent<TextMeshProUGUI>().text = $"X<sub>{variableCount}";
                    eqStuff.transform.name = $"X{variableCount}";
                    eqStuff.GetComponentInChildren<TMP_InputField>().text = defaultCoefficientValue.ToString();
                }
                eqStuffs.Add(eqStuff);
            }
            equations.Add(eqStuffs);
        }
    }

    public void SolveEquations()
    {
        GaussianMethod();
    }

    private void GaussianMethod()
    {
        float[,] augmentedMatrix = GetAugmentedMatrix();
        //TestGaussian(augmentedMatrix);
        //PrintMatrix(augmentedMatrix);
        List<float> variables = SolveUsingGaussianElimination(augmentedMatrix);
        if (variables != null)
        {
            string result = "";
            for (int i = 0; i < variableCount; i++)
            {
                result += $"X<sub>{i + 1}</sub> = {variables[i]};";
                if (i != variableCount - 1)
                    result += "   ";
            }
            resultText.text = result;
        }
        else
        {
            resultText.text = "Doesn't have a unique solution or any solution at all.";
        }
    }

    public List<float> SolveUsingGaussianElimination(float[,] M)
    {
        // input checks
        int rowCount = M.GetUpperBound(0) + 1;
        if (M == null || M.Length != rowCount * (rowCount + 1))
        {
            Debug.LogError("The algorithm must be provided with a (n x n+1) matrix.");
            return null;
        }
        if (rowCount < 1)
        {
            Debug.LogError("The matrix must at least have one row.");
            return null;
        }

        // pivoting
        for (int col = 0; col + 1 < rowCount; col++)
        {
            if (M[col, col] == 0)
            // check for zero coefficients
            {
                // find non-zero coefficient
                int swapRow = col + 1;
                for (; swapRow < rowCount; swapRow++)
                    if (M[swapRow, col] != 0) break;

                if (M[swapRow, col] != 0) // found a non-zero coefficient?
                {
                    // yes, then swap it with the above
                    float[] tmp = new float[rowCount + 1];
                    for (int i = 0; i < rowCount + 1; i++)
                    {
                        tmp[i] = M[swapRow, i]; M[swapRow, i] = M[col, i]; M[col, i] = tmp[i];
                    }
                }
                else 
                    return null; // no, then the matrix has no unique solution
            }
        }
        PrintMatrix(M, "after pivot");

        // elimination
        for (int sourceRow = 0; sourceRow + 1 < rowCount; sourceRow++)
        {
            for (int destRow = sourceRow + 1; destRow < rowCount; destRow++)
            {
                float df = M[sourceRow, sourceRow];
                float sf = M[destRow, sourceRow];
                for (int i = 0; i < rowCount + 1; i++)
                    M[destRow, i] = M[destRow, i] * df - M[sourceRow, i] * sf;
            }
        }
        PrintMatrix(M, "after elimination");

        // back-insertion
        for (int row = rowCount - 1; row >= 0; row--)
        {
            float f = M[row, row];
            if (f == 0)
                return null;

            for (int i = 0; i < rowCount + 1; i++)
                M[row, i] /= f;
            for (int destRow = 0; destRow < row; destRow++)
            {
                M[destRow, rowCount] -= M[destRow, row] * M[row, rowCount]; M[destRow, row] = 0;
            }
        }
        PrintMatrix(M, "after insertion");

        List<float> variables = new List<float>();
        int  numOfColumn = M.GetLength(1);
        for (int i = 0; i < variableCount; i++)
        {
            variables.Add(M[i, numOfColumn - 1]);
        }
        return variables;
    }

    public void TestGaussian(float[,] m)
    {

    }


    private void SolveUsingMatrixMethod()
    {
        if(variableCount != equationCount)
        {
            resultText.text = "Can't solve. Number of unknowns must be equal to number of equations";
        }
        float[,] coefficient = new float[variableCount, variableCount];
        float[,] constant = new float[variableCount, 1];
        GetCoefficientAndConstantMatrix(ref coefficient, ref constant);

        float[,] coefficientInverse = Inverse(coefficient);

        float[,] variableMatrix = MatrixMultiply(coefficientInverse, constant);

        string result = "";
        for (int i = 0; i < variableCount; i++)
        {
            result += $"X<sub>{i + 1}</sub> = {variableMatrix[i, 0]};";
            if (i != variableCount - 1)
                result += "   ";
        }
        resultText.text = result;
    }

    private float[,] GetAugmentedMatrix()
    {
        float[,] augmented = new float[equationCount, variableCount + 1];
        int eqCount = 0;
        foreach (List<GameObject> equation in equations)
        {
            int valueCount = 0;
            foreach (GameObject eqStuff in equation)
            {
                if (eqStuff.CompareTag("Variable") || eqStuff.CompareTag("Constant"))
                {
                    TMP_InputField input = eqStuff.GetComponentInChildren<TMP_InputField>();
                    string valueString = input.text;
                    if (valueString == "")
                        valueString = input.placeholder.GetComponent<TextMeshProUGUI>().text;

                    float value = float.Parse(valueString);
                    augmented[eqCount, valueCount++] = value;
                }
            }
            eqCount++;
        }
        return augmented;
    }

    private void GetCoefficientAndConstantMatrix(ref float[,] coefficient, ref float[,] constant)
    {
        int len = coefficient.GetLength(0);
        int eqCount = 0;
        foreach (List<GameObject> equation in equations)
        {
            int coefficientCount = 0;
            foreach (GameObject eqStuff in equation)
            {
                if (eqStuff.CompareTag("Variable") || eqStuff.CompareTag("Constant"))
                {
                    TMP_InputField input = eqStuff.GetComponentInChildren<TMP_InputField>();
                    string valueString;
                    if (input.text == "")
                        valueString = input.placeholder.GetComponent<TextMeshProUGUI>().text;
                    else
                        valueString = input.text;
                    float value = float.Parse(valueString);
                    if (eqStuff.CompareTag("Variable"))
                    {
                        coefficient[eqCount, coefficientCount] = value;
                        coefficientCount++;
                    }
                    else
                    {
                        constant[eqCount, 0] = value;
                    }
                }
            }
            eqCount++;
            if (eqCount == len)
                break;
        }
    }


    float[,] GetCofactor(float[,] a, int p, int q)
    {
        int len = a.GetLength(0);
        float[,] temp = new float[len - 1, len - 1];

        int i = 0, j = 0;
        // Looping for each element of the matrix 
        for (int row = 0; row < len; row++)
        {
            for (int col = 0; col < len; col++)
            {
                // Copying into temporary matrix only those element 
                // which are not in given row and column 
                if (row != p && col != q)
                {
                    temp[i, j++] = a[row, col];

                    // Row is filled, so increase row index and 
                    // reset col index 
                    if (j == len - 1)
                    {
                        j = 0;
                        i++;
                    }
                }
            }
        }
        return temp;
    }

    float Determinant(float[,] a)
    {
        int n = a.GetLength(0);
        float D = 0; // Initialize result 
        // Base case : if matrix contains single element 
        if (n == 1)
            return a[0, 0];

        int sign = 1; // To store sign multiplier 
        float[,] cofactor;
        // Iterate for each element of first row 
        for (int f = 0; f < n; f++)
        {
            // Getting Cofactor of A[0,f] 
            cofactor = GetCofactor(a, 0, f);
            D += sign * a[0, f] * Determinant(cofactor);

            // terms are to be added with alternate sign 
            sign = -sign;
        }
        return D;
    }

    float[,] Adjoint(float[,] a)
    {
        int len = a.GetLength(0);
        float[,] adj = new float[len, len];
        if (len == 1)
        {
            adj[0, 0] = 1;
            return adj;
        }

        for (int i = 0; i < len; i++)
        {
            for (int j = 0; j < len; j++)
            {
                // Get cofactor of A[i,j] 
                float[,] cofactor = GetCofactor(a, i, j);

                // sign of adj[j,i] positive if sum of row 
                // and column indexes is even. 
                int sign = ((i + j) % 2 == 0) ? 1 : -1;

                // Interchanging rows and columns to get the 
                // transpose of the cofactor matrix 
                adj[j, i] = (sign) * (Determinant(cofactor));
            }
        }
        return adj;
    }

    float[,] Inverse(float[,] a)
    {
        float det = Determinant(a);
        int len = a.GetLength(0);
        float[,] inverse = new float[len, len];
        if (det == 0)
            return inverse;

        // Find adjoint 
        float[,] adj = Adjoint(a);

        // Find Inverse using formula "inverse(A) = adj(A)/det(A)" 
        for (int i = 0; i < len; i++)
            for (int j = 0; j < len; j++)
                inverse[i, j] = adj[i, j] / det;

        return inverse;
    }

    private float[,] MatrixMultiply(float[,] a, float[,] b)
    {
        int aRow = a.GetLength(0), aColumn = a.GetLength(1);
        int bRow = b.GetLength(0), bColumn = b.GetLength(1);
        if (aColumn != bRow)
        {
            Debug.LogError("Can't multiply, shape not compatible");
            return null;
        }
        float[,] mulMatrix = new float[aRow, bColumn];

        for (int i = 0; i < aRow; i++)
        {
            for (int j = 0; j < bColumn; j++)
            {
                mulMatrix[i, j] = 0;
                for (int k = 0; k < aColumn; k++)
                    mulMatrix[i, j] += (a[i, k] * b[k, j]);
            }
        }

        return mulMatrix;
    }

    private void PrintMatrix(float[,] a, string title = "test")
    {
        print($"{title} start-----------");
        for (int i = 0; i < a.GetLength(0); i++)
        {
            string rowValue = "";
            for (int j = 0; j < a.GetLength(1); j++)
                rowValue += a[i, j] + " ";
            print(rowValue);
        }
        print($"{title} end-------------");
    }
}
