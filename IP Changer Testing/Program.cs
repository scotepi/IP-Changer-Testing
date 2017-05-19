using System;
using System.Net.NetworkInformation;

// For var_dump;
using System.Text;
using System.Reflection;
using System.Collections;

namespace IP_Changer_Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");

            DisplayDnsConfiguration();



            for (int i = 0; i < 10; i++)
            {
                Console.Write("Enter an IP: ");
                string ip = Console.ReadLine();
                if (CheckIPAddressv4(ip))
                {
                    Console.WriteLine("Congrats! {0} Is Valid!", ip);
                } else
                {
                    Console.WriteLine("{0} is an Invalid IP", ip);
                }

                Console.WriteLine();
            }

            Console.WriteLine("Done for Now");
            Console.ReadLine();
            
        }

        static bool CheckIPAddressv4(string ipString)
        {
            // Nothing was passed
            if (String.IsNullOrWhiteSpace(ipString))
            {
                return false;
            }

            // We have 4 octets
            string[] splitValues = ipString.Split('.');
            if (splitValues.Length != 4)
            {
                return false;
            }

            foreach (String octetString in splitValues)
            {

                int octet;

                try
                {
                    octet = Int16.Parse(octetString);
                }
                catch
                {
                    return false;
                }
                

                if (0 <= octet && octet <= 255)
                {
                    // Within Range
                } else
                {
                    return false;
                }
            }

            return true;
        }

        static void DisplayDnsConfiguration()
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters)
            {
                // For testing, only bother with up interfaces
                if (adapter.OperationalStatus.ToString() == "Up")
                {
                    
                    Console.WriteLine("Adapter Name ..........: {0}", adapter.Name);
                    Console.WriteLine("Adapter Description ...: {0}", adapter.Description);
                    Console.WriteLine("Adapter Status ........: {0}", adapter.OperationalStatus);
                    Console.WriteLine("Adapter Id ............: {0}", adapter.Id);
                    Console.WriteLine("Adapter Status ........: {0}", adapter.OperationalStatus);
                    Console.WriteLine("Adapter Speed .........: {0}", adapter.Speed);
                    Console.WriteLine("Adapter MAC Address ...: {0}", adapter.GetPhysicalAddress());


                    IPInterfaceProperties ip = adapter.GetIPProperties();
                    Console.WriteLine("    DNS Enabled .......: {0}", ip.IsDnsEnabled);
                    Console.WriteLine("    DynamicDNS Enabled : {0}", ip.IsDynamicDnsEnabled);
                    Console.WriteLine("    DNS Suffix ........: {0}", ip.DnsSuffix);

                    for (int i = 0; i < ip.DnsAddresses.Count; i++)
                    {
                        Console.WriteLine("    DNS Address .......: {0}", ip.DnsAddresses[i]);
                    }

                    for (int i = 0; i < ip.GatewayAddresses.Count; i++)
                    {
                        Console.WriteLine("    Gateway Address ...: {0}", ip.GatewayAddresses[i].Address);
                    }

                    for (int i = 0; i < ip.DhcpServerAddresses.Count; i++)
                    {
                        Console.WriteLine("    DHCP Address ......: {0}", ip.DhcpServerAddresses[i]);
                    }
                    for (int i = 0; i < ip.WinsServersAddresses.Count; i++)
                    {
                        Console.WriteLine("    WINS Address ......: {0}", ip.WinsServersAddresses[i]);
                    }

                    IPv4InterfaceProperties ipv4 = ip.GetIPv4Properties();
                    Console.WriteLine("    v4 DHCP ...........: {0}", ipv4.IsDhcpEnabled);
                    Console.WriteLine("    v4 Forwarding .....: {0}", ipv4.IsForwardingEnabled);
                    Console.WriteLine("    v4 AutoIP Active ..: {0}", ipv4.IsAutomaticPrivateAddressingActive);
                    Console.WriteLine("    v4 MTU.............: {0}", ipv4.Mtu);


                    IPv6InterfaceProperties ipv6 = ip.GetIPv6Properties();
                    Console.WriteLine("    v6 DHCP ...........: {0}", ipv6.Index);
                    Console.WriteLine("    v6 MTU ............: {0}", ipv6.Mtu);



                    foreach (UnicastIPAddressInformation ipUni in ip.UnicastAddresses)
                    {

                        // We only care about IPv4, v6 is InterNetworkV6
                        if (ipUni.Address.AddressFamily.ToString() == "InterNetwork")
                        {
                            Console.WriteLine("    IP Address ........: {0}", ipUni.Address);
                            Console.WriteLine("        Netmask .........: {0}", ipUni.IPv4Mask);
                            Console.WriteLine("        CIDR ............: {0}", ipUni.PrefixLength);
                            Console.WriteLine("        PrefixOrigin ....: {0}", ipUni.PrefixOrigin);
                            Console.WriteLine("        SuffixOrigin ....: {0}", ipUni.SuffixOrigin);

                        }





                    }

                    /**
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    Console.WriteLine(adapter.Description);


                    Console.WriteLine(var_dump(adapter, 0));
                    
                    Console.WriteLine(adapter.Description);
                    Console.WriteLine("  DNS suffix .............................. : {0}",
                        properties.DnsSuffix);
                    Console.WriteLine("  DNS enabled ............................. : {0}",
                        properties.IsDnsEnabled);
                    Console.WriteLine("  Dynamically configured DNS .............. : {0}",
                        properties.IsDynamicDnsEnabled);
                    **/

                    Console.WriteLine("--------------------------------------------");
                    Console.WriteLine();
                }
            }
            
        }



  
        static string var_dump(object obj, int recursion)
        {
            StringBuilder result = new StringBuilder();

            // Protect the method against endless recursion
            if (recursion < 5)
            {
                // Determine object type
                Type t = obj.GetType();

                // Get array with properties for this object
                PropertyInfo[] properties = t.GetProperties();

                foreach (PropertyInfo property in properties)
                {
                    try
                    {
                        // Get the property value
                        object value = property.GetValue(obj, null);

                        // Create indenting string to put in front of properties of a deeper level
                        // We'll need this when we display the property name and value
                        string indent = String.Empty;
                        string spaces = "|   ";
                        string trail = "|...";

                        if (recursion > 0)
                        {
                            indent = new StringBuilder(trail).Insert(0, spaces, recursion - 1).ToString();
                        }

                        if (value != null)
                        {
                            // If the value is a string, add quotation marks
                            string displayValue = value.ToString();
                            if (value is string) displayValue = String.Concat('"', displayValue, '"');

                            // Add property name and value to return string
                            result.AppendFormat("{0}{1} = {2}\n", indent, property.Name, displayValue);

                            try
                            {
                                if (!(value is ICollection))
                                {
                                    // Call var_dump() again to list child properties
                                    // This throws an exception if the current property value
                                    // is of an unsupported type (eg. it has not properties)
                                    result.Append(var_dump(value, recursion + 1));
                                }
                                else
                                {
                                    // 2009-07-29: added support for collections
                                    // The value is a collection (eg. it's an arraylist or generic list)
                                    // so loop through its elements and dump their properties
                                    int elementCount = 0;
                                    foreach (object element in ((ICollection)value))
                                    {
                                        string elementName = String.Format("{0}[{1}]", property.Name, elementCount);
                                        indent = new StringBuilder(trail).Insert(0, spaces, recursion).ToString();

                                        // Display the collection element name and type
                                        result.AppendFormat("{0}{1} = {2}\n", indent, elementName, element.ToString());

                                        // Display the child properties
                                        result.Append(var_dump(element, recursion + 2));
                                        elementCount++;
                                    }

                                    result.Append(var_dump(value, recursion + 1));
                                }
                            }
                            catch { }
                        }
                        else
                        {
                            // Add empty (null) property to return string
                            result.AppendFormat("{0}{1} = {2}\n", indent, property.Name, "null");
                        }
                    }
                    catch
                    {
                        // Some properties will throw an exception on property.GetValue()
                        // I don't know exactly why this happens, so for now i will ignore them...
                    }
                }
            }

            return result.ToString();
        }
    }
}