using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public delegate void UQtCellChanged(UQtLeaf left, UQtLeaf entered);
public delegate void UQtCellSwapIn(UQtLeaf leaf);
public delegate void UQtCellSwapOut(UQtLeaf leaf);

public enum QuadTreeAxis
{
    XY,
    XZ,
}

public class UQtConfig
{
    // this value determines the smallest cell size
    // the space-partition process would stop dividing if cell size is smaller than this value
    public float CellSizeThreshold = 50.0f;

    // swap-in distance of cells
    public float CellSwapInDist = 100.0f;
    // swap-out distance of cells 
    //  (would be larger than swap-in to prevent poping)
    public float CellSwapOutDist = 150.0f;

    // time interval to update the focus point,
    // so that a new swap would potentially triggered (in seconds)
    public float SwapTriggerInterval = 0.5f;
    // time interval to update the in/out swapping queues (in seconds)
    public float SwapProcessInterval = 0.2f;

    public QuadTreeAxis Axis = QuadTreeAxis.XZ;
}

// user data stored in quadtree leaves
public interface IQtUserData
{
    Vector3 GetCenter();
    Vector3 GetExtends();

    void SwapIn();
    void SwapOut();

    bool IsSwapInCompleted();
    bool IsSwapOutCompleted();
}

public class UQtNode
{
    public UQtNode(Rect bound)
    {
        _bound = bound;
    }

    public Rect Bound { get { return _bound; } }
    protected Rect _bound;

    public virtual void SetSubNodes(UQtNode[] subNodes)
    {
        _subNodes = subNodes;
    }

    public virtual void Receive(IQtUserData userData)
    {
        if (!UQtAlgo.Intersects(Bound, userData))
        {
            return;
        }

        foreach (var sub in SubNodes)
        {
            sub.Receive(userData);
        }
    }

    public UQtNode[] SubNodes { get { return _subNodes; } }
    public const int SubCount = 4;
    protected UQtNode[] _subNodes = null;
}

public class UQtLeaf : UQtNode
{
    public UQtLeaf(Rect bound) : base(bound)
    {
    }
    public override void SetSubNodes(UQtNode[] subNodes)
    {
        UCore.Assert(false);
    }

    public override void Receive(IQtUserData userData)
    {
        if (!UQtAlgo.Intersects(Bound, userData))
            return;

        if (Bound.Contains(new Vector2(userData.GetCenter().x, userData.GetCenter().z)))
        {
            _ownedObjects.Add(userData);            
        }
        else
        {
            _affectedObjects.Add(userData);
        }
    }

    public bool Contains(IQtUserData userData)
    {
        if (_ownedObjects.Contains(userData))
            return true;
        if (_affectedObjects.Contains(userData))
            return true;
        return false;
    }

    public void SwapIn()
    {
        foreach (var obj in _ownedObjects)
            obj.SwapIn();
        
        foreach (var obj in _affectedObjects)
            obj.SwapIn();
    }

    public void SwapOut()
    {
        foreach (var obj in _ownedObjects)
            obj.SwapOut();
    }

    public bool IsSwapInCompleted()
    {
        foreach (var obj in _ownedObjects)
        {
            if (!obj.IsSwapInCompleted())
                return false;
        }
        foreach (var obj in _affectedObjects)
        {
            if (!obj.IsSwapInCompleted())
                return false;
        }
        return true;
    }

    public bool IsSwapOutCompleted()
    {
        foreach (var obj in _ownedObjects)
        {
            if (!obj.IsSwapOutCompleted())
                return false;
        }
        return true;
    }

    public List<IQtUserData> AffectedObjects { get { return _affectedObjects; } }

    private List<IQtUserData> _ownedObjects = new List<IQtUserData>();
    private List<IQtUserData> _affectedObjects = new List<IQtUserData>();
}

public class UQuadtree
{
    private UQtConfig _config;
    
    public UQuadtree(Rect bound, UQtConfig config)
    {
        _config = config;
        _root = new UQtNode(bound);
        UQtAlgo.BuildRecursively(_root, _config);
    }

    public void Update(Vector2 focusPoint)
    {
        if (EnableDebugLines)
        {
            DrawDebugLines();
        }

        if (Time.time - _lastSwapTriggeredTime > _config.SwapTriggerInterval)
        {
            if (UpdateFocus(focusPoint))
                PerformSwapInOut(_focusLeaf);
            _lastSwapTriggeredTime = Time.time;
        }

        if (Time.time - _lastSwapProcessedTime > _config.SwapProcessInterval)
        {
            ProcessSwapQueues();
            _lastSwapProcessedTime = Time.time;
        }
    }

    public void Receive(IQtUserData qud)
    {
        _root.Receive(qud);
    }

    public event UQtCellChanged FocusCellChanged;
    public event UQtCellSwapIn CellSwapIn;
    public event UQtCellSwapOut CellSwapOut;

    public Rect SceneBound { get { return _root.Bound; } }
    public Vector3 FocusPoint { get { return _focusPoint; } }
    public bool EnableDebugLines { get; set; }

    private void DrawDebugLines()
    {
        UQtAlgo.TraverseAllLeaves(_root, (leaf) => {
            Color c = Color.gray;

            if (leaf == _focusLeaf)
            {
                c = Color.blue;
            }
            else if (_swapInQueue.Contains(leaf))
            {
                c = Color.green;
            }
            else if (_swapOutQueue.Contains(leaf))
            {
                c = Color.red;
            }
            else if (_holdingLeaves.Contains(leaf))
            {
                c = Color.white;
            }

            if (_config.Axis == QuadTreeAxis.XY)
            {
                UCore.DrawRectXY(leaf.Bound, 0.1f, c, 1.0f); 
            }
            else if (_config.Axis == QuadTreeAxis.XZ)
            {
                UCore.DrawRectXZ(leaf.Bound, 0.1f, c, 1.0f); 
            }
            else
            {
                
            }
        });
    }

    private bool UpdateFocus(Vector2 focusPoint)
    {
        _focusPoint = focusPoint;

        UQtLeaf newLeaf = UQtAlgo.FindLeafRecursively(_root, _focusPoint);
        if (newLeaf == _focusLeaf)
            return false;

        if (FocusCellChanged != null)
            FocusCellChanged(_focusLeaf, newLeaf);

        _focusLeaf = newLeaf;
        return _focusLeaf != null;
    }

    private void PerformSwapInOut(UQtLeaf activeLeaf)
    {
        // 1. the initial in/out leaves generation
        List<UQtLeaf> inLeaves;
        List<UQtLeaf> outLeaves;
        UQtAlgo.GenerateSwappingLeaves(_root, activeLeaf, _holdingLeaves, out inLeaves, out outLeaves, _config);

        // 2. filter out leaves which are already in the ideal states
        inLeaves.RemoveAll((leaf) => { return _swapInQueue.Contains(leaf); });

        // 3. append these new items to in/out queue
        SwapIn(inLeaves);
        SwapOut(outLeaves);
    }

    private void SwapIn(List<UQtLeaf> inLeaves)
    {
        foreach (var leaf in inLeaves)
        {
            leaf.SwapIn();
            _swapInQueue.Add(leaf);
            if (CellSwapIn != null)
                CellSwapIn(leaf);
        }
    }

    private void SwapOut(List<UQtLeaf> outLeaves)
    {
        foreach (var leaf in outLeaves)
        {
            leaf.SwapOut();
            _holdingLeaves.Remove(leaf);

            var affected = leaf.AffectedObjects;
            foreach (var item in affected)
            {
                if (!IsHoldingUserData(item))
                    item.SwapOut();
            }

            _swapOutQueue.Add(leaf);
            if (CellSwapOut != null)
                CellSwapOut(leaf);
        }
    }

    private bool IsHoldingUserData(IQtUserData userData)
    {
        foreach (var hLeaf in _holdingLeaves)
        {
            if (hLeaf.Contains(userData))
            {
                return true;
            }
        }

        return false;
    }

    private void ProcessSwapQueues()
    {
        foreach (var leaf in _swapInQueue)
        {
            if (leaf.IsSwapInCompleted())
            {
                _holdingLeaves.Add(leaf);
            }
        }

        _swapInQueue.RemoveAll((leaf) => { return _holdingLeaves.Contains(leaf); });
        _swapOutQueue.RemoveAll((leaf) => { return leaf.IsSwapOutCompleted(); });
    }

    private UQtNode _root;

    private Vector3 _focusPoint;
    private UQtLeaf _focusLeaf;

    private List<UQtLeaf> _holdingLeaves = new List<UQtLeaf>();
    private List<UQtLeaf> _swapInQueue = new List<UQtLeaf>();
    private List<UQtLeaf> _swapOutQueue = new List<UQtLeaf>();

    private float _lastSwapTriggeredTime = float.MinValue;
    private float _lastSwapProcessedTime = float.MinValue;

}
