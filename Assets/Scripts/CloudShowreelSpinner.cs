using UnityEngine;
using System.Collections;

namespace Chadderbox.Clouds
{
    //NOTE: For some reason, this script really doesn't work. I can't necessarilly be bothered fixing it.
    public class CloudShowreelSpinner : MonoBehaviour
    {
        [SerializeField] private Vector3 spinSpeed = Vector3.zero;
        [SerializeField] private GameObject[] clouds = null;
        private GameObject currentCloud = null;
        private int cloudIndex = 0;
        private bool notChanged = false;

        void Start()
        {  
            currentCloud = Instantiate(clouds[cloudIndex], transform.position, Quaternion.identity, this.transform);

            //Init the not changed coroutine
            StartCoroutine("CheckNotChanged");
        }

        void Update()
        {   
            //Spin the model
            this.transform.rotation = Quaternion.Euler(this.transform.rotation.eulerAngles + (spinSpeed * Time.deltaTime));
            
            /*
            * I am only going to be spinning on the Y axis
            * so I am just checking the Y axis
            */
            if (transform.eulerAngles.y < 10 && notChanged) //Check for a full rotation
            {
                //Perform cloud swap and get ready for the next time
                notChanged = false;
                Destroy(currentCloud);
                ++cloudIndex;
                currentCloud = Instantiate(clouds[cloudIndex], transform.position, Quaternion.identity, this.transform);
                StartCoroutine("CheckNotChanged");
            }
        }

        private IEnumerator CheckNotChanged()
        {  
            //This is a bit of a janky solution, but I don't care about performance here and it works
            yield return new WaitForSeconds((360/spinSpeed.y) / 3); //make sure this isn't 0 haha
            notChanged = true;
        }
    }
}
