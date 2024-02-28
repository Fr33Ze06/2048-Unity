using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GameManager : MonoBehaviour
{
    [SerializeField] private int _width = 4;
    [SerializeField] private int _height = 4;
    [SerializeField] private Node _nodePrefab;
    [SerializeField] private Block _blockPrefab;
    [SerializeField] private SpriteRenderer _boardPrefab;
    [SerializeField] private List<BlockType> _types;
    private List<Node> _nodes;
    private List<Block> _blocks;
    private GameState _state;
    private int _round;

    private BlockType GetBlockTypeByValue(int value) => _types.First(t => t.Value == value);

    void Start()
    {
        ChangeState(GameState.GenerateLevel);
    }

    private void ChangeState(GameState newState)
    {
        _state = newState;

        switch (newState){
            case GameState.GenerateLevel:
                GenerateGrid();
                break;
            case GameState.SpawningBlocks:
                SpawnBlocks(_round++ == 0 ? 2 : 1);
                break;
            case GameState.WaitingInput:
                break;
            case GameState.Moving:
                break;
            case GameState.Win:
                break;
            case GameState.Lose:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }
    }

    void Update()
    {
        if (_state != GameState.WaitingInput) return;

        if (Input.GetKeyDown(KeyCode.LeftArrow)) Shift(Vector2.left);
        
    }

    void GenerateGrid()
    {
        _round = 0;
        _nodes = new List<Node>();
        _blocks = new List<Block>();
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var node = Instantiate(_nodePrefab, new Vector2(x,y), Quaternion.identity);
                _nodes.Add(node);
            }
        }
        var center = new Vector2((float) _width / 2 - 0.5f, (float) _height / 2 - 0.5f);

        var board = Instantiate(_boardPrefab, center, Quaternion.identity);
        board.size = new Vector2(_width, _height);
    
        Camera.main.transform.position = new Vector3(center.x, center.y, -10);
    
        ChangeState(GameState.SpawningBlocks);
    }

    void SpawnBlocks(int amount)
    {   
        var freeNodes = _nodes.Where(n => n.OccupiedBlock == null).OrderBy(b => Random.value).ToList();
        
        foreach (var node in freeNodes.Take(amount))
        {
            var block = Instantiate(_blockPrefab, node.Pos, Quaternion.identity);
            block.SetBlock(node);
            block.Init(GetBlockTypeByValue(Random.value > 0.8f ? 4 : 2));
        }

        if (freeNodes.Count() == 1){
            //Lost
            return;
        }
        ChangeState(GameState.WaitingInput);
    }

    void Shift(Vector2 dir){
        var orderedBlocks = _blocks.OrderBy(b => b.Pos.x).ThenBy(b => b.Pos.y);
        if (dir == Vector2.right || dir == Vector2.up) orderedBlocks.Reverse();
    
        foreach (var block in orderedBlocks){
            var next = block.Node;
            do {
                block.SetBlock(next); //8min video part2
            } while (true);
        }
    }

}

[Serializable]
public struct BlockType {
    public int Value;
    public Color Color;
}

public enum GameState {
    GenerateLevel,
    SpawningBlocks,
    WaitingInput,
    Moving,
    Win,
    Lose
}