using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;

public partial class NPC : CharacterBase, IInteractableObject
{
    const double _pathRecalcWaitTime = 1;
    const double _teleportWaitTime = 5;

    [Export]
    private string _dialogueFilePath;
    [Export]
    private string _dialogueFilePath2;
    [Export]
    private string _sysName;
    [Export]
    private NavigationAgent3D _navigationAgent;
    [Export]
    private int _speed = 2;
    [Export]
    private AnimationTree _animTree;
    [Export]
    private Area3D _visionArea;

    public string SysName => _sysName;

    public string InteractText => "Speak";
    public string InteractionSignalName { get; set; } = nameof(SignalsHelper.DialogueStart) + "(1)";

    public bool _skip;

    private PointOfInterest _finishPoint;
    private Vector3 _currentPoint;
    private double _pathRecalcTimer;
    private double _teleportTimer;

    private float _rotationTime;
    private float _currentRotationTime;
    private bool _isFirstStep;

    private float _eyeShakeTimer;

    public override void _Ready()
    {
        if (_navigationAgent == null)
        {
            _navigationAgent = GetNode<NavigationAgent3D>("NavigationAgent3D");
        }

        if (_animTree == null)
        {
            _animTree = GetNode<AnimationTree>("AnimationTree");
        }

        _animTree.Active = true;
        _rotationTime = 0;
        _currentRotationTime = 0;
        _isFirstStep = true;
        NpcsHolder.Instance.NPCs.Add(this);

        _animTree.Set("parameters/BlinkOneShot/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
    }

    public override void _PhysicsProcess(double delta)
    {
        if (PauseMenu.Instance.IsPaused)
        {
            return;
        }

        if (!IsCharacterStuck((float)delta))
        {
            return;
        }

        ProcessCharacterMovement((float)delta);
        ProcessHeadMovement((float)delta);
    }

    public void Interact()
    {
        SignalsHelper.ParseSignalName(InteractionSignalName);
    }

    private void Ball_OnBallPickUp()
    {
        /*if (_skip)
        {
            return;
        }

        DialogueOld dialogue;
        using (FileStream fs = new FileStream(_dialogueFilePath2, FileMode.Open))
        {
            dialogue = JsonSerializer.Deserialize<DialogueOld>(fs);
            dialogue.DialogueTextArray = dialogue.DialogueTextArray.OrderBy(a => a.Id).ToArray();
        }

        Player.Instance.LookAtOverTime(GlobalPosition);

        DialogueSystemOld.Instance.StartDialogue(dialogue, this);
        _skip = true;*/
    }

    public void ProcessCustomSignal(string signal, string[] parameters = null)
    {
        switch (signal)
        {
            case "PlayerDropBall":
                //PlayerDropBall();
                break;
            case "GoToPoint":
                GoToPoint(parameters);
                break;
        }
    }

    private void ProcessHeadMovement(float delta)
    {
        AddRandomEyeMovement(delta);

        RotateHead(delta, _visionArea.OverlapsBody(Player.Instance));
    }

    private void AddRandomEyeMovement(float delta)
    {
        _eyeShakeTimer += delta;
        if (_eyeShakeTimer < 0.2)
        {
            return;
        }

        _eyeShakeTimer = 0;
        var random = new Random();
        var rightEyeHoriz = (random.NextDouble() - 0.5) / 10;
        var LeftEyeHoriz = (random.NextDouble() - 0.5) / 10;
        var rightEyeVert = (random.NextDouble() - 0.5) / 10;
        var LeftEyeVert = (random.NextDouble() - 0.5) / 10;

        if (rightEyeHoriz < 0.5)
        {
            GetNode("Armature/Skeleton3D/Eyes_Low_R").Set("blend_shapes/eyeLookInRight", rightEyeHoriz);
        }
        else
        {
            GetNode("Armature/Skeleton3D/Eyes_Low_R").Set("blend_shapes/eyeLookOutRight", rightEyeHoriz - 0.5);
        }

        if (LeftEyeHoriz < 0.5)
        {
            GetNode("Armature/Skeleton3D/Eyes_Low_L").Set("blend_shapes/eyeLookInLeft", LeftEyeHoriz);
        }
        else
        {
            GetNode("Armature/Skeleton3D/Eyes_Low_L").Set("blend_shapes/eyeLookOutLeft", LeftEyeHoriz - 0.5);
        }

        if (rightEyeVert < 0.5)
        {
            GetNode("Armature/Skeleton3D/Eyes_Low_R").Set("blend_shapes/eyeLookDownRight", rightEyeVert);
        }
        else
        {
            GetNode("Armature/Skeleton3D/Eyes_Low_R").Set("blend_shapes/eyeLookUpRight", rightEyeVert - 0.5);
        }

        if (LeftEyeVert < 0.5)
        {
            GetNode("Armature/Skeleton3D/Eyes_Low_R").Set("blend_shapes/eyeLookDownLeft", LeftEyeVert);
        }
        else
        {
            GetNode("Armature/Skeleton3D/Eyes_Low_R").Set("blend_shapes/eyeLookUpLeft", LeftEyeVert - 0.5);
        }
    }

    private void RotateHead(float delta, bool isPlayerVisible)
    {
        var skeleton = (GetNode("Armature/Skeleton3D") as Skeleton3D);
        var boneRotation = skeleton.GetBonePoseRotation(4).Normalized();
        var rotation = skeleton.GetBonePoseRotation(3).Normalized();

        if (!isPlayerVisible && Mathf.Abs(boneRotation.AngleTo(rotation)) < 0.1)
        {
            return;
        }

        if (isPlayerVisible)
        {
            var pos = ToLocal(Player.Instance.GetChild<Node3D>(2).GlobalPosition) - skeleton.GetBoneGlobalPose(4).Origin;
            pos = new Vector3(pos.X, -pos.Y + 0.2f, pos.Z);

            rotation = skeleton
                .GetBonePose(4)
                .LookingAt(pos)
                .Basis
                .GetRotationQuaternion()
                .Normalized();
        }

        var rotationSpeed = delta * 10;

        if ((bool)_animTree.Get("parameters/RotationOneShot/active"))
        {
            rotationSpeed = delta * 2;
        }

        var yRotation = boneRotation.Slerp(rotation, rotationSpeed);

        (GetNode("Armature/Skeleton3D") as Skeleton3D).SetBonePoseRotation(4, yRotation);
    }

    private void ProcessCharacterMovement(float delta)
    {
        if (isCharacterRotating(delta))
        {
            return;
        }

        var velocity = Vector3.Zero;

        if (!IsOnFloor())
        {
            velocity.Y -= Gravity * delta;
        }

        if (!_navigationAgent.IsNavigationFinished())
        {
            var next = _navigationAgent.GetNextPathPosition();

            Turn(next);

            var dir = (next - GlobalPosition).Normalized();
            velocity.X = dir.X * _speed;
            velocity.Z = dir.Z * _speed;

            var moveSpeed = new Vector2(velocity.X, velocity.Z).Length();

            if (moveSpeed > 0 && moveSpeed < 1)
            {
                _animTree.Set("parameters/StateMachine/conditions/isIdle", false);
                _animTree.Set("parameters/StateMachine/conditions/isRunning", false);
                _animTree.Set("parameters/StateMachine/conditions/isWalking", true);
            }
            else
            {
                _animTree.Set("parameters/StateMachine/conditions/isIdle", false);
                _animTree.Set("parameters/StateMachine/conditions/isRunning", true);
                _animTree.Set("parameters/StateMachine/conditions/isWalking", false);
            }
        }
        else
        {
            _animTree.Set("parameters/StateMachine/conditions/isIdle", true);
            _animTree.Set("parameters/StateMachine/conditions/isRunning", false);
            _animTree.Set("parameters/StateMachine/conditions/isWalking", false);

            velocity.X = 0;
            velocity.Z = 0;
        }

        Velocity = velocity;
        MoveAndSlide();
    }

    private bool isCharacterRotating(float delta)
    {
        if (_rotationTime > _currentRotationTime)
        {
            _currentRotationTime += delta;
            return true;
        }
        else if (_rotationTime > 0)
        {
            if (_isFirstStep)
            {
                var skeleton = GetNode<Node3D>("Armature").GetNode<Skeleton3D>("Skeleton3D");
                var oldTransform = (GetChild(0) as Node3D).Transform;
                (GetChild(0) as Node3D).Rotation = new Vector3(0, 3.1415927f, 0);

                var bones = skeleton.GetBoneCount();
                var customRotation = _animTree.GetAnimation("CustomRotationAnimation");
                customRotation.Clear();
                for (int i = 0; i < bones; i++)
                {
                    var boneName = skeleton.GetBoneName(i);
                    if (i == 4)
                    {
                        continue;
                    }

                    var trackIndexPosition = customRotation.AddTrack(Animation.TrackType.Position3D);
                    var trackIndexRotation = customRotation.AddTrack(Animation.TrackType.Rotation3D);
                    customRotation.TrackSetPath(trackIndexPosition, "Armature/Skeleton3D:" + boneName);
                    customRotation.TrackSetPath(trackIndexRotation, "Armature/Skeleton3D:" + boneName);
                    if (i == 0)
                    {
                        var transform = skeleton.GetBonePose(i);

                        var oldRotation = 3.1415927f + oldTransform.Basis.GetEuler().Y;
                        var rotation = transform.Rotated(Vector3.Up, oldRotation).Basis.GetRotationQuaternion();
                        var position = transform.Rotated(Vector3.Up, oldRotation).Origin;

                        customRotation.TrackInsertKey(trackIndexPosition, 0.0f, position);
                        customRotation.TrackInsertKey(trackIndexRotation, 0.0f, rotation);
                    }
                    else
                    {
                        customRotation.TrackInsertKey(trackIndexRotation, 0.0f, skeleton.GetBonePoseRotation(i));
                        customRotation.TrackInsertKey(trackIndexPosition, 0.0f, skeleton.GetBonePosePosition(i));
                    }

                    customRotation.Length = 4f;
                }

                _animTree.Set("parameters/RotationTransition/transition_request", "rotationHelper");
                _animTree.Set("parameters/RotationOneShot/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
                _isFirstStep = false;
                return true;
            }

            if (!_isFirstStep)
            {
                _animTree.Set("parameters/RotationOneShot/request", (int)AnimationNodeOneShot.OneShotRequest.FadeOut);
                _currentRotationTime = 0;
                _rotationTime = 0;
                _isFirstStep = true;
            }

            return true;
        }

        return false;
    }

    public void Turn(Vector3 next)
    {
        if (_currentPoint != next)
        {
            _currentPoint = next;
            var oldRotation = Rotation;
            LookAt(_currentPoint);

            Rotation = new Vector3(oldRotation.X, Rotation.Y, oldRotation.Z);
            var yRotation = Rotation.Y - oldRotation.Y;

            if (yRotation > 3.1415927f)
            {
                yRotation = -1 * Mathf.Abs(6.2831855f - yRotation);
            } 
            else if (yRotation < -3.1415927f)
            {
                yRotation = Mathf.Abs(6.2831855f + yRotation);
            }

            if (yRotation <= 0.349066f && yRotation >= -0.349066f)
            {
                (GetChild(0) as Node3D).Rotation = new Vector3(0, 3.1415927f, 0);
                _currentRotationTime = 0;
                _rotationTime = 0;
            }
            else if (yRotation > 0 && yRotation <= 1.5707964f)
            {
                (GetChild(0) as Node3D).GlobalRotation = new Vector3(0, 3.1415927f + oldRotation.Y, 0);
                _rotationTime = yRotation / 1.5707964f * _animTree.GetAnimation("Left Turn").Length;
                _currentRotationTime = 0;
                _animTree.Set("parameters/RotationTransition/transition_request", "left");
                _animTree.Set("parameters/RotationOneShot/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
            }
            else if (yRotation >= -1.5707964f && yRotation < 0)
            {
                (GetChild(0) as Node3D).GlobalRotation = new Vector3(0, 3.1415927f + oldRotation.Y, 0);
                _rotationTime = yRotation / 1.5707964f * _animTree.GetAnimation("Right Turn").Length;
                _currentRotationTime = 0;
                _animTree.Set("parameters/RotationTransition/transition_request", "right");
                _animTree.Set("parameters/RotationOneShot/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
            }
            else if (yRotation > 1.5707964f || yRotation < -1.5707964f)
            {
                (GetChild(0) as Node3D).GlobalRotation = new Vector3(0, 3.1415927f + oldRotation.Y, 0);
                _rotationTime = yRotation / 3.1415927f * _animTree.GetAnimation("Right Turn 180").Length;
                _currentRotationTime = 0;
                _animTree.Set("parameters/RotationTransition/transition_request", "back");
                _animTree.Set("parameters/RotationOneShot/request", (int)AnimationNodeOneShot.OneShotRequest.Fire);
            }

            _rotationTime = Mathf.Abs(_rotationTime);
        }
    }

    private bool IsCharacterStuck(float delta)
    {
        if (!_navigationAgent.IsTargetReachable())
        {
            _pathRecalcTimer += delta;
            if (_pathRecalcTimer > _pathRecalcWaitTime)
            {
                SetTargetPosition(_finishPoint.GlobalPosition);
                _pathRecalcTimer = 0;
            }

            _teleportTimer += delta;
            if (_teleportTimer > _teleportWaitTime)
            {
                GlobalPosition = _navigationAgent.TargetPosition;
                _teleportTimer = 0;
            }

            return false;
        }

        _pathRecalcTimer = 0;
        _teleportTimer = 0;

        return true;
    }

    private void GoToPoint(string[] parameters)
    {
        _finishPoint = PointsHolder.Instance.Points.Single(a => a.Id == parameters[0]);
        SetTargetPosition(_finishPoint.GlobalPosition);
    }

    public void SetTargetPosition(Vector3 pos)
    {
        var map = GetWorld3D().NavigationMap;
        var p = NavigationServer3D.MapGetClosestPoint(map, pos);
        _navigationAgent.TargetPosition = p;
    }
}