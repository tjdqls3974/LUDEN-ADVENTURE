using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playermove : MonoBehaviour
{
    public float maxSpeed;
    public float jumpPower;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator animator;
    bool isDamaged = false;
    Vector3 startPos;
    float maxDistance = 0f;
    public TimeController timeController; // TimeController 연결용 변수
    public float MaxDistance => maxDistance;  // 다른 스크립트(TimeController) 연동

    void Start()
    {
        animator = GetComponent<Animator>();
        startPos = transform.position; // 시작 위치 저장
    }
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        // 이거슨 움직이는거요
        if (!isDamaged)
        {
            float h = Input.GetAxisRaw("Horizontal");
            rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);
        }
        // 이거슨 속도 제한이라는거시다
        if (!isDamaged)
        {
            if (rigid.velocity.x > maxSpeed) // 오른쪽 속도제한
                rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
            else if (rigid.velocity.x < maxSpeed * (-1)) // 왼쪽 속도제한
                rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
        }

        // 착지빔
        if (rigid.velocity.y < 0) { 
            Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0));
            RaycastHit2D rayHit = Physics2D.Raycast(rigid.position, Vector3.down, 1.1f, LayerMask.GetMask("Platform"));
            if (rayHit.collider != null)
            {
                if (rayHit.distance < 1f)
                {
                    animator.SetBool("isjumping", false);
                }
            }
        }
    }
    void Update()
    {
        //저어엄프
        if (Input.GetButtonDown("Jump") && Mathf.Abs(rigid.velocity.y) < 0.2f)  // <- 중복점프 야다 # 원래 0.01에서 0.2로 수정 ( 점프 씹힘 테스트 )
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            animator.SetBool("isjumping", true);
        }

        // 키 떼면 멈춤
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(0, rigid.velocity.y);
        }
        // 방향전환
        if (rigid.velocity.x < 0)
            spriteRenderer.flipX = true;
        else if (rigid.velocity.x > 0)
            spriteRenderer.flipX = false;
        // 이거슨 애니메이션이라는거시다.
        if (rigid.velocity.normalized.x == 0)
            animator.SetBool("iswalking", false);
        else
            animator.SetBool("iswalking", true);
        //낙사처리
        if (transform.position.y < -10f) // y좌표 -10 이하로 떨어지면
        {
            Respawn();
        }
        // 최대 이동 거리 측정
        float currentDistance = transform.position.x - startPos.x;
        if (currentDistance > maxDistance)
        {
            maxDistance = currentDistance;
        }

    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            // 함정카드 발동
            Debug.Log("너는 미끼를 물어버린 것이여"); // 나중에 지워야하오
            Ondamaged();
        }
    }

    void Ondamaged()
    {
        isDamaged = true;
        gameObject.layer = 11; // 피격레이어로 변경
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);// 투명도 조절
        rigid.velocity = Vector2.zero;
        rigid.AddForce(new Vector2(-30, 10) , ForceMode2D.Impulse);
        Invoke("Offdamaged", 2);
        animator.SetTrigger("doDamaged"); // 애니
    }
    void Offdamaged()
    {
        isDamaged = false;
        gameObject.layer = 10; // 일반레이어로 변경
        spriteRenderer.color = new Color(1, 1, 1, 1);// 투명도 원상복구
    }
    void Respawn()
    {
        transform.position = startPos; // 시작 위치로 이동
        rigid.velocity = Vector2.zero; // 속도 초기화
    }
    void OnTriggerEnter2D(Collider2D other) // 깃발 도착 감지
    {
        if (other.CompareTag("Finish"))
        {
            if (timeController != null)
                timeController.isTimeOver = true;
            UnityEngine.SceneManagement.SceneManager.LoadScene("end");
        }
    }
}



