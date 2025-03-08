using System;
using System.Collections.Generic;
using Godot.Collections;
using Array = Godot.Collections.Array;

namespace Godot {
    // Enums //
    public enum ChassisType {
        TwoLegs,
        FourLegs,
        SixLegs,
        Treads,
        Hover
    }
    // Helper Functions //
    public static class GlobalHelper {
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
        public Mech() : this(new Chassis(), new Hull()) { //empty args, uses defaults
            
        }
    }

    public abstract class MechPart {
        public string Id;
        public float Integrity;
        public float Armor;
        //Methods
        protected static Dictionary GetRawPartData(string partName, string partType) {
            var data = (Dictionary)ResourceLoader.Load<Json>("res://MechPartData.json").Data;
            var rawPartData = (Dictionary)((Dictionary)data[partType])[partName];
            return rawPartData;
        }
    }

    public class Chassis : MechPart {
        public ChassisType Type;
        public float MoveSpeed;
        public float RotateSpeed;
        public float WeightCapacity;
        public float RotorSpeed;
        public Sprite2D Sprite;
        public Chassis(string partName = "PlaceholderFourLegs") {
            var rawPartData = GetRawPartData(partName, "Chassis");
            this.Id = partName;
            this.Type = Enum.Parse<ChassisType>((string)rawPartData["ChassisType"]);
            this.Integrity = (float)rawPartData["Integrity"];
            this.Armor = (float)rawPartData["Armor"];
            this.Sprite = new Sprite2D();
            Sprite.Scale = new Vector2(0.5f, 0.5f);
            Sprite.ZIndex = -1;
            Sprite.Texture = ResourceLoader.Load<Texture2D>((string)rawPartData["Texture"]);
            Sprite.Name = "ChassisSprite";
            this.WeightCapacity = (float)rawPartData["WeightCapacity"];
            this.RotorSpeed = (float)rawPartData["RotorSpeed"];
            this.MoveSpeed = (float)rawPartData["MoveSpeed"];
            if (this.Type == ChassisType.Treads || this.Type == ChassisType.TwoLegs) {
                this.RotateSpeed = (float)rawPartData["RotateSpeed"];
            } else {
                this.RotateSpeed = 0f;
            }
        }
    }

    public class Hull : MechPart {
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
        //Constructors
        public Hull( string partName = "Placeholder") {
            Dictionary rawPartData = GetRawPartData(partName, "Hull");
            this.Id = partName;
            this.Sprite = new Sprite2D();
            Sprite.Scale = new Vector2(0.25f, 0.25f);
            Sprite.Texture = ResourceLoader.Load<Texture2D>((string)rawPartData["Texture"]);
            Sprite.Name = "HullSprite";
            this.Integrity = (float)rawPartData["Integrity"];
            this.Armor = (float)rawPartData["Armor"];
            this.Weight = (float)rawPartData["Weight"];
            this.RotorSpeed = (float)rawPartData["RotorSpeed"];
            //weapon slots
            Primaries = new Primary[(byte)rawPartData["PrimarySlots"]];
            PrimaryOffsets = new Vector2[(byte)rawPartData["PrimarySlots"]];
            for (int i = 0; i <= (int)rawPartData["PrimarySlots"]-1; i += 1) {
                Array offsets = (Array)((Array)rawPartData["PrimaryOffsets"])[i];
                PrimaryOffsets[i] = new Vector2((float)offsets[0], (float)offsets[1]);
            }
            Secondaries = new Secondary[(byte)rawPartData["SecondarySlots"]];
            SecondaryOffsets = new Vector2[(byte)rawPartData["SecondarySlots"]];
            for (int i = 0; i <= (int)rawPartData["SecondarySlots"]-1; i += 1) {
                Array offsets = (Array)((Array)rawPartData["SecondaryOffsets"])[i];
                SecondaryOffsets[i] = new Vector2((float)offsets[0], (float)offsets[1]);
            }
            Utilities = new Utility[(byte)rawPartData["UtilitySlots"]];
            UtilityOffsets = new Vector2[(byte)rawPartData["UtilitySlots"]];
            for (int i = 0; i <= (int)rawPartData["UtilitySlots"]-1; i += 1) {
                Array offsets = (Array)((Array)rawPartData["UtilityOffsets"])[i];
                UtilityOffsets[i] = new Vector2((float)offsets[0], (float)offsets[1]);
            }
        }
        //Methods
        public void AttachPart(MechPart part, int slot = 1) {
            switch (part) {
                case Primary primaryPart:
                    if (slot > this.Primaries.Length) throw new System.Exception("Invalid primary slot '" + slot + "' in hull '" + this.Id + "'");
                    var rawPartData = MechPart.GetRawPartData(primaryPart.Id, "Primary");
                    AnimatedSprite2D partSprite = new AnimatedSprite2D();
                    partSprite.RotationDegrees = -90f;
                    partSprite.SpriteFrames = ResourceLoader.Load<SpriteFrames>((string)rawPartData["SpriteFrames"]);
                    partSprite.Position = PrimaryOffsets[slot - 1] + new Vector2(0f, 120f);
                    this.Primaries[slot - 1] = primaryPart;
                    this.Sprite.AddChild(partSprite);
                    break;
                case Secondary secondaryPart:
                    if (slot - 1 > this.Secondaries.Length) throw new System.Exception("Invalid secondary slot '" + slot + "' in hull '" + this.Id + "'");
                    break;
                case Utility utilityPart:
                    if (slot - 1 > this.Utilities.Length) throw new System.Exception("Invalid utility slot '" + slot + "' in hull '" + this.Id + "'");
                    break;
                default:
                    throw new System.Exception("Attempted to attach part of invalid type");
            }
        }
    }

    public class Primary : MechPart {
        public float Weight;
        public AnimatedSprite2D Sprite;
        public Primary(string partName = "Cannon") {
            var rawPartData = GetRawPartData(partName, "Primary");
            this.Id = partName;
            this.Integrity = (float)rawPartData["Integrity"];
            this.Armor = (float)rawPartData["Armor"];
            this.Sprite = new AnimatedSprite2D();
            Sprite.SpriteFrames = ResourceLoader.Load<SpriteFrames>((string)rawPartData["SpriteFrames"]);
            this.Weight = (float)rawPartData["Weight"];
        }
        public virtual void Fire() {

        }
    }
    public class  Secondary : MechPart {
        
    }
    public class Utility : MechPart {
        
    }
}