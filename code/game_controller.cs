using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InGameCodeEditor;

public class game_controller : MonoBehaviour
{
    public GameObject builder;
    private GameObject builder_instance;
    public GameObject title;
    public GameObject debug_window;
    private GameObject debug_window_instance;
    private worldBuilder world_builder;

    private InputField zone_weight_input;
    private InputField votes_weight_input;
    private InputField population_weight_input;

    private InputField[] votes_zone_input = new InputField[6];

    private List<GameObject> slimes = new List<GameObject>();

    public GameObject slime_prefab;

    public CodeEditor game_editor;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("elo??");
        
        builder_instance =
                  Instantiate(builder, new Vector3(0, 0 ,0)
                  , Quaternion.identity);
        world_builder = builder_instance.GetComponent<worldBuilder>();
        world_builder.game_editor = game_editor;
        debug_window_instance =
                  Instantiate(debug_window, new Vector3(0, 0, 0)
                  , Quaternion.identity);

        zone_weight_input = GameObject.Find("zone_weight_input").GetComponent<InputField>();
        population_weight_input = GameObject.Find("population_weight_input").GetComponent<InputField>();
        votes_weight_input = GameObject.Find("votes_weight_input").GetComponent<InputField>();

        votes_zone_input[0] = GameObject.Find("votes_zone_1").GetComponent<InputField>();
        votes_zone_input[1] = GameObject.Find("votes_zone_2").GetComponent<InputField>();
        votes_zone_input[2] = GameObject.Find("votes_zone_3").GetComponent<InputField>();
        votes_zone_input[3] = GameObject.Find("votes_zone_4").GetComponent<InputField>();
        votes_zone_input[4] = GameObject.Find("votes_zone_5").GetComponent<InputField>();
        votes_zone_input[5] = GameObject.Find("votes_zone_6").GetComponent<InputField>();




    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            //spawn_slime(4, 4);
            //spawn_slime(54, 48);
            //spawn_slime(57, 52);
            //spawn_slime(52, 51);
            //spawn_slime(51, 64);
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            //world_builder.set_avaliability_with_id(7, 8, 0);
            //world_builder.set_avaliability_with_id(7, 9, 0);
            //world_builder.set_avaliability_with_id(7, 10, 0);
            //world_builder.set_avaliability_with_id(7, 11, 0);
            //world_builder.set_avaliability_with_id(7, 12, 0);
            //world_builder.set_avaliability_with_id(7, 13, 0);
            //world_builder.set_avaliability_with_id(7, 14, 0);
            //world_builder.set_avaliability_with_id(8, 14, 0);
            //world_builder.set_avaliability_with_id(9, 14, 0);
            //world_builder.set_avaliability_with_id(10, 14, 0);
            //world_builder.set_avaliability_with_id(11, 14, 0);
            //world_builder.set_avaliability_with_id(12, 14, 0);
            //world_builder.set_avaliability_with_id(13, 14, 0);
            //world_builder.set_avaliability_with_id(14, 14, 0);
            //world_builder.set_avaliability_with_id(15, 14, 0);
        }
    }

    bool spawn_slime(int pos_x , int pos_y)
    {
        Title t = world_builder.Find_title_with_id(pos_x, pos_y);
        if (world_builder.Find_avaliability_with_id(pos_x , pos_y) == 1)
        {
            world_builder.set_avaliability_with_id(pos_x, pos_y , 0);
            Vector3 spawn_point = t.tit.transform.position;
            spawn_point.y += 0.318f;
            spawn_point.z -= 1f;
            GameObject s = Instantiate(slime_prefab, spawn_point
                      , Quaternion.identity);
            s.GetComponent<slime>().inicialize_discrete_position(pos_x, pos_y);
            s.GetComponent<slime>().give_itself_itself(s.GetComponent<slime>());
            return true;
        }
        else
        {
            return false;
        }
    }

    public void show_pop()
    {
        world_builder.show_pop();
    }

    public void show_vote()
    {
        world_builder.show_vote();
    }

    public void show_zonep()
    {
        world_builder.show_zonep();
    }

    public void system_start()
    {
        world_builder.system_start();
    }

    public void system_stop()
    {
        world_builder.system_stop();
    }

    public void system_reset()
    {
        world_builder.system_reset();
    }


    public void zone_weight()
    {
        string s = zone_weight_input.text;
        world_builder.zone_weight(float.Parse(s));
        Debug.Log("zone update");
        Debug.Log(float.Parse(s));

    }

    public void vote_weight()
    {
        string s = votes_weight_input.text;
        world_builder.vote_weight(float.Parse(s));
        Debug.Log("vote update");
        Debug.Log(float.Parse(s));
    }
    public void pop_weight()
    {
        string s = population_weight_input.text;
        world_builder.pop_weight(float.Parse(s));
        Debug.Log("pop update");
        Debug.Log(float.Parse(s));
    }
    public void vote_1()
    {
        string s = votes_zone_input[0].text;
        world_builder.set_vote_target_1(float.Parse(s));
      
    }
    public void vote_2()
    {
        string s = votes_zone_input[1].text;
        world_builder.set_vote_target_2(float.Parse(s));

    }
    public void vote_3()
    {
        string s = votes_zone_input[2].text;
        world_builder.set_vote_target_3(float.Parse(s));

    }
    public void vote_4()
    {
        string s = votes_zone_input[3].text;
        world_builder.set_vote_target_4(float.Parse(s));

    }
    public void vote_5()
    {
        string s = votes_zone_input[4].text;
        world_builder.set_vote_target_5(float.Parse(s));

    }
    public void vote_6()
    {
        string s = votes_zone_input[5].text;
        world_builder.set_vote_target_6(float.Parse(s));

    }
    public void execute_script()
    {
        //Debug.Log(game_editor.execute_script());
    }

    public void aplication_quit()
    {
        Application.Quit();
    }



}
