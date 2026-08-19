using System.Runtime.InteropServices;

using FlyleafLib.MediaFramework.MediaDemuxer;

namespace FlyleafLib.MediaFramework.MediaStream;

public unsafe class AttachmentStream : StreamBase
{
    public string FileName { get; private set; }
    public string MimeType { get; private set; }
    public byte[] Data { get; private set; }

    public AttachmentStream(Demuxer demuxer, AVStream* st) : base(demuxer, st)
        => Type = MediaType.Data;

    public override void Initialize()
    {
        AVDictionaryEntry* tag = null;
        while ((tag = av_dict_get(AVStream->metadata, "", tag, DictReadFlags.IgnoreSuffix)) != null)
        {
            var key = BytePtrToStringUTF8(tag->key);

            if (key.Equals("filename", StringComparison.OrdinalIgnoreCase))
                FileName = BytePtrToStringUTF8(tag->value);
            else if (key.Equals("mimetype", StringComparison.OrdinalIgnoreCase))
                MimeType = BytePtrToStringUTF8(tag->value);
        }

        if (cp->extradata_size > 0 && cp->extradata != null)
        {
            Data = new byte[cp->extradata_size];
            Marshal.Copy((IntPtr)cp->extradata, Data, 0, cp->extradata_size);
        }

        if (CanDebug)
            Demuxer.Log.Debug($"Stream Info (Filled)\r\n{GetDump()}");
    }
}
