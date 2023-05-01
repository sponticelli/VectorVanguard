using System;
using UnityEditor;
using VectorVanguard.Attributes;

namespace VectorVanguard.Editors
{
  [InitializeOnLoad]
  public class ScriptExecutionOrderManager
  {
    static ScriptExecutionOrderManager()
    {
      foreach (var script in MonoImporter.GetAllRuntimeMonoScripts())
      {
        var classType = script.GetClass();
        if (classType == null) continue;
        foreach (var attr in Attribute.GetCustomAttributes(classType, typeof(ScriptExecutionOrder)))
        {
          var newOrder = ((ScriptExecutionOrder)attr).order;
          if (MonoImporter.GetExecutionOrder(script) != newOrder) MonoImporter.SetExecutionOrder(script, newOrder);
        }
      }
    }
  }
}