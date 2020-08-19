﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace linerider.Game.LineGenerator
{
    public abstract class Generator : GameService
    {
        public string name;
        protected List<GameLine> lines; //Array of lines generated by this class

        public Generator() { }
        public Generator(string _name)
        {
            name = _name;
            lines = new List<GameLine>();
        }

        public abstract void Generate_Internal(TrackWriter trk); //This function contains the line generation, for a given generator, and is called by Generate()
        public abstract void Generate_Preview_Internal(TrackWriter trk); //This function should generate the lines which will be shown in the preview when the menu is open

        public void Generate() //Generates the lines, updating UndoManager and the track in the process
        {
            game.Track.UndoManager.BeginAction();
            using (var trk = game.Track.CreateTrackWriter())
            {
                Generate_Internal(trk);
            }
            game.Track.NotifyTrackChanged();
            game.Track.UndoManager.EndAction();
        }

        public void Generate_Preview() //Generates the preview lines, updating the track but not UndoManager
        {
            using (var trk = game.Track.CreateTrackWriter())
            {
                trk.DisableUndo();
                Generate_Preview_Internal(trk);
            }
            game.Track.NotifyTrackChanged();
        }

        public void ReGenerate_Preview()
        {
            DeleteLines();
            Generate_Preview();
        }
        public void DeleteLines() //Delete all lines in the array
        {
            using (var trk = game.Track.CreateTrackWriter())
            {
                trk.DisableUndo();
                if (lines.Count() == 0)
                    return;
                foreach (GameLine line in lines)
                {
                    trk.RemoveLine(line);
                }
                lines.Clear();
                game.Track.Invalidate();
                game.Track.NotifyTrackChanged();
            }  
        }

        protected GameLine CreateLine( //Creates a line from a pair of vectors (modified from Tool.cs)
            TrackWriter trk,
            Vector2d start,
            Vector2d end,
            LineType type,
            bool inv,
            int multiplier = 1, //Only applies to red lines (smh)
            float width=1.0f) //Width only applicable to green lines
        {
            GameLine added = null;
            switch (type)
            {
                case LineType.Blue:
                    added = new StandardLine(start, end, inv);
                    break;

                case LineType.Red:
                    var red = new RedLine(start, end, inv)
                    { Multiplier = multiplier };
                    red.CalculateConstants();//multiplier needs to be recalculated
                    added = red;
                    break;

                case LineType.Scenery:
                    added = new SceneryLine(start, end)
                    { Width = width };
                    break;
            }
            trk.AddLine(added);
            game.Track.Invalidate();
            return added;
        }
    }
}
