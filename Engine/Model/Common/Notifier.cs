﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Engine.Model.Common
{
  public abstract class Notifier<TNotifierContext> : MarshalByRefObject
  {
    private readonly object syncObject = new object();
    private List<TNotifierContext> contexts;

    public Notifier()
    {
      contexts = new List<TNotifierContext>();
    }

    public void Add(TNotifierContext context)
    {
      lock (syncObject)
        contexts.Add(context);
    }

    public bool Remove(TNotifierContext context)
    {
      lock (syncObject)
        return contexts.Remove(context);
    }

    protected virtual void Notify<TArgs>(Action<TNotifierContext, TArgs> methodInvoker, TArgs args) where TArgs : EventArgs
    {
      TNotifierContext[] localContexts;
      lock (syncObject)
        localContexts = contexts.ToArray();

      foreach (var context in localContexts)
        methodInvoker(context, args);
    }
  }
}
