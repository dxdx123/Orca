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

public class RakNetListFilterQuery : IDisposable {
  private HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal RakNetListFilterQuery(IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new HandleRef(this, cPtr);
  }

  internal static HandleRef getCPtr(RakNetListFilterQuery obj) {
    return (obj == null) ? new HandleRef(null, IntPtr.Zero) : obj.swigCPtr;
  }

  ~RakNetListFilterQuery() {
    Dispose();
  }

  public virtual void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          SLikeNetPINVOKE.delete_RakNetListFilterQuery(swigCPtr);
        }
        swigCPtr = new HandleRef(null, IntPtr.Zero);
      }
      GC.SuppressFinalize(this);
    }
  }

    public FilterQuery this[int index]  
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

  public RakNetListFilterQuery() : this(SLikeNetPINVOKE.new_RakNetListFilterQuery__SWIG_0(), true) {
  }

  public RakNetListFilterQuery(RakNetListFilterQuery original_copy) : this(SLikeNetPINVOKE.new_RakNetListFilterQuery__SWIG_1(RakNetListFilterQuery.getCPtr(original_copy)), true) {
    if (SLikeNetPINVOKE.SWIGPendingException.Pending) throw SLikeNetPINVOKE.SWIGPendingException.Retrieve();
  }

  public RakNetListFilterQuery CopyData(RakNetListFilterQuery original_copy) {
    RakNetListFilterQuery ret = new RakNetListFilterQuery(SLikeNetPINVOKE.RakNetListFilterQuery_CopyData(swigCPtr, RakNetListFilterQuery.getCPtr(original_copy)), false);
    if (SLikeNetPINVOKE.SWIGPendingException.Pending) throw SLikeNetPINVOKE.SWIGPendingException.Retrieve();
    return ret;
  }

  public FilterQuery Get(uint position) {
    FilterQuery ret = new FilterQuery(SLikeNetPINVOKE.RakNetListFilterQuery_Get(swigCPtr, position), false);
    return ret;
  }

  public void Push(FilterQuery input, string file, uint line) {
    SLikeNetPINVOKE.RakNetListFilterQuery_Push(swigCPtr, FilterQuery.getCPtr(input), file, line);
    if (SLikeNetPINVOKE.SWIGPendingException.Pending) throw SLikeNetPINVOKE.SWIGPendingException.Retrieve();
  }

  public FilterQuery Pop() {
    FilterQuery ret = new FilterQuery(SLikeNetPINVOKE.RakNetListFilterQuery_Pop(swigCPtr), false);
    return ret;
  }

  public void Insert(FilterQuery input, uint position, string file, uint line) {
    SLikeNetPINVOKE.RakNetListFilterQuery_Insert__SWIG_0(swigCPtr, FilterQuery.getCPtr(input), position, file, line);
    if (SLikeNetPINVOKE.SWIGPendingException.Pending) throw SLikeNetPINVOKE.SWIGPendingException.Retrieve();
  }

  public void Insert(FilterQuery input, string file, uint line) {
    SLikeNetPINVOKE.RakNetListFilterQuery_Insert__SWIG_1(swigCPtr, FilterQuery.getCPtr(input), file, line);
    if (SLikeNetPINVOKE.SWIGPendingException.Pending) throw SLikeNetPINVOKE.SWIGPendingException.Retrieve();
  }

  public void Replace(FilterQuery input, FilterQuery filler, uint position, string file, uint line) {
    SLikeNetPINVOKE.RakNetListFilterQuery_Replace__SWIG_0(swigCPtr, FilterQuery.getCPtr(input), FilterQuery.getCPtr(filler), position, file, line);
    if (SLikeNetPINVOKE.SWIGPendingException.Pending) throw SLikeNetPINVOKE.SWIGPendingException.Retrieve();
  }

  public void Replace(FilterQuery input) {
    SLikeNetPINVOKE.RakNetListFilterQuery_Replace__SWIG_1(swigCPtr, FilterQuery.getCPtr(input));
    if (SLikeNetPINVOKE.SWIGPendingException.Pending) throw SLikeNetPINVOKE.SWIGPendingException.Retrieve();
  }

  public void RemoveAtIndex(uint position) {
    SLikeNetPINVOKE.RakNetListFilterQuery_RemoveAtIndex(swigCPtr, position);
  }

  public void RemoveAtIndexFast(uint position) {
    SLikeNetPINVOKE.RakNetListFilterQuery_RemoveAtIndexFast(swigCPtr, position);
  }

  public void RemoveFromEnd(uint num) {
    SLikeNetPINVOKE.RakNetListFilterQuery_RemoveFromEnd__SWIG_0(swigCPtr, num);
  }

  public void RemoveFromEnd() {
    SLikeNetPINVOKE.RakNetListFilterQuery_RemoveFromEnd__SWIG_1(swigCPtr);
  }

  public uint Size() {
    uint ret = SLikeNetPINVOKE.RakNetListFilterQuery_Size(swigCPtr);
    return ret;
  }

  public void Clear(bool doNotDeallocateSmallBlocks, string file, uint line) {
    SLikeNetPINVOKE.RakNetListFilterQuery_Clear(swigCPtr, doNotDeallocateSmallBlocks, file, line);
  }

  public void Preallocate(uint countNeeded, string file, uint line) {
    SLikeNetPINVOKE.RakNetListFilterQuery_Preallocate(swigCPtr, countNeeded, file, line);
  }

  public void Compress(string file, uint line) {
    SLikeNetPINVOKE.RakNetListFilterQuery_Compress(swigCPtr, file, line);
  }

}

}
