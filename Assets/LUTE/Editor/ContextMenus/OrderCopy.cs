using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

//Generates a context menu item which when clicked on creates a copy of the selected order class 
//and places it in the same folder as the original order class.
//This is useful for creating new orders that are similar to existing orders.
//Users can either modify the new order or use it as a starting point for a new order.
public class OrderCopy : MonoBehaviour
{
    [MenuItem("Assets/LUTE/Orders/Copy Order")]
    public static void CopyOrder()
    {
        //Get the selected order class
        var order = UnityEditor.Selection.activeObject as MonoScript;
        if (order == null)
        {
            Debug.LogError("No order selected");
            return;
        }

        var newType = order.GetClass();
        if (!newType.IsSubclassOf(typeof(Order)))
        {
            Debug.LogError("Selected class is not a subclass of Order");
            return;
        }

        //Get the path of the selected order class
        var path = UnityEditor.AssetDatabase.GetAssetPath(order);
        if (path == null)
        {
            Debug.LogError("No path found for order");
            return;
        }

        //Get the name of the selected order class
        var name = order.name;
        if (name == null)
        {
            Debug.LogError("No name found for order");
            return;
        }

        //Get the folder path of the selected order class
        var folderPath = System.IO.Path.GetDirectoryName(path);
        if (folderPath == null)
        {
            Debug.LogError("No folder path found for order");
            return;
        }

        //Get the text of the selected order class
        var text = System.IO.File.ReadAllText(path);
        if (text == null)
        {
            Debug.LogError("No text found for order");
            return;
        }

        string baseClassCategory = "";
        // Search for the first occurrence of "OrderInfo("
        int startIndex = text.IndexOf("OrderInfo(");

        // Check if "OrderInfo(" is found
        if (startIndex != -1)
        {
            // Move the index to the end of "OrderInfo("
            startIndex += "OrderInfo(".Length;

            // Find the closing parenthesis after the "OrderInfo("
            int endIndex = text.IndexOf(")", startIndex);

            // Check if the closing parenthesis is found
            if (endIndex != -1)
            {
                // Extract the substring between "OrderInfo(" and ")"
                baseClassCategory = text.Substring(startIndex, endIndex - startIndex);
            }
            else
            {
                Debug.LogWarning("Closing parenthesis not found.");
            }
        }
        else
        {
            Debug.LogWarning("OrderInfo( not found in the input text.");
        }

        //Create a new order class with the same text as the selected order class
        var newPath = folderPath + "/" + name + "Custom.cs";
        //if the order already exists then we must add a number to the end of the file name
        int i = 1;
        while (System.IO.File.Exists(newPath))
        {
            newPath = folderPath + "/" + name + "Custom" + i + ".cs";
            i++;
        }

        //in some cases, the name of the base class is two or more words that use camel case and we need to add a space between them to ensure they are found
        string spacedName = ConvertCamelCaseToSpace(name);
        baseClassCategory = baseClassCategory.Replace(spacedName, "Custom " + spacedName).Trim();

        string className = name + "Custom : ";
        if (i > 1)
        {
            className = "";
            className = name + "Custom" + (i - 1) + " : ";
            baseClassCategory = baseClassCategory.Replace("Custom " + spacedName, "Custom " + spacedName + (i - 1)).Trim();
        }

        // Generate using statements as a single string
        string usingStatementsString = FindUsingStatements(text);

        string helpText = "Derived from " + name + " but with custom behavior.";
        string classText =
            usingStatementsString + "\n\n" +
            "[OrderInfo(" + baseClassCategory + ")]  \n" +
            "[AddComponentMenu(\"\")]\n" +
            "public class " + className + name + "\n" +
            "{\n" +
            "    public override void OnEnter()\n" +
            "    {\n" +
            "        base.OnEnter();\n" +
            "    }\n" +
            "\n" +
            "    public override string GetSummary()\n" +
            "    {\n" +
            "        return base.GetSummary();\n" +
            "    }\n" +
            "}";

        System.IO.File.WriteAllText(newPath, classText);
        //search for an editor file for the selected order class
        string editorFile = FindFileInDirectory(Application.dataPath + "/LUTE/Editor", name + "Editor" + ".cs");
        if (editorFile != null)
        {
            //if there is an editor file then we must create a new one for the new type of order we created
            string editorFilePath = System.IO.Path.GetDirectoryName(editorFile);
            string editorUsingStatementsString = FindUsingStatements(System.IO.File.ReadAllText(editorFile));
            string typeOf = "typeof(" + name + "Custom))]";
            string fileName = name + "CustomEditor.cs";
            string editorClassName = name + "CustomEditor : " + name + "Editor\n";
            if (i > 1)
            {
                typeOf = "typeof(" + name + "Custom" + (i - 1) + "))]";
                fileName = name + "CustomEditor" + (i - 1) + ".cs";
                editorClassName = name + "CustomEditor" + (i - 1) + " : " + name + "Editor\n";
            }
            string classEditorText =
                editorUsingStatementsString + "\n\n" +
                "[CustomEditor(" + typeOf + "\n" +
                "public class " + editorClassName +
                "{\n" +
                "    public override void OnInspectorGUI()\n" +
                "    {\n" +
                "        base.OnInspectorGUI();\n" +
                "    }\n" +
                "}\n";
            System.IO.File.WriteAllText(editorFilePath + "/" + fileName, classEditorText);
        }

        UnityEditor.AssetDatabase.Refresh();
    }

    static string FindUsingStatements(string classFileContent)
    {
        // Define a regular expression pattern for finding using statements
        string pattern = @"^using\s[^\r\n]*;";

        // Use Regex to find matches
        MatchCollection matches = Regex.Matches(classFileContent, pattern, RegexOptions.Multiline);

        // Concatenate the matches into a single string
        string usingStatementsString = string.Join(Environment.NewLine, matches.Cast<Match>().Select(match => match.Value));

        return usingStatementsString;
    }

    static string FindFileInDirectory(string directory, string targetFile)
    {
        foreach (var file in Directory.GetFiles(directory))
        {
            if (Path.GetFileName(file) == targetFile)
            {
                return file;
            }
        }

        foreach (var subdirectory in Directory.GetDirectories(directory))
        {
            var result = FindFileInDirectory(subdirectory, targetFile);
            if (result != null)
            {
                return result;
            }
        }

        return null;
    }

    static string ConvertCamelCaseToSpace(string input)
    {
        // Use regular expression to find camel case occurrences
        string pattern = @"(?:\b|[a-z](?=[A-Z])|[A-Z](?=[A-Z][a-z]))";
        Regex regex = new Regex(pattern);

        // Replace camel case occurrences with spaced string using MatchEvaluator
        string spacedString = regex.Replace(input, m => m.Value + " ");

        // Trim any leading or trailing spaces
        spacedString = spacedString.Trim();

        return spacedString;
    }
}
