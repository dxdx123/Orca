using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class QuadTreeMapManager
{
    private const float MAP_WIDTH = 5.12f;
    private const float MAP_HEIGHT = 5.12f;
    
    private static QuadTreeMapManager _instance;

    public static QuadTreeMapManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new QuadTreeMapManager();
            }

            return _instance;
        }
    }

    private GameContext _gameContext;
    private string _mapName;
    private float _mapWidth;
    private float _mapHeight;
    
    private UQuadtree _quadtree;
    
    private QuadTreeMapManager()
    {}
    
    private Rect MapBound
    {
        get
        {
            float mapWidth = _mapWidth / 100.0f;
            float mapHeight = _mapHeight / 100.0f;
            
            return new Rect(0, 0, mapWidth, mapHeight);
        }
    }

    public void Initialize(GameContext gameContext, string mapName, float mapWidth, float mapHeight)
    {
        _gameContext = gameContext;
        _mapName = mapName;
        _mapWidth = mapWidth;
        _mapHeight = mapHeight;

        InitializeInternal();
    }

    private void InitializeInternal()
    {
        var config = new UQtConfig()
        {
            CellSizeThreshold = 5.12f,
            CellSwapInDist = 5.12f * 2.5f,
            CellSwapOutDist = 5.12f * 4.0f,
            Axis = QuadTreeAxis.XY,
        };
        
        _quadtree = new UQuadtree(MapBound, config);
        // _quadtree.EnableDebugLines = true;

        InitializeMapBlock();
    }

    private void InitializeMapBlock()
    {
        float mapWidthUnit = _mapWidth / 100.0f;
        float mapHeightUnit = _mapHeight / 100.0f;
        
        int mapWidth = Mathf.CeilToInt(mapWidthUnit / MAP_WIDTH);
        int mapHeight = Mathf.CeilToInt(mapHeightUnit / MAP_HEIGHT);
        
        for (int i = 0; i < mapWidth; ++i)
        {
            for (int j = 0; j < mapHeight; ++j)
            {
                MapWrapper mapWrapper = new MapWrapper(_gameContext, _mapName, i, j);
                
                _quadtree.Receive(mapWrapper);
            }
        }
    }

    public void Update(Vector2 position)
    {
        if (_quadtree != null)
        {
            // UCore.DrawRectXY(MapBound, 0.1f, Color.yellow);

            _quadtree.Update(position);
        }
    }
    
    class MapWrapper : IQtUserData
    {
        public const float HALF_WIDTH = MAP_WIDTH / 2;
        public const float HALF_HEIGHT = MAP_HEIGHT / 2;

        private GameContext _gameContext;
        private string _mapName;
        private int _x;
        private int _y;

        private GameEntity _mapEntity;
        private bool _loaded;
        
        public MapWrapper(GameContext gameContext, string mapName, int x, int y)
        {
            _gameContext = gameContext;
            _mapName = mapName;
            
            _x = x;
            _y = y;
        }
        
        public Vector3 GetCenter()
        {
            float x = (2 * _x + 1) * HALF_WIDTH;
            float y = (2 * _y + 1) * HALF_HEIGHT;
            float z = y;
            
            // 因为这里QuadTree坐标轴是 XZ
            return new Vector3(x, y, z);
        }

        public Vector3 GetExtends()
        {
            // 因为这里QuadTree坐标轴是 XZ
            return new Vector3(HALF_WIDTH, HALF_HEIGHT, HALF_HEIGHT);
        }

        public void SwapIn()
        {
            // Debug.Log($"!!! SwapIn {_x} {_y}");
            if (!_loaded)
            {
                SwapInInternal();
                _loaded = true;
            }
            else
            {
                // nothing
            }
        }

        private void SwapInInternal()
        {
            var position = new Vector3(_x * MAP_WIDTH, _y * MAP_HEIGHT, 0.0f);
            var rotation = Quaternion.identity;

            var gameObject = AssetPoolManager.Instance.SpawnMap(position, rotation);
            ViewController viewController = gameObject.GetComponent<ViewController>();

            _mapEntity = _gameContext.CreateEntity();
            viewController.Initialize(_mapEntity);

            _mapEntity.AddView(viewController);

            // add view
            string itemName = $"{_x.ToString()}_{_y.ToString()}";
            string dataPath = $"Assets/Res/_Map/{_mapName}/{itemName} Data/{itemName}.prefab";

            ResourceManager.Instance.GetAsset<GameObject>(dataPath, this)
                .Then(spriteCollectionGo =>
                {
                    var collectionData = spriteCollectionGo.GetComponent<tk2dSpriteCollectionData>();
                    var sprite = gameObject.GetComponent<tk2dSprite>();

                    sprite.SetSprite(collectionData, itemName);
                })
                .Catch(ex => { Debug.LogException(ex); });
        }

        public void SwapOut()
        {
            // Debug.LogWarning($"!!! SwapOut {_x} {_y}");
            if (_loaded)
            {
                SwapOutInternal();
                _loaded = false;
            }
            else
            {
                // nothing
            }
        }

        private void SwapOutInternal()
        {
            // return to pool
            ViewController viewController = _mapEntity.view.viewController as ViewController;
            Assert.IsNotNull(viewController);
            
            viewController.Destroy();

            // destroy asset
            string itemName = $"{_x.ToString()}_{_y.ToString()}";
            string dataPath = $"Assets/Res/_Map/{_mapName}/{itemName} Data/{itemName}.prefab";
            ResourceManager.Instance.DestroyAsset(dataPath, this);
        }

        public bool IsSwapInCompleted()
        {
            return true;
        }

        public bool IsSwapOutCompleted()
        {
            return true;
        }
    }
}
