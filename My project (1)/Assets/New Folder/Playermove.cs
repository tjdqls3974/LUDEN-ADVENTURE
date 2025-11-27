using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Playermove : MonoBehaviour
{
    public int attempts = 0;
    public float maxSpeed;
    public float jumpPower;
    Rigidbody2D rigid;
    SpriteRenderer spriteRenderer;
    Animator animator;
    bool isDamaged = false;
    Vector3 startPos;
    float maxDistance = 0f;
    public TimeController timeController; // TimeController ����� ����
    public float MaxDistance => maxDistance;  // �ٸ� ��ũ��Ʈ(TimeController) ����

    private data data; // data ����� ����

    void Start()
    {
        animator = GetComponent<Animator>();
        startPos = transform.position; // ���� ��ġ ����
        attempts = 0;
        data = FindObjectOfType<data>();
        Debug.Log("Player Name: " + data.name);
        Debug.Log("Student Number: " + data.stnum);
        Debug.Log("Contacts: " + data.contacts);
    }
    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        // �̰Ž� �����̴°ſ�
        if (!isDamaged)
        {
            float h = Input.GetAxisRaw("Horizontal");
            rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);
        }
        // �̰Ž� �ӵ� �����̶�°Žô�
        if (!isDamaged)
        {
            if (rigid.velocity.x > maxSpeed) // ������ �ӵ�����
                rigid.velocity = new Vector2(maxSpeed, rigid.velocity.y);
            else if (rigid.velocity.x < maxSpeed * (-1)) // ���� �ӵ�����
                rigid.velocity = new Vector2(maxSpeed * (-1), rigid.velocity.y);
        }

        // ������
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
        //�������
        if (Input.GetButtonDown("Jump") && Mathf.Abs(rigid.velocity.y) < 0.2f)  // <- �ߺ����� �ߴ� # ���� 0.01���� 0.2�� ���� ( ���� ���� �׽�Ʈ )
        {
            rigid.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            animator.SetBool("isjumping", true);
        }

        // Ű ���� ����
        if (Input.GetButtonUp("Horizontal"))
        {
            rigid.velocity = new Vector2(0, rigid.velocity.y);
        }
        // ������ȯ
        if (rigid.velocity.x < 0)
            spriteRenderer.flipX = true;
        else if (rigid.velocity.x > 0)
            spriteRenderer.flipX = false;
        // �̰Ž� �ִϸ��̼��̶�°Žô�.
        if (rigid.velocity.normalized.x == 0)
            animator.SetBool("iswalking", false);
        else
            animator.SetBool("iswalking", true);
        //����ó��
        if (transform.position.y < -10f) // y��ǥ -10 ���Ϸ� ��������
        {
            Respawn();
        }
        // �ִ� �̵� �Ÿ� ����
        // float currentDistance = transform.position.x - startPos.x;
        float distance = Vector2.Distance(transform.position, startPos);
        if (distance > maxDistance)
        {
            maxDistance = distance;
            data.distance = Mathf.CeilToInt(maxDistance).ToString();
        }

    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            // ����ī�� �ߵ�
            Debug.Log("�ʴ� �̳��� ������� ���̿�"); // ���߿� �������Ͽ�
            Ondamaged();
        }
    }

    void Ondamaged()
    {
        isDamaged = true;
        gameObject.layer = 11; // �ǰݷ��̾�� ����
        spriteRenderer.color = new Color(1, 1, 1, 0.4f);// ������ ����
        rigid.velocity = Vector2.zero;
        rigid.AddForce(new Vector2(-30, 10) , ForceMode2D.Impulse);
        Invoke("Offdamaged", 2);
        animator.SetTrigger("doDamaged"); // �ִ�
    }
    void Offdamaged()
    {
        isDamaged = false;
        gameObject.layer = 10; // �Ϲݷ��̾�� ����
        spriteRenderer.color = new Color(1, 1, 1, 1);// ������ ���󺹱�
    }
    void Respawn()
    {
        transform.position = startPos; // ���� ��ġ�� �̵�
        rigid.velocity = Vector2.zero; // �ӵ� �ʱ�ȭ
        data.attempts++; // ������ 1 ���
    }
    void OnTriggerEnter2D(Collider2D other) // ��� ���� ����
    {
        if (other.CompareTag("Finish"))
        {
            if (timeController != null)
                timeController.isTimeOver = true;
            UnityEngine.SceneManagement.SceneManager.LoadScene("end");
            data.attempts = attempts;
            data.cleared = true;
            data.record = timeController.FormatTime(timeController.times);
        }
    }
}



