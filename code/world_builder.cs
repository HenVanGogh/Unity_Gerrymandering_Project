using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using System;



public class World_builder : MonoBehaviour
{
    public GameObject base_title;
    private List<Title> titles = new List<Title>();

    void Start()
    {
        Debug.Log("elo");
        int tmp_id = 0;
        for(int i = 0; i < 18; i++)
        {
            for (int k = 0; k < 18; k++)
            {
                if( Math.Sqrt((Math.Pow(i - 10, 2) + Math.Pow(k - 10, 2))) < 6.0)
                {
                    titles.Add(new Title() { TitleName = "boring box", TitleId = 15,
                    TitleRoughness = 1, TitleAvailability = 1, Position = new int[] { i , k } ,
                    Initialized = 0
                    });
                    Debug.Log(i + "  " + k);
                }
            }
        }

        create_all_initialized_titles();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    bool isInicialized(Title t)
    {
        return t.Initialized == 0;
    }

    bool isMarkToDelete(Title t)
    {
        return t.Initialized == 2;
    }

    void create_all_initialized_titles()
    {
        List<Title> to_initialized = new List<Title>(titles.FindAll(isInicialized));
        foreach (Title t in to_initialized)
        {
            float pos_x = t.Position[0] * (float)-0.4373;
            float pos_y = t.Position[0] * (float)-0.2196;
            pos_x = pos_x + ((float)0.4373 * t.Position[1]);
            pos_y = pos_y - ((float)0.2196 * t.Position[1]);
            GameObject title_to_init = 
                Instantiate(base_title, new Vector3(pos_x, pos_y + (float)3.5, t.Position[0] * t.Position[1] * (float)0.005555)
                , Quaternion.identity);
 
        }
    }

    void destroy_all_marked_titles()
    {
        List<Title> to_disable = new List<Title>(titles.FindAll(isMarkToDelete));
        foreach (Title t in to_disable)
        {
            t.tit.SetActive(false);

        }

    }
}
/*
public class Title : IEquatable<Title>
{
    public string TitleName { get; set; }

    public int TitleId { get; set; }
    public int TitleRoughness { get; set; }
    public int TitleAvailability { get; set; }
    public int[] Position { get; set; }
    public int Initialized { get; set; }
    public GameObject tit { get; set; }

    //zone part 
    public int zoneId { get; set; }
    public int population { get; set; }


    public override string ToString()
    {
        return "ID: " + TitleId + "   Name: " + TitleName;
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
*/