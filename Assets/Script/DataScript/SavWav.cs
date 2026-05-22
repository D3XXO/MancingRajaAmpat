using System;
using System.IO;
using UnityEngine;

public static class SavWav
{
    private const int HEADER_SIZE = 44;

    public static byte[] ConvertToWav(AudioClip clip)
    {
        using (var stream = new MemoryStream())
        {
            stream.Write(new byte[HEADER_SIZE], 0, HEADER_SIZE);
            ConvertAndWrite(stream, clip);
            WriteHeader(stream, clip);
            return stream.ToArray();
        }
    }

    private static void ConvertAndWrite(MemoryStream stream, AudioClip clip)
    {
        var samples = new float[clip.samples * clip.channels];
        clip.GetData(samples, 0);

        short[] intData = new short[samples.Length];
        byte[] bytesData = new byte[samples.Length * 2];

        const float rescaleFactor = 32767;

        for (int i = 0; i < samples.Length; i++)
        {
            intData[i] = (short)(samples[i] * rescaleFactor);
            byte[] byteArr = BitConverter.GetBytes(intData[i]);
            byteArr.CopyTo(bytesData, i * 2);
        }

        stream.Write(bytesData, 0, bytesData.Length);
    }

    private static void WriteHeader(MemoryStream stream, AudioClip clip)
    {
        int hz = clip.frequency;
        int channels = clip.channels;

        stream.Seek(0, SeekOrigin.Begin);

        stream.Write(System.Text.Encoding.UTF8.GetBytes("RIFF"), 0, 4);
        stream.Write(BitConverter.GetBytes(stream.Length - 8), 0, 4);
        stream.Write(System.Text.Encoding.UTF8.GetBytes("WAVE"), 0, 4);
        stream.Write(System.Text.Encoding.UTF8.GetBytes("fmt "), 0, 4);
        stream.Write(BitConverter.GetBytes(16), 0, 4);
        
        UInt16 audioFormat = 1; // PCM
        stream.Write(BitConverter.GetBytes(audioFormat), 0, 2);
        stream.Write(BitConverter.GetBytes(channels), 0, 2);
        
        stream.Write(BitConverter.GetBytes(hz), 0, 4);
        
        stream.Write(BitConverter.GetBytes(hz * channels * 2), 0, 4);
        
        UInt16 blockAlign = (ushort)(channels * 2);
        stream.Write(BitConverter.GetBytes(blockAlign), 0, 2);
        
        UInt16 bps = 16;
        stream.Write(BitConverter.GetBytes(bps), 0, 2);
        
        stream.Write(System.Text.Encoding.UTF8.GetBytes("data"), 0, 4);
        stream.Write(BitConverter.GetBytes(stream.Length - HEADER_SIZE), 0, 4);
    }
}