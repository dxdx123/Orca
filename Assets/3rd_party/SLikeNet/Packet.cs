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

public class Packet : IDisposable {
  private HandleRef swigCPtr;
  protected bool swigCMemOwn;

  internal Packet(IntPtr cPtr, bool cMemoryOwn) 
  {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = new HandleRef(this, cPtr);
  }

  internal static HandleRef getCPtr(Packet obj)
  {
     if (obj != null)
     {
        if (obj.dataIsCached)
        {
           obj.SetPacketData(obj.data, obj.data.Length); //If an individual index was modified we need to copy the data before passing to C++
        }
	obj.dataIsCached=false;
     }
     return (obj == null) ? new HandleRef(null, IntPtr.Zero) : obj.swigCPtr;
  }

  ~Packet() {
    Dispose();
  }

  public virtual void Dispose() {
    lock(this) {
      if (swigCPtr.Handle != IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          SLikeNetPINVOKE.delete_Packet(swigCPtr);
        }
        swigCPtr = new HandleRef(null, IntPtr.Zero);
      }
      GC.SuppressFinalize(this);
    }
  }

    private bool dataIsCached = false;
    private byte[] dataCache;

  public SystemAddress systemAddress {
    set {
      SLikeNetPINVOKE.Packet_systemAddress_set(swigCPtr, SystemAddress.getCPtr(value));
    } 
    get {
      IntPtr cPtr = SLikeNetPINVOKE.Packet_systemAddress_get(swigCPtr);
      SystemAddress ret = (cPtr == IntPtr.Zero) ? null : new SystemAddress(cPtr, false);
      return ret;
    } 
  }

  public RakNetGUID guid {
    set {
      SLikeNetPINVOKE.Packet_guid_set(swigCPtr, RakNetGUID.getCPtr(value));
    } 
    get {
      IntPtr cPtr = SLikeNetPINVOKE.Packet_guid_get(swigCPtr);
      RakNetGUID ret = (cPtr == IntPtr.Zero) ? null : new RakNetGUID(cPtr, false);
      return ret;
    } 
  }

  public uint length {
    set {
      SLikeNetPINVOKE.Packet_length_set(swigCPtr, value);
    } 
    get {
      uint ret = SLikeNetPINVOKE.Packet_length_get(swigCPtr);
      return ret;
    } 
  }

  public uint bitSize {
    set {
      SLikeNetPINVOKE.Packet_bitSize_set(swigCPtr, value);
    } 
    get {
      uint ret = SLikeNetPINVOKE.Packet_bitSize_get(swigCPtr);
      return ret;
    } 
  }

  public byte[] data {
	set 
	{
	    	dataCache=value;
		dataIsCached = true;
		SetPacketData (value, value.Length);
	    

	}
get
        {
            byte[] returnBytes;
            if (!dataIsCached)
            {
                IntPtr cPtr = SLikeNetPINVOKE.Packet_data_get (swigCPtr);
                int len = (int)((Packet)swigCPtr.Wrapper).length;
		if (len<=0)
		{
			return null;
		}
                returnBytes = new byte[len];
                Marshal.Copy(cPtr, returnBytes, 0, len);
                dataCache = returnBytes;
                dataIsCached = true;
            }
            else
            {
                returnBytes = dataCache;
            }
            return returnBytes;
        }
  }

  public bool deleteData {
    set {
      SLikeNetPINVOKE.Packet_deleteData_set(swigCPtr, value);
    } 
    get {
      bool ret = SLikeNetPINVOKE.Packet_deleteData_get(swigCPtr);
      return ret;
    } 
  }

  public bool wasGeneratedLocally {
    set {
      SLikeNetPINVOKE.Packet_wasGeneratedLocally_set(swigCPtr, value);
    } 
    get {
      bool ret = SLikeNetPINVOKE.Packet_wasGeneratedLocally_get(swigCPtr);
      return ret;
    } 
  }

  public Packet() : this(SLikeNetPINVOKE.new_Packet(), true) {
  }

  public void SetPacketData(byte[] inByteArray, int numBytes) {
    SLikeNetPINVOKE.Packet_SetPacketData(swigCPtr, inByteArray, numBytes);
  }

}

}
