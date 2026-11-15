using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace HudumaSimVR
{
    public class TrainingSession
    {
        public ComplaintScenario Scenario { get; private set; }
        public int Step { get; private set; }
        public int Emotion { get; private set; }
        public string LastReaction { get; private set; }
        public bool Finished => Scenario != null && Step >= Scenario.choices.Length;
        private readonly List<ReplyOption> answers = new List<ReplyOption>();

        public void Start(ComplaintScenario scenario)
        {
            Scenario = scenario; Step = 0; Emotion = 75; LastReaction = scenario.opening;
            answers.Clear();
        }
        public void Answer(int optionIndex)
        {
            if (Scenario == null || Finished) return;
            var options = Scenario.choices[Step];
            if (optionIndex < 0 || optionIndex >= options.Length) return;
            var chosen = options[optionIndex];
            answers.Add(chosen);
            Emotion = Mathf.Clamp(Emotion + chosen.emotionDelta, 0, 100);
            LastReaction = chosen.reaction;
            Step++;
        }
        public SessionResult GetResult()
        {
            if (!Finished || answers.Count == 0) throw new InvalidOperationException("Finish the scenario first.");
            float l = 0, c = 0, s = 0;
            foreach (var a in answers) { l += a.listening; c += a.calmness; s += a.solution; }
            l /= answers.Count; c /= answers.Count; s /= answers.Count;
            float total = ScoreCalculator.Weighted(l, c, s);
            return new SessionResult {
                scenarioId = Scenario.id, scenarioTitle = Scenario.title, sector = Scenario.sector.ToString(),
                completedUtc = DateTime.UtcNow.ToString("o"), listening = l, calmness = c,
                solution = s, total = total, finalEmotion = Emotion,
                feedback = total >= 80 ? "Strong performance. Continue practicing clear, empathetic follow-up." :
                    total >= 60 ? "Develop more specific next steps and acknowledge the complainant's feelings." :
                    "Retry this scenario. Listen without blame, remain calm, and explain an actionable resolution path."
            };
        }
        public string SaveResult()
        {
            var result = GetResult();
            var directory = Path.Combine(Application.persistentDataPath, "sessions");
            Directory.CreateDirectory(directory);
            var filename = result.scenarioId + "_" + DateTime.UtcNow.ToString("yyyyMMdd_HHmmss_fff") + ".json";
            var path = Path.Combine(directory, filename);
            File.WriteAllText(path, JsonUtility.ToJson(result, true));
            return path;
        }
    }
}