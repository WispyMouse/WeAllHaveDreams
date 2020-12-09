using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigurationData
{
    public string ConfigurationType;

    public ConfigurationData()
    {
        ConfigurationType = this.GetType().ToString();
    }
}
