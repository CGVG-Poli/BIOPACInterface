using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

public class ParserController : MonoBehaviour
{
    private static ParserController instance;
    [SerializeField] private SeparatorType separatorType = SeparatorType.Tab;
    private char separatorChar = '\t';
    [SerializeField] private TextAsset template;
    [SerializeField] private TextAsset inputFile;
    [SerializeField] private Text onScreenTextInputFields, onScreenTextValues;
    [SerializeField] private Button okButton;
    [SerializeField] private GameObject parserTestPanel;
    [SerializeField] private bool readTemplateFromFile = true;
    [SerializeField] private bool readSourceFromFile = true;
    private List<string> allFields = new List<string>();
    private List<int> relevantFieldIndexes = new List<int>();
    private List<string> inputLines = new List<string>();
    private int currentLineIndex = 0; //used only server-side

    public static ParserController Instance
    {
        get
        {
            return instance;
        }
    }

    private void Awake()
    {
        Assert.IsNull(instance);
        instance = this;
    }


    void Start()
    {
        okButton.onClick.AddListener(OkButtonPressed);
    }

    public void StartParsingOperations()
    {
        ResetAll();

        SwitchSeparatorType();

        ParseTemplate();

        PrintInputFields();

        ParseInputFile();

        PrintInputsDEBUG();
    }

    public SeparatorType SeparatorType
    {
        get
        {
            return separatorType;
        }
        set
        {
            separatorType = value;
        }
    }

    public void SetReadSourceFromFile(bool readFromFile)
    {
        readSourceFromFile = readFromFile;
    }

    public void SetReadTemplateFromFile(bool readFromFile)
    {
        readTemplateFromFile = readFromFile;
    }

    private void ResetAll()
    {
        allFields.Clear();
        relevantFieldIndexes.Clear();
        inputLines.Clear();
        currentLineIndex = 0;
    }

    private void SwitchSeparatorType()
    {
        switch (separatorType)
        {
            case SeparatorType.Comma:
                separatorChar = ',';
                break;
            case SeparatorType.Semicolon:
                separatorChar = ';';
                break;
            case SeparatorType.Pipe:
                separatorChar = '|';
                break;
            case SeparatorType.Space:
                separatorChar = ' ';
                break;
            case SeparatorType.Tab:
            default:
                separatorChar = '\t';
                break;
        }
    }

    private void ParseTemplate()
    {
        string templateText = "";

        if (!readTemplateFromFile)
        {
            templateText = template.text;
        }
        else
        {
            string dirPath = Application.dataPath + "/Template.txt";
            StreamReader sr = new StreamReader(dirPath);
            templateText = sr.ReadToEnd();
        }

        string[] lines = templateText.Split('\n'); //line 0 is ALLFIELDS, line 1 is RELEVANTFIELDS

        //get all fields
        string[] allFieldsArray = lines[0].Split(separatorChar);

        //get all relevantFields
        List<string> allRelevantFieldsArray = new List<string>();
        allRelevantFieldsArray.AddRange(lines[1].Split(separatorChar));

        //put all fields in the list (start from 1, since allFieldsArray[0] should be "ALLFIELDS")
        for (int i = 1; i < allFieldsArray.Length; i++)
        {
            allFields.Add(allFieldsArray[i]);
            if (allRelevantFieldsArray.Contains(allFieldsArray[i]))
            {
                relevantFieldIndexes.Add(i-1);
            }

        }
    }

    private void PrintInputFields()
    {
        if (onScreenTextInputFields == null)
        {
            return;
        }

        onScreenTextInputFields.text = "<b>Input Fields:</b>\n";

        for (int i = 0; i < allFields.Count; i++)
        {
            if (relevantFieldIndexes.Contains(i))
            {
                onScreenTextInputFields.text += $"<color=yellow> {allFields[i]} </color>";
            }
            else
            {
                onScreenTextInputFields.text += allFields[i];
            }

            onScreenTextInputFields.text += "\n";
        }
    }

    private void ParseInputFile()
    {
        
        string inputText = "";

        if (!readTemplateFromFile)
        {
            inputText = inputFile.text;
        }
        else
        {
            string dirPath = Application.dataPath + "/Source.txt";
            StreamReader sr = new StreamReader(dirPath);
            inputText = sr.ReadToEnd();
        }

        //split input file in lines
        string[] lines = inputText.Split('\n');

        //find the index of the first relevant line
        //OCCHIO QUA <- si suppone che il primo field sia sempre il primo di template, se ci sono errori in lettura sono qua
        int firstRelevantLineIndex = 0;
        for (int i = 0; i < lines.Length; i++)
        {
            if (lines[i].Contains(allFields[0]))
            {
                firstRelevantLineIndex = i+1;
                break;
            }
        }

        //OLD VERSION -> Memorize relevant fields server - side
        /*
        //memorize relevant values into a string
        for (int i = firstRelevantLineIndex; i < lines.Length; i++)
        {
            //split into columms
            string[] allValues = lines[i].Split(separatorChar);
            //find relevant fields
            string inputLine = "";
            for (int j = 0; j < relevantFieldIndexes.Count; j++)
            {
                if (relevantFieldIndexes[j] >= allValues.Length)
                {
                    inputLine += "<color=red>READ ERROR</color>";
                }
                else
                {
                    inputLine += allValues[relevantFieldIndexes[j]] + separatorChar;
                }
            }
            inputLines.Add(inputLine);
        }
        */

        //NEW VERSION -> Just memorize the whole line
        for (int i = firstRelevantLineIndex; i < lines.Length; i++)
        {
            inputLines.Add(lines[i]);
        }
    }

    public List<string> ParseRelevantFieldsToString(string inputString)
    {
        //parse a line to find relevant values, put those values into a list of strings, return that list
        string[] allValues = inputString.Split(separatorChar);
        List<string> outputValues = new List<string>();

        for (int i = 0; i < relevantFieldIndexes.Count; i++)
        {
            if (relevantFieldIndexes[i] > allValues.Length)
            {
                outputValues.Add("READ ERROR");
                Debug.LogError("ERROR PARSING RELEVANT FIELDS!!!!");
            }
            else
            {
                outputValues.Add(allValues[relevantFieldIndexes[i]]);
            }
        }

        return outputValues;
    }

    public List<double> ParseRelevantFieldsToDouble(string inputString)
    {
        //parse a line to find relevant values, immediately parse them to doubles, put them into a list, then return that list
        string[] allValues = inputString.Split(separatorChar);
        List<double> outputValues = new List<double>();

        for (int i = 0; i < relevantFieldIndexes.Count; i++)
        {
            if (relevantFieldIndexes[i] > allValues.Length)
            {
                outputValues.Add(0.0d); //this should not happen
                Debug.LogError("ERROR PARSING RELEVANT FIELDS!!!!");
            }
            else
            {
                outputValues.Add(double.Parse(allValues[relevantFieldIndexes[i]]));
            }
        }

        return outputValues;
    }

    private void PrintInputsDEBUG()
    {
        if (onScreenTextValues == null)
        {
            return;
        }

        onScreenTextValues.text = "<b>Values:</b>\n"; 
        //print only the first 50 lines, for debug
        for (int i = 0; i < inputLines.Count; i++)
        {
            if (i >= 49)
            {
                break;
            }
            onScreenTextValues.text += inputLines[i] + "\n";
        }
    }

    public string RandomLine
    {
        get
        {
            return inputLines[Random.Range(0, inputLines.Count)];
        }
    }

    public string CurrentLine
    {
        get
        {
            return inputLines[currentLineIndex];
        }
    }

    public bool AdvanceToNextLine()
    {
        currentLineIndex++;

        if (currentLineIndex >= inputLines.Count)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void OkButtonPressed()
    {
        parserTestPanel.SetActive(false);
        ClientServerSelector.Instance.gameObject.SetActive(true);
        GeneralSetupInterface.Instance.gameObject.SetActive(true);
    }

}
