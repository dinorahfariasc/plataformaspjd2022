using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{


    public static GameManager Instance; // instancia do sigleton
    
    [SerializeField] private string guiName; // nome da interface

    [SerializeField] private string levelName; // nome da fase do jogo

    [SerializeField] private GameObject playerAndCameraPreFab; //referencia pro prefab do jogador + camera

    // Start is called before the first frame update

    private void Awake()
    {
        // condicao de criacao do singleton
        if (Instance == null)
        {
            Instance = this;
            
            // impedor que o objeto indicado entre parenteses seja destruido
            DontDestroyOnLoad(this.gameObject);
            
        }
        else Destroy(this.gameObject);
    }

    void Start()
    {
        // se estiver iniciando a partir de intialization, carregue o jogo do jeito de antes
        if (SceneManager.GetActiveScene().name == "Initialization")
            StartGameFromInitialization();
        else // caso contrario, esta iniciando a partir do level, carregue o ojogo do modo apropiado
            StartGameFromLevel();

    }

    private void StartGameFromLevel()
    {
        // 1 - carrega a cena de interface de modo aditivo para nao apagar a cena do level da memoria ram
        SceneManager.LoadScene(guiName, LoadSceneMode.Additive);
        
        // 2 - prescisa instanciar o jogador na cena
        Vector3 playerStartPosition = GameObject.Find("PlayerStart").transform.position;

        Instantiate(playerAndCameraPreFab, playerStartPosition, Quaternion.identity);



    }
    private void StartGameFromInitialization()
    {
        
        // 1 - carregar a cena de interface e do jogo 
        SceneManager.LoadScene(guiName);
        //SceneManager.LoadScene(levelName,LoadSceneMode.Additive); // aditive carrega uma nova cena

        SceneManager.LoadSceneAsync(levelName, LoadSceneMode.Additive).completed += operation =>
        {
            
            Scene levelScene = default;
        
        
            for (int i = 0;
                i < SceneManager.sceneCount;
                i++)
            {
                if (SceneManager.GetSceneAt(i).name == levelName)
                {
                    levelScene = SceneManager.GetSceneAt(i);
                    break;
                }
            }
        
   
            if (levelScene != default) SceneManager.SetActiveScene(levelScene);

            // 2 - prescisa instanciar o jogador na cena
            Vector3 playerStartPosition = GameObject.Find("PlayerStart").transform.position;

            Instantiate(playerAndCameraPreFab, playerStartPosition, Quaternion.identity);

            // 3 - começar a partida
            
        };


    }
   
}
