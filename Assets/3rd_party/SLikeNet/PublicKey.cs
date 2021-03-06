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

public class PublicKey : IDisposable {
  private HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal PublicKey(IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new HandleRef(this, cPtr);
  }

  internal static HandleRef getCPtr(PublicKey obj) {
    return (obj == null) ? new HandleRef(null, IntPtr.Zero) : obj.swigCPtr;
  }

  ~PublicKey() {
    Dispose();
  }

  public virtual void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          SLikeNetPINVOKE.delete_PublicKey(swigCPtr);
        }
        swigCPtr = new HandleRef(null, IntPtr.Zero);
      }
      GC.SuppressFinalize(this);
    }
  }

  public PublicKeyMode publicKeyMode {
    set {
      SLikeNetPINVOKE.PublicKey_publicKeyMode_set(swigCPtr, (int)value);
    } 
    get {
      PublicKeyMode ret = (PublicKeyMode)SLikeNetPINVOKE.PublicKey_publicKeyMode_get(swigCPtr);
      return ret;
    } 
  }

  public string remoteServerPublicKey {
    set {
      SLikeNetPINVOKE.PublicKey_remoteServerPublicKey_set(swigCPtr, value);
    } 
    get {
      string ret = SLikeNetPINVOKE.PublicKey_remoteServerPublicKey_get(swigCPtr);
      return ret;
    } 
  }

  public string myPublicKey {
    set {
      SLikeNetPINVOKE.PublicKey_myPublicKey_set(swigCPtr, value);
    } 
    get {
      string ret = SLikeNetPINVOKE.PublicKey_myPublicKey_get(swigCPtr);
      return ret;
    } 
  }

  public string myPrivateKey {
    set {
      SLikeNetPINVOKE.PublicKey_myPrivateKey_set(swigCPtr, value);
    } 
    get {
      string ret = SLikeNetPINVOKE.PublicKey_myPrivateKey_get(swigCPtr);
      return ret;
    } 
  }

  public PublicKey() : this(SLikeNetPINVOKE.new_PublicKey(), true) {
  }

}

}
