// top-level object
public class Bmson
{
    public string version = "1.0.0";        // bmson version
    public BmsonInfo info = new();           // information, e.g. title, artist, …
    public List<BarLine> lines = new();          // location of bar-lines in pulses
    public List<BpmEvent>? bpm_events;     // bpm changes
    public List<StopEvent>? stop_events;    // stop events
    public List<SoundChannel> sound_channels = new(); // note data
    public BGA bga = new();            // bga data
}

// header information
public class BmsonInfo
{
    public string title = "";                 // self-explanatory
    public string subtitle = "";         // self-explanatory
    public string artist = "";                // self-explanatory
    public List<string> subartists = new();       // ["key:value"]
    public string genre = "";                 // self-explanatory
    public string mode_hint = "beat-7k"; // layout hints, e.g. "beat-7k", "popn-5k", "generic-nkeys"
    public string chart_name = "NORMAL";            // e.g. "HYPER", "FOUR DIMENSIONS"
    public int level;                 // self-explanatory
    public double init_bpm;              // self-explanatory
    public double judge_rank = 100;      // relative judge width
    public double total = 100;           // relative lifebar gain
    public string? back_image = "";            // background image filename
    public string? eyecatch_image = "";        // eyecatch image filename
    public string? banner_image = "";          // banner image filename
    public string? preview_music = "";         // preview music filename
    public int resolution;      // pulses per quarter note
}

// bar-line event
public class BarLine
{
    public int y; // pulse number
}
// sound channel
public class SoundChannel
{
    public string name = ""; // sound file name
    public List<Note> notes = new();   // notes using this sound
}

// sound note
public class Note
{
    public int x;           // lane
    public int y; // pulse number
    public int l; // length (0: normal note; greater than zero (length in pulses): long note)
    public bool c;       // continuation flag
}

// bpm note
public class BpmEvent
{
    public int y; // pulse number
    public double bpm;      // bpm
}

// stop note
public class StopEvent
{
    public int y;        // pulse number
    public int duration; // stop duration (pulses to stop)
}

// for any custom classes of timing,
// follow format as bpmevent or stopevent.
// bga
public class BGA
{
    public List<BGAHeader> bga_header = new();   // picture id and filename
    public List<BGAEvent> bga_events = new();   // picture sequence
    public List<BGAEvent> layer_events = new(); // picture sequence overlays bga_notes
    public List<BGAEvent> poor_events = new();  // picture sequence when missed
}

// picture file
public class BGAHeader
{
    public int id; // self-explanatory
    public string name = "";   // picture file name
}

// bga note
public class BGAEvent
{
    public int y;  // pulse number
    public int id; // corresponds to BGAHeader.id
}
