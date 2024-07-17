using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using DialogueSystem.Localization;

namespace DialogueSystem
{
    public class CSVExport : MonoBehaviour
    {
        public DialogueScript script;

        public void GenerateCSV()
        {
            List<string> tcvContent = new List<string>();

            string filePath = Application.dataPath + "/data.tsv";

            /* GENEROWANIE HEADER */
            List<string> headerTexts = new List<string>();
            headerTexts.Add("GUID ID");
            foreach (LocalizationEnum language in (LocalizationEnum[])Enum.GetValues(typeof(LocalizationEnum)))
            {
                headerTexts.Add(language.ToString());
            }
            string filanHeader = "";
            foreach (string header in headerTexts)
            {
                if (filanHeader != "") { filanHeader += "\t"; }
                filanHeader += header;
            }

            TextWriter tw = new StreamWriter(filePath, false);
            tw.WriteLine(filanHeader);
            tw.Close();
            /* GENEROWANIE HEADER */

            /* Generaowanie Tesktu*/
            // Dialogue Node
            for (int i = 0; i < script.DialogueNodeDatas.Count; i++)
            {
                List<string> dialogueNodeContent = new List<string>();
                dialogueNodeContent.Add(script.DialogueNodeDatas[i].NodeGuid);

                for (int j = 0; j < script.DialogueNodeDatas[i].TextType.Count; j++)
                {
                    dialogueNodeContent.Add(script.DialogueNodeDatas[i].TextType[j].LanguageGenericType);
                }

                string dialogueNodeFinal = "";
                foreach (string text in dialogueNodeContent)
                {
                    if (dialogueNodeFinal != "") { dialogueNodeFinal += "\t"; }
                    dialogueNodeFinal += text;
                }
                tcvContent.Add(dialogueNodeFinal);
            }
            // Choice Dialogue Node
            for (int i = 0; i < script.DialogueChoiceNodeDatas.Count; i++)
            {
                List<string> choiceNodeContent = new List<string>();
                choiceNodeContent.Add(script.DialogueChoiceNodeDatas[i].NodeGuid);

                for (int j = 0; j < script.DialogueChoiceNodeDatas[i].TextType.Count; j++)
                {
                    choiceNodeContent.Add(script.DialogueChoiceNodeDatas[i].TextType[j].LanguageGenericType);
                }

                string choiceNodeFinal = "";
                foreach (string text in choiceNodeContent)
                {
                    if (choiceNodeFinal != "") { choiceNodeFinal += "\t"; }
                    choiceNodeFinal += text;
                }
                tcvContent.Add(choiceNodeFinal);

                for (int j = 0; j < script.DialogueChoiceNodeDatas[i].DialogueNodePorts.Count; j++)
                {
                    List<string> choiceNodeChoiceContent = new List<string>();
                    choiceNodeChoiceContent.Add(script.DialogueChoiceNodeDatas[i].DialogueNodePorts[j].PortGuid);

                    for (int k = 0; k < script.DialogueChoiceNodeDatas[i].DialogueNodePorts[j].TextLanguage.Count; k++)
                    {
                        choiceNodeChoiceContent.Add(script.DialogueChoiceNodeDatas[i].DialogueNodePorts[j].TextLanguage[k].LanguageGenericType);
                    }

                    string choiceNodeChoiceFinal = "";
                    foreach (string text in choiceNodeChoiceContent)
                    {
                        if (choiceNodeChoiceFinal != "") { choiceNodeChoiceFinal += "\t"; }
                        choiceNodeChoiceFinal += text;
                    }
                    tcvContent.Add(choiceNodeChoiceFinal);
                }
            }
            // Choice Dialogue Node
            for (int i = 0; i < script.TimerChoiceNodeDatas.Count; i++)
            {
                List<string> choiceNodeContent = new List<string>();
                choiceNodeContent.Add(script.TimerChoiceNodeDatas[i].NodeGuid);

                for (int j = 0; j < script.TimerChoiceNodeDatas[i].TextType.Count; j++)
                {
                    choiceNodeContent.Add(script.TimerChoiceNodeDatas[i].TextType[j].LanguageGenericType);
                }

                string choiceNodeFinal = "";
                foreach (string text in choiceNodeContent)
                {
                    if (choiceNodeFinal != "") { choiceNodeFinal += "\t"; }
                    choiceNodeFinal += text;
                }
                tcvContent.Add(choiceNodeFinal);

                for (int j = 0; j < script.TimerChoiceNodeDatas[i].DialogueNodePorts.Count; j++)
                {
                    List<string> choiceNodeChoiceContent = new List<string>();
                    choiceNodeChoiceContent.Add(script.TimerChoiceNodeDatas[i].DialogueNodePorts[j].PortGuid);

                    for (int k = 0; k < script.TimerChoiceNodeDatas[i].DialogueNodePorts[j].TextLanguage.Count; k++)
                    {
                        choiceNodeChoiceContent.Add(script.TimerChoiceNodeDatas[i].DialogueNodePorts[j].TextLanguage[k].LanguageGenericType);
                    }

                    string choiceNodeChoiceFinal = "";
                    foreach (string text in choiceNodeChoiceContent)
                    {
                        if (choiceNodeChoiceFinal != "") { choiceNodeChoiceFinal += "\t"; }
                        choiceNodeChoiceFinal += text;
                    }
                    tcvContent.Add(choiceNodeChoiceFinal);
                }
            }
            /* Generaowanie Tesktu*/

            tw = new StreamWriter(filePath, true);
            for (int i = 0; i < tcvContent.Count; i++)
            {
                tw.WriteLine($"{tcvContent[i]}");
            }
            tw.Close();

            Debug.Log("TSV file generated at: " + filePath);
        }

        public void ImportText()
        {
            // Define the file path for the text file
            string filePath = Application.dataPath + "/data.tsv";
            // Check if the file exists
            if (!File.Exists(filePath))
            {
                // Log an error if the file does not exist
                Debug.LogError("File does not exist at path: " + filePath);
                return;
            }

            try
            {
                // Open the file for reading
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string line;
                    bool headerSkipped = false;
                    // Read each line of the file
                    while ((line = sr.ReadLine()) != null)
                    {
                        // Skip the header line
                        if (!headerSkipped)
                        {
                            headerSkipped = true;
                            continue;
                        }

                        // Split the line into fields
                        string[] fields = line.Split('\t');

                        // Get GUID
                        string nodeGuid = fields[0];

                        // Update Dialogue Node data
                        for (int i = 0; i < script.DialogueNodeDatas.Count; i++)
                        {
                            if (nodeGuid == script.DialogueNodeDatas[i].NodeGuid)
                            {
                                // Update text for each language
                                for (int j = 0; j < fields.Length - 1; j++)
                                {
                                    script.DialogueNodeDatas[i].TextType[j].LanguageGenericType = fields[j + 1];
                                }
                            }
                        }

                        // Update Choice Node data
                        for (int i = 0; i < script.DialogueChoiceNodeDatas.Count; i++)
                        {
                            // Update text for Choice Node
                            if (nodeGuid == script.DialogueChoiceNodeDatas[i].NodeGuid)
                            {
                                // Update text for each language
                                for (int j = 0; j < fields.Length - 1; j++)
                                {
                                    script.DialogueChoiceNodeDatas[i].TextType[j].LanguageGenericType = fields[j + 1];
                                }
                            }
                            // Update text for Answer Nodes
                            for (int j = 0; j < script.DialogueChoiceNodeDatas[i].DialogueNodePorts.Count; j++)
                            {
                                if (script.DialogueChoiceNodeDatas[i].DialogueNodePorts[j].PortGuid == nodeGuid)
                                {
                                    // Update text for each language
                                    for (int k = 0; k < fields.Length - 1; k++)
                                    {
                                        script.DialogueChoiceNodeDatas[i].DialogueNodePorts[j].TextLanguage[k].LanguageGenericType = fields[k + 1];
                                    }
                                }
                            }
                        }
                    }
                }

                // Log success message
                Debug.Log("Text imported successfully.");
            }
            catch (Exception e)
            {
                // Log error message if an exception occurs
                Debug.LogError("Error while importing text: " + e.Message);
            }
        }
    }
}
