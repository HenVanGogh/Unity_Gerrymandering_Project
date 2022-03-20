using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Title : IEquatable<Title>
{
    public string TitleName { get; set; }

    public int TitleId { get; set; }
    public int TitleRoughness { get; set; }
    public int TitleAvailability { get; set; }
    public int[] Position { get; set; }
    public int Initialized { get; set; }
    public GameObject tit { get; set; }

    public Color col { get; set; }

    //zone part 
    public int zoneId { get; set; }
    public int alternative_zone { get; set; }
    public float population { get; set; }
    public float side { get; set; }
    public Color side_color { get; set; }

    public bool contested { get; set; }
    public bool recurrent_visited { get; set; }



    public override string ToString()
    {
        return "ID: " + TitleId + "   Name: " + TitleName;
    }

    public int avialiability()
    {
        return TitleAvailability;
    }

    public override int GetHashCode()
    {
        return TitleId;
    }
    public bool Equals(Title other)
    {
        if (other == null) return false;
        return (this.TitleId.Equals(other.TitleId));
    }

    // Should also override == and != operators.
}