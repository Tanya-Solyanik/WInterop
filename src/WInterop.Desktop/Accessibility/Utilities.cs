// Copyright (c) Jeremy W. Kuhne. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace WInterop.Accessibility;

public class Utilities
{
    // UIAutomationCoreAPI.h
    public static int UiaRootObjectId = -25;

    public static string ObjectIdentifierToString(int identifier)
    {
        if (identifier > 0)
        {
            return $"Custom Object ID {identifier}";
        }

        if (identifier == UiaRootObjectId)
        {
            return $"Requesting UI Automation provider";
        }

        if (Enum.IsDefined(typeof(ObjectIdentifier), identifier))
        {
            return $"{Enum.GetName<ObjectIdentifier>((ObjectIdentifier)identifier)}";
        }

        return $"{identifier:x}";
    }
}