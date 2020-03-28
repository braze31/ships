using UnityEngine;
using UnityEngine.UI;

public class Shield : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    public float health;
    public GameObject ownShip;
    RawImage image;
    Canvas imageCanvas;
    Image item;
    private float fadeStart = 1f;
    private bool changeAlpha;
    float startAlphaValue = 1f;
    private float damage;
    private float hpBar;

    void Start()
    {
        image = gameObject.GetComponent<RawImage>();
        fadeStart = Time.time;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Rocket" || other.tag == "RocketS")
        {
            other.GetComponent<Rocket>().ReduceDamageRocket();
        }
        if (other.tag == "Bomb")
        {
            other.GetComponent<Bomb>().ReduceDamageRocket();
        }
    }

    //void OnTriggerExit2D(Collider2D other)
    //{
    //    if (other.tag == "Rocket" || other.tag == "RocketS")
    //    {
    //        other.GetComponent<Rocket>().BackDamageRocket();
    //    }
    //}

    public void TakeNewColorA()
    {
        startAlphaValue -= 0.1f;
    }

    public void TakeHpbarValueFromImage(Image _item)
    {
        item = _item;
        health = item.transform.parent.GetComponent<DropZone>().healthBar.bar.fillAmount;
    }

    public void TakeDamage(float _damage)
    {

        DropZone dz = item.transform.parent.GetComponent<DropZone>();
        dz.healthBar.ReduceHPSystem(0.15f, gameObject);
        startAlphaValue -= damage;
        if (dz.healthBar.bar.fillAmount <= 0.05f)
        {
            Destroy(gameObject);
        }
        //if (item.transform.parent.GetComponent<DropZone>().healthBar.bar.fillAmount<=0.4)
        //{
        //    Destroy(gameObject);
        //}
    }

    // Update is called once per frame
    void Update()
    {
        if (!changeAlpha)
        {
            float alpha = startAlphaValue - (Time.time - fadeStart) / 10f;
            Color nc = image.color;
            nc.a = alpha;
            image.color = nc;
        }
        if (image.color.a <= 0.5f)
        {
            Destroy(gameObject);
        }
        if (item != null)
        {
            hpBar = item.transform.parent.GetComponent<DropZone>().healthBar.bar.fillAmount;
            //Debug.Log(item.transform.parent.GetComponent<DropZone>().healthBar.bar.fillAmount);
            if (hpBar <= 0.1f)
            {
                Destroy(gameObject);
            }
        }
    }
}
