using Bestdori;
using Newtonsoft.Json;
using System.Globalization;
using System.Text.RegularExpressions;

Console.Write("输入要下载的歌曲 id:");
var id = int.Parse(Console.ReadLine());

var diffmap = new Dictionary<string, string>
{
    { "0", "easy" },
    { "1", "normal" },
    { "2", "hard" },
    { "3", "expert" },
    { "4", "special" },
};

var folder = ((id - 1) / 10 + 1) * 10;

var client = new HttpClient();
client.DefaultRequestHeaders.UserAgent.ParseAdd("BandoriToBmson/1.0");

async Task<T> Get<T>(string url)
{
    var result = await client.GetAsync(url);
    var json = await result.Content.ReadAsStringAsync();

    if (typeof(T) == typeof(string))
    {
        return (T)(object)json;
    }

    return JsonConvert.DeserializeObject<T>(json)!;
}

async Task<byte[]> GetBytes(string url)
{
    var result = await client.GetAsync(url);
    return await result.Content.ReadAsByteArrayAsync();
}

Console.WriteLine("获取信息中...");

var bands = await Get<Bands>("https://bestdori.com/api/bands/all.1.json");
var song = await Get<Song>($"https://bestdori.com/api/songs/{id}.json");

Console.WriteLine(song.MusicTitle[0]);

if (!Directory.Exists(song.BgmFile))
    Directory.CreateDirectory(song.BgmFile);

if (!File.Exists($"{song.BgmFile}/bgm{id}.mp3"))
{
    Console.WriteLine("下载BGM中...");

    var bgm = await GetBytes($"https://bestdori.com/assets/jp/sound/bgm{id:000}_rip/bgm{id:000}.mp3");
    File.WriteAllBytes($"{song.BgmFile}/bgm{id}.mp3", bgm);
}

if (!File.Exists($"{song.BgmFile}/{song.JacketImage[0]}.png"))
{
    Console.WriteLine("下载封面中...");

    var jacket = await GetBytes($"https://bestdori.com/assets/jp/musicjacket/musicjacket{folder}_rip/assets-star-forassetbundle-startapp-musicjacket-musicjacket{folder}-{song.JacketImage[0]}-jacket.png");
    File.WriteAllBytes($"{song.BgmFile}/{song.JacketImage[0]}.png", jacket);
}

if (!File.Exists($"{song.BgmFile}/bd.wav"))
    File.Copy("bd.wav", $"{song.BgmFile}/bd.wav");

if (!File.Exists($"{song.BgmFile}/flick.wav"))
    File.Copy("bd.wav", $"{song.BgmFile}/flick.wav");

Console.WriteLine("转换谱面中...");

foreach (var diff in song.Difficulty)
{
    Console.WriteLine($"下载 {diff.Key}...");
    var chart_file = await Get<string>($"https://bestdori.com/assets/jp/musicscore/musicscore{folder}_rip/{song.BgmFile}_{diffmap[diff.Key]}.txt");
    chart_file = chart_file.Replace("\r", "");
    var resolution_per_measure = 1920;

    var lines = chart_file.Split("\n");

    var chart = new Bmson();
    chart.info = new BmsonInfo
    {
        title = song.MusicTitle[0],
        artist = bands[song.BandId.ToString()].BandName[0],
        genre = "Bandori",
        level = song.Difficulty[diff.Key].PlayLevel,
        back_image = song.JacketImage[0],
        resolution = resolution_per_measure / 4,
        chart_name = diff.Key switch
        {
            "0" => "BEGINNER",
            "1" => "NORMAL",
            "2" => "HYPER",
            "3" => "ANOTHER",
            "4" => "LEGGENDARIA",
            _ => throw new IndexOutOfRangeException("unexcepted rank"),
        },
    };

    Console.WriteLine(chart.info.chart_name);

    chart.lines = new List<BarLine>();

    chart.bga = new BGA();

    chart.sound_channels = new()
    {
        new SoundChannel
        {
            name = $"bgm{id}.mp3",
            notes = new List<Note>
            {
                new Note
                {
                    x = 0,
                    y = 0,
                    l = 0,
                    c = true,
                }
            },
        },
        new SoundChannel
        {
            name = "bd.wav",
            notes = new List<Note>(),
        },
        new SoundChannel
        {
            name = "bd.wav",
            notes = new List<Note>(),
        },
        new SoundChannel
        {
            name = "flick.wav",
            notes = new List<Note>(),
        }
    };

    chart.bpm_events = new List<BpmEvent>();

    var tap_channel = chart.sound_channels[2];
    var tap_channel_1 = chart.sound_channels[1];
    var flick_channel = chart.sound_channels[3];

    var tap_notes = new List<Note>();

    var channel_map = new Dictionary<string, string>();
    var charges = new int[8];

    foreach (var line in lines)
    {
        var match = Regex.Match(line, "#BPM (\\d+)");
        if (match.Success)
        {
            chart.info.init_bpm = int.Parse(match.Groups[1].Value);
            continue;
        }

        match = Regex.Match(line, "#WAV(.{2}) (.+)");
        if (match.Success)
        {
            switch (match.Groups[2].Value)
            {
                case "bd.wav":
                case "skill.wav":
                case "fever_note.wav":
                case "slide_a.wav":
                case "slide_end_a.wav":
                case "slide_b.wav":
                case "slide_end_b.wav":
                    channel_map.Add(match.Groups[1].Value, "tap");
                    continue;

                case "flick.wav":
                case "fever_note_flick.wav":
                case "slide_end_flick_a.wav":
                case "slide_end_flick_b.wav":
                    channel_map.Add(match.Groups[1].Value, "flick");
                    continue;
            }

            continue;
        }

        match = Regex.Match(line, "#(\\d{3})(\\d{2}):(.+)");
        if (!match.Success) continue;

        var mesaure = int.Parse(match.Groups[1].Value);
        var notes = match.Groups[3].Value;

        var ms_res = notes.Length / 2;
        var ms_pulse = mesaure * resolution_per_measure;
        var pulse_per_unit = resolution_per_measure / ms_res;

        var channel = int.Parse(match.Groups[2].Value);

        var is_charge = false;
        if (channel > 20)
        {
            channel -= 40;
            is_charge = true;
        }

        var x = channel switch
        {
            11 => 1,
            12 => 2,
            13 => 3,
            14 => 4,
            15 => 5,
            16 => 0,
            18 => 6,
            2 => 8,
            3 => 8,
            _ => 0,
        };

        if (chart.lines.Count(v => v.y == ms_pulse) == 0)
        {
            chart.lines.Add(new BarLine
            {
                y = ms_pulse,
            });
        }

        for (var i = 0; i < ms_res; i++)
        {
            var ch = notes.Substring(i * 2, 2);

            if (x == 8)
            {
                chart.bpm_events.Add(new BpmEvent
                {
                    bpm = int.Parse(ch, NumberStyles.HexNumber),
                    y = ms_pulse + pulse_per_unit * i,
                });

                continue;
            }

            if (!channel_map.ContainsKey(ch)) continue;

            var note = new Note();
            note.x = x + 1;
            note.y = ms_pulse + pulse_per_unit * i;
            note.c = true;
            note.l = 0;

            Console.Write($"{ch}, ({note.y}) = {note.x}, ");

            if (is_charge) note.l = -1;

            tap_notes.Add(note);

            if (channel_map[ch] == "flick")
            {
                if (flick_channel.notes.FirstOrDefault(v => v.y == note.y) != null)
                    continue;

                var flick = new Note();
                flick.x = 8;
                flick.y = note.y;
                flick.c = false;
                flick.l = 0;

                flick_channel.notes.Add(flick);
            }
        }

        Console.WriteLine("");
    }

    tap_notes.Sort((a, b) => a.y - b.y);

    foreach (var note in tap_notes)
    {
        if (!note.c) continue;

        note.c = false;

        if (note.l == -1)
        {
            var end = tap_notes.Find(v => v.c && v.l == -1 && v.x == note.x && v.y > note.y);

            if (end == null)
            {
                note.l = (note.y / resolution_per_measure + 1) * resolution_per_measure - note.y;
            }
            else
            {
                end.c = false;
                note.l = end.y - note.y;
            }
        }

        var conflict = tap_notes.Find(v => v.c && v.y == note.y);
        if (conflict != null)
        {
            conflict.c = false;
            if (conflict.l == -1)
            {
                var end = tap_notes.Find(v => v.c && v.l == -1 && v.x == conflict.x && v.y > conflict.y);

                if (end == null)
                {
                    conflict.l = (conflict.y / resolution_per_measure + 1) * resolution_per_measure - conflict.y;
                }
                else
                {
                    end.c = false;
                    conflict.l = end.y - conflict.y;
                }
            }

            tap_channel_1.notes.Add(conflict);
        }

        tap_channel.notes.Add(note);
    }

    File.WriteAllText($"{song.BgmFile}/{song.BgmFile}_{diffmap[diff.Key]}.bmson", JsonConvert.SerializeObject(chart));
}
