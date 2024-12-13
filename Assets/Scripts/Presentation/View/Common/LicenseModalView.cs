using UnityEngine;
using TMPro;
using System.Collections.Generic;
using WatermelonGameClone.Domain;

namespace WatermelonGameClone.Presentation
{
    public class LicenseRepository : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI licenseText;

        public void DisplayLicenses(IReadOnlyList<License> licenses)
        {
            licenseText.text = string.Empty;

            foreach (var license in licenses)
            {
                licenseText.text += $"Name: {license.Name}\n\n";
                licenseText.text += $"License: {license.LicenseType}\n";
                licenseText.text += $"Copyright: {license.Copyright}\n";
                licenseText.text += $"Terms:\n{string.Join("\n", license.Terms)}\n\n";
            }
        }
    }
}
