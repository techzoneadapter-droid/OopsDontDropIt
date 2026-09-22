using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class PrototypeBuilder
{
    private const string ScenePath = "Assets/Scenes/Gameplay.unity";

    [MenuItem("Oops Dont Drop It/Build Cute Prototype")]
    public static void BuildPrototype()
    {
        EnsureFolder("Assets/Scenes");
        EnsureFolder("Assets/Materials");
        EnsureFolder("Assets/Prefabs");
        EnsureFolder("Assets/Models");
        EnsureFolder("Assets/Animations");
        EnsureFolder("Assets/UI");
        EnsureFolder("Assets/Audio");

        var peach = CreateMaterial("Peach", new Color(1.00f, 0.67f, 0.58f));
        var cream = CreateMaterial("Cream", new Color(1.00f, 0.93f, 0.77f));
        var mint = CreateMaterial("Mint", new Color(0.55f, 0.88f, 0.76f));
        var sky = CreateMaterial("Sky", new Color(0.57f, 0.80f, 0.98f));
        var lavender = CreateMaterial("Lavender", new Color(0.73f, 0.67f, 0.96f));
        var cocoa = CreateMaterial("Cocoa", new Color(0.29f, 0.20f, 0.18f));
        var white = CreateMaterial("White", new Color(0.99f, 0.98f, 0.94f));
        var yolk = CreateMaterial("Yolk", new Color(1.00f, 0.77f, 0.20f));
        var grass = CreateMaterial("Grass", new Color(0.66f, 0.89f, 0.67f));

        var trayPhysics = CreatePhysicsMaterial();

        Scene scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        // Lighting
        var lightGO = new GameObject("Directional Light");
        var light = lightGO.AddComponent<Light>();
        light.type = LightType.Directional;
        light.intensity = 1.25f;
        light.color = new Color(1.0f, 0.95f, 0.88f);
        lightGO.transform.rotation = Quaternion.Euler(48f, -28f, 0f);

        RenderSettings.ambientLight = new Color(0.62f, 0.67f, 0.74f);

        // Ground and cute corridor
        var ground = Primitive("Ground", PrimitiveType.Cube, null,
            new Vector3(0f, -0.3f, 12f), new Vector3(6.4f, 0.6f, 30f), cream, true);

        Primitive("LeftGrass", PrimitiveType.Cube, null,
            new Vector3(-5.1f, -0.24f, 12f), new Vector3(3.8f, 0.45f, 30f), grass, true);
        Primitive("RightGrass", PrimitiveType.Cube, null,
            new Vector3(5.1f, -0.24f, 12f), new Vector3(3.8f, 0.45f, 30f), grass, true);

        for (int i = 0; i < 8; i++)
        {
            float z = 2.5f + i * 3.3f;
            CreateCuteBush(new Vector3(-4.0f, 0.25f, z), i % 2 == 0 ? mint : sky);
            CreateCuteBush(new Vector3(4.0f, 0.25f, z + 1.2f), i % 2 == 0 ? lavender : mint);
        }

        // Carrier root
        var carrier = new GameObject("Carrier");
        carrier.transform.position = new Vector3(0f, 0f, 0f);
        var carrierBody = carrier.AddComponent<Rigidbody>();
        carrierBody.isKinematic = true;
        carrierBody.interpolation = RigidbodyInterpolation.Interpolate;
        carrier.AddComponent<CarrierController>();

        // Chibi placeholder
        var body = Primitive("Body", PrimitiveType.Capsule, carrier.transform,
            new Vector3(0f, 0.82f, -0.72f), new Vector3(0.72f, 0.78f, 0.55f), peach, false);
        var head = Primitive("Head", PrimitiveType.Sphere, carrier.transform,
            new Vector3(0f, 1.88f, -0.72f), new Vector3(0.92f, 0.92f, 0.92f), cream, false);
        Primitive("EyeL", PrimitiveType.Sphere, head.transform,
            new Vector3(-0.22f, 0.12f, -0.43f), new Vector3(0.10f, 0.12f, 0.07f), cocoa, false);
        Primitive("EyeR", PrimitiveType.Sphere, head.transform,
            new Vector3(0.22f, 0.12f, -0.43f), new Vector3(0.10f, 0.12f, 0.07f), cocoa, false);
        Primitive("CheekL", PrimitiveType.Sphere, head.transform,
            new Vector3(-0.34f, -0.10f, -0.42f), new Vector3(0.10f, 0.06f, 0.05f), peach, false);
        Primitive("CheekR", PrimitiveType.Sphere, head.transform,
            new Vector3(0.34f, -0.10f, -0.42f), new Vector3(0.10f, 0.06f, 0.05f), peach, false);

        Primitive("ArmL", PrimitiveType.Capsule, carrier.transform,
            new Vector3(-0.72f, 1.25f, -0.02f), new Vector3(0.18f, 0.58f, 0.18f), cream, false)
            .transform.rotation = Quaternion.Euler(70f, 0f, -18f);
        Primitive("ArmR", PrimitiveType.Capsule, carrier.transform,
            new Vector3(0.72f, 1.25f, -0.02f), new Vector3(0.18f, 0.58f, 0.18f), cream, false)
            .transform.rotation = Quaternion.Euler(70f, 0f, 18f);

        // Tray
        var tray = Primitive("Tray", PrimitiveType.Cube, carrier.transform,
            new Vector3(0f, 1.42f, 0.35f), new Vector3(2.8f, 0.14f, 1.75f), mint, true);
        tray.GetComponent<BoxCollider>().material = trayPhysics;
        tray.AddComponent<TrayController>();

        // Egg
        var egg = Primitive("Egg", PrimitiveType.Sphere, null,
            new Vector3(0f, 2.02f, 0.35f), new Vector3(0.62f, 0.82f, 0.62f), white, false);
        var eggCollider = egg.AddComponent<CapsuleCollider>();
        eggCollider.direction = 1;
        eggCollider.radius = 0.46f;
        eggCollider.height = 1.28f;
        eggCollider.material = trayPhysics;

        var eggRb = egg.AddComponent<Rigidbody>();
        eggRb.mass = 0.32f;
        eggRb.linearDamping = 0.08f;
        eggRb.angularDamping = 0.15f;
        eggRb.interpolation = RigidbodyInterpolation.Interpolate;
        eggRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        var carry = egg.AddComponent<CarryObject>();
        SetObjectReference(carry, "tray", tray.transform);

        Primitive("EggEyeL", PrimitiveType.Sphere, egg.transform,
            new Vector3(-0.20f, 0.08f, -0.47f), new Vector3(0.095f, 0.12f, 0.06f), cocoa, false);
        Primitive("EggEyeR", PrimitiveType.Sphere, egg.transform,
            new Vector3(0.20f, 0.08f, -0.47f), new Vector3(0.095f, 0.12f, 0.06f), cocoa, false);
        Primitive("EggCheekL", PrimitiveType.Sphere, egg.transform,
            new Vector3(-0.31f, -0.12f, -0.45f), new Vector3(0.10f, 0.065f, 0.05f), peach, false);
        Primitive("EggCheekR", PrimitiveType.Sphere, egg.transform,
            new Vector3(0.31f, -0.12f, -0.45f), new Vector3(0.10f, 0.065f, 0.05f), peach, false);
        Primitive("EggYolkBadge", PrimitiveType.Sphere, egg.transform,
            new Vector3(0f, -0.22f, -0.48f), new Vector3(0.10f, 0.07f, 0.045f), yolk, false);

        // Obstacles
        CreateSpinner("SpinnerA", new Vector3(0f, 1.82f, 7.8f), lavender, 78f);
        CreateSpinner("SpinnerB", new Vector3(0f, 1.82f, 14.6f), peach, -105f);

        Primitive("BumpLeft", PrimitiveType.Cube, null,
            new Vector3(-1.85f, 0.45f, 11.2f), new Vector3(1.2f, 0.9f, 1.0f), sky, true)
            .transform.rotation = Quaternion.Euler(0f, 20f, 0f);
        Primitive("BumpRight", PrimitiveType.Cube, null,
            new Vector3(1.85f, 0.45f, 18.0f), new Vector3(1.2f, 0.9f, 1.0f), mint, true)
            .transform.rotation = Quaternion.Euler(0f, -20f, 0f);

        // Finish arch
        Primitive("FinishLeft", PrimitiveType.Cube, null,
            new Vector3(-2.25f, 1.7f, 22.2f), new Vector3(0.35f, 3.4f, 0.45f), peach, true);
        Primitive("FinishRight", PrimitiveType.Cube, null,
            new Vector3(2.25f, 1.7f, 22.2f), new Vector3(0.35f, 3.4f, 0.45f), peach, true);
        Primitive("FinishTop", PrimitiveType.Cube, null,
            new Vector3(0f, 3.25f, 22.2f), new Vector3(4.8f, 0.35f, 0.45f), lavender, true);

        var goal = new GameObject("GoalTrigger");
        goal.transform.position = new Vector3(0f, 1.4f, 22.0f);
        var goalCollider = goal.AddComponent<BoxCollider>();
        goalCollider.size = new Vector3(5.5f, 3.0f, 0.65f);
        goalCollider.isTrigger = true;
        goal.AddComponent<GoalTrigger>();

        // Camera
        var cameraGO = new GameObject("Main Camera");
        cameraGO.tag = "MainCamera";
        var cam = cameraGO.AddComponent<Camera>();
        cam.fieldOfView = 52f;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = new Color(0.74f, 0.90f, 1.0f);
        cameraGO.transform.position = new Vector3(0f, 4.6f, -7.4f);
        var follow = cameraGO.AddComponent<FollowCamera>();
        SetObjectReference(follow, "target", carrier.transform);

        // UI
        var canvasGO = new GameObject("HUD");
        var canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1080f, 1920f);
        scaler.matchWidthOrHeight = 0.5f;
        canvasGO.AddComponent<GraphicRaycaster>();

        Text progressText = CreateText("ProgressText", canvasGO.transform, "0%", 64,
            new Vector2(0.5f, 1f), new Vector2(0.5f, 1f), new Vector2(0f, -85f), new Vector2(320f, 100f));
        progressText.fontStyle = FontStyle.Bold;

        Text messageText = CreateText("MessageText", canvasGO.transform, "DRAG TO BALANCE", 72,
            new Vector2(0.5f, 0.58f), new Vector2(0.5f, 0.58f), Vector2.zero, new Vector2(900f, 260f));
        messageText.fontStyle = FontStyle.Bold;

        // Game state
        var gameGO = new GameObject("GameState");
        var game = gameGO.AddComponent<GameStateController>();
        SetObjectReference(game, "carrier", carrier.transform);
        SetObjectReference(game, "messageText", messageText);
        SetObjectReference(game, "progressText", progressText);
        SetFloat(game, "finishZ", 22f);

        PlayerSettings.defaultInterfaceOrientation = UIOrientation.Portrait;
        QualitySettings.vSyncCount = 0;

        EditorSceneManager.SaveScene(scene, ScenePath);
        EditorBuildSettings.scenes = new[] { new EditorBuildSettingsScene(ScenePath, true) };
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeGameObject = carrier;
        EditorGUIUtility.PingObject(AssetDatabase.LoadAssetAtPath<SceneAsset>(ScenePath));
        Debug.Log("Oops Dont Drop It prototype built successfully. Press Play and drag with the mouse/touch.");
    }

    private static GameObject CreateSpinner(string name, Vector3 position, Material mat, float speed)
    {
        var bar = Primitive(name, PrimitiveType.Cube, null, position, new Vector3(4.3f, 0.28f, 0.30f), mat, true);
        var rb = bar.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.Interpolate;
        var spinner = bar.AddComponent<SpinnerObstacle>();
        SetFloat(spinner, "degreesPerSecond", speed);
        return bar;
    }

    private static void CreateCuteBush(Vector3 position, Material mat)
    {
        var root = new GameObject("CuteBush");
        root.transform.position = position;
        Primitive("BushA", PrimitiveType.Sphere, root.transform,
            new Vector3(-0.35f, 0.35f, 0f), new Vector3(0.75f, 0.65f, 0.75f), mat, false);
        Primitive("BushB", PrimitiveType.Sphere, root.transform,
            new Vector3(0.35f, 0.30f, 0.05f), new Vector3(0.82f, 0.60f, 0.82f), mat, false);
        Primitive("BushC", PrimitiveType.Sphere, root.transform,
            new Vector3(0f, 0.62f, 0f), new Vector3(0.72f, 0.72f, 0.72f), mat, false);
    }

    private static GameObject Primitive(
        string name,
        PrimitiveType type,
        Transform parent,
        Vector3 localPosition,
        Vector3 localScale,
        Material material,
        bool keepCollider)
    {
        GameObject go = GameObject.CreatePrimitive(type);
        go.name = name;

        if (parent != null)
        {
            go.transform.SetParent(parent, false);
            go.transform.localPosition = localPosition;
        }
        else
        {
            go.transform.position = localPosition;
        }

        go.transform.localScale = localScale;

        var renderer = go.GetComponent<Renderer>();
        if (renderer != null && material != null) renderer.sharedMaterial = material;

        if (!keepCollider)
        {
            var collider = go.GetComponent<Collider>();
            if (collider != null) Object.DestroyImmediate(collider);
        }

        return go;
    }

    private static Text CreateText(
        string name,
        Transform parent,
        string value,
        int fontSize,
        Vector2 anchorMin,
        Vector2 anchorMax,
        Vector2 anchoredPosition,
        Vector2 size)
    {
        var go = new GameObject(name, typeof(RectTransform), typeof(CanvasRenderer), typeof(Text));
        go.transform.SetParent(parent, false);

        var rect = go.GetComponent<RectTransform>();
        rect.anchorMin = anchorMin;
        rect.anchorMax = anchorMax;
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = anchoredPosition;
        rect.sizeDelta = size;

        var text = go.GetComponent<Text>();
        text.text = value;
        text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        text.fontSize = fontSize;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = new Color(0.20f, 0.16f, 0.17f);
        text.horizontalOverflow = HorizontalWrapMode.Wrap;
        text.verticalOverflow = VerticalWrapMode.Overflow;

        var outline = go.AddComponent<Outline>();
        outline.effectColor = new Color(1f, 1f, 1f, 0.65f);
        outline.effectDistance = new Vector2(3f, -3f);

        return text;
    }

    private static Material CreateMaterial(string name, Color color)
    {
        string path = $"Assets/Materials/{name}.mat";
        var existing = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (existing != null)
        {
            if (existing.HasProperty("_BaseColor")) existing.SetColor("_BaseColor", color);
            else existing.color = color;
            EditorUtility.SetDirty(existing);
            return existing;
        }

        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");

        var material = new Material(shader) { name = name };
        if (material.HasProperty("_BaseColor")) material.SetColor("_BaseColor", color);
        else material.color = color;

        material.SetFloat("_Smoothness", 0.22f);
        AssetDatabase.CreateAsset(material, path);
        return material;
    }

    private static PhysicMaterial CreatePhysicsMaterial()
    {
        const string path = "Assets/Materials/TrayPhysics.physicMaterial";
        var existing = AssetDatabase.LoadAssetAtPath<PhysicMaterial>(path);
        if (existing != null) return existing;

        var mat = new PhysicMaterial("TrayPhysics")
        {
            dynamicFriction = 0.16f,
            staticFriction = 0.20f,
            bounciness = 0.03f,
            frictionCombine = PhysicMaterialCombine.Minimum,
            bounceCombine = PhysicMaterialCombine.Minimum
        };
        AssetDatabase.CreateAsset(mat, path);
        return mat;
    }

    private static void EnsureFolder(string path)
    {
        if (AssetDatabase.IsValidFolder(path)) return;

        string[] parts = path.Split('/');
        string current = parts[0];

        for (int i = 1; i < parts.Length; i++)
        {
            string next = current + "/" + parts[i];
            if (!AssetDatabase.IsValidFolder(next))
                AssetDatabase.CreateFolder(current, parts[i]);
            current = next;
        }
    }

    private static void SetObjectReference(Object component, string propertyName, Object value)
    {
        var serialized = new SerializedObject(component);
        var property = serialized.FindProperty(propertyName);
        property.objectReferenceValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }

    private static void SetFloat(Object component, string propertyName, float value)
    {
        var serialized = new SerializedObject(component);
        var property = serialized.FindProperty(propertyName);
        property.floatValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }
}
