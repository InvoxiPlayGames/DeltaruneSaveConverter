using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeltaruneSaveConverter
{
    enum GMSListItemType
    {
        GMSListReal,
        GMSListString,
        
        // found in someone's chapter 2 save - contents for it just had real value 1
        GMSListRealThirteen = 13,

        // found in someone's chapter 4 save - no idea what the contents were meant to be... but 8 bytes? get real
        GMSListRealWeird = 10
    }
    class GMSListItem
    {
        public GMSListItemType type;
    }
    class GMSListStringItem : GMSListItem
    {
        public string stringValue;
        public GMSListStringItem(string value)
        {
            type = GMSListItemType.GMSListString;
            stringValue = value;
        }
        public override string ToString()
        {
            return stringValue;
        }
    }
    class GMSListRealItem : GMSListItem
    {
        public double realValue;
        public GMSListRealItem(double value)
        {
            type = GMSListItemType.GMSListReal;
            realValue = value;
        }
        public override string ToString()
        {
            return realValue.ToString();
        }
    }

    class GMSListDecoder
    {
        private byte[] rawlist;
        private List<GMSListItem> list;

        public GMSListDecoder(string listhex)
        {
            if (!listhex.StartsWith("2E010000") && !listhex.StartsWith("2F010000"))
            {
                throw new Exception("String was passed to GMSListDecoder that is not a ds_list.");
            } else if (listhex.Length % 2 != 0)
            {
                throw new Exception("String was passed to GMSListDecoder that is not an even length.");
            }

            rawlist = new byte[listhex.Length / 2];
            for (int i = 0; i < listhex.Length; i += 2) rawlist[i / 2] = Convert.ToByte(listhex.Substring(i, 2), 16);
            //items = BitConverter.ToInt32(rawlist, 4);
            list = new List<GMSListItem>();

            for (int i = 8; i < rawlist.Length;)
            {
                GMSListItemType type = (GMSListItemType)BitConverter.ToInt32(rawlist, i);
                i += 4;
                if (type == GMSListItemType.GMSListReal || type == GMSListItemType.GMSListRealWeird || type == GMSListItemType.GMSListRealThirteen)
                {
                    double value = BitConverter.ToDouble(rawlist, i);
                    i += 8;
                    list.Add(new GMSListRealItem(value));
                } else if (type == GMSListItemType.GMSListString)
                {
                    int stringLength = BitConverter.ToInt32(rawlist, i);
                    i += 4;
                    string stringValue = "";
                    if (stringLength > 0)
                        stringValue = Encoding.ASCII.GetString(rawlist, i, stringLength);
                    list.Add(new GMSListStringItem(stringValue));
                    i += stringLength;
                } else
                {
                    throw new Exception($"Unknown list type {BitConverter.ToInt32(rawlist, i - 4)} found in GMSListDecoder at pos {i - 4}");
                }
            }
        }

        public int ListSize()
        {
            return list.Count;
        }

        public string GetString(int i)
        {
            if (GetType(i) != GMSListItemType.GMSListString) return "";
            return ((GMSListStringItem)list[i]).stringValue;
        }

        public double GetReal(int i)
        {
            if (GetType(i) != GMSListItemType.GMSListReal) return 0;
            return ((GMSListRealItem)list[i]).realValue;
        }

        public void ToRealArray(ref double[] output, int length)
        {
            for (int i = 0; i < length; i++)
            {
                output[i] = GetReal(i);
            }
        }

        public void ToStringArray(ref string[] output, int length)
        {
            for (int i = 0; i < length; i++)
            {
                output[i] = GetString(i);
            }
        }

        public GMSListItemType GetType(int i)
        {
            return list[i].type;
        }
    }

    class GMSListEncoder
    {
        private byte[] rawlist;
        private List<GMSListItem> list;

        public GMSListEncoder()
        {
            list = new List<GMSListItem>();
        }

        public GMSListEncoder(IEnumerable<double> reallist)
        {
            list = new List<GMSListItem>();
            foreach(double item in reallist)
                list.Add(new GMSListRealItem(item));
        }

        public GMSListEncoder(IEnumerable<string> stringlist)
        {
            list = new List<GMSListItem>();
            foreach (string item in stringlist)
                list.Add(new GMSListStringItem(item));
        }

        public void AddReal(double real)
        {
            list.Add(new GMSListRealItem(real));
        }

        public void AddString(string stringVal)
        {
            list.Add(new GMSListStringItem(stringVal));
        }

        public byte[] GetRaw()
        {
            List<byte> temp = new();
            temp.AddRange(BitConverter.GetBytes(0x12E));
            temp.AddRange(BitConverter.GetBytes(list.Count));
            for (int i = 0; i < list.Count; i++)
            {
                GMSListItemType type = list[i].type;
                temp.AddRange(BitConverter.GetBytes((int)type));
                if (type == GMSListItemType.GMSListReal)
                {
                    temp.AddRange(BitConverter.GetBytes(((GMSListRealItem)list[i]).realValue));
                } else if (type == GMSListItemType.GMSListString)
                {
                    string stringValue = ((GMSListStringItem)list[i]).stringValue;
                    temp.AddRange(BitConverter.GetBytes(stringValue.Length));
                    temp.AddRange(Encoding.UTF8.GetBytes(stringValue, 0, stringValue.Length));
                } else
                {
                    throw new Exception("Unknown list type found in GMSListEncoder");
                }
            }
            rawlist = temp.ToArray();
            return rawlist;
        }

        public string GetString()
        {
            GetRaw();
            return BitConverter.ToString(rawlist).Replace("-", "");
        }
    }
}
