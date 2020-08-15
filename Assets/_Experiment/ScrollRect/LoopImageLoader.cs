using System;
using System.Collections;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using UnityEngine;
using UnityEngine.Assertions;

public class LoopImageLoader : MonoBehaviour, IEnhancedScrollerDelegate
{
    public CellItem _cellItemAsset;
    
    private EnhancedScroller _enhancedScroller;

    private List<string> _urlList = new List<string>()
    {
        "https://static.wixstatic.com/media/4a0a97_1f69f9ec04654e3aa23fd1546e907c5d~mv2.jpg/v1/fill/w_200,h_200,al_c,q_80/4a0a97_1f69f9ec04654e3aa23fd1546e907c5d~mv2.jpg", 
        "https://static.wixstatic.com/media/4a0a97_7636649a1ff9458aa8e3da3894c21c30~mv2.jpg/v1/fill/w_200,h_200,al_c,q_80/4a0a97_7636649a1ff9458aa8e3da3894c21c30~mv2.jpg", 
        "https://static.wixstatic.com/media/4a0a97_cebe144a25af4074a4c9c4e117f5151f~mv2.jpg/v1/fill/w_200,h_200,al_c,q_80/4a0a97_cebe144a25af4074a4c9c4e117f5151f~mv2.jpg", 
        "https://static.wixstatic.com/media/4a0a97_942a74dcc9904220ad8a083175525065~mv2.jpg/v1/fill/w_200,h_200,al_c,q_80/4a0a97_942a74dcc9904220ad8a083175525065~mv2.jpg", 
        "https://static.wixstatic.com/media/4a0a97_a850de011c6d466d846e11bb2b17c76c~mv2.jpg/v1/fill/w_200,h_200,al_c,q_80/4a0a97_a850de011c6d466d846e11bb2b17c76c~mv2.jpg", 
        "https://static.wixstatic.com/media/4a0a97_26e3de48092d43109dbce7cbd416e55d~mv2.jpg/v1/fill/w_200,h_200,al_c,q_80/4a0a97_26e3de48092d43109dbce7cbd416e55d~mv2.jpg", 
        "https://static.wixstatic.com/media/4a0a97_cdee6730fab84e57812cf8fc35b5691e~mv2.jpg/v1/fill/w_200,h_200,al_c,q_80/4a0a97_cdee6730fab84e57812cf8fc35b5691e~mv2.jpg", 
        "https://static.wixstatic.com/media/4a0a97_d01a681b6d474221b1896fefd6f4a15c~mv2.jpg/v1/fill/w_200,h_200,al_c,q_80/4a0a97_d01a681b6d474221b1896fefd6f4a15c~mv2.jpg", 
        "https://static.wixstatic.com/media/4a0a97_29761607d78d46de8815c7bca0e12905~mv2.jpg/v1/fill/w_200,h_200,al_c,q_80/4a0a97_29761607d78d46de8815c7bca0e12905~mv2.jpg", 
        "https://static.wixstatic.com/media/4a0a97_3dd5926473b9464889f3a4a0e88b79e2~mv2.jpg/v1/fill/w_200,h_200,al_c,q_80/4a0a97_3dd5926473b9464889f3a4a0e88b79e2~mv2.jpg", 
        "https://static.wixstatic.com/media/4a0a97_81a0d4d103294848863f0d2e6e955c2a~mv2.jpg/v1/fill/w_200,h_200,al_c,q_80/4a0a97_81a0d4d103294848863f0d2e6e955c2a~mv2.jpg", 
        "https://static.wixstatic.com/media/4a0a97_223f9ee4eb994faba6bdab9ee705c4c5~mv2.jpg/v1/fill/w_200,h_200,al_c,q_80/4a0a97_223f9ee4eb994faba6bdab9ee705c4c5~mv2.jpg", 
        "https://static.wixstatic.com/media/4a0a97_7221aa8567a1492b9d06d0f46cb6421c~mv2.jpg/v1/fill/w_200,h_200,al_c,q_80/4a0a97_7221aa8567a1492b9d06d0f46cb6421c~mv2.jpg", 
    };
    
    private void Awake()
    {
        _enhancedScroller = GetComponent<EnhancedScroller>();
    }

    private void OnEnable()
    {
        InitializeData();

        RegisterEvents();
    }

    private void OnDisable()
    {
        UnRegisterEvents();
    }

    private void UnRegisterEvents()
    {
        _enhancedScroller.cellViewWillRecycle -= OncellViewWillRecycle;
        
        _enhancedScroller.scrollerScrolled -= OnScrollerScrolled;
    }

    private void InitializeData()
    {
        // nothing
    }

    private void RegisterEvents()
    {
        _enhancedScroller.Delegate = this;
        _enhancedScroller.cellViewWillRecycle += OncellViewWillRecycle;

        _enhancedScroller.scrollerScrolled += OnScrollerScrolled;
    }

    public const int NEAR_BY_SIZE = 5;
    private int _lastIndex = -1;
    
    private HashSet<int> _loadedSet = new HashSet<int>();
    
    private List<int> _clearList = new List<int>();
    private List<int> _loadList = new List<int>();
    
    private void OnScrollerScrolled(EnhancedScroller scroller, Vector2 val, float scrollposition)
    {
        int start = scroller.StartDataIndex;
        int end = scroller.EndDataIndex;

        int index = Mathf.RoundToInt((start + end) / 2.0f);
        if (index == _lastIndex)
        {
            // nothing
        }
        else
        {
            HandleNearby(index);
            _lastIndex = index;
        }
    }

    private void HandleNearby(int theIndex)
    {
        _clearList.Clear();
        _loadList.Clear();
        
        int lower = Mathf.Max(0, theIndex - NEAR_BY_SIZE);
        int upper = Mathf.Min(_urlList.Count, theIndex + NEAR_BY_SIZE);

        for (int i = lower; i < upper; ++i)
        {
            if (_loadedSet.Contains(i))
            {
                // nothing
            }
            else
            {
                _loadList.Add(i);
            }
        }

        foreach (var i in _loadedSet)
        {
            if (lower <= i && i < upper)
            {
                // in range
            }
            else
            {
                _clearList.Add(i);
            }
        }
        
        // reset
        _loadedSet.Clear();
        for (int i = lower; i < upper; ++i)
        {
            _loadedSet.Add(i);
        }
        
        // load it
        foreach (var i in _loadList)
        {
            string path = _urlList[i];
            
            ResourceManagerAsset.Instance.GetAssetAsset<Texture2D>(AssetWebRequestTextureLoader.Instance, path, this)
                .Then(_ => { })
                .Catch(ex => Debug.LogException(ex));
        }
        
        // clear it
        foreach (var i in _clearList)
        {
            string path = _urlList[i];
            
            ResourceManagerAsset.Instance.DestroyAsset(path, this);
        }
    }

    private void OncellViewWillRecycle(EnhancedScrollerCellView cellview)
    {
        // Destroy View
        CellItem cellItem = cellview as CellItem;
        Assert.IsNotNull(cellItem);
        
        cellItem.DestroyTexture();
    }

    // Events
    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return _urlList.Count;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return 200.0f;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        // Load View
        CellItem cellItem = scroller.GetCellView(_cellItemAsset) as CellItem;

        cellItem.name = "Cell " + dataIndex.ToString();

        string path = _urlList[dataIndex];
        cellItem.SetTexture(path);
        
        return cellItem;
    }
}
