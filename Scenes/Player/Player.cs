using Godot;
using System;
using System.Dynamic;
using System.Runtime.CompilerServices;

public partial class Player : Godot.CharacterBody2D
{
	public int defaultSpeed = 400;
	int actualSpeed;
	int speed;

	Sprite2D playerSprite;
	GpuParticles2D particlesStationary;
	GpuParticles2D particlesMoving;
	private string path = "res://Gimp/Player/";
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		actualSpeed = defaultSpeed;
		playerSprite = GetNode<Sprite2D>("PlayerIcon");
		particlesStationary = GetNode<GpuParticles2D>("ParticlesStationary");
		particlesMoving = GetNode<GpuParticles2D>("ParticlesMoving");
	}
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionPressed("Sprint"))
		{
			speed = (int)(actualSpeed * 1.5f);
		}
		else 
		{
			speed = actualSpeed;
		}
	}
	public void GetInput()
	{
		Vector2 inputDirection = Input.GetVector("MoveLeft", "MoveRight", "MoveUp", "MoveDown");
		Velocity = inputDirection * speed; 
	}

	public override void _PhysicsProcess(double delta)
	{
		GetInput();
		MoveAndSlide();
		UpdatePlayerTexture();
	}

	private void UpdatePlayerTexture()
	{
		if (Input.IsActionPressed("MoveLeft"))
		{
			playerSprite.Texture = GD.Load<Texture2D>(path + "playerLeft.png");
			playerSprite.RotationDegrees = -15;
			particlesStationary.Emitting = false;
			particlesMoving.Emitting = true;
		}
		else if (Input.IsActionPressed("MoveRight"))
		{
			playerSprite.Texture = GD.Load<Texture2D>(path + "playerRight.png");
			playerSprite.RotationDegrees = 15;
			particlesStationary.Emitting = false;
			particlesMoving.Emitting = true;
		}
		else if (Input.IsActionPressed("MoveUp"))
		{
			playerSprite.Texture = GD.Load<Texture2D>(path + "playerUp.png");
			playerSprite.RotationDegrees = 0;
			particlesStationary.Emitting = false;
			particlesMoving.Emitting = true;
		}
		else if (Input.IsActionPressed("MoveDown"))
		{
			playerSprite.Texture = GD.Load<Texture2D>(path + "player.png");
			playerSprite.RotationDegrees = 0;
			particlesStationary.Emitting = false;
			particlesMoving.Emitting = true;
		}
		else
		{
			playerSprite.Texture = GD.Load<Texture2D>(path + "player.png");
			playerSprite.RotationDegrees = 0;
			particlesStationary.Emitting = true;
			particlesMoving.Emitting = false;
		}
	}

	public string Save()
	{
		string X = Position.X.ToString().Replace(',', '.');
		string Y = Position.Y.ToString().Replace(',', '.');

		return $"[{X}, {Y}]";
	}
	
	public void Load(string data)
	{
		dynamic parsedData = Newtonsoft.Json.JsonConvert.DeserializeObject(data);
		Position = new ((float)parsedData[0], (float)parsedData[1]);
		//GD.Print("Player Loaded");
	}

	private void CollidedWithWater(Node2D node)
	{
		actualSpeed = 150;
	}

	private void ExitedFromWater(Node2D node)
	{
		actualSpeed = defaultSpeed;
	}
}
