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
        dialog.resetIndex();

        yield return new WaitForSeconds(.2f);

        dialog.setSentences(thisDialogSentences);
        dialog.setTypingSound(Random.Range(0, dialog.typingSounds.Count - 1));

        yield return new WaitForSeconds(.5f);

        dialog.startDialog();
    }
}
