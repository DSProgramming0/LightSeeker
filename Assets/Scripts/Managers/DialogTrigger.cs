using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogTrigger : MonoBehaviour
{
    [SerializeField] private Dialog dialog;

    [SerializeField] private List<string> thisDialogSentences;
    private bool hasPlayed = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (!hasPlayed)
            {
                StartCoroutine(startingDialog());
            }
        }
    }

    private IEnumerator startingDialog()
    {
        hasPlayed = true;
        dialog.clearSentences();

        yield return new WaitForSeconds(1f);

        dialog.setSentences(thisDialogSentences);

        yield return new WaitForSeconds(1f);

        dialog.startDialog();
    }
}
