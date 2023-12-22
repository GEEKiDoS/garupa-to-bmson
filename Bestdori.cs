namespace Bestdori
{
    using System;
    using System.Collections.Generic;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Song
    {
        [JsonProperty("bgmId")]
        public string BgmId { get; set; }

        [JsonProperty("bgmFile")]
        public string BgmFile { get; set; }

        [JsonProperty("tag")]
        public string Tag { get; set; }

        [JsonProperty("bandId")]
        public int BandId { get; set; }

        [JsonProperty("achievements")]
        public Achievement[] Achievements { get; set; }

        [JsonProperty("jacketImage")]
        public string[] JacketImage { get; set; }

        [JsonProperty("seq")]
        public int Seq { get; set; }

        [JsonProperty("musicTitle")]
        public string[] MusicTitle { get; set; }

        [JsonProperty("lyricist")]
        public string[] Lyricist { get; set; }

        [JsonProperty("composer")]
        public string[] Composer { get; set; }

        [JsonProperty("arranger")]
        public string[] Arranger { get; set; }

        [JsonProperty("howToGet")]
        public string[] HowToGet { get; set; }

        [JsonProperty("publishedAt")]
        public string[] PublishedAt { get; set; }

        [JsonProperty("closedAt")]
        public string[] ClosedAt { get; set; }

        [JsonProperty("difficulty")]
        public Dictionary<string, Difficulty> Difficulty { get; set; }

        [JsonProperty("length")]
        public double Length { get; set; }

        [JsonProperty("notes")]
        public Dictionary<string, int> Notes { get; set; }

        [JsonProperty("bpm")]
        public Dictionary<string, Bpm[]> Bpm { get; set; }
    }

    public partial class Achievement
    {
        [JsonProperty("musicId")]
        public int MusicId { get; set; }

        [JsonProperty("achievementType")]
        public string AchievementType { get; set; }

        [JsonProperty("rewardType")]
        public string RewardType { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("rewardId", NullValueHandling = NullValueHandling.Ignore)]
        public int? RewardId { get; set; }
    }

    public partial class Bpm
    {
        [JsonProperty("bpm")]
        public int BpmBpm { get; set; }

        [JsonProperty("start")]
        public double Start { get; set; }

        [JsonProperty("end")]
        public double End { get; set; }
    }

    public partial class Difficulty
    {
        [JsonProperty("playLevel")]
        public int PlayLevel { get; set; }

        [JsonProperty("multiLiveScoreMap")]
        public Dictionary<string, MultiLiveScoreMap> MultiLiveScoreMap { get; set; }

        [JsonProperty("notesQuantity")]
        public int NotesQuantity { get; set; }

        [JsonProperty("scoreC")]
        public int ScoreC { get; set; }

        [JsonProperty("scoreB")]
        public int ScoreB { get; set; }

        [JsonProperty("scoreA")]
        public int ScoreA { get; set; }

        [JsonProperty("scoreS")]
        public int ScoreS { get; set; }

        [JsonProperty("scoreSS")]
        public int ScoreSs { get; set; }

        [JsonProperty("publishedAt", NullValueHandling = NullValueHandling.Ignore)]
        public string[] PublishedAt { get; set; }
    }

    public partial class MultiLiveScoreMap
    {
        [JsonProperty("musicId")]
        public int MusicId { get; set; }

        [JsonProperty("musicDifficulty")]
        public string MusicDifficulty { get; set; }

        [JsonProperty("multiLiveDifficultyId")]
        public int MultiLiveDifficultyId { get; set; }

        [JsonProperty("scoreS")]
        public int ScoreS { get; set; }

        [JsonProperty("scoreA")]
        public int ScoreA { get; set; }

        [JsonProperty("scoreB")]
        public int ScoreB { get; set; }

        [JsonProperty("scoreC")]
        public int ScoreC { get; set; }

        [JsonProperty("multiLiveDifficultyType")]
        public string MultiLiveDifficultyType { get; set; }

        [JsonProperty("scoreSS")]
        public int ScoreSs { get; set; }

        [JsonProperty("scoreSSS")]
        public int ScoreSss { get; set; }
    }

    public partial class Band
    {
        [JsonProperty("bandName")]
        public List<string> BandName { get; set; }
    }

    public class Bands : Dictionary<string, Band> { }
}
