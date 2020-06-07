using System.Collections;
using System.Collections.Generic;
using Entitas;
using UniRx;
using UnityEngine;

public class SheepManager
{
    private static SheepManager _instance;
    
    public static SheepManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new SheepManager();
            }

            return _instance;
        }
    }

    private List<GameEntity> _sheepList;
    private HashSet<GameEntity> _sheepBlowSet;
    
    private SheepManager()
    {}
    
    public void Initialize(List<GameEntity> sheepList)
    {
        _sheepList = sheepList;
        
        _sheepBlowSet = new HashSet<GameEntity>();
    }

    public void BlowSheep(GameEntity entity)
    {
        BlowSheepInternal(entity);
        
        MainThreadDispatcher.StartUpdateMicroCoroutine(BlowNextSheep(entity));
    }

    private IEnumerator BlowNextSheep(GameEntity entity)
    {
        while (true)
        {
            float time = 0.0f;
            while (time <= 0.2f)
            {
                yield return null;
                time += Time.deltaTime;
            }

            GameEntity nextSheep = FindNextToBlow(entity);
            if (nextSheep == null)
            {
                _sheepBlowSet.Clear();
                yield break;
            }
            else
            {
                BlowSheepInternal(nextSheep);
                entity = nextSheep;
            }
        }
    }

    private void BlowSheepInternal(GameEntity entity)
    {
        entity.ReplaceState(CharacterState.Die);
        _sheepBlowSet.Add(entity);
    }

    private GameEntity FindNextToBlow(GameEntity entity)
    {
        GameEntity nextEntity = null;

        float minDistance = float.MaxValue;
        
        foreach (var e in _sheepList)
        {
            if (e == entity)
            {
                // nothing
            }
            else
            {
                if (_sheepBlowSet.Contains(e))
                {
                    // nothing
                }
                else
                {
                    // distance
                    Vector2 srcPos = new Vector2(entity.position.x, entity.position.y);
                    Vector2 destPos = new Vector2(e.position.x, e.position.y);

                    float distance = (destPos - srcPos).SqrMagnitude();
                    if (distance < minDistance)
                    {
                        minDistance = distance;
                        nextEntity = e;
                    }
                    else
                    {
                        // nothing
                    }
                }
            }
        }

        return nextEntity;
    }
}
