using Godot;
using System;

using Godot;

public partial class VolumeSliderUI : HSlider
{
	private int _busIndex;

	private AudioStreamPlayer2D _testAudioStream;
	
	private const int _initialValue = -15;
	
	public override void _Ready()
	{
		// Get the bus index of the "Master" bus (change if you use a different bus)
		_busIndex = AudioServer.GetBusIndex("Master");

		// Set slider range (decibels). 
		// Adjust these values as needed for your project.
		MinValue = -60;  // Lower bound in dB
		MaxValue = 0;    // Upper bound in dB
		Step = 0.5;      // Optional step size

		// Set the slider initial value to the current bus volume in dB
		AudioServer.SetBusVolumeDb(_busIndex, _initialValue);
		Value = AudioServer.GetBusVolumeDb(_busIndex);
        
		// Connect the ValueChanged event to our callback
		ValueChanged += OnValueChanged;
		
		// Get the test audio stream player
		_testAudioStream = GetNode<AudioStreamPlayer2D>("TestAudioStream");
	}

	// This method is called every time the slider value changes
	private void OnValueChanged(double value)
	{
		// Set the bus volume in decibels to match the slider
		AudioServer.SetBusVolumeDb(_busIndex, (float)value);
		
		// Play a test sound to hear the volume change
		_testAudioStream.Play();
	}
}
