using System;
using System.IO;
using System.Security;
using System.Xml;

namespace PacketGenerator
{

    class Program
    {
        static string GenPackets;
        static ushort packetid;
        static string packetEnums;

        static string clientRegister;
        static string serverRegister;


        static void Main(string[] args)
        {

            string pdlPath = "../../PDL.xml";


            XmlReaderSettings settings = new XmlReaderSettings()
            {
                IgnoreComments = true,
                IgnoreWhitespace = true
            };

            if (args.Length >= 1)
                pdlPath = args[0];

            using (XmlReader r = XmlReader.Create(pdlPath, settings))
            {
                r.MoveToContent();

                while(r.Read() && r.NodeType == XmlNodeType.Element)
                {
                    if(r.Depth == 1)
                    {
                        ParsePacket(r);
                    }

                    //Console.WriteLine($"{r.Name} : {r["name"]}");
                }

                string fileText = string.Format(PacketFormat.fileFormat, packetEnums,  GenPackets);
                File.WriteAllText("GenPackets.cs", fileText);

                string ClientmanagerText = string.Format(PacketFormat.managerFormat, clientRegister);
                File.WriteAllText("ClientPacketManager.cs", ClientmanagerText);

                string ServermanagerText = string.Format(PacketFormat.managerFormat, serverRegister);
                File.WriteAllText("ServerPacketManager.cs", ServermanagerText);
            }
        }

        public static void ParsePacket(XmlReader r)
        {
            if (r.NodeType == XmlNodeType.EndElement)
            {
                Console.WriteLine("XML Reader NoteType is EndElement");
                return;
            }
                

            if(r.Name.ToLower() != "packet")
            {
                Console.WriteLine("Invalid Packet Node");
                return;
            }

            string packetName = r["name"];
            if (string.IsNullOrEmpty(packetName))
            {
                Console.WriteLine("Packet without Name!");
                return;
            }

            Tuple<String, string, string>  t = ParseMember(r);
            GenPackets += string.Format(PacketFormat.packetFormat,
                packetName, t.Item1, t.Item2, t.Item3);

            packetEnums += string.Format(PacketFormat.packetEnumFormat, packetName, ++packetid) + Environment.NewLine + "\t";

            if(packetName.StartsWith("S_") || packetName.StartsWith("s_"))
                clientRegister += string.Format(PacketFormat.managerRegisterFormat, packetName) + Environment.NewLine;
            else
                serverRegister += string.Format(PacketFormat.managerRegisterFormat, packetName) + Environment.NewLine;
        }

        // {1} 멤버 변수
        // {2} 멤버 변수 Read
        // {3} 멤버 변수 Write
        public static Tuple<String, string, string> ParseMember(XmlReader r)
        {
            string packetName = r["name"];

            string membercode = "";
            string readCode = "";
            string writeCode = "";

            int depth = r.Depth + 1;
            while(r.Read())
            {
                if (r.Depth != depth)
                    break;

                string memberName = r["name"];
                if(string.IsNullOrEmpty(memberName))
                {
                    Console.WriteLine("Member without name");
                    return null;
                }

                if (string.IsNullOrEmpty(membercode) == false)
                    membercode += Environment.NewLine;
                if (string.IsNullOrEmpty(readCode) == false)
                    readCode += Environment.NewLine;
                if (string.IsNullOrEmpty(writeCode) == false)
                    writeCode += Environment.NewLine;

                string memberType = r.Name.ToLower();
                switch(memberType)
                {
                    
                    case "byte":
                    case "sbyte":
                        membercode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readByteFormat, memberName, memberType);
                        writeCode += string.Format(PacketFormat.writeByteFormat, memberName, memberType);
                        break;
                    case "bool":
                    case "short":
                    case "ushort":
                    case "int":
                    case "long":
                    case "float":
                    case "double":
                        membercode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readFormat, memberName, ToMemberType(memberType), memberType);
                        writeCode += string.Format(PacketFormat.writeFormat, memberName, memberType);
                        break;
                    case "string":
                        membercode += string.Format(PacketFormat.memberFormat, memberType, memberName);
                        readCode += string.Format(PacketFormat.readStringFormat, memberName);
                        writeCode += string.Format(PacketFormat.writeStringFormat, memberName);
                        break;
                    case "list":
                        Tuple<string, string, string> t = ParseList(r);
                        membercode += t.Item1;
                        readCode += t.Item2;
                        writeCode += t.Item3;
                        break;

                    default:
                        break;
                }
            }

            membercode = membercode.Replace("\n", "\n\t");
            readCode = readCode.Replace("\n", "\t\t\t");
            writeCode = writeCode.Replace("\n", "\t\t\t");
            return new Tuple<string, string, string>(membercode, readCode, writeCode);
        }

        public static string ToMemberType(string memberType)
        {
            switch(memberType)
            {
                case "bool":
                    return "ToBoolean";
                case "short":
                    return "ToInt16";
                case "ushort":
                    return "ToUInt16";
                case "int":
                    return "ToInt32";
                case "long":
                    return "ToInt64";
                case "float":
                    return "ToSingle";
                case "double":
                    return "ToDouble";
                default:
                    return "";
            }
        }

        public static string FirstCharToUpper(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            return input[0].ToString().ToUpper() + input.Substring(1);
        }

        public static string FirstCharToLower(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            return input[0].ToString().ToLower() + input.Substring(1);
        }

        public static Tuple<string, string, string> ParseList(XmlReader r)
        {
            string listName = r["name"];

            if(string.IsNullOrEmpty(listName))
            {
                Console.WriteLine("List without Name");
                return null;
            }

            Tuple<string, string, string> t = ParseMember(r);
            string memberCode = string.Format(PacketFormat.memberListFormat,
                FirstCharToUpper(listName),
                FirstCharToLower(listName),
                t.Item1,
                t.Item2,
                t.Item3);

            string readCode = string.Format(PacketFormat.readListFormat,
                FirstCharToUpper(listName),
                FirstCharToLower(listName));

            string writeCode = string.Format(PacketFormat.writeListFormat,
            FirstCharToUpper(listName),
            FirstCharToLower(listName));

            return new Tuple<string, string, string>(memberCode, readCode, writeCode);
        }
    }
}
