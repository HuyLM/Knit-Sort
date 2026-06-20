using AtoGame.OtherModules.Assets.AtoUnity.OtherModules.Tutorial.Editor.GenerationKey;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace AtoGame.OtherModules.Tutorial
{
    [CustomEditor(typeof(TutorialConfig))]
    public class TutorialControllerInspector : Editor
    {
        private TutorialConfig tutorialConfig;
        private bool isChecked;
        protected virtual void OnEnable()
        {
            tutorialConfig = target as TutorialConfig;
            isChecked = false;
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("CheckData", GUILayout.Height(50)))
            {
                CheckDatas();
            }
            EditorGUI.BeginDisabledGroup(isChecked == false);
            if (GUILayout.Button("Generate Tutorial Keys", GUILayout.Height(50)))
            {
                CheckDatas();
                if(isChecked)
                {
                    GenerateTutorialKeys();
                }
            }
            EditorGUI.EndDisabledGroup();
        }

        private void CheckDatas()
        {
            isChecked = false;

            bool hasDupplicated = false;
            Dictionary<int, string> keyDic = new Dictionary<int, string>();
            keyDic.Add(0, "Empty");

            TutorialData[] tutorialDatas = tutorialConfig.GetTutorialDatas();
            for (int i = 0; i < tutorialDatas.Length; ++i)
            {
                if (CheckAddKeyDic(tutorialDatas[i].Key, tutorialDatas[i].KeyName, tutorialDatas[i].name, keyDic))
                {
                    keyDic.Add(tutorialDatas[i].Key, tutorialDatas[i].KeyName);
                }
                else
                {
                    hasDupplicated = true;
                }

            }
            TutorialData[] extraTutorialDatas = tutorialConfig.GetExtraTutorialDatas();
            for (int i = 0; i < extraTutorialDatas.Length; ++i)
            {
                if (CheckAddKeyDic(extraTutorialDatas[i].Key, extraTutorialDatas[i].KeyName, extraTutorialDatas[i].name, keyDic))
                {
                    keyDic.Add(extraTutorialDatas[i].Key, extraTutorialDatas[i].KeyName);
                }
                else
                {
                    hasDupplicated = true;
                }
            }
        
            bool endKeysNotFound = false;

            if(tutorialConfig.GetEndTutorialKeys() == null || tutorialConfig.GetEndTutorialKeys().Length == 0)
            {
                Debug.LogError($"[Tutorial-Editor] CheckEndTutorialKeys failed because empty endTutorialKeys field");
            }
            foreach(var endKey in tutorialConfig.GetEndTutorialKeys())
            {
                TutorialData tutorialData = tutorialConfig.FindTutorialData(endKey);
                if(tutorialData == null)
                {
                    Debug.LogError($"[Tutorial-Editor] CheckEndTutorialKeys failed because endKey = ({endKey}) not found");
                    endKeysNotFound = true;
                }
            }

            if (hasDupplicated == true)
            {
                return;
            }
            if (endKeysNotFound == true)
            {
                return;
            }
            isChecked = true;
        }

        private void GenerateTutorialKeys()
        {
            //Pop-up a file explorer and ask the client to click where to save the project.
            string outputPath = EditorUtility.SaveFilePanelInProject(
                                     title: "Save Location",
                                     defaultName: "TutorialKey",
                                     extension: "cs",
                                     message: "Where do you want to save this script?");

            //Create a new instance of our generator. 
            TutorialKeyGenerator generator = new TutorialKeyGenerator();

            //Create our session. 
            generator.Session = new Dictionary<string, object>();

            //Get the class name
            string className = Path.GetFileNameWithoutExtension(outputPath);

            //Save it to our session. 
            generator.Session["m_ClassName"] = className;

            //Grab all our layers from Unity. 
            bool hasDupplicated = false;
            Dictionary<int, string> keyDic = new Dictionary<int, string>();

            TutorialData[] tutorialDatas = tutorialConfig.GetTutorialDatas();
            for (int i = 0; i < tutorialDatas.Length; ++i)
            {
                if (CheckAddKeyDic(tutorialDatas[i].Key, tutorialDatas[i].KeyName, tutorialDatas[i].name, keyDic))
                {
                    keyDic.Add(tutorialDatas[i].Key, tutorialDatas[i].KeyName);
                }
                else
                {
                    hasDupplicated = true;
                }
              
            }
            TutorialData[] extraTutorialDatas = tutorialConfig.GetExtraTutorialDatas();
            for (int i = 0; i < extraTutorialDatas.Length; ++i)
            {
                if(CheckAddKeyDic(extraTutorialDatas[i].Key, extraTutorialDatas[i].KeyName, extraTutorialDatas[i].name, keyDic))
                {
                    keyDic.Add(extraTutorialDatas[i].Key, extraTutorialDatas[i].KeyName);
                }
                else
                {
                    hasDupplicated = true;
                }
            }
            if(hasDupplicated == true)
            {
                return;
            }

            //Add our layers to our generator. 
            generator.Session["m_TutorialKeys"] = keyDic;

            //Initialize the template (loads the values from the session into the template)
            generator.Initialize();

            //Generate the definition
            string classDef = generator.TransformText();

            //Write the class to disk
            File.WriteAllText(outputPath, classDef);

            //Tell Unity to refresh. 
            AssetDatabase.Refresh();
        }
        /// <summary>
        /// kiem tra key va keyName trong fileName co bi trung o trong keyDic hay khong
        /// </summary>
        /// <param name="key"></param>
        /// <param name="keyName"></param>
        /// <param name="fileName"></param>
        /// <param name="keyDic"></param>
        /// <returns></returns>
        private bool CheckAddKeyDic(int key, string keyName, string fileName, Dictionary<int, string> keyDic)
        {
            foreach(var value in keyDic)
            {
                if(value.Key == key)
                {
                    TutorialData tutorialData = tutorialConfig.FindTutorialData(value.Key);
                    if(tutorialData == null)
                    {
                        return false;
                    }
                    Debug.LogError($"[Tutorial-Editor] CheckAddKeyDic failed because key = ({key}) dupplicated in file = ({fileName}) and file = ({tutorialData.name})");
                    return false;
                } 
                if (value.Value.Equals(keyName))
                {
                    TutorialData tutorialData = tutorialConfig.FindTutorialData(value.Key);
                    if (tutorialData == null)
                    {
                        return false;
                    }
                    Debug.LogError($"[Tutorial-Editor] CheckAddKeyDic failed because name = ({keyName}) dupplicated in file = ({fileName}) and file = ({tutorialData.name})");
                    return false;
                }
            }
            return true;
        }
    }
}
