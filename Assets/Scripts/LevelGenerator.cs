using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public sealed class LevelGenerator : MonoBehaviour
{
    [SerializeField] private GridObject[] _templates;
    [SerializeField] private Transform _player;
    [SerializeField] private float _viewRadius;
    [SerializeField] private float _cellSize;
    [SerializeField] private Vector3Int _terrainPosition;
    [SerializeField] private int _minOnGroundYPosition;
    [SerializeField] private int _maxOnGroundYPosition;

    private readonly HashSet<Vector3Int> _collisionsMatrix = new();

    private void Start()
    {
        TryCreateOnLayer(GridLayer.Ground, _terrainPosition);
    }

    private void Update()
    {
        FillRadius(_player.position, _viewRadius);
    }

    private void FillRadius(Vector3 center, float viewRadius)
    {
        var cellCountOnAxis = (int)(viewRadius / _cellSize);
        var fillAreaCenter = WorldToGridPosition(center);

        for (var x = -cellCountOnAxis; x < cellCountOnAxis; x++)
        {
            for (var z = -cellCountOnAxis; z < cellCountOnAxis; z++)
                TryCreateOnLayer(GridLayer.OnGround, fillAreaCenter + new Vector3Int(x, 0, z));
        }
    }

    private void TryCreateOnLayer(GridLayer layer, Vector3Int gridPosition)
    {
        if (layer == GridLayer.OnGround)
            gridPosition.y = Random.Range(_minOnGroundYPosition, _maxOnGroundYPosition);

        if (_collisionsMatrix.Contains(gridPosition))
            return;
        
        _collisionsMatrix.Add(gridPosition);

        var template = GetRandomTemplate(layer);

        if (template == null)
            return;

        var position = GridToWorldPosition(gridPosition);

        Instantiate(template, position, Quaternion.identity, transform);
    }

    private GridObject GetRandomTemplate(GridLayer layer)
    {
        var variants = _templates.Where(template => template.Layer == layer);

        var gridObjects = variants as GridObject[] ?? variants.ToArray();
        
        if (gridObjects.Count() == 1)
            return gridObjects.First();

        const int MinChance = 0;
        const int MaxChance = 100;
        
        return gridObjects.FirstOrDefault(template => template.Chance > Random.Range(MinChance, MaxChance));
    }

    private Vector3 GridToWorldPosition(Vector3Int gridPosition)
    {
        return new Vector3(
            gridPosition.x * _cellSize,
            gridPosition.y * _cellSize,
            gridPosition.z * _cellSize);
    }

    private Vector3Int WorldToGridPosition(Vector3 worldPosition)
    {
        return new Vector3Int(
            (int)(worldPosition.x / _cellSize),
            (int)(worldPosition.y / _cellSize),
            (int)(worldPosition.z / _cellSize));
    }
}
