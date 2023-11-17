using System.Collections;
using UnityEngine;

namespace Finals {
public class Projectile : MonoBehaviour {
    private bool _isFired = false;
    
    public void Init(AIController aiCtrl, float damage, float duration = 1f) {
        if (_isFired) return;
        _isFired = true;
        this.transform.LookAt(aiCtrl.transform);
        StartCoroutine(Fire_Co(aiCtrl, damage, duration));
    }

    private IEnumerator Fire_Co(AIController aiCtrl, float damage, float duration = 1f) {
        Vector3 origPos = transform.position;
        float t = 0;
        for (t = 0 ; t < 1.0f ; t+= Time.deltaTime / duration) {
            transform.position = Vector3.Lerp(origPos, aiCtrl.transform.position + Vector3.up, t);
            yield return new WaitForEndOfFrame();
        }
        aiCtrl.ApplyDamage(damage);
        Destroy(this.gameObject);
    }
}
}