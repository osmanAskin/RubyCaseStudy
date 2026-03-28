using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(menuName = "Levels/Level Data", fileName = "Level ")]
public class LevelData : SerializedScriptableObject
{
    [InfoBox(
        "🧩 LEVEL CREATION GUIDE\n\n" +
        "1️⃣ **Assign Texture** → Under *Level Texture Settings*, drag & drop a texture you want to use as the color source.\n" +
        "   - The system will automatically detect its width/height as the cube grid size.\n\n" +
        "2️⃣ **Click 'Create Colors'** → This analyzes the texture and groups similar colors.\n" +
        "   - Use *Color Threshold* to control how close colors need to be to count as the same.\n\n" +
        "3️⃣ **Initialize Grid** → Generates an empty shooter grid with your defined grid size.\n" +
        "   - You can later resize it safely using 'Update Shooter Grid Size'.\n\n" +
        "4️⃣ **Select Colors Below** → In the *Color Palette Preview* section at the bottom, click on any color box.\n" +
        "   - The selected color will be highlighted as 'Selected'.\n\n" +
        "5️⃣ **Paint the Grid** → Click on cells in the grid to assign the currently selected color.\n" +
        "   - Press **Z** to increase the shoot count, **X** to decrease it for the hovered cell.\n\n" +
        "6️⃣ **Auto Distribution (Optional)** → 'Distribute Colors To Grid' automatically fills the grid based on color sizes.\n\n" +
        "7️⃣ **Validation** → Use 'Check Shooter Values' to confirm that total shoot counts match the color data sizes.\n\n" +
        "💡 Tip: Keep the grid clean — gray cells are considered empty. Use smaller thresholds for more color variety.",
        InfoMessageType = InfoMessageType.Info)]
    [ShowInInspector, ReadOnly, PropertyOrder(-100)]
    private string _infoBoxHeader => "";

    [Serializable]
    public class LevelColorData
    {
        public int id;
        public Color color;
        public int size;

        public LevelColorData(int id, Color color, int size)
        {
            this.id = id;
            this.color = color;
            this.size = size;
        }
    }

    [BoxGroup("Level Texture Settings")] public Texture2D levelTexture;

    [BoxGroup("Level Texture Settings")]
    [ReadOnly, Tooltip("Automatically set from the level texture. Do not modify manually.")]
    public Vector2Int colorCubeGridSize;

    [TableList(AlwaysExpanded = true)] [BoxGroup("Level Color Settings")]
    public List<LevelColorData> levelColors = new();

    [Range(0.01f, 0.5f)]
    [Tooltip("Color difference threshold (higher = fewer color groups)")]
    [BoxGroup("Level Color Settings")]
    public float colorThreshold = 0.1f;

    [HideInInspector] public List<LevelColorData> runtimeColorStats = new();

    [BoxGroup("Shooter Grid Settings")] public Vector2Int shooterGridSize = new Vector2Int(3, 5);

#if UNITY_EDITOR
    [TableMatrix(DrawElementMethod = "DrawElement", SquareCells = true)]
#else
    [TableMatrix(SquareCells = true)]
    [BoxGroup("🧩 Shooter Grid Setup")]
#endif
    public CellData[,] CellsData;

    [HideInInspector] public int selectedColorIndex = -1;

    [ShowInInspector, ReadOnly, PropertyOrder(-5)]
    [LabelText("🎨 Selected Color")]
    [GUIColor("@GetSelectedColorPreview()")]
    private string SelectedColorLabel =>
        selectedColorIndex >= 0 && selectedColorIndex < levelColors.Count
            ? $"ID: {levelColors[selectedColorIndex].id} | Size: {levelColors[selectedColorIndex].size}"
            : "None Selected";

    private Color GetSelectedColorPreview()
    {
        if (selectedColorIndex < 0 || selectedColorIndex >= levelColors.Count)
            return Color.gray;
        return levelColors[selectedColorIndex].color;
    }

#if UNITY_EDITOR
    [BoxGroup("Shooter Grid Settings")]
    [Button("Initialize Grid")]
    public void InitializeGrid()
    {
        CellsData = new CellData[shooterGridSize.x, shooterGridSize.y];
        for (int x = 0; x < shooterGridSize.x; x++)
        for (int y = 0; y < shooterGridSize.y; y++)
            CellsData[x, y] = new CellData(new Vector2Int(x, y));

        SetRuntimeColorStats();
    }

    private void SetRuntimeColorStats()
    {
        runtimeColorStats = levelColors.Select(c => new LevelColorData(c.id, c.color, c.size)).ToList();
    }

    [OnInspectorGUI("DrawColorGrid")]
    [BoxGroup("Color Palette Preview")]
    private void DrawColorGrid()
    {
        if (runtimeColorStats == null || runtimeColorStats.Count == 0)
        {
            GUILayout.Label("No color stats yet. Run CreateColors first.");
            return;
        }

        const int cellSize = 40;
        const int columns = 6;
        int padding = 4;

        int total = runtimeColorStats.Count;
        int rows = Mathf.CeilToInt(total / (float)columns);

        GUILayout.BeginVertical();
        for (int y = 0; y < rows; y++)
        {
            GUILayout.BeginHorizontal();
            for (int x = 0; x < columns; x++)
            {
                int index = y * columns + x;
                if (index >= total)
                    break;

                var colorData = runtimeColorStats[index];
                var rect = GUILayoutUtility.GetRect(cellSize, cellSize, GUILayout.Width(cellSize),
                    GUILayout.Height(cellSize));

                UnityEditor.EditorGUI.DrawRect(rect, colorData.color);

                if (index == selectedColorIndex)
                    UnityEditor.Handles.DrawSolidRectangleWithOutline(rect, Color.clear, Color.yellow);
                else
                    UnityEditor.Handles.DrawSolidRectangleWithOutline(rect, Color.clear, Color.black * 0.4f);

                GUIStyle sizeStyle = new GUIStyle(GUI.skin.label)
                {
                    alignment = TextAnchor.MiddleCenter,
                    fontSize = 11,
                    fontStyle = FontStyle.Bold,
                    normal = { textColor = GetReadableTextColor(colorData.color) }
                };
                GUI.Label(rect, colorData.size.ToString(), sizeStyle);

                if (index == selectedColorIndex)
                {
                    Rect selectedRect = new Rect(rect.x, rect.y + 2, rect.width, 14);
                    GUIStyle selectedStyle = new GUIStyle(GUI.skin.label)
                    {
                        alignment = TextAnchor.UpperCenter,
                        fontSize = 10,
                        fontStyle = FontStyle.Bold,
                        normal = { textColor = GetReadableTextColor(colorData.color) }
                    };
                    GUI.Label(selectedRect, "Selected", selectedStyle);
                }

                if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
                {
                    selectedColorIndex = index;
                    GUI.changed = true;
                }

                GUILayout.Space(padding);
            }

            GUILayout.EndHorizontal();
            GUILayout.Space(padding);
        }

        GUILayout.EndVertical();
    }

    [BoxGroup("🧩 Shooter Grid Setup")]
    private CellData DrawElement(Rect rect, CellData value, int x, int y)
    {
        if (value == null)
            value = new CellData(new Vector2Int(x, y));

        float padding = 1.5f;
        Rect innerRect = new Rect(
            rect.x + padding,
            rect.y + padding,
            rect.width - padding * 2f,
            rect.height - padding * 2f
        );

        UnityEditor.EditorGUI.DrawRect(innerRect, value.cellColor);

        if (value.cellColor != Color.gray)
        {
            GUIStyle style = new GUIStyle(GUI.skin.label)
            {
                alignment = TextAnchor.MiddleCenter,
                fontStyle = FontStyle.Bold,
                normal = { textColor = GetReadableTextColor(value.cellColor) }
            };
            GUI.Label(rect, value.shootCount.ToString(), style);
        }

        if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
        {
            if (selectedColorIndex >= 0 && selectedColorIndex < levelColors.Count)
            {
                var data = levelColors[selectedColorIndex];

                if (value.cellColor != data.color)
                {
                    value.shootCount = 0;
                }

                value.cellColor = data.color;
                value.colorID = data.id;
            }

            GUI.changed = true;
            UnityEditor.EditorUtility.SetDirty(this);
        }

        if (rect.Contains(Event.current.mousePosition) && value.colorID >= 0 &&
            value.colorID < runtimeColorStats.Count && value.cellColor != Color.gray)
        {
            LevelColorData colorData = null;
            foreach (var runtimeColorStat in runtimeColorStats)
            {
                if (runtimeColorStat.id == value.colorID)
                {
                    colorData = runtimeColorStat;
                }
            }

            if (Event.current.type == EventType.KeyDown)
            {
                if (Event.current.keyCode == KeyCode.Z && colorData.size > 0)
                {
                    value.shootCount++;
                    colorData.size--;
                    Event.current.Use();
                    GUI.changed = true;
                    UnityEditor.EditorUtility.SetDirty(this);
                }
                else if (Event.current.keyCode == KeyCode.X && value.shootCount > 0)
                {
                    value.shootCount--;
                    colorData.size++;

                    Event.current.Use();
                    GUI.changed = true;
                    UnityEditor.EditorUtility.SetDirty(this);
                }
            }
        }

        return value;
    }

    [BoxGroup("Level Color Settings")]
    [Button("Create Colors")]
    private void CreateColors()
    {
        ConsoleClearer.ClearConsole();
        if (levelTexture == null)
        {
            Debug.LogError("Level texture is missing!");
            return;
        }

        if (!levelTexture.isReadable)
        {
            Debug.LogError($"Texture '{levelTexture.name}' is not readable. Enable Read/Write in import settings.");
            return;
        }

        Color[] pixels = levelTexture.GetPixels();
        if (pixels == null || pixels.Length == 0)
        {
            Debug.LogError("No pixel data found.");
            return;
        }

        levelColors.Clear();
        List<Color> groupedColors = new();

        foreach (Color c in pixels)
        {
            bool found = groupedColors.Any(g => IsSimilarColor(g, c, colorThreshold));
            if (!found)
                groupedColors.Add(c);
        }

        int idCounter = 0;
        foreach (var groupColor in groupedColors)
        {
            int count = pixels.Count(p => IsSimilarColor(p, groupColor, colorThreshold));
            levelColors.Add(new LevelColorData(idCounter, groupColor, count));
            idCounter++;
        }

        levelColors = levelColors.OrderByDescending(c => c.size).ToList();

        for (int i = 0; i < levelColors.Count; i++)
            levelColors[i].id = i;

        Debug.Log($"Detected {levelColors.Count} grouped colors from {levelTexture.name}");

        SetRuntimeColorStats();
    }

    [Button("Distribute Colors To Grid")]
    private void DistributeColorsToGrid()
    {
        ConsoleClearer.ClearConsole();
        if (CellsData == null || CellsData.Length == 0)
        {
            Debug.LogError("Grid not initialized!");
            return;
        }

        if (levelColors == null || levelColors.Count == 0)
        {
            Debug.LogError("No colors available! Run CreateColors first.");
            return;
        }

        int width = shooterGridSize.x;
        var totalShooterCount = 0;
        Dictionary<Color, List<int>> shooterData = new();
        List<Color> colors = new();

        foreach (var lc in levelColors)
        {
            var shooterCount = Mathf.FloorToInt(lc.size / 15f);
            var remainder = lc.size % 15;

            shooterData.Add(lc.color, new List<int>());
            for (int x = 0; x < shooterCount; x++)
                shooterData[lc.color].Add(15);

            if (shooterCount == 0)
            {
                shooterData[lc.color].Add(lc.size);
                remainder = 0;
                totalShooterCount++;
            }

            for (int y = 0; y < remainder; y++)
            {
                var index = Random.Range(0, shooterCount);
                var value = shooterData[lc.color][index];
                value++;
                shooterData[lc.color][index] = value;
            }

            totalShooterCount += shooterCount;
            colors.Add(lc.color);
        }

        int height = Mathf.CeilToInt(totalShooterCount / (float)width);
        shooterGridSize.y = height;
        InitializeGrid();

        for (int x = 0; x < width; x++)
        for (int y = 0; y < height; y++)
        {
            if (shooterData.Count == 0) continue;
            var cell = CellsData[x, y];
            cell.cellColor = Color.gray;
            cell.shootCount = 0;

            var randomColor = colors.GetRandomElement();
            if (!shooterData.ContainsKey(randomColor)) continue;
            var dictIndex = Random.Range(0, shooterData[randomColor].Count);
            cell.cellColor = randomColor;
            cell.shootCount = shooterData[randomColor][dictIndex];
            cell.colorID = levelColors.First(l => l.color == randomColor).id;

            shooterData[randomColor].RemoveAt(dictIndex);
            if (shooterData[randomColor].Count == 0)
                colors.Remove(randomColor);
        }

        Debug.Log("✅ Colors distributed evenly across grid.");

        foreach (var runtimeColorStat in runtimeColorStats)
        {
            runtimeColorStat.size = 0;
        }
    }

    private bool IsSimilarColor(Color a, Color b, float threshold)
    {
        float dr = a.r - b.r;
        float dg = a.g - b.g;
        float db = a.b - b.b;
        float distance = Mathf.Sqrt(dr * dr + dg * dg + db * db);
        return distance < threshold;
    }

    private Color GetReadableTextColor(Color bg)
    {
        float brightness = (0.299f * bg.r + 0.587f * bg.g + 0.114f * bg.b);
        return brightness > 0.5f ? Color.black : Color.white;
    }

    private void OnValidate()
    {
        if (levelTexture != null)
            SetCubeSize();
    }

    [BoxGroup("Level Texture Settings")]
    [Button("Set Cube Grid Size")]
    public void SetCubeSize()
    {
        if (levelTexture == null)
        {
            Debug.LogError("Texture is missing!");
            return;
        }

        colorCubeGridSize = new Vector2Int(levelTexture.width, levelTexture.height);
    }

    [BoxGroup("Shooter Grid Settings")]
    [Button]
    public void UpdateShooterGridSize()
    {
        if (CellsData == null)
        {
            InitializeGrid();
            return;
        }

        int newWidth = shooterGridSize.x;
        int newHeight = shooterGridSize.y;

        int oldWidth = CellsData.GetLength(0);
        int oldHeight = CellsData.GetLength(1);

        var newGrid = new CellData[newWidth, newHeight];

        for (int x = 0; x < Mathf.Min(oldWidth, newWidth); x++)
        {
            for (int y = 0; y < Mathf.Min(oldHeight, newHeight); y++)
            {
                newGrid[x, y] = CellsData[x, y];
            }
        }

        for (int x = 0; x < newWidth; x++)
        {
            for (int y = 0; y < newHeight; y++)
            {
                if (newGrid[x, y] == null)
                    newGrid[x, y] = new CellData(new Vector2Int(x, y));
            }
        }

        CellsData = newGrid;

#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(this);
#endif

        Debug.Log($"✅ Shooter grid resized to {newWidth}x{newHeight}. Old: {oldWidth}x{oldHeight}");
    }

    [Button]
    public void CheckShooterValues()
    {
        ConsoleClearer.ClearConsole();
        var sizeDict = new Dictionary<int, int>();
        for (int i = 0; i < CellsData.GetLength(0); i++)
        {
            for (int j = 0; j < CellsData.GetLength(1); j++)
            {
                var id = CellsData[i, j].colorID;
                foreach (var lc in levelColors)
                {
                    if (lc.id == id)
                    {
                        if (sizeDict.ContainsKey(id))
                        {
                            sizeDict[id] += CellsData[i, j].shootCount;
                        }
                        else
                        {
                            sizeDict.Add(id, CellsData[i, j].shootCount);
                        }
                    }
                }
            }
        }

        foreach (var lc in levelColors)
        {
            if (sizeDict[lc.id] == lc.size)
            {
                continue;
            }

            var dif = lc.size - sizeDict[lc.id];
            if (dif > 0)
            {
                Debug.LogError("Add " + dif + " with ID -" + lc.id + "-");
            }
            else
            {
                Debug.LogError("Remove " + Mathf.Abs(dif) + " with ID -" + lc.id + "-");
            }
        }

        Debug.Log("Check is completed.");
    }

#endif
}
