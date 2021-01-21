using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

// 점수 및 스테이지 관리
public class GameManager : MonoBehaviour
{
    public int totalPoint;
    public int stagePoint;
    public int stageIndex;
    public int hp;
    public PlayerMove player;
    public GameObject[] Stages;

    // UI
    public Image[] UIhp;
    public Text UIPoint;
    public Text UIStage;
    public GameObject UIRestartBtn;

    public void NextStage()
    {
        // Change Stage
        if(stageIndex < Stages.Length-1){
            Stages[stageIndex].SetActive(false);
            stageIndex++;
            Stages[stageIndex].SetActive(true);
            PlayerReposition();

            UIStage.text = "STAGE" + (stageIndex + 1);
        }else{  // Game Clear
            // Player Contorl Lock
            Time.timeScale = 0;
            // Result UI
            Debug.Log("Clear!");

            // Restart Button UI
            UIRestartBtn.SetActive(true);
            Text btnText = UIRestartBtn.GetComponentInChildren<Text>();
            btnText.text = "Clear!";
            UIRestartBtn.SetActive(true);
        }

        // Calculate Point
        totalPoint += stagePoint;
        stagePoint = 0;
    }

    void Update(){
        UIPoint.text = (totalPoint + stagePoint).ToString();
    }

    public void HpDown(){
        if(hp > 1){
            hp--;
            UIhp[hp].color = new Color(0, 0, 0, 0.2f);
        }else{
            UIhp[0].color = new Color(0, 0, 0, 0.2f);

            // Player Die Effect
            player.OnDie();

            // Result UI
            Debug.Log("죽었습니다.");

            // Retry Button UI
            UIRestartBtn.SetActive(true);
        }
    }

    void OnTriggerEnter2D(Collider2D collision){
        if(collision.gameObject.tag == "Player"){
            // Player Reposition
            if(hp > 1){
                PlayerReposition();
            }

            // HP Down
            HpDown();
        }
    }

    void PlayerReposition(){
        player.transform.position = new Vector3(-7.5f, 0.5f, -1);
        player.VelocityZero();
    }

    public void Restart(){
        Time.timeScale = 1;
        SceneManager.LoadScene(0);
    }
}
