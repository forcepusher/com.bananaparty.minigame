using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

// Wrapper around Unity's internal StandardShaderGUI that adds an Ambient Color field.
// Uses reflection so we keep the exact same UI/behavior as Standard plus our extra control.
public class StandardWithOverridesGUI : ShaderGUI
{
    private static bool _showAmbientSection = true;
    private MaterialProperty _ambientColor;

    // Reflected StandardShaderGUI instance + methods
    private object _innerStandardGui;
    private MethodInfo _innerFindProperties;
    private MethodInfo _innerOnGUI;

    private void EnsureInnerGui()
    {
        if (_innerStandardGui != null)
            return;

        // Type name used by Unity's built-in Standard inspector
        var editorAssembly = typeof(MaterialEditor).Assembly;
        var standardGuiType = editorAssembly.GetType("UnityEditor.StandardShaderGUI");
        if (standardGuiType == null)
            return; // Fallback: we'll just use default ShaderGUI

        _innerStandardGui = Activator.CreateInstance(standardGuiType);
        _innerFindProperties = standardGuiType.GetMethod(
            "FindProperties",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic
        );
        _innerOnGUI = standardGuiType.GetMethod(
            "OnGUI",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
            null,
            new[] { typeof(MaterialEditor), typeof(MaterialProperty[]) },
            null
        );
    }

    public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] props)
    {
        EnsureInnerGui();

        // Our extra ambient property FIRST, so it appears at the top
        _ambientColor = FindProperty("_AmbientColor", props, false);

        if (_ambientColor != null)
        {
            _showAmbientSection = EditorGUILayout.Foldout(_showAmbientSection, "Ambient Override", true);
            if (_showAmbientSection)
            {
                EditorGUI.indentLevel++;
                materialEditor.ColorProperty(_ambientColor, "Ambient Color");
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.Space();
        }

        // Then let the real Standard shader inspector draw everything it knows about
        if (_innerStandardGui != null && _innerOnGUI != null)
        {
            if (_innerFindProperties != null)
                _innerFindProperties.Invoke(_innerStandardGui, new object[] { props });

            _innerOnGUI.Invoke(_innerStandardGui, new object[] { materialEditor, props });
        }
        else
        {
            // Fallback: default material inspector
            base.OnGUI(materialEditor, props);
        }
    }
}

