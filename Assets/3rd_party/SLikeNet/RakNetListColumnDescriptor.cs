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

public class RakNetListColumnDescriptor : IDisposable {
  private HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal RakNetListColumnDescriptor(IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new HandleRef(this, cPtr);
  }

  internal static HandleRef getCPtr(RakNetListColumnDescriptor obj) {
    return (obj == null) ? new HandleRef(null, IntPtr.Zero) : obj.swigCPtr;
  }

  ~RakNetListColumnDescriptor() {
    Dispose();
  }

  public virtual void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          SLikeNetPINVOKE.delete_RakNetListColumnDescriptor(swigCPtr);
        }
        swigCPtr = new HandleRef(null, IntPtr.Zero);
      }
      GC.SuppressFinalize(this);
    }
  }

    public ColumnDescriptor this[int index]  
    {  
        get   
        {
            return Get((uint)index); // use indexto retrieve and return another value.    
        }  
        set   
        {
            Replace(value, value, (uint)index, "Not used", 0);// use index and value to set the value somewhere.   
        }  
    } 

  public RakNetListColumnDescriptor() : this(SLikeNetPINVOKE.new_RakNetListColumnDescriptor__SWIG_0(), true) {
  }

  public RakNetListColumnDescriptor(RakNetListColumnDescriptor original_copy) : this(SLikeNetPINVOKE.new_RakNetListColumnDescriptor__SWIG_1(RakNetListColumnDescriptor.getCPtr(original_copy)), true) {
    if (SLikeNetPINVOKE.SWIGPendingException.Pending) throw SLikeNetPINVOKE.SWIGPendingException.Retrieve();
  }

  public RakNetListColumnDescriptor CopyData(RakNetListColumnDescriptor original_copy) {
    RakNetListColumnDescriptor ret = new RakNetListColumnDescriptor(SLikeNetPINVOKE.RakNetListColumnDescriptor_CopyData(swigCPtr, RakNetListColumnDescriptor.getCPtr(original_copy)), false);
    if (SLikeNetPINVOKE.SWIGPendingException.Pending) throw SLikeNetPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public ColumnDescriptor Get(uint position) {
    ColumnDescriptor ret = new ColumnDescriptor(SLikeNetPINVOKE.RakNetListColumnDescriptor_Get(swigCPtr, position), false);
    return ret;
  }

  public void Push(ColumnDescriptor input, string file, uint line) {
    SLikeNetPINVOKE.RakNetListColumnDescriptor_Push(swigCPtr, ColumnDescriptor.getCPtr(input), file, line);
    if (SLikeNetPINVOKE.SWIGPendingException.Pending) throw SLikeNetPINVOKE.SWIGPendingException.Retrieve();
  }

  public ColumnDescriptor Pop() {
    ColumnDescriptor ret = new ColumnDescriptor(SLikeNetPINVOKE.RakNetListColumnDescriptor_Pop(swigCPtr), false);
    return ret;
  }

  public void Insert(ColumnDescriptor input, uint position, string file, uint line) {
    SLikeNetPINVOKE.RakNetListColumnDescriptor_Insert__SWIG_0(swigCPtr, ColumnDescriptor.getCPtr(input), position, file, line);
    if (SLikeNetPINVOKE.SWIGPendingException.Pending) throw SLikeNetPINVOKE.SWIGPendingException.Retrieve();
  }

  public void Insert(ColumnDescriptor input, string file, uint line) {
    SLikeNetPINVOKE.RakNetListColumnDescriptor_Insert__SWIG_1(swigCPtr, ColumnDescriptor.getCPtr(input), file, line);
    if (SLikeNetPINVOKE.SWIGPendingException.Pending) throw SLikeNetPINVOKE.SWIGPendingException.Retrieve();
  }

  public void Replace(ColumnDescriptor input, ColumnDescriptor filler, uint position, string file, uint line) {
    SLikeNetPINVOKE.RakNetListColumnDescriptor_Replace__SWIG_0(swigCPtr, ColumnDescriptor.getCPtr(input), ColumnDescriptor.getCPtr(filler), position, file, line);
    if (SLikeNetPINVOKE.SWIGPendingException.Pending) throw SLikeNetPINVOKE.SWIGPendingException.Retrieve();
  }

  public void Replace(ColumnDescriptor input) {
    SLikeNetPINVOKE.RakNetListColumnDescriptor_Replace__SWIG_1(swigCPtr, ColumnDescriptor.getCPtr(input));
    if (SLikeNetPINVOKE.SWIGPendingException.Pending) throw SLikeNetPINVOKE.SWIGPendingException.Retrieve();
  }

  public void RemoveAtIndex(uint position) {
    SLikeNetPINVOKE.RakNetListColumnDescriptor_RemoveAtIndex(swigCPtr, position);
  }

  public void RemoveAtIndexFast(uint position) {
    SLikeNetPINVOKE.RakNetListColumnDescriptor_RemoveAtIndexFast(swigCPtr, position);
  }

  public void RemoveFromEnd(uint num) {
    SLikeNetPINVOKE.RakNetListColumnDescriptor_RemoveFromEnd__SWIG_0(swigCPtr, num);
  }

  public void RemoveFromEnd() {
    SLikeNetPINVOKE.RakNetListColumnDescriptor_RemoveFromEnd__SWIG_1(swigCPtr);
  }

  public uint Size() {
    uint ret = SLikeNetPINVOKE.RakNetListColumnDescriptor_Size(swigCPtr);
    return ret;
  }

  public void Clear(bool doNotDeallocateSmallBlocks, string file, uint line) {
    SLikeNetPINVOKE.RakNetListColumnDescriptor_Clear(swigCPtr, doNotDeallocateSmallBlocks, file, line);
  }

  public void Preallocate(uint countNeeded, string file, uint line) {
    SLikeNetPINVOKE.RakNetListColumnDescriptor_Preallocate(swigCPtr, countNeeded, file, line);
  }

  public void Compress(string file, uint line) {
    SLikeNetPINVOKE.RakNetListColumnDescriptor_Compress(swigCPtr, file, line);
  }

}

}
