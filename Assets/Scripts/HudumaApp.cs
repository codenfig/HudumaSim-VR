using System;
using System.Collections.Generic;
using UnityEngine;

namespace HudumaSimVR
{
    // Self-contained desktop prototype. Unity UI and XR can replace OnGUI without changing session logic.
    public class HudumaApp : MonoBehaviour
    {
        private enum Page { Welcome, Sectors, Scenarios, Training, Result }
        private Page page = Page.Welcome;
        private Sector sector;
        private readonly TrainingSession session = new TrainingSession();
        private List<ComplaintScenario> scenarios;
        private Vector2 scroll;
        private GUIStyle heading, body, button, note;
        private string savedPath = "";
        private GameObject avatar;
        private Renderer avatarRenderer;
        private Camera sceneCamera;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Boot()
        {
            if (FindObjectOfType<HudumaApp>() == null)
                new GameObject("HudumaSim VR Application").AddComponent<HudumaApp>();
        }
        private void Start()
        {
            scenarios = ScenarioLibrary.All();
            BuildEnvironment();
        }
        private void BuildEnvironment()
        {
            sceneCamera = Camera.main;
            if (sceneCamera == null)
            {
                var cameraObject = new GameObject("Main Camera");
                sceneCamera = cameraObject.AddComponent<Camera>();
                cameraObject.tag = "MainCamera";
            }
            sceneCamera.transform.position = new Vector3(0, 1.7f, -5.8f);
            sceneCamera.transform.LookAt(new Vector3(0, 1.3f, 1f));
            sceneCamera.backgroundColor = new Color(0.09f, 0.15f, 0.21f);
            var lightObject = new GameObject("Key Light");
            var light = lightObject.AddComponent<Light>();
            light.type = LightType.Directional; light.intensity = 1.4f;
            lightObject.transform.rotation = Quaternion.Euler(45, -25, 0);
            CreatePrimitive(PrimitiveType.Plane, "Floor", new Vector3(0, -0.02f, 0), new Vector3(1.5f, 1, 1.5f), new Color(0.42f, 0.49f, 0.52f));
            CreatePrimitive(PrimitiveType.Cube, "Service Desk", new Vector3(0, 0.6f, 0.7f), new Vector3(2.6f, 1.1f, 0.65f), new Color(0.25f, 0.31f, 0.39f));
            var torso = CreatePrimitive(PrimitiveType.Capsule, "Complainant", new Vector3(0, 1.65f, 2.1f), new Vector3(0.65f, 1.0f, 0.5f), new Color(0.22f, 0.57f, 0.74f));
            avatar = torso; avatarRenderer = torso.GetComponent<Renderer>();
            CreatePrimitive(PrimitiveType.Sphere, "Complainant Head", new Vector3(0, 2.48f, 2.1f), new Vector3(0.48f, 0.48f, 0.48f), new Color(0.64f, 0.43f, 0.31f));
        }
        private GameObject CreatePrimitive(PrimitiveType type, string name, Vector3 pos, Vector3 scale, Color color)
        {
            var obj = GameObject.CreatePrimitive(type);
            obj.name = name; obj.transform.position = pos; obj.transform.localScale = scale;
            var renderer = obj.GetComponent<Renderer>();
            if (renderer != null) renderer.material.color = color;
            return obj;
        }
        private void Update()
        {
            if (avatar != null) avatar.transform.rotation = Quaternion.Euler(0, Mathf.Sin(Time.time * 0.8f) * 5f, 0);
            if (avatarRenderer != null && session.Scenario != null)
                avatarRenderer.material.color = Color.Lerp(new Color(0.22f, 0.62f, 0.56f), new Color(0.85f, 0.24f, 0.22f), session.Emotion / 100f);
        }
        private void InitStyles()
        {
            if (heading != null) return;
            heading = new GUIStyle(GUI.skin.label) { fontSize = 28, fontStyle = FontStyle.Bold, wordWrap = true };
            body = new GUIStyle(GUI.skin.label) { fontSize = 18, wordWrap = true };
            note = new GUIStyle(GUI.skin.label) { fontSize = 15, wordWrap = true };
            button = new GUIStyle(GUI.skin.button) { fontSize = 17, wordWrap = true, alignment = TextAnchor.MiddleLeft, padding = new RectOffset(14, 14, 10, 10) };
        }
        private void OnGUI()
        {
            InitStyles();
            float width = Mathf.Min(650, Screen.width * 0.92f);
            var area = new Rect(20, 20, width, Screen.height - 40);
            GUI.Box(new Rect(area.x - 8, area.y - 8, area.width + 16, area.height + 16), "");
            GUILayout.BeginArea(area);
            scroll = GUILayout.BeginScrollView(scroll);
            GUILayout.Label("HudumaSim VR", heading);
            GUILayout.Label("Practice. Respond. Improve.", note);
            GUILayout.Space(14);
            switch (page)
            {
                case Page.Welcome:
                    GUILayout.Label("Complaint Management Training Prototype", body);
                    GUILayout.Label("Train with nine simulated complaints across Tax Ombudsman, Banking and Telecommunications. This first version uses choices and works without internet.", body);
                    GUILayout.Space(12);
                    if (Action("Start Training")) page = Page.Sectors;
                    break;
                case Page.Sectors:
                    GUILayout.Label("Select a training sector", body);
                    foreach (Sector candidate in Enum.GetValues(typeof(Sector)))
                        if (Action(SectorName(candidate))) { sector = candidate; page = Page.Scenarios; scroll = Vector2.zero; }
                    if (Action("Back")) page = Page.Welcome;
                    break;
                case Page.Scenarios:
                    GUILayout.Label("Choose a scenario — " + SectorName(sector), body);
                    foreach (var scenario in scenarios)
                        if (scenario.sector == sector && Action(scenario.id + " — " + scenario.title))
                        {
                            session.Start(scenario); savedPath = ""; page = Page.Training; scroll = Vector2.zero;
                        }
                    if (Action("Back to sectors")) page = Page.Sectors;
                    break;
                case Page.Training:
                    GUILayout.Label(session.Scenario.title, body);
                    GUILayout.Label("Complainant frustration: " + session.Emotion + "/100", note);
                    GUILayout.Space(8);
                    GUILayout.Label("COMPLAINANT: " + session.LastReaction, body);
                    GUILayout.Space(10);
                    if (session.Finished)
                    {
                        if (Action("View assessment")) { page = Page.Result; scroll = Vector2.zero; }
                    }
                    else
                    {
                        GUILayout.Label("STEP " + (session.Step + 1) + " OF " + session.Scenario.choices.Length, note);
                        GUILayout.Label(session.Scenario.prompts[session.Step], body);
                        var choices = session.Scenario.choices[session.Step];
                        for (int i = 0; i < choices.Length; i++)
                        {
                            int selected = i;
                            if (Action((i + 1) + ". " + choices[i].text)) { session.Answer(selected); scroll = Vector2.zero; break; }
                        }
                    }
                    if (Action("Exit scenario")) page = Page.Scenarios;
                    break;
                case Page.Result:
                    var result = session.GetResult();
                    GUILayout.Label("Training assessment", body);
                    GUILayout.Label("Overall: " + result.total.ToString("0.0") + "/100", heading);
                    GUILayout.Label("Listening (30%): " + result.listening.ToString("0.0"), body);
                    GUILayout.Label("Calmness (30%): " + result.calmness.ToString("0.0"), body);
                    GUILayout.Label("Solution (40%): " + result.solution.ToString("0.0"), body);
                    GUILayout.Label("Final complainant frustration: " + result.finalEmotion + "/100", body);
                    GUILayout.Label("Feedback: " + result.feedback, body);
                    if (Action("Save assessment as JSON")) savedPath = session.SaveResult();
                    if (!string.IsNullOrEmpty(savedPath)) GUILayout.Label("Saved locally: " + savedPath, note);
                    if (Action("Retry scenario")) { session.Start(session.Scenario); savedPath = ""; page = Page.Training; }
                    if (Action("Choose another scenario")) page = Page.Scenarios;
                    break;
            }
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }
        private bool Action(string label) { GUILayout.Space(5); return GUILayout.Button(label, button, GUILayout.MinHeight(52)); }
        private static string SectorName(Sector s) => s == Sector.TaxOmbudsman ? "Tax Ombudsman" : s == Sector.Banking ? "Banking" : "Telecommunications";
    }
}