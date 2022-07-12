using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Configuration;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.MeshOperations;
using TMPro;

public class PlayerController : MonoBehaviour
{

    public int coins = 0;



    public TMP_Text coinText;
    
    // guarda uma referencia para os controles que criamos no inputAction
    private Controls _gameControls; 
    //guarda referencia para o playerInput, que é quem conecta o dispositivo de controle ao codigo
    private PlayerInput _playerInput;
    // referencia para a camera principal (main) do jogo
    private Camera _mainCamera;
    // guarda o movimento que esta sendo lido do controle do jogador 
    private Vector2 _moveInput;
    // guarda a referencia para o componente fisico do jogador que usaremos para mover o jogador 
    private Rigidbody _rigidbody;

    private bool _isGrounded;
        
    // velocidade que o jogador vai se mover
    public float moveMultiplier;

    //velocidade maxima que o jogadr vai poder andar em cada eixo (x e z somente pois noa queremos limitar o y)
    public float maxVelocity;

    public float rayDistance;

    public LayerMask layerMask;

    public float jumpForce;
    
    
    
    
    // serve para atualizarmos o objeto 
    private void OnEnable()
    {
        // associa a variavel o componente rigidbody presente no objeto do jogador no unity
        _rigidbody = GetComponent<Rigidbody>();
        
        // instancia um novo objeto da classe GameControls
        _gameControls = new Controls();
        
        // associa a variavel o componente playerinput presente no objeto do jogador na unity
        _playerInput = GetComponent<PlayerInput>();

        // associa a nossa variavel o valor presente na variavel main da classe camera que é a camera principal do jogo  
        _mainCamera = Camera.main;

        // inscrevendo o delegate pata funcal que é chamada uma tecla/botao no controle é apertado 
        _playerInput.onActionTriggered += OnActionTriggered;
        
        
    }

    // chama quando o objeto é desativado 
    private void OnDisable()
    {
        // desinscrever o delegate
        _playerInput.onActionTriggered -= OnActionTriggered;
        
    }
    
    // delegate para adicionarmos funcionalidade quando o jogador aperta um botao
    // o parametro obj, da classe InputAction.CallbackContext traz as ainformacoes do botao 
    // que foi apertado pelo jogador 
    private void OnActionTriggered(InputAction.CallbackContext obj)
    {
        // compara a informacao trazida pela obj, checando se o nome da acao executada 
        // tem o mesmo nome da acao de mover o jogador (Movement.name)
        if (obj.action.name.CompareTo(_gameControls.Gameplay.Move.name)== 0)
        {
            // caso a acao seja de mocer, passamos o valor que esta vindo no obj, que 
            // como definimos no InputAction, é um Vector2, para a variacel _moveInput
            _moveInput = obj.ReadValue<Vector2>();

        }

        if (obj.action.name.CompareTo(_gameControls.Gameplay.Jump.name) == 0)
        {
            if(obj.performed) Jump();
        }
        

    }

    // executa a movimentacao o jogador atraves da fisica
    private void Move()
    {
        // passamo o vetor que aponta para a direcao que a camera esta olhando 
        Vector3 camFoward = _mainCamera.transform.forward;
        Vector3 camRight = _mainCamera.transform.right;
        camFoward.y = 0;
        camRight.y = 0;
        
        // usamos AddForce para adicionar uma forca gradual para o jogador, quanto mais 
        // tempo seguramos a tecla mais rapia a bolinha vai 
        _rigidbody.AddForce(
            // multiplicamos o input que move o jogador para a frente pelo vetoe que aponta 
            // para a frente da camera
            (_mainCamera.transform.forward * _moveInput.y +
             // multiplica o input qye move o jogador para a direita pelo vetor que aponta 
             // para a direita da camera 
                             _mainCamera.transform.right * _moveInput.x)
            // multiplica esse resultado pela velocidade e pela variavel de deltaTime
            * moveMultiplier * Time.deltaTime);
    }

    
    
    private void FixedUpdate()
    {
        Move();
        LimitVelocity();
    }
    
    //funcao que vai limitar a velocidade macima do jogador 
    private void LimitVelocity()
    {
        Vector3 velocity = _rigidbody.velocity;
        
        if (Mathf.Abs(velocity.x)> maxVelocity) velocity.x = Mathf.Sign(velocity.x) * maxVelocity;
        
        

        velocity.z = Mathf.Clamp(value: velocity.z, min: -maxVelocity, maxVelocity);
        _rigidbody.velocity = velocity;

    }

    private void CheckGround()
    {
        RaycastHit collision;

        if (Physics.Raycast(origin: transform.position, direction: Vector3.down, out collision, rayDistance,
            layerMask))
        {
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false; 
        }
    }
    
    private void Jump()
    {
        if (_isGrounded)
        {
          _rigidbody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);  
        } 
    
    }

    private void Update()
    {
        CheckGround();
    }

    private void OnDrawGizmos()
    {
        Debug.DrawRay(start:transform.position, dir: Vector3.down * rayDistance, Color.yellow);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Coin"))
        {
            coins++;
            
            coinText.text = coins.ToString();
            Destroy(other.gameObject);
        }
    }
    
}

