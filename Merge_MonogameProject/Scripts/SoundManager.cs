using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Media;

namespace Merge_MonogameProject;

public class SoundManager
{
    public static SoundManager Instance { get; } = new SoundManager();
    
    private Song bgm;
    private SoundEffect clank;
    private float volume = 1.0f;

    public void LoadSounds(ContentManager content)
    {
        bgm = content.Load<Song>("Audio/Music");
        clank = content.Load<SoundEffect>("Audio/clank");
        
        MediaPlayer.Play(bgm);
    }
    
    
    public void PauseMusic()
    {
        MediaPlayer.Pause();
    }
    public void ResumeMusic()
    {
        MediaPlayer.Resume();
    }

    public void ChangeVolume(float newVolume)
    {
        volume = float.Clamp(newVolume, 0, 1);
        MediaPlayer.Volume = volume;
    }


    public void PlaySfx()
    {
        SoundEffectInstance oneShotSfx = clank.CreateInstance();
        oneShotSfx.Volume = volume;
        oneShotSfx.Play();
    }
}