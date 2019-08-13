using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

var magic = new byte[] { 73, 84, 66, 45, 76, 69, 86, 69, 76 };
var bits = new ulong[] {
    0x1, 0x2, 0x4, 0x8,
    0x10, 0x20, 0x40, 0x80,
    0x100, 0x200, 0x400, 0x800,
    0x1000, 0x2000, 0x4000, 0x8000,
    0x10000, 0x20000, 0x40000, 0x80000,
    0x100000, 0x200000, 0x400000, 0x800000,
    0x1000000, 0x2000000, 0x4000000, 0x8000000,
    0x10000000, 0x20000000, 0x40000000, 0x80000000,
    0x100000000, 0x200000000, 0x400000000, 0x800000000,
    0x1000000000, 0x2000000000, 0x4000000000, 0x8000000000,
    0x10000000000, 0x20000000000, 0x40000000000, 0x80000000000,
    0x100000000000, 0x200000000000, 0x400000000000, 0x800000000000,
    0x1000000000000, 0x2000000000000, 0x4000000000000, 0x8000000000000,
    0x10000000000000, 0x20000000000000, 0x40000000000000, 0x80000000000000,
    0x100000000000000, 0x200000000000000, 0x400000000000000, 0x800000000000000,
    0x1000000000000000, 0x2000000000000000, 0x4000000000000000, 0x8000000000000000,
};

if (Args.Count == 0) return;
if (!File.Exists(Args[0])) return;
var target = (Args.Count > 1 && !Path.GetFullPath(Args[0]).Equals(Path.GetFullPath(Args[1]), System.StringComparison.InvariantCultureIgnoreCase))
    ? Args[1] : null;
if (Args[0].EndsWith(".level.txt")) {
    using (var w = new BinaryWriter(File.OpenWrite(
        target ?? Args[0].Substring(0, Args[0].Length - 4)
    ))) {
        var lines = File
            .ReadAllLines(Args[0])
            .Where(l => !string.IsNullOrWhiteSpace(l) && !l.StartsWith("// "))
            .ToArray();
        w.Write(magic);
        for (var i = 0; i < 17; ++i) {
            uint u = 0;
            for (var b = 0; b < 32; ++b) {
                if (lines[i][b] == '#')
                    u |= (uint)bits[b];
            }
            w.Write(u);
        }
        for (var i = 17; i < 34; ++i) {
            ulong u = 0;
            for (var b = 0; b < 32; ++b) {
                switch(lines[i][b]) {
                    case '0':
                        break;
                    case '1':
                        u |= bits[b * 2];
                        break;
                    case '2':
                        u |= bits[b * 2 + 1];
                        break;
                    case '3':
                        u |= bits[b * 2];
                        u |= bits[b * 2 + 1];
                        break;
                }
            }
            w.Write(u);
        }
        // name;x;y;rotation[;burning]
        var regex = new Regex(@"^\s*([a-z0-9_]+)\s*;\s*(\d{1,3})x?\s*;\s*(\d{1,3})y?\s*;\s*(\d{1,3})°?(\s*;\s*B)?\s*$", RegexOptions.Compiled);
        var objects = lines.Skip(34).Select(l => regex.Match(l)).Where(m => m.Success).ToArray();
        w.Write((short)objects.Count());
        foreach (var obj in objects) {
            w.Write(obj.Groups[1].Value);
            w.Write(ushort.Parse(obj.Groups[2].Value));
            w.Write(ushort.Parse(obj.Groups[3].Value));
            w.Write(short.Parse(obj.Groups[4].Value));
            w.Write(obj.Groups[5].Success);
        }
        w.Flush();
        w.Close();
    }
} else if (Args[0].EndsWith(".level")) {
    using (var r = new BinaryReader(File.OpenRead(Args[0])))
    using (var w = new StreamWriter(target ?? Args[0] + ".txt")) {
        var bytes = r.ReadBytes(9);
        for (var i = 0; i < 9; ++i)
            if (bytes[i] != magic[i])
                return;
        for (var i = 0; i < 17; ++i) {
            uint u = r.ReadUInt32();
            for (var b = 0; b < 32; ++b)
                w.Write(((u & bits[b]) > 0) ? '#' : '_');
            w.WriteLine();
        }
        for (var i = 0; i < 17; ++i) {
            ulong u = r.ReadUInt64();
            for (var b = 0; b < 32; ++b) {
                w.Write((
                    ((u & bits[b * 2]) > 0 ? 1 : 0) +
                    ((u & bits[b * 2 + 1]) > 0 ? 2 : 0)
                ).ToString());
            }
            w.WriteLine();
        }
        var c = r.ReadInt16();
        for (int i = 0; i < c; ++i)
            w.WriteLine($"{r.ReadString()};{r.ReadUInt16()}x;{r.ReadUInt16()}y;{r.ReadInt16()}°{(r.ReadBoolean() ? ";B" : "")}");
        w.Flush();
        w.Close();
    }
}
