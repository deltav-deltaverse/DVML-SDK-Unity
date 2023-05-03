﻿using UnityEditor;
using UnityEngine;

namespace BlockadeLabsSDK
{
#if UNITY_EDITOR
    [CustomEditor(typeof(BlockadeImaginarium))]
    public class BlockadeImaginariumEditor : Editor
    {
        private SerializedProperty assignToMaterial;
        private SerializedProperty enableSkyboxGUI;
        private SerializedProperty apiKey;
        private SerializedProperty skyboxStyleFields;
        private SerializedProperty skyboxStyles;
        private SerializedProperty skyboxStyleOptions;
        private SerializedProperty skyboxStyleOptionsIndex;
        private bool showApi = true;
        private bool showBasic;
        private bool showSkybox;

        void OnEnable()
        {
            assignToMaterial = serializedObject.FindProperty("assignToMaterial");
            enableSkyboxGUI = serializedObject.FindProperty("enableSkyboxGUI");
            apiKey = serializedObject.FindProperty("apiKey");
            skyboxStyleFields = serializedObject.FindProperty("skyboxStyleFields");
            skyboxStyles = serializedObject.FindProperty("skyboxStyles");
            skyboxStyleOptions = serializedObject.FindProperty("skyboxStyleOptions");
            skyboxStyleOptionsIndex = serializedObject.FindProperty("skyboxStyleOptionsIndex");
        }

        public override void OnInspectorGUI()
        {
            EditorUtility.SetDirty(target);

            serializedObject.Update();

            var blockadeImaginarium = (BlockadeImaginarium)target;

            showApi = EditorGUILayout.Foldout(showApi, "API");

            if (showApi)
            {
                EditorGUILayout.PropertyField(apiKey);
                GUILayout.Space(5);
                
                EditorGUILayout.BeginHorizontal();
                
                EditorGUILayout.LabelField("Initialize/Refresh");

                if (skyboxStyleFields.arraySize > 0)
                {
                    if (GUILayout.Button("Refresh"))
                    {
                        _ = blockadeImaginarium.GetSkyboxStyleOptions();
                    }
                }
                else
                {
                    if (GUILayout.Button("Initialize"))
                    {
                        _ = blockadeImaginarium.GetSkyboxStyleOptions();
                    }
                }

                EditorGUILayout.EndHorizontal();
            }

            if (skyboxStyleFields.arraySize > 0)
            {
                showBasic = EditorGUILayout.Foldout(showBasic, "Basic Settings");

                if (showBasic)
                {
                    EditorGUILayout.PropertyField(enableSkyboxGUI);
                    EditorGUILayout.PropertyField(assignToMaterial);
                }

                if (!EditorApplication.isPlayingOrWillChangePlaymode)
                {
                    showSkybox = EditorGUILayout.Foldout(showSkybox, "Skybox Generator");

                    if (showSkybox)
                    {
                        // Iterate over skybox style fields and render them in the GUI
                        if (blockadeImaginarium.skyboxStyleFields.Count > 0)
                        {
                            RenderSkyboxEditorFields(blockadeImaginarium);
                        }
                    
                        if (blockadeImaginarium.PercentageCompleted() >= 0 && blockadeImaginarium.PercentageCompleted() < 100)
                        {
                            if (GUILayout.Button("Cancel (" + blockadeImaginarium.PercentageCompleted() + "%)"))
                            {
                                blockadeImaginarium.Cancel();
                            }
                        }
                        else
                        {
                            if (GUILayout.Button("Generate Skybox"))
                            {
                                _ = blockadeImaginarium.InitializeSkyboxGeneration(
                                    blockadeImaginarium.skyboxStyleFields,
                                    blockadeImaginarium.skyboxStyles[blockadeImaginarium.skyboxStyleOptionsIndex].id
                                );
                            }
                        }
                    }
                }
            }
            
            serializedObject.ApplyModifiedProperties();
        }

        private void RenderSkyboxEditorFields(BlockadeImaginarium blockadeImaginarium)
        {
            // Begin horizontal layout
            EditorGUILayout.BeginHorizontal();
            
            // Create label for the style field
            EditorGUILayout.LabelField("Style", GUILayout.MinWidth(100));
            
            EditorGUI.BeginChangeCheck();

            blockadeImaginarium.skyboxStyleOptionsIndex = EditorGUILayout.Popup(
                blockadeImaginarium.skyboxStyleOptionsIndex,
                blockadeImaginarium.skyboxStyleOptions,
                GUILayout.Width(260)
            );

            if (EditorGUI.EndChangeCheck())
            {
                blockadeImaginarium.GetSkyboxStyleFields(blockadeImaginarium.skyboxStyleOptionsIndex);
            }
            
            // End horizontal layout
            EditorGUILayout.EndHorizontal();

            foreach (var field in blockadeImaginarium.skyboxStyleFields)
            {
                // Begin horizontal layout
                EditorGUILayout.BeginHorizontal();
                
                // Create label for field
                EditorGUILayout.LabelField(field.name, GUILayout.MinWidth(100));
            
                // Create text field for field value
                field.value = EditorGUILayout.TextArea(field.value,  GUILayout.Height(100), GUILayout.Width(260));
            
                // End horizontal layout
                EditorGUILayout.EndHorizontal();
            }
        }
    }
#endif
}