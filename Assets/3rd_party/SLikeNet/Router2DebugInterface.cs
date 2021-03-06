/* ----------------------------------------------------------------------------
 * This file was automatically generated by SWIG (http://www.swig.org).
 * Version 2.0.12
 *
 * Do not make changes to this file unless you know what you are doing--modify
 * the SWIG interface file instead.
 * ----------------------------------------------------------------------------- */

namespace SLNet {

using System;
using System.Runtime.InteropServices;

public class Router2DebugInterface : IDisposable {
  private HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal Router2DebugInterface(IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new HandleRef(this, cPtr);
  }

  internal static HandleRef getCPtr(Router2DebugInterface obj) {
    return (obj == null) ? new HandleRef(null, IntPtr.Zero) : obj.swigCPtr;
  }

  ~Router2DebugInterface() {
    Dispose();
  }

  public virtual void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          SLikeNetPINVOKE.delete_Router2DebugInterface(swigCPtr);
        }
        swigCPtr = new HandleRef(null, IntPtr.Zero);
      }
      GC.SuppressFinalize(this);
    }
  }

  public Router2DebugInterface() : this(SLikeNetPINVOKE.new_Router2DebugInterface(), true) {
  }

  public virtual void ShowFailure(string message) {
    SLikeNetPINVOKE.Router2DebugInterface_ShowFailure(swigCPtr, message);
  }

  public virtual void ShowDiagnostic(string message) {
    SLikeNetPINVOKE.Router2DebugInterface_ShowDiagnostic(swigCPtr, message);
  }

}

}
