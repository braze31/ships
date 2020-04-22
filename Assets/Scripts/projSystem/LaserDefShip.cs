using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserDefShip : MonoBehaviour
{
    private GameObject _target;
    private float sign;
    private float offset;
    private float angle;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        if (_target)
        {
            transform.position = Vector2.MoveTowards(transform.position, _target.transform.position, 1500f * Time.deltaTime);
            Vector3 direction = _target.transform.position - transform.position;
            sign = (direction.y >= 0) ? 1 : -1;
            offset = (sign >= 0) ? 0 : 360;

            angle = Vector2.Angle(Vector2.right, direction) * sign + offset;

            gameObject.GetComponent<RectTransform>().rotation = Quaternion.Euler(0f, 0f, angle-90f);
            if (Vector2.Distance(transform.position,_target.transform.position) <= 0.2f)
            {
                if (_target.tag == "Rocket" || _target.tag == "RocketS")
                {
                    _target.GetComponent<Rocket>().HealthRocket -= 20f;
                    Destroy(gameObject);
                }
            }
        }
        if (_target == null)
        {
            Destroy(gameObject, 0.15f);
        }
    }

    public void TakeTarget(GameObject target)
    {
        if (target)
        {
            _target = target;
        }
    }
}
