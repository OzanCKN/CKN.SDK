using System;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using System.Text;

namespace CKN.Sdk.Infrastructure.Security;

/// <summary>
/// Generates a unique hardware identifier based on the machine's MAC address.
/// </summary>
public static class HardwareIdGenerator
{
    /// <summary>
    /// Gets the unique hardware ID (HWID) of the current machine.
    /// </summary>
    /// <returns>A hashed string representing the hardware ID.</returns>
    public static string GetHardwareId()
    {
        try
        {
            // Find the first operational network interface that has a valid MAC address.
            var macAddress = NetworkInterface.GetAllNetworkInterfaces()
                .Where(nic => nic.OperationalStatus == OperationalStatus.Up && nic.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                .Select(nic => nic.GetPhysicalAddress().ToString())
                .FirstOrDefault(mac => !string.IsNullOrEmpty(mac));

            if (string.IsNullOrEmpty(macAddress))
            {
                // Fallback to MachineName if no network interface is found
                macAddress = Environment.MachineName;
            }

            // Create a hash of the MAC address to obscure the raw value
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(macAddress));

            // Format as a readable hex string
            var sb = new StringBuilder();
            foreach (var b in hashBytes)
            {
                sb.Append(b.ToString("X2"));
            }

            return sb.ToString();
        }
        catch
        {
            // Fallback in case of permission issues
            return "UNKNOWN-HWID";
        }
    }
}
