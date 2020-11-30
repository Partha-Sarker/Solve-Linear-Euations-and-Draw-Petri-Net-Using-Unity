using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class LinearEquation : MonoBehaviour
{
    public TMP_InputField variableCountInput, equationCountInput;
    public TextMeshProUGUI resultText;
    public GameObject matrix, equation, variable, constant, plus, equal;
    private int variableCount, equationCount;
    public int defaultCoefficientValue = 0, defaultConstantValue = 0;
    public int defaultEquationCount = 3, defaultVariableCount = 3;
    public int floatRound = 4;
    List<List<GameObject>> equations;
    List<GameObject> eqStuffs;
    TMP_InputField[] allInputs;
    private bool shiftDown = false;
    //private float[,] defaultMat;

    private void Start()
    {
        variableCount = defaultVariableCount;
        equationCount = defaultEquationCount;
        allInputs = FindObjectsOfType<TMP_InputField>();
        Array.Reverse(allInputs);
        CreateAllEq();
        //defaultMat = new float[6, 6] {
        //    { 1, 1, 1, 1, 1, 1 },
        //    { 2, -2, 3, 0, 0, 0 },
        //    { 0, 1, -4, 0, 0, 0 },
        //    { 0, 1, 0, -1, 3, 0 },
        //    { 0, 0, 1, 1, -5, 0 },
        //    { -2, 0, 0, 0, 2, 0 }
        //};
        //defaultMat = new float[6, 6] {
        //    { 1, 1, 1, 1, 1, 1 },
        //    { 2, -2, 3, 0, 0, 0 },
        //    { 0, 1, -4, 0, 0, 0 },
        //    { 0, 1, 0, -1, 3, 0 },
        //    { 0, 0, 1, 1, -5, 0 },
        //    { -2, 0, 0, 0, 2, 0 }
        //};
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
            variableCount = defaultVariableCount;
        if (equationCount < 1)
            equationCount = defaultEquationCount;

        RemoveAllEq();
        CreateAllEq();
        allInputs = FindObjectsOfType<TMP_InputField>();
        Array.Reverse(allInputs);
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
        try
        {
            float[,] forwardElimMat = ForwardElimination(augmentedMatrix);
            float[,] upperTriagnleMat = GetUpperTriangleMat(forwardElimMat);
            float[] result = GetUnknownsFromUpperTriangleMat(upperTriagnleMat);
            string resultString = "";
            for (int i = 0; i < variableCount; i++)
            {
                resultString += $"X<sub>{i + 1}</sub> = {result[i]};";
                if (i != variableCount - 1)
                    resultString += "   ";
            }
            resultText.text = resultString;
            //PrintMatrix(result);
        }
        catch (Exception)
        {
            resultText.text = "Doesn't have a unique solution or any solution at all.";
        }


    }

    private float[,] ForwardElimination(float[,] augmentedMatrix)
    {
        int rowCount = augmentedMatrix.GetLength(0), columnCount = augmentedMatrix.GetLength(1);
        if (rowCount < columnCount - 1)
            throw new Exception();

        if (columnCount == 1)
        {
            for (int i = 0; i < rowCount; i++)
            {
                float value = (float)Math.Round((decimal)augmentedMatrix[i, 0], floatRound);
                if (value != 0.0f)
                    throw new Exception();
            }
            return augmentedMatrix;
        }

        int maxIndex = GetAbsMaxIndex(augmentedMatrix);
        SwapRow(ref augmentedMatrix, 0, maxIndex);
        float pivot = (float)Math.Round((decimal)augmentedMatrix[0, 0], floatRound);
        print($"pivot: {pivot}");
        if (pivot == 0.0f)
            throw new Exception();

        MultiplyRowByValue(ref augmentedMatrix, 0, augmentedMatrix[0, 0], false);

        for (int i = 1; i < rowCount; i++)
        {
            float[,] row0 = GetRow(augmentedMatrix, 0);
            float[,] targetRow = GetRow(augmentedMatrix, i);
            MultiplyRowByValue(ref row0, 0, targetRow[0, 0]);
            AddRowToMat(ref targetRow, row0, 0, false);
            SetRow(ref augmentedMatrix, i, targetRow);
        }
        float[,] subMat = GetSubMatrix(augmentedMatrix, 1, 1);
        float[,] forwardSubMat = ForwardElimination(subMat);
        SetSubMatrix(ref augmentedMatrix, forwardSubMat);
        //PrintMatrix(augmentedMatrix);
        return augmentedMatrix;
    }

    private float[] GetUnknownsFromUpperTriangleMat(float[,] upperTriangleMat)
    {
        int rowCount = upperTriangleMat.GetLength(0);
        float[] unknowns = new float[rowCount];
        for (int i = rowCount - 1; i >= 0; i--)
        {
            unknowns[i] = upperTriangleMat[i, rowCount] / upperTriangleMat[i, i];
            for (int j = i - 1; j >= 0; j--)
            {
                upperTriangleMat[j, rowCount] -= upperTriangleMat[j, i] * unknowns[i];
            }
        }
        for (int i = 0; i < rowCount; i++)
            unknowns[i] = (float)Math.Round((decimal)unknowns[i], floatRound);
        return unknowns;
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

    private int GetAbsMaxIndex(float[,] mat)
    {
        int index = 0, rowCount = mat.GetLength(0);
        float currentMax = float.MinValue;

        for (int i = 0; i < rowCount; i++)
        {
            float value = Mathf.Abs(mat[i, 0]);
            if (value > currentMax)
            {
                currentMax = value;
                index = i;
            }
        }
        return index;
    }

    private float[,] GetSubMatrix(float[,] mat, int startRowIndex, int startColIndex)
    {
        int rowCount = mat.GetLength(0), colCount = mat.GetLength(1);
        int newRowCount = rowCount - startRowIndex;
        int newColCount = colCount - startColIndex;
        float[,] subMat = new float[newRowCount, newColCount];
        for (int i = 0; i < newRowCount; i++)
        {
            for (int j = 0; j < newColCount; j++)
            {
                subMat[i, j] = mat[i + startRowIndex, j + startColIndex];
            }
        }

        return subMat;
    }

    private float[,] GetUpperTriangleMat(float[,] mat)
    {
        int colCount = mat.GetLength(1);
        float[,] subMat = new float[colCount - 1, colCount];
        for (int i = 0; i < colCount - 1; i++)
        {
            for (int j = 0; j < colCount; j++)
            {
                subMat[i, j] = mat[i, j];
            }
        }
        return subMat;
    }

    private void SetSubMatrix(ref float[,] mat, float[,] subMat)
    {
        int subMatRow = subMat.GetLength(0), subMatCol = subMat.GetLength(1);
        int rowOffset = mat.GetLength(0) - subMatRow, colOffset = mat.GetLength(1) - subMatCol;
        for (int i = 0; i < subMatRow; i++)
        {
            for (int j = 0; j < subMatCol; j++)
            {
                mat[i + rowOffset, j + colOffset] = subMat[i, j];
            }
        }
    }

    private void SwapRow(ref float[,] mat, int index1, int index2)
    {
        int columnCount = mat.GetLength(1);
        for (int i = 0; i < columnCount; i++)
        {
            float temp = mat[index1, i];
            mat[index1, i] = mat[index2, i];
            mat[index2, i] = temp;
        }
    }

    private void MultiplyRowByValue(ref float[,] mat, int index, float multiple, bool multiply = true)
    {
        int columnCount = mat.GetLength(1);
        for (int i = 0; i < columnCount; i++)
        {
            if (multiply)
                mat[index, i] *= multiple;
            else
                mat[index, i] /= multiple;
        }
    }

    private void AddValueToRow(ref float[,] mat, int index, float value, bool add = true)
    {
        int columnCount = mat.GetLength(1);
        for (int i = 0; i < columnCount; i++)
        {
            if (add)
                mat[index, i] += value;
            else
                mat[index, i] -= value;
        }
    }

    private void AddRowToRow(ref float[,] mat, int index1, int index2, bool add = true)
    {
        int columnCount = mat.GetLength(1);
        for (int i = 0; i < columnCount; i++)
        {
            if (add)
                mat[index1, i] += mat[index2, i];
            else
                mat[index1, i] -= mat[index2, i];
        }
    }

    private void AddRowToMat(ref float[,] mat, float[,] row, int index, bool add = true)
    {
        int columnCount = mat.GetLength(1);
        for (int i = 0; i < columnCount; i++)
        {
            if (add)
                mat[index, i] += row[0, i];
            else
                mat[index, i] -= row[0, i];
        }
    }

    private float[,] GetRow(float[,] mat, int index)
    {
        int columnCount = mat.GetLength(1);
        float[,] rowMat = new float[1, columnCount];
        for (int i = 0; i < columnCount; i++)
        {
            rowMat[0, i] = mat[index, i];
        }
        return rowMat;
    }

    private void SetRow(ref float[,] mat, int index, float[,] row)
    {
        int columnCount = mat.GetLength(1);
        for (int i = 0; i < columnCount; i++)
        {
            mat[index, i] = row[0, i];
        }
    }

    #region gaussian algo google
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
        int numOfColumn = M.GetLength(1);
        for (int i = 0; i < variableCount; i++)
        {
            variables.Add(M[i, numOfColumn - 1]);
        }
        return variables;
    }
    #endregion


    #region matrix method for solving linear equation
    private void SolveUsingMatrixMethod()
    {
        if (variableCount != equationCount)
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

    #endregion

    private void PrintMatrix(float[,] a, string title = "test")
    {
        string value = $"{title} start-----------\n";
        for (int i = 0; i < a.GetLength(0); i++)
        {
            for (int j = 0; j < a.GetLength(1); j++)
            {
                value += string.Format("{0:0.00}   ", a[i, j]);
            }
            value += "\n";
        }
        print(value);
        //print($"{title} end-------------");
    }

    private void PrintMatrix(float[] a, string title = "test")
    {
        string value = $"{title} start-----------\n";
        for (int i = 0; i < a.GetLength(0); i++)
        {
            value += string.Format("{0:0.00}   ", a[i]);
        }
        print(value);
        //print($"{title} end-------------");
    }

    //TestGaussian(augmentedMatrix);
    //PrintMatrix(augmentedMatrix);
    //List<float> variables = SolveUsingGaussianElimination(augmentedMatrix);
    //if (variables != null)
    //{
    //    string result = "";
    //    for (int i = 0; i < variableCount; i++)
    //    {
    //        result += $"X<sub>{i + 1}</sub> = {variables[i]};";
    //        if (i != variableCount - 1)
    //            result += "   ";
    //    }
    //    resultText.text = result;
    //}
    //else
    //{
    //    resultText.text = "Doesn't have a unique solution or any solution at all.";
    //}

}