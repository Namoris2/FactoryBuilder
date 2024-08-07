using Godot;
using Godot.Collections;
using System;

public partial class PlayerCamera : Camera2D
{
	Vector2 minZoom = new (.4f, .4f);
	Vector2 maxZoom = new (2.4f, 2.4f);
	Vector2 zoomSpeed = new (.2f, .2f);

	bool animationsPaused = false;
	Node2D animatedBuildingPartsController;

	[Signal]
	public delegate void CameraZoomChangedEventHandler(float zoom);

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animatedBuildingPartsController = GetNode<Node2D>("/root/main/World/AnimatedBuildingPartsController");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("ZoomIn") && this.Zoom[0] < maxZoom[0])
		{
			this.Zoom += zoomSpeed / 4;
			ResumeBuildingAnimations();
		}

		if (Input.IsActionPressed("ZoomOut") && this.Zoom[0] > minZoom[0])
		{
			this.Zoom -= zoomSpeed / 4;
			PauseBuildingAnimations();
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			// zoom in
			if (mouseEvent.ButtonIndex == MouseButton.WheelUp && this.Zoom[0] < maxZoom[0])
			{
				this.Zoom += zoomSpeed;
				ResumeBuildingAnimations();
			}

			// zoom out
			if (mouseEvent.ButtonIndex == MouseButton.WheelDown && this.Zoom[0] > minZoom[0])
			{
				this.Zoom -= zoomSpeed;
				PauseBuildingAnimations();
			}
		}
	}

	private void PauseBuildingAnimations()
	{
		if (animationsPaused || (int)(Zoom[0] * 10) > 2) { return; }

		Array<Node> parts = GetTree().GetNodesInGroup("AnimatedPart");
	
		foreach (Node part in parts)
		{
			AnimationPlayer animationPlayer = part.GetNode<AnimationPlayer>("AnimationPlayer");
			animationPlayer.Pause();
		}
		animationsPaused = true;
		animatedBuildingPartsController.Hide();
		//GD.Print("Pausing Animations");
	}

	private void ResumeBuildingAnimations()
	{
		if (!animationsPaused || (int)(Zoom[0] * 10) <= 2) { return; }

		Array<Node> parts = GetTree().GetNodesInGroup("AnimatedPart");

		foreach (Node part in parts)
		{
			AnimationPlayer animationPlayer = part.GetNode<AnimationPlayer>("AnimationPlayer");
			animationPlayer.Play();
		}
		animationsPaused = false;
		animatedBuildingPartsController.Show();
		//GD.Print("Resuming Animations");
	}
}

