using Godot;
using System;

public class AudioPlayer : Node2D
{
    private Button PlayStopButton;                              // The play button node
    private Button PauseButton;                                 // The Pause button node
    private Button ExitButton;                                  // The exit button node
    private AudioStreamPlayer Music;                            // The audio stream player music node
    private AudioStreamPlayer SoundEffect;                      // The audio stream player sound effect node
    private float MusicSliderPosition = 0.0f;                   // The music slider position
    private HSlider MusicSlider;                                // The horizontal music slider
    private bool UpdateMusicSliderPositionFromClick = false;    // Flag used to update the music slider position on click

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // Get all the nodes from the Music
        PlayStopButton = GetNode<Button>("CanvasLayer/MarginControl/VBoxContainer/Play");
        PauseButton = GetNode<Button>("CanvasLayer/MarginControl/VBoxContainer/Pause");
        ExitButton = GetNode<Button>("CanvasLayer/MarginControl/VBoxContainer/Exit");
        Music = GetNode<AudioStreamPlayer>("MusicPlayer");
        SoundEffect = GetNode<AudioStreamPlayer>("SoundEffect");
        MusicSlider = GetNode<HSlider>("CanvasLayer/MarginControl/VBoxContainer/MusicPosition");

        PauseButton.Disabled = true;        // Disable the pause button
        MusicSlider.Editable = false;       // Disable the music slider
    }
    private void OnPlayToggled(bool pressed)
    {
        // if the play button is pressed
        if(pressed)
        {
            PlayStopButton.Text = "Stop";       // Change the text on the play button to: Stop
            Music.Play();                       // Start to play the music
            PauseButton.Disabled = false;       // Make the pause button clickable
            MusicSlider.Editable = true;        // Make the music slider editable / dragable
        }
        // If the play button was un-presssed
        else
        {
            PlayStopButton.Text = "Play";       // Change the text on the play button to: Play
            Music.Stop();                       // Stop the music from playing
            ResetPauseButton();                 // Reset the pause button
            ResetMusicSlider();                 // Reset the music slider
        }
    }
    private void ResetMusicSlider()
    {
        MusicSlider.Editable = false;       // Disable the music slider
        MusicSlider.Value = 0.0f;           // Rest the music slider value to zero
    }
    private void ResetPauseButton()
    {
        PauseButton.Disabled = true;        // Disable the pause button
        Music.StreamPaused = false;         // Reset the stream paused to false
        PauseButton.Pressed = false;        // Set the pause button pressed state to false
        PauseButton.Text = "Pause";         // Reset the pause button text to: Pause
    }

    private void OnPauseToggled(bool pressed)
    {
        // If the pause button is pressed
        if(pressed)
        {
            PauseButton.Text = "Unpause";   // Change the text on the button to: Unpause
            Music.StreamPaused = true;      // Set the music stream to paused
            MusicSlider.Editable = false;   // Make the music slider uneditable
        }
        // If the pause button was un-pressed
        else
        {
            PauseButton.Text = "Pause";     // Change the text on the button to Pause
            Music.StreamPaused = false;     // Un-pause the music stream
            MusicSlider.Editable = true;    // Make the music slider editable again
        }
    }
    private void OnPlaySoundEffectPressed()
    {
        // Play the sound effect whenever the "Play Sound Effect" button is pressed
        SoundEffect.Play();
    }
    private void OnExitPressed()
    {
        // Exit the application if the Exit button was pressed
        GetTree().Quit();
    }

    private void OnMusicPositionGuiInput(InputEvent @event)
    {
        // When the user clicked on the slider
        if(@event is InputEventMouseButton mouseEvent && mouseEvent.IsPressed())
        {
            // Check which button
            switch((ButtonList)mouseEvent.ButtonIndex)
            {
                // If it was the left mouse button
                case ButtonList.Left:
                    //Flag that the music slider should be updated because of the click
                    UpdateMusicSliderPositionFromClick = true;
                    // Set the music slider position to the value of the music slider
                    MusicSliderPosition = (float)MusicSlider.Value;
                break;
            }
        }
    }
    // Whenever the music slider position value was changed
    private void OnMusicPositionValueChanged(float value)
    {
        // If the user clicked on the slider to move it
        if(UpdateMusicSliderPositionFromClick)
        {
            MusicSliderPosition = value;                    // Update the music slider position
            float musicLength = Music.Stream.GetLength();   // Get the length of the music stream

            // If the slider position isn't at the begining
            if(MusicSliderPosition > 0.0f && MusicSliderPosition < 99.9f)
            {
                // Convert the slider position to the position in the audio stream
                float position = musicLength * (MusicSliderPosition*0.01f);
                // Seek to the position in the audio stream
                Music.Seek(position);
            }
            UpdateMusicSliderPositionFromClick = false;     // Reset the update music slider position from click to: false
        }
    }

    private void UpdateMusicSliderPosition()
    {
        // If the music is playing
        if(Music.Playing)
        {
            // Get the length of the music
            float musicLength = Music.Stream.GetLength();
            // Convert the current position in the audio stream to a value between 0 and 100
            float progress = (Music.GetPlaybackPosition() / musicLength) * 100;
            // Update the slider value, so we know where in the song we are
            MusicSlider.Value = progress;
        }
    }
    public override void _Process(float delta)
    {
        // Update the music slider position whenever the music is playing
        UpdateMusicSliderPosition();
    }
}
