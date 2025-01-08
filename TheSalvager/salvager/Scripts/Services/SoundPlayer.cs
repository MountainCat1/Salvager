using Godot;

namespace Services;

public interface ISoundPlayer
{
    /// <summary>
    /// Play a sound without positional data
    /// </summary>
    void PlaySound(AudioStream audioStream);
    /// <summary>
    /// Play a sound with positional data
    /// </summary>
    void PlaySound(AudioStream audioStream, Vector2 position);
}

public partial class SoundPlayer : Node2D, ISoundPlayer
{
    public void PlaySound(AudioStream audioStream)
    {
        // Create an AudioStreamPlayer for non-positional audio
        var audioPlayer = new AudioStreamPlayer()
        {
            Stream = audioStream
        };
        
        // Clean up the node when the sound finishes playing
        audioPlayer.Finished += () =>
        {
            audioPlayer.QueueFree();
        };
        
        // Play the sound and add the node to the scene
        AddChild(audioPlayer);
        audioPlayer.Play();
    }
    
    public void PlaySound(AudioStream audioStream, Vector2 position)
    {
        // Create an AudioStreamPlayer2D for positional audio
        var audioPlayer = new AudioStreamPlayer2D
        {
            Stream = audioStream,
            
            // Fine-tune attenuation settings for spatial audio
            Attenuation = 1.0f, // Higher values make sound fade out faster
        };

        // Set the position if provided
        audioPlayer.Position = position;

        // Clean up the node when the sound finishes playing
        audioPlayer.Finished += () =>
        {
            audioPlayer.QueueFree();
        };

        // Play the sound and add the node to the scene
        AddChild(audioPlayer);
        audioPlayer.Play();
    }
}