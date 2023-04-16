using System;

namespace VectorVanguard.Attributes
{
  /// <summary>
  /// Attribute to set the execution order of a script.
  /// Unity has DefaultExecutionOrder attribute but it is not adding the script to the execution order list.
  /// </summary>
  public class ScriptExecutionOrder : Attribute
  {
    public int order;
    public ScriptExecutionOrder(int order) { this.order = order; }
  }
}