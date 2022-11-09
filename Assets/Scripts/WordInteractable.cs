using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class WordInteractable : MonoBehaviour
{
    public static Action<WordInteractable, WordInteractable> CompletedWordAction;
    
    [SerializeField] private GameObject collisionVFX;
    [SerializeField] private GameObject correctWordVFX;
    [SerializeField] private AudioClip collisionSFX;
    [SerializeField] private AudioClip correctWordSFX;
    [SerializeField] private AudioClip incorrectWordSFX;
    [SerializeField] private Collider normalCollider;
    [SerializeField] private Collider grabbedCollider;
    public GameObject grabIcon;

    public int attempts { get; private set; }
    public Rigidbody rb { get; private set; }
    public SpriteRenderer spriteRenderer { get; private set; }
    public TextMeshPro textMeshPro { get; private set; }
    
    [HideInInspector] public Word wordData;
    [HideInInspector] public XRGrabInteractable grabInteractable;
    private bool isGrabbed;
    private float bounceSpeed = 0.5f;
    private WaitForSeconds collisionDelay = new WaitForSeconds(1f);
    private GameObject instantiatedVfx;
    private IEnumerator collisionCoroutine;
    private AudioSource audioSource;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>();
        textMeshPro = GetComponent<TextMeshPro>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        audioSource = GetComponent<AudioSource>();
    }
    private void Start()
    {
        grabInteractable.selectEntered.AddListener(Grabbed);
        grabInteractable.selectExited.AddListener(UnGrabbed);
        grabInteractable.hoverEntered.AddListener(Hovered);
        grabInteractable.hoverExited.AddListener(UnHovered);
    }

    private void UnHovered(HoverExitEventArgs arg0)
    {
        textMeshPro.color = Color.white;
    }

    private void Hovered(HoverEnterEventArgs arg0)
    {
        textMeshPro.color = Color.yellow;
    }

    private void OnDestroy()
    {
        if(instantiatedVfx != null)
        {
            Destroy(instantiatedVfx);
        }
        grabInteractable.selectEntered.RemoveListener(Grabbed);
        grabInteractable.selectExited.RemoveListener(UnGrabbed);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isGrabbed && collision.transform.TryGetComponent(out WordInteractable wordInteractable))
        {
            collisionCoroutine = WordCollisionCoroutine(collision.contacts[0].point, wordInteractable);
            StartCoroutine(collisionCoroutine);
        }
        else if (!isGrabbed)
        {
            rb.AddForce((UnityEngine.Random.insideUnitSphere * 0.5f) + (bounceSpeed * collision.contacts[0].normal), ForceMode.Impulse);

        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collisionCoroutine != null)
        {
            StopCoroutine(collisionCoroutine);
            audioSource.Stop();
        }
        if (instantiatedVfx != null)
        {
            Destroy(instantiatedVfx);
        }
    }

    private IEnumerator WordCollisionCoroutine(Vector3 contactPoint, WordInteractable otherWord)
    {
        instantiatedVfx = Instantiate(collisionVFX, contactPoint, Quaternion.identity);
        audioSource.clip = collisionSFX;
        audioSource.loop = true;
        audioSource.Play();
        yield return collisionDelay;
        if (otherWord.wordData == wordData && wordData.englishWord != textMeshPro.text)
        {
            Vector3 centerPoint = (gameObject.transform.position + otherWord.gameObject.transform.position) / 2;
            audioSource.PlayOneShot(correctWordSFX);
            Instantiate(correctWordVFX, centerPoint, Quaternion.identity);
            textMeshPro.color = Color.green;
            otherWord.textMeshPro.color = Color.green;
            yield return new WaitForSeconds(0.5f);
            Destroy(instantiatedVfx);
            Destroy(otherWord.gameObject);
            Destroy(gameObject);
            CompletedWordAction.Invoke(this, otherWord);
        }
        else if (wordData.englishWord != textMeshPro.text && otherWord.wordData != wordData)
        {
            audioSource.PlayOneShot(incorrectWordSFX);
            textMeshPro.color = Color.red;
            otherWord.textMeshPro.color = Color.red;
            attempts++;
        }
    }
    private void Grabbed(SelectEnterEventArgs arg0)
    {
        isGrabbed = true;
        grabbedCollider.isTrigger = false;
        normalCollider.isTrigger = true;
        Debug.Log("Grabbed");
    }

    private void UnGrabbed(SelectExitEventArgs arg0)
    {
        isGrabbed = false;
        grabbedCollider.isTrigger = true;
        normalCollider.isTrigger = false;
        textMeshPro.color = Color.white;
        Debug.Log("UnGrabbed");
    }

    private void Update()
    {
        transform.forward = Camera.main.transform.forward;
    }
}
