using Godot;
using Godot.Collections;
using Array = Godot.Collections.Array;
using System;
using System.Runtime.CompilerServices;
using System.Collections.Specialized;

namespace MechGame {
    // Enums //
    public enum ChassisType {
        TwoLegs,
        FourLegs,
        SixLegs,
        Treads,
        Hover
    }
    // Helper Functions //
    public static class HelperClass {
        public static void AssignPlayerToMech(Node2D scene, Mech mech) {
            scene.AddChild(mech.Controller);
            mech.Controller.AddChild(ResourceLoader.Load<PackedScene>("res://Scenes/MechCamera.tscn").Instantiate());
        }
        public static float Lerpf(float a, float b, float t) => a + (b - a) * t;
    }

    // Mech Classes //
    public class Mech {
        public MechController Controller;
        public CharacterBody2D CharacterBody => Controller;
        public Chassis Chassis;
        public Hull Hull;
        public float Heat;
        //Constructors
        public Mech(Chassis chassis, Hull hull) {
            this.Controller = ResourceLoader.Load<PackedScene>("res://Scenes/MechScene.tscn").Instantiate<MechController>();
            this.Controller.MechObject = this;
            this.Chassis = chassis;
            this.CharacterBody.AddChild(chassis.Sprite);
            this.Hull = hull;
            this.CharacterBody.AddChild(hull.Sprite);
        }
        public Mech() : this(new PlaceholderFourLegs(), new PlaceholderHull()) { //empty args, uses defaults

        }
    }

    public abstract class MechPart {
        public string Name;
    }

    public abstract class Chassis : MechPart {
        public float Integrity;
        public float Armor;
        public ChassisType Type;
        public float MoveSpeed;
        public float RotateSpeed;
        public float WeightCapacity;
        public float RotorSpeed;
        public Sprite2D Sprite;
        public static Sprite2D CreateSprite(string texturePath) {
            Sprite2D sprite = new Sprite2D();
            sprite.Scale = new Vector2(0.5f, 0.5f);
            sprite.ZIndex = -1;
            sprite.Texture = ResourceLoader.Load<Texture2D>(texturePath);
            sprite.Name = "ChassisSprite";
            return sprite;
        }
    }

    public abstract class Hull : MechPart {
        public float Integrity;
        public float Armor;
        public Mech Mech;
        public float Weight;
        public float RotorSpeed;
        public Sprite2D Sprite;
        public Primary[] Primaries;
        public Vector2[] PrimaryOffsets;
        public Secondary[] Secondaries;
        public Vector2[] SecondaryOffsets;
        public Utility[] Utilities;
        public Vector2[] UtilityOffsets;
        //Methods
        public void AttachPart(MechPart part, int slot = 1) {
            switch (part) {
                case Primary primaryPart:
                    if (slot > this.PrimaryOffsets.Length) throw new System.Exception("Invalid primary slot '" + slot + "' in hull '" + this.Name + "'");
                    this.Primaries[slot - 1] = primaryPart;
                    this.Sprite.AddChild(primaryPart.Sprite);
                    primaryPart.Sprite.Position = PrimaryOffsets[slot - 1] + new Vector2(0f, 120f);
                    primaryPart.Sprite.RotationDegrees = -90f;
                    break;
                case Secondary secondaryPart:
                    if (slot - 1 > this.SecondaryOffsets.Length) throw new System.Exception("Invalid secondary slot '" + slot + "' in hull '" + this.Name + "'");
                    break;
                case Utility utilityPart:
                    if (slot - 1 > this.UtilityOffsets.Length) throw new System.Exception("Invalid utility slot '" + slot + "' in hull '" + this.Name + "'");
                    break;
                default:
                    throw new System.Exception("Attempted to attach part of invalid type");
            }
        }
        public static Sprite2D CreateSprite(string texturePath) {
            Sprite2D sprite = new Sprite2D();
            sprite.Texture = ResourceLoader.Load<Texture2D>(texturePath);
            sprite.Scale = new Vector2(0.25f, 0.25f); //todo: synchronize all sprite scales (chassis, hull, weapons)
            sprite.ZIndex = -1;
            sprite.RotationDegrees = -90f;
            sprite.Name = "HullSprite";
            return sprite;
        }
    }

    public abstract class Primary : MechPart {
        public float Integrity;
        public float Armor;
        public float Weight;
        public AnimatedSprite2D Sprite;
        public static AnimatedSprite2D CreateSprite(string texturePath) {
            AnimatedSprite2D sprite = new AnimatedSprite2D();
            sprite.SpriteFrames = ResourceLoader.Load<SpriteFrames>(texturePath);
            return sprite;
        }
    }
    public abstract class Secondary : MechPart {

    }
    public abstract class Utility : MechPart {

    }

    // Chassis Database //
    public class PlaceholderTwoLegs : Chassis {
        public PlaceholderTwoLegs() { //base() means it also runs the base class constructor
            Name = "Placeholder (2 Legs)";
            Integrity = 100;
            Armor = 5;
            WeightCapacity = 200;
            Type = ChassisType.TwoLegs;
            MoveSpeed = 250;
            RotateSpeed = 180;
            RotorSpeed = 75;
            Sprite = CreateSprite("res://Textures/MechParts/Chassis/placeholderChassis.png");
        }
    }
    public class PlaceholderFourLegs : Chassis {
        public PlaceholderFourLegs() {
            Name = "Placeholder (4 Legs)";
            Integrity = 100;
            Armor = 5;
            WeightCapacity = 200;
            Type = ChassisType.FourLegs;
            MoveSpeed = 300;
            RotateSpeed = 0;
            RotorSpeed = 90;
            Sprite = CreateSprite("res://Textures/MechParts/Chassis/placeholderChassis.png");
        }
    }
    public class PlaceholderTreads : Chassis {
        public PlaceholderTreads() {
            Name = "Placeholder (Treads)";
            Integrity = 100;
            Armor = 5;
            WeightCapacity = 200;
            Type = ChassisType.Treads;
            MoveSpeed = 200;
            RotateSpeed = 120;
            RotorSpeed = 60;
            Sprite = CreateSprite("res://Textures/MechParts/Chassis/placeholderChassisTreads.png");
        }
    }
    // Hull Database //
    public class PlaceholderHull : Hull {
        public PlaceholderHull() {
            Name = "Placeholder Hull";
            Integrity = 100;
            Armor = 5;
            Weight = 100;
            RotorSpeed = 180;
            Sprite = CreateSprite("res://Textures/MechParts/Hull/placeholderHull.png");
            Primaries = new Primary[2];
            PrimaryOffsets = new Vector2[] {
                new Vector2(110, -35),
                new Vector2(-110, -35)
            };
            Secondaries = new Secondary[2];
            SecondaryOffsets = new Vector2[] {
                new Vector2(100, -75),
                new Vector2(-100, -75)
            };
            Utilities = new Utility[1];
            UtilityOffsets = new Vector2[] {
                new Vector2(0, -100)
            };
        }
    }
    // Primary Database //
    public class Cannon : Primary {
        public Cannon() {
            Name = "Cannon";
            Integrity = 50;
            Sprite = CreateSprite("res://Textures/MechParts/Primary/Cannon/CannonFrames.tres");
        }
    }
    // Secondary Database //
    // Utility Database //
}
