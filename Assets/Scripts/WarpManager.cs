using System.Collections.Generic;
using UnityEngine;

public class WarpManager : Singleton<WarpManager>
{
    private readonly List<Transform> _registeredTransforms = new();

    [SerializeField] private Camera mainCamera;
    
    public void RegisterTransform(Transform registered)
    {
        _registeredTransforms.Add(registered);
    }
    
    public void UnregisterTransform(Transform registered)
    {
        _registeredTransforms.Remove(registered);
    }

    private void Update()
    {
        foreach (var registeredTransform in _registeredTransforms)
        {
            var viewportPosition = mainCamera.WorldToViewportPoint(registeredTransform.position);
            var outOfBounds = false;
        
            switch (viewportPosition.x)
            {
                case < 0:
                    viewportPosition.x = 1;
                    outOfBounds = true;
                    break;
                case > 1:
                    viewportPosition.x = 0;
                    outOfBounds = true;
                    break;
            }

            switch (viewportPosition.y)
            {
                case < 0:
                    viewportPosition.y = 1;
                    outOfBounds = true;
                    break;
                case > 1:
                    viewportPosition.y = 0;
                    outOfBounds = true;
                    break;
            }

            if (!outOfBounds) continue;
        
            var newWorldPosition = mainCamera.ViewportToWorldPoint(viewportPosition);
            registeredTransform.position = newWorldPosition;
            
        }
    }
}
