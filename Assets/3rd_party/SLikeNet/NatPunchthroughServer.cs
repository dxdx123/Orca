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

public class NatPunchthroughServer : PluginInterface2 {
  private HandleRef swigCPtr;

  internal NatPunchthroughServer(IntPtr cPtr, bool cMemoryOwn) : base(SLikeNetPINVOKE.NatPunchthroughServer_SWIGUpcast(cPtr), cMemoryOwn) {
    swigCPtr = new HandleRef(this, cPtr);
  }

  internal static HandleRef getCPtr(NatPunchthroughServer obj) {
    return (obj == null) ? new HandleRef(null, IntPtr.Zero) : obj.swigCPtr;
  }

  ~NatPunchthroughServer() {
    Dispose();
  }

  public override void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          SLikeNetPINVOKE.delete_NatPunchthroughServer(swigCPtr);
        }
        swigCPtr = new HandleRef(null, IntPtr.Zero);
      }
      GC.SuppressFinalize(this);
      base.Dispose();
    }
  }

  public static NatPunchthroughServer GetInstance() {
    IntPtr cPtr = SLikeNetPINVOKE.NatPunchthroughServer_GetInstance();
    NatPunchthroughServer ret = (cPtr == IntPtr.Zero) ? null : new NatPunchthroughServer(cPtr, false);
    return ret;
  }

  public static void DestroyInstance(NatPunchthroughServer i) {
    SLikeNetPINVOKE.NatPunchthroughServer_DestroyInstance(NatPunchthroughServer.getCPtr(i));
  }

  public NatPunchthroughServer() : this(SLikeNetPINVOKE.new_NatPunchthroughServer(), true) {
  }

  public void SetDebugInterface(NatPunchthroughServerDebugInterface i) {
    SLikeNetPINVOKE.NatPunchthroughServer_SetDebugInterface(swigCPtr, NatPunchthroughServerDebugInterface.getCPtr(i));
  }

  public ulong lastUpdate {
    set {
      SLikeNetPINVOKE.NatPunchthroughServer_lastUpdate_set(swigCPtr, value);
    } 
    get {
      ulong ret = SLikeNetPINVOKE.NatPunchthroughServer_lastUpdate_get(swigCPtr);
      return ret;
    } 
  }

}

}
