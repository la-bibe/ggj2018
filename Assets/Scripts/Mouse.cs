﻿using UnityEngine;
using UnityEngine.Tilemaps;

public class Mouse : MonoBehaviour
{
    public Tile PathRenderTile;
    public Tile MouseRenderTile;

    private Map _map;
    private Tilemap _renderMap;
    private PlayerController _player;
    private Vector3Int _prevPos;

    private void Awake()
    {
        _map = FindObjectOfType<Map>();
        _player = FindObjectOfType<PlayerController>();
    }

    private void Start()
    {
        _prevPos = _player.Position;
        GameObject tileMapGameObject = new GameObject("__renderPathTileMap") {tag = "Ignore"};
        tileMapGameObject.transform.parent = _map.transform;
        _renderMap = tileMapGameObject.AddComponent<Tilemap>();
        TilemapRenderer tilemapRenderer = tileMapGameObject.AddComponent<TilemapRenderer>();
        tilemapRenderer.sortingLayerName = "Path";
    }

    private Vector3Int PosOnMap()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return new Vector3Int((int) (mousePos.x - _map.X), (int) (mousePos.y - _map.Y), 0);
    }

    private bool IsValidCoord(Vector3Int pos)
    {
        return _map.Width > pos.x && pos.x >= 0 && _map.Height > pos.y && pos.y >= 0;
    }

    private void RenderPath()
    {
        _renderMap.ClearAllTiles();
        Path path = _map.NavigateTo(_player.Position, _prevPos);
        if (path == null) return;
        _renderMap.SetTile(_prevPos + _map.TopLeft, MouseRenderTile);
        int i = _player.RemainingMoves;
        while (path.Length != 0 && i > 0)
        {
            Vector3Int node = path.Next();
            _renderMap.SetTile(node + _map.TopLeft, PathRenderTile);
            i -= 1;
        }
    }

    private void Update()
    {
        if (!GameManager.Instance.PlayerTurn || _player.IsMoving || _player.HasObjective()) return;
        Vector3Int mousePosition = PosOnMap();
        if (!IsValidCoord(mousePosition)) return;
        if (Input.GetMouseButton(1))
        {
            _renderMap.ClearAllTiles();
            _player.SetObjective(mousePosition);
        }
        else if (_prevPos != mousePosition)
        {
            _prevPos = mousePosition;
            RenderPath();
        }
    }
}