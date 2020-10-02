using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TestCollision : MonoBehaviour
{
    public Tilemap _tilemap;
    public TileBase _tileBase;

    // Start is called before the first frame update
    void Start()
    {
        _tilemap.SetTile(new Vector3Int(0, 0, 0), _tileBase);
    }

    // Update is called once per frame
    void Update()
    {
        List<Vector3Int> blocked = new List<Vector3Int>();

        foreach(var pos in _tilemap.cellBounds.allPositionsWithin)
        {
            var tile = _tilemap.GetTile(pos);
            if (tile != null)
                blocked.Add(pos);
        }
    }
}
