using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    [SerializeField] private ShooterGridSystem shooterGridSystem;
    [SerializeField] public ColorCubeGridSystem colorCubeGridSystem;
    [SerializeField] private ReservedSlotGridSystem reservedSlotGridSystem;
    [SerializeField] private Conveyor conveyor;

    public LevelData LevelData { get; private set; }
    public Conveyor Conveyor => conveyor;
    public ShooterGridSystem ShooterGridSystem => shooterGridSystem;
    public ReservedSlotGridSystem ReservedSlotGridSystem => reservedSlotGridSystem;

    private GameSettings _gameSettings;

    private List<ColorCube> _cubes = new List<ColorCube>();
    private List<Shooter> _shooters = new List<Shooter>();
    private List<ConveyorArrow> _arrows = new List<ConveyorArrow>();
    private List<Node> _reservedSlots = new List<Node>();
    private List<Node> _shooterNodes = new List<Node>();
    private List<Node> _colorCubeNodes = new List<Node>();
    private List<GameObject> _plates = new List<GameObject>();

    private ObjectPool _pool;

    public void Init(LevelData data, GameSettings settings, ObjectPool pool)
    {
        LevelData = data;
        _gameSettings = settings;
        _pool = pool;

        shooterGridSystem.Init(pool, data.shooterGridSize);

        var spaceX = colorCubeGridSystem.gridSpaceX *
                     (_gameSettings.defaultTextureWidth / data.colorCubeGridSize.x);
        var spaceZ = colorCubeGridSystem.gridSpaceZ *
                     (_gameSettings.defaultTextureHeight / data.colorCubeGridSize.y);

        spaceX = spaceZ = Mathf.Min(spaceX, spaceZ);

        colorCubeGridSystem.gridSpaceX = spaceX;
        colorCubeGridSystem.gridSpaceZ = spaceZ;

        var cubeScale = 1f * (data.colorCubeGridSize.x > data.colorCubeGridSize.y
            ? _gameSettings.defaultTextureWidth / data.colorCubeGridSize.x
            : _gameSettings.defaultTextureHeight / data.colorCubeGridSize.y);

        colorCubeGridSystem.Init(pool, data.colorCubeGridSize);
        reservedSlotGridSystem.Init(pool, new Vector2Int(_gameSettings.reservedSlotCount, 1));

        CreateShooters(pool);
        CreateColorCubes(cubeScale, pool);
        reservedSlotGridSystem.SetSlotValues(_gameSettings.reservedSlotWarningEffectDuration,
            _gameSettings.reservedSlotWarningEffectCount);

        conveyor.Init(pool, this, _gameSettings);
    }

    private void CreateShooters(ObjectPool pool)
    {
        for (int i = 0; i < LevelData.CellsData.GetLength(0); i++)
        {
            for (int j = 0; j < LevelData.CellsData.GetLength(1); j++)
            {
                var data = LevelData.CellsData[i, j];
                if (data.shootCount == 0) continue;
                var node = shooterGridSystem.GetNode(i, shooterGridSystem.gridHeight - j - 1);

                var shooter = pool.SpawnFromPool(PoolTags.Shooter, node.transform.position,
                    node.transform.rotation).GetComponent<Shooter>();
                shooter.transform.SetParent(node.transform);
                shooter.Init(data, _gameSettings);
                shooter.Initialize(node);
                node.AssignNodeObject(shooter);

                _shooters.Add(shooter);
            }
        }
    }

    private void CreateColorCubes(float scale, ObjectPool pool)
    {
        Texture2D texture = LevelData.levelTexture;
        for (int i = 0; i < texture.width; i++)
        {
            for (int j = 0; j < texture.height; j++)
            {
                if (texture.GetPixel(i, j).a <= 0.1f) continue;

                var node = colorCubeGridSystem.GetNode(i, j);
                var cube = pool.SpawnFromPool(PoolTags.ColorCube, node.transform.position, node.transform.rotation)
                    .GetComponent<ColorCube>();
                cube.transform.SetParent(node.transform);

                cube.transform.localScale = new Vector3(scale, cube.transform.localScale.y, scale);
                cube.Init(texture.GetPixel(i, j), LevelData.levelColors, LevelData.colorThreshold);
                cube.Initialize(node);
                node.AssignNodeObject(cube);

                _cubes.Add(cube);
            }
        }
    }

    public ReservedSlot GetAvailableReservedSlot()
    {
        return reservedSlotGridSystem.GetAvailableSlot();
    }

    public void Reset()
    {
        foreach (var colorCube in _cubes)
        {
            colorCube.transform.SetParent(_pool.transform);
            _pool.DestroyToPool(PoolTags.ColorCube, colorCube.gameObject);
        }

        foreach (var shooter in _shooters)
        {
            shooter.transform.SetParent(_pool.transform);
            _pool.DestroyToPool(PoolTags.Shooter, shooter.gameObject);
        }

        foreach (var arrow in _arrows)
        {
            arrow.transform.SetParent(_pool.transform);
            _pool.DestroyToPool(PoolTags.ConveyorArrow, arrow.gameObject);
        }

        _reservedSlots.AddRange(reservedSlotGridSystem.GetAllNodes());
        foreach (var slot in _reservedSlots)
        {
            slot.transform.SetParent(_pool.transform);
            _pool.DestroyToPool(PoolTags.ReservedSlot, slot.gameObject);
        }

        _shooterNodes.AddRange(shooterGridSystem.GetAllNodes());
        foreach (var node in _shooterNodes)
        {
            node.transform.SetParent(_pool.transform);
            _pool.DestroyToPool(PoolTags.ShooterNode, node.gameObject);
        }

        _colorCubeNodes.AddRange(colorCubeGridSystem.GetAllNodes());
        foreach (var cubeNode in _colorCubeNodes)
        {
            cubeNode.transform.SetParent(_pool.transform);
            _pool.DestroyToPool(PoolTags.ColorCubeNode, cubeNode.gameObject);
        }

        foreach (var plate in _plates)
        {
            plate.transform.SetParent(_pool.transform);
            _pool.DestroyToPool(PoolTags.ShooterPlate, plate);
        }

        _cubes.Clear();
        _shooters.Clear();
        _arrows.Clear();
        _reservedSlots.Clear();
        _shooterNodes.Clear();
        _colorCubeNodes.Clear();
        _plates.Clear();
    }

    public void SpawnedNewArrows(ConveyorArrow arrow)
    {
        _arrows.Add(arrow);
    }

    public void SpawnedPlate(GameObject plate)
    {
        _plates.Add(plate);
    }
}
