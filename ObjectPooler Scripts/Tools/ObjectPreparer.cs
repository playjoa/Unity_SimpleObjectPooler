using UnityEngine;

namespace JoaoMilone.Pooler.Tools
{
    public static class ObjectPreparer
    {
        public static void PrepareObject(GameObject auxObject, Transform tf)
        {
            if (auxObject.activeSelf)
                auxObject.SetActive(false);

            auxObject.transform.position = tf.position;
            auxObject.transform.rotation = tf.rotation;
            auxObject.SetActive(true);
        }
        
        public static  void PrepareObject(GameObject auxObject, Vector3 position)
        {
            if (auxObject.activeSelf)
                auxObject.SetActive(false);

            auxObject.transform.position = position;
            auxObject.SetActive(true);
        }

        public static  void PrepareObject(GameObject auxObject, Vector3 position, Quaternion idt)
        {
            if (auxObject.activeSelf)
                auxObject.SetActive(false);

            auxObject.transform.position = position;
            auxObject.transform.rotation = idt;
            auxObject.SetActive(true);
        }

        public static  void PrepareObjectWithLocalPosition(GameObject auxObject, Vector3 localPosition)
        {
            if (auxObject.activeSelf)
                auxObject.SetActive(false);

            auxObject.transform.localPosition = localPosition;
            auxObject.SetActive(true);
        }
    }
}