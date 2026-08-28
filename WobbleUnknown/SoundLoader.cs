using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using UnityEngine;
using Object = UnityEngine.Object;

namespace WobbleUnknown
{
    public static class SoundLoader
    {
        public static readonly Dictionary<string, AudioClip> Sounds = new Dictionary<string, AudioClip>();

        public static void LoadAllEmbeddedSounds()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceNames = assembly.GetManifestResourceNames()
                .Where(name => name.Contains(".Sounds.") && name.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            foreach (var resourceName in resourceNames)
            {
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {
                    if (stream == null) continue;

                    var fileData = new byte[stream.Length];
                    stream.Read(fileData, 0, fileData.Length);

                    var soundName = GetCleanFileName(resourceName);

                    var clip = OpenWavAsAudioClip(fileData, soundName);
                    if (clip != null)
                    {
                        Sounds[soundName] = clip;
                        WobbleUnknownPlugin.Instance.Log.LogInfo($"Successfully embedded sound loaded: '{soundName}'");
                    }
                }
            }
        }

        private static string GetCleanFileName(string resourceName)
        {
            var soundsIndex = resourceName.IndexOf(".Sounds.", StringComparison.OrdinalIgnoreCase);
            var sub = soundsIndex != -1 ? resourceName.Substring(soundsIndex + 8) : resourceName;
            
            if (sub.EndsWith(".wav", StringComparison.OrdinalIgnoreCase))
            {
                sub = sub.Substring(0, sub.Length - 4);
            }

            return sub;
        }

        private static AudioClip OpenWavAsAudioClip(byte[] wavBytes, string clipName)
        {
            try
            {
                var channels = BitConverter.ToInt16(wavBytes, 22);
                var sampleRate = BitConverter.ToInt32(wavBytes, 24);
                var pos = 12;

                while (pos < wavBytes.Length - 4)
                {
                    if (wavBytes[pos] == 'd' && wavBytes[pos + 1] == 'a' && wavBytes[pos + 2] == 't' && wavBytes[pos + 3] == 'a')
                    {
                        pos += 4;
                        break;
                    }

                    pos++;
                }

                var subChunk2Size = BitConverter.ToInt32(wavBytes, pos);
                pos += 4;

                var sampleCount = subChunk2Size / 2;
                var audioData = new float[sampleCount];

                for (var i = 0; i < sampleCount; i++)
                {
                    var sample = BitConverter.ToInt16(wavBytes, pos);
                    audioData[i] = sample / 32768f;
                    pos += 2;
                }

                var clip = AudioClip.Create(clipName, sampleCount / channels, channels, sampleRate, false);
                clip.SetData(audioData, 0);
                return clip;
            }
            catch (Exception ex)
            {
                WobbleUnknownPlugin.Instance.Log.LogError($"Failed to parse WAV '{clipName}': {ex.Message}");
                return null;
            }
        }

        public static void PlayRandSound()
        {
            if (Sounds.Count == 0) return;

            var randomIndex = UnityEngine.Random.Range(0, Sounds.Count);
            var randomSound = Sounds.ElementAt(randomIndex).Value;

            var audioSource = new GameObject("TempAudio").AddComponent<AudioSource>();
            audioSource.clip = randomSound;
            audioSource.pitch = UnityEngine.Random.Range(0.9f, 1.1f);
            audioSource.Play();

            Object.Destroy(audioSource.gameObject, randomSound.length);
        }
    }
}