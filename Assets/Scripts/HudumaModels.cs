using System;
using System.Collections.Generic;
using UnityEngine;

namespace HudumaSimVR
{
    public enum Sector { TaxOmbudsman, Banking, Telecommunications }
    public enum Skill { Listening, Calmness, Solution }

    [Serializable]
    public class ReplyOption
    {
        public string text;
        public string reaction;
        public int listening;
        public int calmness;
        public int solution;
        public int emotionDelta;
        public ReplyOption(string text, string reaction, int listening, int calmness, int solution, int emotionDelta)
        {
            this.text = text; this.reaction = reaction; this.listening = listening;
            this.calmness = calmness; this.solution = solution; this.emotionDelta = emotionDelta;
        }
    }

    [Serializable]
    public class ComplaintScenario
    {
        public string id;
        public Sector sector;
        public string title;
        public string opening;
        public string[] prompts;
        public ReplyOption[][] choices;
        public ComplaintScenario(string id, Sector sector, string title, string opening, string[] prompts, ReplyOption[][] choices)
        {
            this.id = id; this.sector = sector; this.title = title; this.opening = opening;
            this.prompts = prompts; this.choices = choices;
        }
    }

    [Serializable]
    public class SessionResult
    {
        public string scenarioId;
        public string scenarioTitle;
        public string sector;
        public string completedUtc;
        public float listening;
        public float calmness;
        public float solution;
        public float total;
        public int finalEmotion;
        public string feedback;
    }

    public static class ScoreCalculator
    {
        public static float Weighted(float listening, float calmness, float solution)
        {
            return Mathf.Round((0.30f * listening + 0.30f * calmness + 0.40f * solution) * 10f) / 10f;
        }
    }
}