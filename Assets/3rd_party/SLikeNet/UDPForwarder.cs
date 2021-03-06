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

public class UDPForwarder : IDisposable {
  private HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal UDPForwarder(IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new HandleRef(this, cPtr);
  }

  internal static HandleRef getCPtr(UDPForwarder obj) {
    return (obj == null) ? new HandleRef(null, IntPtr.Zero) : obj.swigCPtr;
  }

  ~UDPForwarder() {
    Dispose();
  }

  public virtual void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          SLikeNetPINVOKE.delete_UDPForwarder(swigCPtr);
        }
        swigCPtr = new HandleRef(null, IntPtr.Zero);
      }
      GC.SuppressFinalize(this);
    }
  }

  public UDPForwarder() : this(SLikeNetPINVOKE.new_UDPForwarder(), true) {
  }

  public void Startup() {
    SLikeNetPINVOKE.UDPForwarder_Startup(swigCPtr);
  }

  public void Shutdown() {
    SLikeNetPINVOKE.UDPForwarder_Shutdown(swigCPtr);
  }

  public void SetMaxForwardEntries(ushort maxEntries) {
    SLikeNetPINVOKE.UDPForwarder_SetMaxForwardEntries(swigCPtr, maxEntries);
  }

  public int GetMaxForwardEntries() {
    int ret = SLikeNetPINVOKE.UDPForwarder_GetMaxForwardEntries(swigCPtr);
    return ret;
  }

  public int GetUsedForwardEntries() {
    int ret = SLikeNetPINVOKE.UDPForwarder_GetUsedForwardEntries(swigCPtr);
    return ret;
  }

  public UDPForwarderResult StartForwarding(SystemAddress source, SystemAddress destination, uint timeoutOnNoDataMS, string forceHostAddress, ushort socketFamily, out ushort forwardingPort, out int forwardingSocket) {
    UDPForwarderResult ret = (UDPForwarderResult)SLikeNetPINVOKE.UDPForwarder_StartForwarding(swigCPtr, SystemAddress.getCPtr(source), SystemAddress.getCPtr(destination), timeoutOnNoDataMS, forceHostAddress, socketFamily, out forwardingPort, out forwardingSocket);
    if (SLikeNetPINVOKE.SWIGPendingException.Pending) throw SLikeNetPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public void StopForwarding(SystemAddress source, SystemAddress destination) {
    SLikeNetPINVOKE.UDPForwarder_StopForwarding(swigCPtr, SystemAddress.getCPtr(source), SystemAddress.getCPtr(destination));
    if (SLikeNetPINVOKE.SWIGPendingException.Pending) throw SLikeNetPINVOKE.SWIGPendingException.Retrieve();
  }

}

}
