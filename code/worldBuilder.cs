using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using System.Threading.Tasks;

using UnityEngine.UI;
using InGameCodeEditor;

using System.Threading;
using System.Threading.Tasks;

using System.IO;

using Random = UnityEngine.Random;



public class worldBuilder : MonoBehaviour
{
    public GameObject base_title;
    public GameObject popup;
    public GameObject ghost_title;

    bool lookup_mode = false;
    bool showing_info = false;
    int[] current_pointer_pos;
    GameObject info_object;
    GameObject ghost_t;


    private List<Title> titles = new List<Title>();

    public Title[,] world_map = new Title[200, 200];
    public int[,] avalibility_table = new int[200, 200];
    public float[,] roughness_table = new float[200, 200];



    private int num_of_titles = 0;

    private Title previous_pointed_title;

    public Title currently_targeted_title;

    private slime selected_mob;
    private bool mob_wainting_for_target = false;

    //Region controll----------------------------------------------
    private static int number_of_regions = 6;
    private int[] number_of_titles_in_zone   = new int[number_of_regions];
    private float[] population_in_zone       = new float[number_of_regions];
    public float[] vote_in_zone             = new float[number_of_regions];
    private int[,] locations_sums            = new int[number_of_regions, 2];
    //calculated
    private float[,] centers_of_mass = new float[number_of_regions, 2];
    private bool[] vote_result_in_zone = new bool[number_of_regions];
    public float current_score = -1;
    public float current_score_equality = 0;
    public float current_score_votes = 0;
    public float current_score_zonequality = 0;
    public float system_temperature = 0;

    public int num_of_iter = 0;

    public float total_population_vote = 0;
    public float sided_population_vote = 0;
    //----------------------------------------------
    GameObject[] zone_info_object = new GameObject[number_of_regions];

    public CodeEditor game_editor;




    private List<Vector2> generated_random_points;
    private List<Color> generated_random_colors;

    static int size_x = 100;
    static int size_y = 100;


    //Randomizer
    private List<Title> titles_to_change = new List<Title>();
    private bool spam_randomizer = false;

    public Slider[] sliders_inputs = new Slider[number_of_regions];
    //public Slider[] texts_inputs = new Slider[number_of_regions];
    public Slider[] sliders_outputs = new Slider[number_of_regions];
    public float[] tmp_vote_val_table = new float[number_of_regions];

    public float zone_bonus = 1;
    public float vote_bonus = 1;
    public float pop_bonus = 1;

    void Start()
    {
        world_map = new Title[200, 200];
        int uniqueID = 0;
        generated_random_points = random_list_of_points(number_of_regions, 99);
        generated_random_colors = random_list_of_colors(number_of_regions);

        foreach (int i in number_of_titles_in_zone)
        {
            number_of_titles_in_zone[i] = 0;
        }


        
        for (int i = 0; i < size_x; i++)
        {
            for (int k = 0; k < size_y; k++)
            {
                uniqueID += 1;
                //if (Math.Sqrt((Math.Pow(i - 10, 2) + Math.Pow(k - 10, 2))) < 100.0)
                //{
                //float sample = Mathf.PerlinNoise((float)(i+ size_x)/ (float)(2*size_x), (float)(k + size_y) / (float)(2 * size_y));
                //if (sample < 0.4)
                //{
                float population_density = 0;
                population_density += Mathf.PerlinNoise((float)i / 10f, (float)k / 10f) * 0.4f;
                population_density += Mathf.Clamp((Mathf.PerlinNoise((float)(i + 100) / 14f, (float)(k + 100) / 10f) * 4) - 2.7f, 0, 2);
                population_density += Mathf.Clamp((Mathf.PerlinNoise((float)(i + 300) / 14f, (float)(k + 300) / 10f) * 4) - 3f, 0, 0.4f);

                float f_side = Mathf.PerlinNoise((float)i / 10f, (float)k / 10f) - 0.51f;
                //f_side = -1;

                int zone_id = assign_closest_point(new Vector2((float)i, (float)k), generated_random_points);


                Title t = new Title()
                {
                    TitleName = "boring box",
                    TitleId = uniqueID,
                    TitleRoughness = 1,
                    TitleAvailability = 1,
                    alternative_zone = -1,
                    Position = new int[] { i, k },
                    Initialized = 0,
                    col = new Color(1, 1, 1),
                    population = population_density,
                    side = f_side,
                    side_color = generated_random_colors[zone_id],
                    zoneId = zone_id,
                    contested = false,
                    recurrent_visited = false

                };
                number_of_titles_in_zone[t.zoneId] += 1;
                titles.Add(t);
                world_map[i, k] = t;
                avalibility_table[i, k] = t.TitleAvailability;
                roughness_table[i, k] = (float)t.TitleRoughness;

            }
        }

        create_all_initialized_titles();
        updateList_to_change();
        start_zone_data();


        //Sliders zone-------------------------------------------------------------------------
        sliders_inputs[0]  = GameObject.Find("Slider 1 input").GetComponent<Slider>();
        sliders_outputs[0] = GameObject.Find("Slider 1").GetComponent<Slider>();

        sliders_inputs[1]  = GameObject.Find("Slider 2 input").GetComponent<Slider>();
        sliders_outputs[1] = GameObject.Find("Slider 2").GetComponent<Slider>();

        sliders_inputs[2]  = GameObject.Find("Slider 3 input").GetComponent<Slider>();
        sliders_outputs[2] = GameObject.Find("Slider 3").GetComponent<Slider>();

        sliders_inputs[3]  = GameObject.Find("Slider 4 input").GetComponent<Slider>();
        sliders_outputs[3] = GameObject.Find("Slider 4").GetComponent<Slider>();

        sliders_inputs[4]  = GameObject.Find("Slider 5 input").GetComponent<Slider>();
        sliders_outputs[4] = GameObject.Find("Slider 5").GetComponent<Slider>();

        sliders_inputs[5]  = GameObject.Find("Slider 6 input").GetComponent<Slider>();
        sliders_outputs[5] = GameObject.Find("Slider 6").GetComponent<Slider>();

        //votes_zone_1 texts_inputs
        correct_slider();


        //----------------------------------------------------------------------------------

        //show_zonep();
        System.DateTime theTime = System.DateTime.Now;
        string date = theTime.Year + "_" + theTime.Month + "_" + theTime.Day;
        string time = date + "_" + theTime.Hour + "_" + theTime.Minute + "_" + theTime.Second;
        writer = new StreamWriter(time + ".txt", true);
        writer.Write("total_score" + "," + "current_score_zonequality" + "," + "current_score_equality" + "," + "current_score_votes" + ","
            + "zone_bonus" + "," + "pop_bonus" + "," + "vote_bonus" + "," +
            "zone_1_target" + "," + "zone_1_result" + "," +
            "zone_2_target" + "," + "zone_2_result" + "," +
            "zone_3_target" + "," + "zone_3_result" + "," +
            "zone_4_target" + "," + "zone_4_result" + "," +
            "zone_5_target" + "," + "zone_5_result" + "," +
            "zone_6_target" + "," + "zone_6_result"
            + '\n');

    }
    void correct_slider()
    {
        for (int i = number_of_regions - 1; i > 0; i--)
        {
            if (sliders_inputs[i].value < sliders_inputs[i - 1].value)
            {
                sliders_inputs[i - 1].value = sliders_inputs[i].value;
            }
        }
    }
    ///
    void updateList_to_change()
    {
        for (int i = 0; i < 100; i++)
        {
            for (int k = 0; k < 100; k++)
            {
                if (!cross_find(i, k))// if not found in cross section
                {
                    Title a = Find_title_with_id(i, k);
                    world_map[i, k].contested = false;
                    if (titles_to_change != null)
                    {

                        if (titles_to_change.Any(item => item.TitleId == a.TitleId))
                        {
                            var itemToRemove = titles_to_change.Single(r => r.TitleId == a.TitleId);
                            titles_to_change.Remove(itemToRemove);
                        }
                    }
                }


            }
        }
    }
    
    private bool cross_find_optimized(int i, int k)
    {
        Title a = Find_title_with_id(i, k);
        if (a != null)
        {
 
            for (int m = -1; m < 2; m++)
            {
                for (int n = -1; n < 2; n++)
                {
                    if (Mathf.Abs(n) != Mathf.Abs(m))
                    {
                        Title b = Find_title_with_id(i + m, k + n);
                        if (a != null && b != null)
                        {
                            if (b.zoneId != a.zoneId)
                            {
                                a.alternative_zone = b.zoneId;

                                world_map[i, k].contested = true;
                                if (titles_to_change == null)
                                {
                                    titles_to_change.Add(a);
                                }
                                else if (!titles_to_change.Any(item => item.TitleId == a.TitleId))
                                {
                                    titles_to_change.Add(a);
                                }
                                return true;

                            }
                        }
                    }
                }

            }
            world_map[i, k].contested = false;
            titles_to_change = titles_to_change.Where(note => note.TitleId != a.TitleId).ToList();
            return false;
        }
        return false;
    }
    void updateList_to_change_optimized(List<int[]> input_list)
    {
       foreach(int[] pos in input_list)
        {
            for (int mb = -1; mb < 2; mb++)
            {
                for (int nb = -1; nb < 2; nb++)
                {
                    cross_find_optimized(pos[0] +mb, pos[1] + nb);
                }
            }
        }

    }
    void updateList_to_change_paralell(List<int[]> input_list)
    {
        Parallel.ForEach(input_list, pos =>
        {
            for (int mb = -1; mb < 2; mb++)
            {
                for (int nb = -1; nb < 2; nb++)
                {
                    cross_find_optimized(pos[0] + mb, pos[1] + nb);
                }
            }
        }
            );

    }
    private bool cross_find(int i, int k)
    {
        Title a = Find_title_with_id(i, k);
        for (int m = -1; m < 2; m++)
        {
            for (int n = -1; n < 2; n++)
            {
                Title b = Find_title_with_id(i + m, k + n);
                if (a != null && b != null)
                {
                    if (world_map[i + m, k + n].zoneId != world_map[i, k].zoneId)
                    {
                        world_map[i, k].alternative_zone = world_map[i + m, k + n].zoneId;
                        if (Mathf.Abs(n) != Mathf.Abs(m))
                        {
                            world_map[i, k].contested = true;
                            if (titles_to_change == null)
                            {
                                titles_to_change.Add(a);
                            }
                            else if (!titles_to_change.Any(item => item.TitleId == a.TitleId))
                            {
                                titles_to_change.Add(a);

                            }
                            return true;

                        }
                    }
                }
            }

        }
        return false;
    }


  

    int checkIntegrity(int[] position)
    {
        Title t = Find_title_with_id(position[0], position[1]);
        int result = recurent_integrity_check(position, t.zoneId, 0);
        for (int i = 0; i < size_x; i++)
        {
            for (int k = 0; k < size_x; k++)
            {
                world_map[i, k].recurrent_visited = false;
            }
        }
        return result; //== number_of_titles_in_zone[t.zoneId];
    }

    int recurent_integrity_check(int[] position, int zone_id , int value)
    {
        Title t1 = Find_title_with_id(position[0], position[1]);
        if (t1 != null)
        {
            for (int m = -1; m < 2; m++)
            {
                for (int n = -1; n < 2; n++)
                {
                    if ((Mathf.Abs(n) != Mathf.Abs(m)))
                    {
                        Title t2 = Find_title_with_id(position[0] + m, position[1] + n);
                        if (t2 != null)
                        {
                            if (!t2.recurrent_visited)
                            {
                                
                                t2.recurrent_visited = true;
                                
                                int[] new_position = new int[] { position[0] + m, position[1] + n };
                                if (world_map[new_position[0] , new_position[1]].zoneId == world_map[position[0], position[1]].zoneId)
                                {
                                    //t2.col = new Color(0.1f, 0.1f, 0);
                                    value = recurent_integrity_check(new_position, zone_id, value+1);
                                }
                            }
                        }

                    }

                }
            }
        }
    

        
        return value;

    }

    void start_zone_data()
    {
        for (int i = 0; i < number_of_regions; i++)
        {
            number_of_titles_in_zone[i] = 0;
            vote_in_zone[i] = 0;
            population_in_zone[i] = 0;
            locations_sums[i, 0] = 0;
            locations_sums[i, 1] = 0;
            centers_of_mass[i, 0] = 0;
            centers_of_mass[i, 1] = 0;
        }
        foreach (Title t in titles)
        {
            number_of_titles_in_zone[t.zoneId] += 1;
            vote_in_zone[t.zoneId] += t.side * t.population;
            sided_population_vote += t.side * t.population;
            total_population_vote += Math.Abs(t.side * t.population);
            population_in_zone[t.zoneId] += t.population;
            locations_sums[t.zoneId, 0] += t.Position[0];
            locations_sums[t.zoneId, 1] += t.Position[1];
        }
        for (int i = 0; i < number_of_regions; i++)
        {
            centers_of_mass[i, 0] = locations_sums[i, 0] / number_of_titles_in_zone[i];
            centers_of_mass[i, 1] = locations_sums[i, 1] / number_of_titles_in_zone[i];
            vote_result_in_zone[i] = vote_in_zone[i] > 0;
            Title t = Find_title_with_id((int)Mathf.Round(centers_of_mass[i, 0]), (int)Mathf.Round(centers_of_mass[i, 1]));
            if (t == null)
            {
                Debug.Log("zone_empty");
            }
            else
            {
                Vector3 Pose = new Vector3(t.tit.transform.position.x, t.tit.transform.position.y, t.tit.transform.position.z - 0.5f);
                zone_info_object[i] = Instantiate(popup, Pose, Quaternion.identity);

                string s = "";
                s += "vote_result_in_zone - " + vote_result_in_zone[i] + '\n';
                s += "population_in_zone - " + population_in_zone[i] + '\n';
                s += "centers_of_mass - " + centers_of_mass[i, 0] + " " + centers_of_mass[i, 1] + '\n';

                s += "num_zones - " + number_of_titles_in_zone[i];

                zone_info_object[i].GetComponent<TextMesh>().text = s;
            }
        }
    }
    float update_zone_data()
    {
        
        float mean_of_population = 0;
        for (int i = 0; i < number_of_regions; i++)
        {
            mean_of_population += population_in_zone[i];
            centers_of_mass[i, 0] = locations_sums[i, 0] / number_of_titles_in_zone[i];
            centers_of_mass[i, 1] = locations_sums[i, 1] / number_of_titles_in_zone[i];
            vote_result_in_zone[i] = vote_in_zone[i] > 0;
            Title t = Find_title_with_id((int)Mathf.Round(centers_of_mass[i, 0]), (int)Mathf.Round(centers_of_mass[i, 1]));
            if (t == null)
            {
                Debug.Log("zone_empty");
            }
            else
            {
                Vector3 Pose = new Vector3(t.tit.transform.position.x, t.tit.transform.position.y, t.tit.transform.position.z - 0.5f);
                zone_info_object[i].transform.position = Pose;

                string s = "";
                s += "zone num - " + i + '\n';
                s += "vote_result_in_zone - " + vote_result_in_zone[i] + '\n';
                s += "population_in_zone - " + population_in_zone[i] + '\n';
                s += "centers_of_mass - " + centers_of_mass[i, 0] + " " + centers_of_mass[i, 1] + '\n';

                s += "num_zones - " + number_of_titles_in_zone[i];

                zone_info_object[i].GetComponent<TextMesh>().text = s;
            }
        }
        mean_of_population /= number_of_regions;
        float standard_deviation = 0;
        float vote_result_points = 0;

        
        
        for (int i = 0; i < number_of_regions; i++)
        {
            tmp_vote_val_table[i] = ((population_in_zone[i] + vote_in_zone[i]) / (population_in_zone[i])) - 1f;
        }
        Array.Sort(tmp_vote_val_table);
        for (int i = 0; i < number_of_regions; i++)
        {
            vote_result_points += Mathf.Abs(tmp_vote_val_table[i] - ((sliders_inputs[i].value-0.5f)*2f));
            sliders_outputs[i].value = (tmp_vote_val_table[i]+1)/2;
        }

        for (int i = 0; i < number_of_regions; i++)
        {
            standard_deviation += Mathf.Pow(population_in_zone[i] - mean_of_population , 2);
        }

        standard_deviation /= number_of_regions;
        standard_deviation = Mathf.Sqrt(standard_deviation);

        long distance_cost = 0;
        Parallel.ForEach(titles,
            () => 0,
            (j, loop, subtotal) =>
            {
                subtotal += (int)Mathf.Pow(get_distance(j.Position, centers_of_mass[j.zoneId, 0], centers_of_mass[j.zoneId, 1]), 2);
                return subtotal;
            },
            (finalResult) => Interlocked.Add(ref distance_cost, finalResult)
        );

        current_score_zonequality = (((float)distance_cost / 10000)) * zone_bonus;
        current_score_equality = standard_deviation * pop_bonus;
        current_score_votes = vote_result_points*500 * vote_bonus;
        sliders_inputs[0] = GameObject.Find("Slider 1 input").GetComponent<Slider>();
        sliders_outputs[0] = GameObject.Find("Slider 1").GetComponent<Slider>();
        writer.Write((current_score_equality + current_score_zonequality + current_score_votes)
            + "," + current_score_zonequality + ","+ current_score_equality + ","+ current_score_votes + ","
            + zone_bonus + "," + pop_bonus + "," + vote_bonus + ","
            + sliders_inputs[0].value + "," + sliders_outputs[0].value + ","
            + sliders_inputs[1].value + "," + sliders_outputs[1].value + ","
            + sliders_inputs[2].value + "," + sliders_outputs[2].value + ","
            + sliders_inputs[3].value + "," + sliders_outputs[3].value + ","
            + sliders_inputs[4].value + "," + sliders_outputs[4].value + ","
            + sliders_inputs[5].value + "," + sliders_outputs[5].value
            + '\n');

        return current_score_equality + current_score_zonequality + current_score_votes;

    }
    float get_distance(int[] pos1,float pos2x , float pos2y)
    {
        return Mathf.Sqrt(Mathf.Pow((float)pos1[0] - pos2x, 2) + Mathf.Pow((float)pos1[1] - pos2y, 2));
    }
    void swap_zone_data(Title t)
    {
        number_of_titles_in_zone[t.zoneId] -= 1;
        number_of_titles_in_zone[t.alternative_zone] += 1;

        locations_sums[t.zoneId, 0] -= t.Position[0];
        locations_sums[t.zoneId, 1] -= t.Position[1];

        locations_sums[t.alternative_zone, 0] += t.Position[0];
        locations_sums[t.alternative_zone, 1] += t.Position[1];

        population_in_zone[t.zoneId] -= t.population;
        population_in_zone[t.alternative_zone] += t.population;

        vote_in_zone[t.zoneId] -= t.side * t.population;
        vote_in_zone[t.alternative_zone] += t.side * t.population;

        centers_of_mass[t.zoneId, 0] = locations_sums[t.zoneId, 0] / number_of_titles_in_zone[0];
        centers_of_mass[t.zoneId, 1] = locations_sums[t.zoneId, 1] / number_of_titles_in_zone[1];
        vote_result_in_zone[t.zoneId] = vote_in_zone[t.zoneId] > 0;

        centers_of_mass[t.alternative_zone, 0] = locations_sums[t.alternative_zone, 0] / number_of_titles_in_zone[0];
        centers_of_mass[t.alternative_zone, 1] = locations_sums[t.alternative_zone, 1] / number_of_titles_in_zone[1];
        vote_result_in_zone[t.alternative_zone] = vote_in_zone[t.alternative_zone] > 0;

        

        int tmp_zone = t.zoneId;
        t.zoneId = t.alternative_zone;
        t.alternative_zone = tmp_zone;
    }

    bool chceck_for_discontinuity(Title t)
    {
        int num_sides = 1;

        //----------------------------this is used to operate loop--------------------------------------------------------
        int[] local_gaps = new int[9];
        int[,] pos_table = { { 1, 0 }, { 1, 1 }, { 0, 1 }, { -1, 1 }, { -1, 0 }, { -1, -1 }, { 0, -1 },{ 1, -1 }, { 1, 0 } };
        int switch_count = 0;
        int current_zone = -1;
        //----------------------------------------------------------------------------------------------------------------
       
        for (int local_pos = 0; local_pos < 9; local_pos++)
        {
            Title a = Find_title_with_id(t.Position[0] + pos_table[local_pos, 0], t.Position[1] + pos_table[local_pos, 1]);

            if (a == null)
            {
                if (current_zone == 0)
                {
                    switch_count++;
                    current_zone = 1;
                }
                if (current_zone == -1)
                {
                    current_zone = 1;
                }
            }
            else if (a.zoneId != t.zoneId)
            {
                if (current_zone == 0)
                {
                    switch_count++;
                    current_zone = 1;
                }
                if (current_zone == -1)
                {
                    current_zone = 1;
                }

            }
            else
            {
                if (current_zone == 1)
                {
                    switch_count++;
                    current_zone = 0;
                }
                if (current_zone == -1)
                {
                    current_zone = 0;
                }

            }

        }
        if (switch_count == 2)
        {
      
            return true;
        }
        if ((switch_count == 1) || (switch_count == 3) || (switch_count == 5) || (switch_count == 0))
        {
            return false;
        }
        return false;
    }
    bool random_change()
    {
        system_temperature = Mathf.Clamp(Mathf.Exp(0.00002f * (float)num_of_iter) / 20f, 0, 100) ;
        List<int[]> pos_to_check_list = new List<int[]>();
        for (int enu = 0; enu < 50; enu++)
        {
            num_of_iter++;
            int index = Random.Range(0, titles_to_change.Count - 1);
            if (chceck_for_discontinuity(titles_to_change[index]))
            {
                swap_zone_data(titles_to_change[index]);
                float tmp_score = update_zone_data();
                if(current_score < 0)
                {
                    current_score = tmp_score;
                }
                if (tmp_score > current_score) // game_editor.execute_script()
                {
                    if (game_editor.execute_script(tmp_score , current_score , num_of_iter))
                    {
                        current_score = tmp_score;
                    }
                    else
                    {
                        swap_zone_data(titles_to_change[index]);
                    }
                }
                else
                {
                    current_score = tmp_score;
                }
                
                //titles_to_change[index].zoneId = titles_to_change[index].alternative_zone;
                pos_to_check_list.Add(titles_to_change[index].Position);
            }
        }
        Debug.Log(current_score);
        updateList_to_change_optimized(pos_to_check_list);
      
        foreach (Title t in titles)
        {
            t.col = generated_random_colors[t.zoneId]; // not so random i guess?
            t.tit.GetComponent<SpriteRenderer>().color = t.col;
        }
        
        return true;
    }
    public StreamWriter writer;
    void Update()
    {
        selection_update_loop(true, Color.blue); //Shows where your mouse points
        mob_target_loop(); //sets mob targets
        correct_slider();
        

        

        



        /*
        if ((Input.GetMouseButtonDown(0)))
        {
            Title t = World_position_to_title(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            t.col = new Color(t.col.r+0.3f, t.col.g, t.col.b, 0.9f);
            t.tit.GetComponent<SpriteRenderer>().color = t.col;
        }
        */
        //if (Input.GetKeyDown(KeyCode.K))
        //{
        //    spam_randomizer = !spam_randomizer;
        //}
        if (spam_randomizer)
        {
            random_change();
            


        }

        //if (Input.GetKeyDown(KeyCode.Q))
        //{
        //    foreach (Title t in titles)
        //    {
        //        t.col = new Color(t.population, t.population, 0);
        //        t.tit.GetComponent<SpriteRenderer>().color = t.col;
        //    }
        //    //Parallel.ForEach(titles, t =>
        //    //{
        //    //    t.col = new Color(t.population, t.population, 0);
        //    //    t.tit.GetComponent<SpriteRenderer>().color = t.col;
        //    //});
        //}
        //if (Input.GetKeyDown(KeyCode.B))
        //{
        //    foreach (Title t in titles)
        //    {

        //        t.tit.GetComponent<SpriteRenderer>().color = t.col;
        //    }
        //    //Parallel.ForEach(titles, t =>
        //    //{
        //    //    t.tit.GetComponent<SpriteRenderer>().color = t.col;
        //    //});
        //}

        //if (Input.GetKeyDown(KeyCode.H))
        //{
        //    foreach (Title t in titles)
        //    {
        //        if (t.contested)
        //        {
        //            t.col = new Color(1, 0, 0);
        //        }
        //        t.tit.GetComponent<SpriteRenderer>().color = t.col;
        //    }
        //    //Parallel.ForEach(titles, t =>
        //    //{
        //    //    if (t.contested)
        //    //    {
        //    //        t.col = new Color(1, 0, 0);
        //    //    }
        //    //    t.tit.GetComponent<SpriteRenderer>().color = t.col;
        //    //});
        //}

        //if (Input.GetKeyDown(KeyCode.E))
        //{
        //    foreach (Title t in titles)
        //    {
        //        t.col = new Color(0.5f + t.side, 0, 0.5f - t.side);
        //        t.tit.GetComponent<SpriteRenderer>().color = t.col;
        //    }
        //    //Parallel.ForEach(titles, t =>
        //    //{
        //    //    t.col = new Color(0.5f + t.side, 0, 0.5f - t.side);
        //    //    t.tit.GetComponent<SpriteRenderer>().color = t.col;
        //    //});
        //}

        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    foreach (Title t in titles)
        //    {
        //        t.col = generated_random_colors[t.zoneId];
        //        t.tit.GetComponent<SpriteRenderer>().color = t.col;
        //    }
        //    //Parallel.ForEach(titles, t =>
        //    //{
        //    //    t.col = generated_random_colors[t.zoneId];
        //    //    t.tit.GetComponent<SpriteRenderer>().color = t.col;
        //    //});
        //}
        //if (Input.GetKeyDown(KeyCode.R))
        //{
        //    lookup_mode = !lookup_mode;
        //}

        
        
        
        
        if (lookup_mode && !showing_info)
        {
            Title t = World_position_to_title(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            if (t != null)
            {
                Vector3 Pose = new Vector3(t.tit.transform.position.x + 0.2f, t.tit.transform.position.y + (float)0.24, t.tit.transform.position.z - 0.5f);
                info_object = Instantiate(popup, Pose, Quaternion.identity);

                //Vector3 Pose_g = new Vector3(t.tit.transform.position.x - (float)0.4392, t.tit.transform.position.y + (float)0.4392, t.tit.transform.position.z - 0.5f);
                Vector3 Pose_g = new Vector3(t.tit.transform.position.x, t.tit.transform.position.y + (float)0.4592, t.tit.transform.position.z - 0.5f);
                ghost_t = Instantiate(ghost_title, Pose_g, Quaternion.identity);
                showing_info = true;
                current_pointer_pos = t.Position;
                //foreach (int ini in number_of_titles_in_zone)
                //{
                //    ini = 0;
                //}
                number_of_titles_in_zone = number_of_titles_in_zone.Select(i =>0).ToArray();

                foreach (Title tii in titles)
                {
                    number_of_titles_in_zone[tii.zoneId] += 1;
                }

                string s = "";
                s += "x - " + t.Position[0] + "   y - " + t.Position[1] + '\n';
                s += "Pop - " + t.population + '\n';
                s += "Side - " + t.side + '\n';
                s += "Zone - " + t.zoneId + '\n';
                s += "recurrent - " + checkIntegrity(t.Position) + '\n';
                s += "num_zones - " + number_of_titles_in_zone[t.zoneId];

                info_object.GetComponent<TextMesh>().text = s;
                
            }
        }
        else if(showing_info)
        {
            Title t = World_position_to_title(Camera.main.ScreenToWorldPoint(Input.mousePosition));
           if ((t == null))
            {
                GameObject.Destroy(info_object);
                GameObject.Destroy(ghost_t);
                
                showing_info = false;
            }
            else if((t.Position != current_pointer_pos) || !lookup_mode) 
            {
                GameObject.Destroy(info_object);
                GameObject.Destroy(ghost_t);
                showing_info = false;
            }
        }




    }
    private void mob_target_loop()
    {
        if ((Input.GetMouseButtonDown(0)) && mob_wainting_for_target)
        {
            Title t = World_position_to_title(Camera.main.ScreenToWorldPoint(Input.mousePosition));
            selected_mob.set_target(t.Position[0], t.Position[1]);
        }
    }
    private void selection_update_loop(bool selection_enabled, Color selection_color)
    {
        Title target_title_tit;
        GameObject target_title;
        SpriteRenderer spriteR;
        Title targ_title = World_position_to_title(Camera.main.ScreenToWorldPoint(Input.mousePosition));
        currently_targeted_title = targ_title;
        if (targ_title != null)
        {
            target_title = targ_title.tit;
            spriteR = target_title.GetComponent<SpriteRenderer>();
            if (selection_enabled)
            {
                spriteR.color = selection_color;
            }

            if (previous_pointed_title != null)
            {
                if (target_title != previous_pointed_title.tit)
                {
                    SpriteRenderer previous_sprite = previous_pointed_title.tit.GetComponent<SpriteRenderer>();
                    previous_sprite.color = previous_pointed_title.col;
                    previous_pointed_title = targ_title;
                }
            }
            else if (target_title != null)
            {
                previous_pointed_title = targ_title;
            }
        }
        else if (previous_pointed_title != null)
        {
            SpriteRenderer previous_sprite = previous_pointed_title.tit.GetComponent<SpriteRenderer>();
            previous_sprite.color = previous_pointed_title.col;
            previous_pointed_title = null;
        }
    }
    public void show_pop()
    {
        foreach (Title t in titles)
        {
            t.col = new Color(t.population, t.population, 0);
            t.tit.GetComponent<SpriteRenderer>().color = t.col;
        }
    }

    public void show_vote()
    {
        foreach (Title t in titles)
        {
            t.col = new Color(0.5f + t.side, 0, 0.5f - t.side);
            t.tit.GetComponent<SpriteRenderer>().color = t.col;
        }
    }

    public void show_zonep()
    {
        foreach (Title t in titles)
        {
            t.col = generated_random_colors[t.zoneId];
            t.tit.GetComponent<SpriteRenderer>().color = t.col;
        }
    }

    public void system_start()
    {
        spam_randomizer = true;
    }

    public void system_stop()
    {
        spam_randomizer = false;
    }

    public void system_reset()
    {
        current_score = -1;
        int uniqueID = 0;
        generated_random_points = random_list_of_points(number_of_regions, 99);
        generated_random_colors = random_list_of_colors(number_of_regions);


        for (int i = 0; i < number_of_regions; i++)
        {
            Destroy(zone_info_object[i]);
        }


        for (int i = 0; i < size_x; i++)
        {
            for (int k = 0; k < size_y; k++)
            {
                
                //if (Math.Sqrt((Math.Pow(i - 10, 2) + Math.Pow(k - 10, 2))) < 100.0)
                //{
                //float sample = Mathf.PerlinNoise((float)(i+ size_x)/ (float)(2*size_x), (float)(k + size_y) / (float)(2 * size_y));
                //if (sample < 0.4)
                //{
                float population_density = 0;
                population_density += Mathf.PerlinNoise((float)i / 10f, (float)k / 10f) * 0.4f;
                population_density += Mathf.Clamp((Mathf.PerlinNoise((float)(i + 100) / 14f, (float)(k + 100) / 10f) * 4) - 2.7f, 0, 2);
                population_density += Mathf.Clamp((Mathf.PerlinNoise((float)(i + 300) / 14f, (float)(k + 300) / 10f) * 4) - 3f, 0, 0.4f);

                float f_side = Mathf.PerlinNoise((float)i / 10f, (float)k / 10f) - 0.51f;
                //f_side = -1;

                int zone_id = assign_closest_point(new Vector2((float)i, (float)k), generated_random_points);

                titles[uniqueID].TitleName = "boring box";
                titles[uniqueID].TitleId = uniqueID;
                titles[uniqueID].TitleRoughness = 1;
                titles[uniqueID].TitleAvailability = 1;
                titles[uniqueID].alternative_zone = -1;
                titles[uniqueID].Position = new int[] { i, k };
                titles[uniqueID].Initialized = 0;
                titles[uniqueID].col = new Color(1, 1, 1);
                titles[uniqueID].population = population_density;
                titles[uniqueID].side = f_side;
                titles[uniqueID].side_color = generated_random_colors[zone_id];
                titles[uniqueID].zoneId = zone_id;
                titles[uniqueID].contested = false;
                titles[uniqueID].recurrent_visited = false;
             

                //Title t = new Title()
                //{
                //    TitleName = "boring box",
                //    TitleId = uniqueID,
                //    TitleRoughness = 1,
                //    TitleAvailability = 1,
                //    alternative_zone = -1,
                //    Position = new int[] { i, k },
                //    Initialized = 0,
                //    col = new Color(1, 1, 1),
                //    population = population_density,
                //    side = f_side,
                //    side_color = generated_random_colors[zone_id],
                //    zoneId = zone_id,
                //    contested = false,
                //    recurrent_visited = false

                //};
                number_of_titles_in_zone[titles[uniqueID].zoneId] += 1;
                titles.Add(titles[uniqueID]);
                world_map[i, k] = titles[uniqueID];
                avalibility_table[i, k] = titles[uniqueID].TitleAvailability;
                roughness_table[i, k] = (float)titles[uniqueID].TitleRoughness;

                uniqueID += 1;
            }
        }

        
        updateList_to_change();
        start_zone_data();
        show_zonep();
    }

    public void zone_weight(float weight)
    {
        current_score = -1;
        zone_bonus = weight;
    }

    public void vote_weight(float weight)
    {
        current_score = -1;
        vote_bonus = weight;
    }
    public void pop_weight(float weight)
    {
        current_score = -1;
        pop_bonus = weight;
    }
    public void set_vote_target_1(float weight)
    {
        current_score = -1;
        sliders_inputs[0].value = (Mathf.Clamp(weight , -1 , 1)+1)/2f;
    }
    public void set_vote_target_2(float weight)
    {
        current_score = -1;
        sliders_inputs[1].value = (Mathf.Clamp(weight, -1, 1) + 1) / 2f;
    }
    public void set_vote_target_3(float weight)
    {
        current_score = -1;
        sliders_inputs[2].value = (Mathf.Clamp(weight, -1, 1) + 1) / 2f;
    }
    public void set_vote_target_4(float weight)
    {
        current_score = -1;
        sliders_inputs[3].value = (Mathf.Clamp(weight, -1, 1) + 1) / 2f;
    }
    public void set_vote_target_5(float weight)
    {
        current_score = -1;
        sliders_inputs[4].value = (Mathf.Clamp(weight, -1, 1) + 1) / 2f;
    }
    public void set_vote_target_6(float weight)
    {
        current_score = -1;
        sliders_inputs[5].value = (Mathf.Clamp(weight, -1, 1) + 1) / 2f;
    }
    private void show_range(int[] pos)
    {

    }
    private Title World_position_to_title(Vector3 world_pos)
    {
        float x_f = ((-5000 * world_pos.x) / 4373) + ((1250 * (-world_pos.y + 3.9357f)) / 549);
        float y_f = ((5000 * world_pos.x) / 4373) + ((1250 * (-world_pos.y + 3.9357f)) / 549);
        int x_id  = (int)Math.Floor(x_f);
        int y_id  = (int)Math.Floor(y_f);

        int[] Pose = new int[] { x_id, y_id };
        /*
        int tit_id = titles.FindIndex(item => Enumerable.SequenceEqual(Pose, item.Position));
        for (int i = 0; i < 100; i++)
        {
            int tit_ide = titles.FindIndex(item => Enumerable.SequenceEqual(Pose, item.Position));
        }
        */
        return Find_title_with_id(x_id, y_id);
        //return titles[tit_id];

    }
    public Title Find_title_with_id(int x_position, int y_position)
    {
        //Fix this to work if title is null
        if ((x_position >= 0) && (x_position <= 99) && (y_position >= 0) && (y_position <= 99))
        {
            return world_map[x_position, y_position];
        }
        else
        {
            return null;
        }
    }

    public int Find_avaliability_with_id(int x_position, int y_position)
    {
        return avalibility_table[x_position, y_position];
    }
    public void set_avaliability_with_id(int x_position, int y_position, int value)
    {
        avalibility_table[x_position, y_position] = value;
        roughness_table[x_position, y_position] = (float)value;
        world_map[x_position, y_position].TitleAvailability = value;
        


        world_map[x_position, y_position].col = 
            new Color(world_map[x_position, y_position].TitleAvailability, 1, 1);
        world_map[x_position, y_position].tit.GetComponent<SpriteRenderer>().color =
            world_map[x_position, y_position].col;
    }


    bool isInicialized(Title t)
    {
        return t.Initialized == 0;
    }


    bool isMarkToDelete(Title t)
    {
        return t.Initialized == 2;
    }

    public void change_slected_mob(slime s)
    {
        if (selected_mob != null)
        {
            if (!selected_mob.during_execution && s != selected_mob) //Probably dont need it idk
            {
                selected_mob.should_i_be_jumping = false;
                selected_mob.waiting_for_orders = false;
                selected_mob = null;
            }
        }
    
        selected_mob = s;
        mob_wainting_for_target = true;
        show_range(s.current_pos);
    }
    

    public void reset_selected_mob_if_unchanged(slime s)
    {
        if(selected_mob == s)
        {
            selected_mob = null;
            mob_wainting_for_target = false;
        }
    }

    void create_all_initialized_titles()
    {
        List<Title> to_initialized = new List<Title>(titles.FindAll(isInicialized));
        foreach (Title t in to_initialized)
        {
            float pos_x = t.Position[0] * (float)-0.4373;
            float pos_y = t.Position[0] * (float)-0.2196;
            pos_x += ((float)0.4373 * t.Position[1]);
            pos_y -= ((float)0.2196 * t.Position[1]);


            pos_x += Random.Range(-0.02f, 0.02f);
            pos_y += Random.Range(-0.02f, 0.02f);
            t.tit =
                Instantiate(base_title, new Vector3(pos_x, pos_y + (float)3.5, (t.Position[0] + 1000) * (t.Position[1] + 1000) * -(float)0.000005555)
                , Quaternion.identity);
            t.tit.transform.localScale = new Vector3(2f, 2f, 1f);
            SpriteRenderer p_sprite = t.tit.GetComponent<SpriteRenderer>();
            p_sprite.color = t.col;

            num_of_titles++;

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

    /////WORLD CREATING
    ///

    List<Vector2> random_list_of_points(int lenght , int range)
    {
        List<Vector2> arrayList = new List<Vector2>();
        for (int i = 0;i < lenght; i++)
        {
            Vector2 tmp_a = new Vector2((int)Random.Range(0 , range), (int)Random.Range(0, range) );
            arrayList.Add(tmp_a);
        }
        return arrayList;
        
    }

    List<Color> random_list_of_colors(int lenght)
    {
        List<Color> arrayList = new List<Color>();
        for (int i = 0; i < lenght; i++)
        {
            Color tmp_a = new Color((float)Random.Range(0, 1000)/1000f, (float)Random.Range(0, 1000) / 1000f, (float)Random.Range(0, 1000) / 1000f);
            arrayList.Add(tmp_a);
            
        }
        return arrayList;
    }

    int assign_closest_point(Vector2 v , List<Vector2> list)
    {

        float[] arr = new float[list.Count()];
        int table_pointer = 0;
        foreach (Vector2 d in list)
        {
            arr[table_pointer] = Vector2.Distance(d, v);
            table_pointer++;
        }
        return Array.IndexOf(arr, arr.Min());

  
    }
}

   



