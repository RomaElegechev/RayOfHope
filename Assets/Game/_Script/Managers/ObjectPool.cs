using PrimeTween;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TypePoolOverflow { Block, Replace }

public class ObjectPool : MonoBehaviour
{
    [SerializeField] private GameObject _prefab;
    [SerializeField] private int _beginPoolSize = 30;
    [SerializeField] private int _maxPoolSize = -1;
    [SerializeField] private TypePoolOverflow _overflowStrategy;


    public GameObject Prefab => _prefab;
    public bool IsPoolFull => _maxPoolSize != -1 && _activeObjects.Count >= _maxPoolSize;


    private Queue<GameObject> _pool = new Queue<GameObject>();
    private List<GameObject> _activeObjects = new List<GameObject>();
    private int _totalCreated;

    private void Awake()
    {
        for (int i = 0; i < _beginPoolSize; i++)
        {
            GameObject obj = Instantiate(_prefab, transform);
            obj.SetActive(false);
            _pool.Enqueue(obj);
            _totalCreated++;
        }
    }

    public GameObject Get()
    {
        if (_pool.Count > 0)
        {
            GameObject obj = _pool.Dequeue();
            _activeObjects.Add(obj);
            obj.SetActive(true);
            return obj;
        }
        else if (_totalCreated < _maxPoolSize)
        {
            GameObject obj = Instantiate(_prefab, transform);
            obj.SetActive(true);
            _activeObjects.Add(obj);
            _totalCreated++;
            return obj;
        }
        else
        {
            switch (_overflowStrategy)
            {
                case TypePoolOverflow.Block:
                    return null;
                case TypePoolOverflow.Replace:
                    Return(_activeObjects[0]);
                    GameObject obj = _pool.Dequeue();
                    _activeObjects.Add(obj);
                    obj.SetActive(true);
                    return obj;
            }
        }
        return null;
    }

    public void Return(GameObject obj, float delay = 0)
    {
        if (delay == 0)
        {
            if (!_activeObjects.Contains(obj)) return;

            ResetObject(obj);
            obj.SetActive(false);
            _activeObjects.Remove(obj);
            _pool.Enqueue(obj);
        }
        else
        {
            StartCoroutine(ReturnAfterDelay(obj, delay));
        }
    }

    private IEnumerator ReturnAfterDelay(GameObject obj, float delay)
    {
        yield return new WaitForSeconds(delay);
        Return(obj);
    }

    private void ResetObject(GameObject obj)
    {
        obj.transform.position = new Vector2(0, 6f);
        obj.transform.rotation = Quaternion.identity;

        
        Tween.StopAll(obj);
        Tween.StopAll(obj.transform);

        Rigidbody2D rb = obj.GetComponent<Rigidbody2D>();
        if (rb != null) rb.linearVelocity = Vector3.zero;

        Animator anim = obj.GetComponent<Animator>();
        if (anim != null)
        {
            anim.Rebind();
            anim.Update(0f);
        }

        TrailRenderer trail = obj.GetComponent<TrailRenderer>();
        if (trail != null) trail.Clear();
    }
}
