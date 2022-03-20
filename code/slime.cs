using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Linq;

public class slime : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    private GameObject TheGameController;
    private worldBuilder controller;

    public Sprite img1;
    public Sprite img2;
    public Sprite img3;
    public Sprite img4;
    public Sprite img5;
    public Sprite img6;

    public bool debug_on;

    public GameObject shadow_pref;
    private GameObject shadow_one;

    public GameObject info_pref;
    private GameObject info_one;

    slime itself;



    // Moving routine----------------------------------
    public bool should_i_be_jumping = false;         //
    private bool jumping_enabled    = false;         //
    private int scale               = 2;             //
    int timer                       = 0;             //
                                                     //
    private Vector3 jump_displacement;               //
    private Vector3 moving_displacement;             //
    private Vector3 move_target;                     // 
                                                     //
    private float speed_multiplyier = (float)3;      //
    private float speed_mod         = (float)14;     //
    private float jumpHeight        = (float)0.02;   //
    private int pending_change      = 0;             //
    private Vector3 tmp_move_target;                 //
    //--------------------------------------------------

    private Vector3 shadow_pos;



    //Moving----------------------------------------------
    public bool waiting_for_orders = false;             //
    public bool during_execution   = false;             //
    public int[] current_pos       = new int[] { 0, 0 };//
    int[] target_pos               = new int[] { 0, 0 };//
    private List<PathFind.Point> main_path;             //
    //----------------------------------------------------


    void Start()
    {
        info_one = Instantiate(info_pref, transform.position, Quaternion.identity);

        /*
        string s = "";
        s += "waiting_for_orders " + waiting_for_orders + "" + '\n';
        s += "during_execution " + during_execution + "" + '\n';
        s += "move_target " + move_target[0] + " "+ move_target[1] + '\n';
        s += "moving_displacement " + moving_displacement[0] + " " + moving_displacement[1] + '\n';
        info_one.GetComponent<TextMesh>().text = s;
        */
        debug_on = false;

        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = img1;

        //Moving routine---------------------------------
        should_i_be_jumping = false;                   //
        moving_displacement = transform.position;      //
        move_target         = transform.position;      //
        jump_displacement   = new Vector3(0, 0, 0);    //
        //-----------------------------------------------

        //Shadow----------------------------------------------------------------
        shadow_pos = transform.position;
        shadow_pos.y += 0.0818f;
        shadow_pos.z += 0.0001f;
        shadow_one = Instantiate(shadow_pref, shadow_pos, Quaternion.identity);
        shadow_one.transform.localScale = new Vector3(0.142557f, 0.1841703f, 1f);
        //----------------------------------------------------------------------

        TheGameController = GameObject.Find("world_builder(Clone)");
        controller = TheGameController.GetComponent<worldBuilder>();
    }

    // Update is called once per frame
    void Update()
    {
        info_one.transform.position = moving_displacement;
        info_one.GetComponent<TextMesh>().fontSize = 10;

        if (debug_on)
        {
            string s = "";
            s += "waiting_for_orders " + waiting_for_orders + "" +                                '\n';
            s += "during_execution " + during_execution + "" +                                    '\n';
            s += "current_pos " + current_pos[0] + " " + current_pos[1] +                         '\n';
            s += "moving_displacement " + moving_displacement[0] + " " + moving_displacement[1] + '\n';
            s += "MAIN";
            info_one.GetComponent<TextMesh>().text = s;
        }
        

        //s += "main_path.Count " + main_path.Count + "" + '\n';
        

        if (during_execution) // && main_path != null
        {

            if (main_path.Count > 1)
            {

                float dist = Vector3.Distance(move_target, moving_displacement);
                if (debug_on)
                {
                    string si = "";
                    si += "waiting_for_orders " + waiting_for_orders + "" +                                '\n';
                    si += "during_execution " + during_execution + "" +                                    '\n';
                    si += "dist " + dist +                                                                 '\n';
                    si += "move_target " + move_target[0] + " " + move_target[1] +                         '\n';
                    si += "moving_displacement " + moving_displacement[0] + " " + moving_displacement[1] + '\n';
                    si += "main_path.Count " + main_path.Count + "" +                                      '\n';
                    info_one.GetComponent<TextMesh>().text = si;
                }
                


                if (dist < 0.01 && pending_change == 0)
                {

                    pending_change = 1;

                    tmp_move_target = controller.Find_title_with_id(main_path[0].x, main_path[0].y).tit.transform.position;
                    tmp_move_target.z = tmp_move_target.z - (float)1;
                    tmp_move_target.y = tmp_move_target.y + (float)0.3;

                    main_path.RemoveAt(0);

                }
            }
            else if (main_path.Count == 1)
            {

                float dist = Vector3.Distance(move_target, moving_displacement);

                if (debug_on)
                {

                    string si = "";
                    si += "waiting_for_orders " + waiting_for_orders + "" +                                '\n';
                    si += "during_execution " + during_execution + "" +                                    '\n';
                    si += "dist " + dist +                                                                 '\n';
                    si += "current_pos " + current_pos[0] + " " + current_pos[1] +                         '\n';
                    si += "moving_displacement " + moving_displacement[0] + " " + moving_displacement[1] + '\n';
                    si += "main_path.Count " + main_path.Count + "" +                                      '\n';
                    si += "LOLZZZZ";
                    info_one.GetComponent<TextMesh>().text = si;
                }


                if (dist < 0.01 && main_path.Count != 0 && pending_change == 0)
                {
                    pending_change = 1;


                    tmp_move_target = controller.Find_title_with_id(main_path[0].x, main_path[0].y).tit.transform.position;
                    tmp_move_target.z = tmp_move_target.z - (float)1;
                    tmp_move_target.y = tmp_move_target.y + (float)0.3;

                    main_path.RemoveAt(0);
                }
           
                }
            else if (main_path.Count == 0)
            {

                float dist = Vector3.Distance(move_target , moving_displacement);
                if (debug_on)
                {
                    string si = "";
                    si += "waiting_for_orders " + waiting_for_orders + "" +                                     '\n';
                    si += "during_execution " + during_execution + "" +                                         '\n';
                    si += "dist " + dist +                                                                      '\n';
                    si += "current_pos " + current_pos[0] + " " + current_pos[1] +                              '\n';
                    si += "moving_displacement " + moving_displacement[0] +      " " + moving_displacement[1] + '\n';
                    si += "main_path.Count " + main_path.Count + "" +                                           '\n';
                    info_one.GetComponent<TextMesh>().text = si;
                }
                

                if (dist < 0.01 && pending_change == 0)
                {
                    should_i_be_jumping = false;
                    during_execution    = false;
                    waiting_for_orders  = false;
                    controller.reset_selected_mob_if_unchanged(itself);
                    current_pos[0] = target_pos[0];
                    current_pos[1] = target_pos[1];
                }
            }
           
        }
        Moving_routine();
    }
    void OnMouseDown()
    {
        if (!during_execution)
        {
            should_i_be_jumping = !should_i_be_jumping;
            waiting_for_orders = !waiting_for_orders;
            if (waiting_for_orders)
            {
                controller.change_slected_mob(itself);
            }
            else
            {
                controller.reset_selected_mob_if_unchanged(itself);
            }
        
            
        
        }
    }
    public void set_target(int x_id , int y_id)
    {
        if ((!during_execution) && (current_pos[0] != x_id || current_pos[1] != y_id))
        {
            controller.reset_selected_mob_if_unchanged(itself);
            during_execution = true;
            target_pos[0] = x_id;
            target_pos[1] = y_id;

            PathFind.Point _from = new PathFind.Point(current_pos[0], current_pos[1]);
            PathFind.Point _to = new PathFind.Point(x_id, y_id);

            PathFind.Grid grid = new PathFind.Grid(
                controller.roughness_table.GetUpperBound(0) + 1,
                controller.roughness_table.GetUpperBound(1) + 1,
                controller.roughness_table);
            List<PathFind.Point> path = PathFind.Pathfinding.FindPath(grid, _from, _to);

            controller.set_avaliability_with_id(x_id, y_id, 0);
            controller.set_avaliability_with_id(current_pos[0], current_pos[1], 1);

            int b_x = current_pos[0];
            int b_y = current_pos[1];
            int enumerate = 0;

            foreach (PathFind.Point pp in path.ToList())
            {
                if (enumerate != path.Count - 1)
                {
                    int f_x = path[enumerate + 1].x;
                    int f_y = path[enumerate + 1].y;
                    if (((b_x == f_x) && (b_y != f_y)) || ((b_x != f_x) && (b_y == f_y)) || (Mathf.Abs(b_x - f_x) == Mathf.Abs(b_y - f_y)))
                    {
                        path[enumerate].to_remove = 1;
                    }
                    else
                    {
                        b_x = pp.x;
                        b_y = pp.y;

                    }
                    enumerate = enumerate + 1;

                }

            }
            path.RemoveAll(s => s.to_remove == 1);
            main_path = path;
            Debug.Log(main_path.Count);
  
            Vector3 tmp_target =
                controller.Find_title_with_id(path[0].x, path[0].y).tit.transform.position;
            move_target[0] = tmp_target.x;
            move_target[1] = tmp_target.y + 0.318f;
            main_path.RemoveAt(0);
        }
    }

    private void Moving_routine()
    {
        if (jumping_enabled)
        {
            float step = speed_multiplyier * Time.deltaTime;

            timer += 1;
            if (timer < 10 * scale)
            {
                spriteRenderer.sprite = img1;
                jump_displacement = new Vector3(0, 0, 0);
                transform.position = jump_displacement + moving_displacement;
                shadow_pos = moving_displacement;
                shadow_pos.y += 0.0818f;
                shadow_pos.z += 0.0001f;
                shadow_one.transform.position = shadow_pos;
                if (!should_i_be_jumping)
                {
                    jumping_enabled = false;
                }

                if (pending_change == 1)
                {
                    pending_change = 0;
                    move_target = tmp_move_target;
                }
            }
            else if (timer < 20 * scale)
            {
                spriteRenderer.sprite = img2;
                jump_displacement += new Vector3(0, (float)0.2 * jumpHeight, 0);
                moving_displacement = Vector3.MoveTowards(moving_displacement, move_target, step);
                transform.position = jump_displacement + moving_displacement;
                shadow_pos = moving_displacement;
                shadow_pos.y += 0.0818f;
                shadow_pos.z += 0.0001f;
                shadow_one.transform.position = shadow_pos;

            }
            else if (timer < 30 * scale)
            {
                spriteRenderer.sprite = img3;
                jump_displacement += new Vector3(0, (float)0.8 * jumpHeight, 0);
                moving_displacement = Vector3.MoveTowards(moving_displacement, move_target, step);
                transform.position  = jump_displacement + moving_displacement;
                shadow_pos          = moving_displacement;
                shadow_pos.y += 0.0818f;
                shadow_pos.z += 0.0001f;
                shadow_one.transform.position = shadow_pos;
            }
            else if (timer < 40 * scale)
            {
                spriteRenderer.sprite = img4;
                jump_displacement -= new Vector3(0, (float)0.5 * jumpHeight, 0);
                moving_displacement = Vector3.MoveTowards(moving_displacement, move_target, step);
                transform.position = jump_displacement + moving_displacement;
                shadow_pos = moving_displacement;
                shadow_pos.y += 0.0818f;
                shadow_pos.z += 0.0001f;
                shadow_one.transform.position = shadow_pos;
            }
            else if (timer < 50 * scale)
            {
                spriteRenderer.sprite = img5;
                jump_displacement -= new Vector3(0, (float)0.4 * jumpHeight, 0);
                moving_displacement = Vector3.MoveTowards(moving_displacement, move_target, step);
                transform.position = jump_displacement + moving_displacement;
                shadow_pos = moving_displacement;
                shadow_pos.y += 0.0818f;
                shadow_pos.z += 0.0001f;
                shadow_one.transform.position = shadow_pos;
            }
            else if (timer < 60 * scale)
            {
                spriteRenderer.sprite = img6;
                jump_displacement -= new Vector3(0, (float)0.1 * jumpHeight, 0);
                moving_displacement = Vector3.MoveTowards(moving_displacement, move_target, step);
                transform.position = jump_displacement + moving_displacement;
                shadow_pos = moving_displacement;
                shadow_pos.y += 0.0818f;
                shadow_pos.z += 0.0001f;
                shadow_one.transform.position = shadow_pos;
            }
            else
            {
                timer = 0;
            }
        }
        else if (should_i_be_jumping)
        {
            jumping_enabled = true;
        }
    }

    public void inicialize_discrete_position(int x_d , int y_d)
    {
        current_pos[0] = x_d;
        current_pos[1] = y_d;
    }

    public void give_itself_itself(slime s)
    {
        itself = s;
    }
}
