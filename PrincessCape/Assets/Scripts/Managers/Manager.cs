using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Manager {
    /// <summary>
    /// Update the specified dt.
    /// </summary>
    /// <returns>The update.</returns>
    /// <param name="dt">Dt.</param>
    void Update(float dt);
}
