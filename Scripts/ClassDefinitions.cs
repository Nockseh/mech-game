using System.Collections.Generic;
using Godot.Collections;

namespace Godot {

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
        public float WeightCapacity;
        public Sprite2D Sprite;
        public Chassis(string partName = "Placeholder") {
            var rawPartData = GetRawPartData(partName, "Chassis");
            this.Id = partName;
            this.Integrity = (float)rawPartData["Integrity"];
            this.Armor = (float)rawPartData["Armor"];
            this.Sprite = new Sprite2D();
            Sprite.Scale = new Vector2(0.5f, 0.5f);
            Sprite.ZIndex = -1;
            Sprite.Texture = ResourceLoader.Load<Texture2D>((string)rawPartData["Texture"]);
            Sprite.Name = "ChassisSprite";
            this.WeightCapacity = (float)rawPartData["WeightCapacity"];
        }
    }

    public class Hull : MechPart {
        public float Weight;
        public Sprite2D Sprite;
        public List<Primary> Primaries = new List<Primary>();
        public List<Vector2> PrimaryOffsets = new List<Vector2>();
        public List<Secondary> Secondaries = new List<Secondary>();
        public List<Vector2> SecondaryOffsets = new List<Vector2>();
        public List<Utility> Utilities = new List<Utility>();
        public List<Vector2> UtilityOffsets = new List<Vector2>();
        //Constructors
        public Hull(string partName = "Placeholder") {
            var rawPartData = GetRawPartData(partName, "Hull");
            this.Id = partName;
            this.Sprite = new Sprite2D();
            Sprite.Scale = new Vector2(0.25f, 0.25f);
            Sprite.Texture = ResourceLoader.Load<Texture2D>((string)rawPartData["Texture"]);
            Sprite.Name = "HullSprite";
            this.Integrity = (float)rawPartData["Integrity"];
            this.Armor = (float)rawPartData["Armor"];
            this.Weight = (float)rawPartData["Weight"];
            //attachment offset points
            for (int i = 1; i <= (int)rawPartData["PrimarySlots"]; i += 1) {
                PrimaryOffsets.Add(new Vector2((float)rawPartData["Primary"+i.ToString()+"X"], (float)rawPartData["Primary"+i.ToString()+"Y"]));
            }
            for (int i = 1; i <= (int)rawPartData["SecondarySlots"]; i += 1) {
                SecondaryOffsets.Add(new Vector2((float)rawPartData["Secondary" + i.ToString() + "X"], (float)rawPartData["Secondary" + i.ToString() + "Y"]));
            }
            for (int i = 1; i <= (int)rawPartData["UtilitySlots"]; i += 1) {
                UtilityOffsets.Add(new Vector2((float)rawPartData["Utility" + i.ToString() + "X"], (float)rawPartData["Utility" + i.ToString() + "Y"]));
            }
        }
        //Methods
        public void AttachPart(MechPart part, int slot = 1) {
            switch (part) {
                case Primary primaryPart:
                    var rawPartData = MechPart.GetRawPartData(primaryPart.Id, "Primary");
                    AnimatedSprite2D partSprite = new AnimatedSprite2D();
                    partSprite.RotationDegrees = -90f;
                    partSprite.SpriteFrames = ResourceLoader.Load<SpriteFrames>((string)rawPartData["SpriteFrames"]);
                    partSprite.Position = PrimaryOffsets[slot - 1] + new Vector2(0f, 120f);
                    this.Primaries[slot - 1] = primaryPart;
                    this.Sprite.AddChild(partSprite);
                    break;
                case Secondary secondaryPart:
                    break;
                case Utility utilityPart:
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